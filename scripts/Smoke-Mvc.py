"""Real HTTP + SQL MVC smoke test. Requires sqlcmd and SQL deployment environment variables.
Creates/drops only its own generated database; never prints connection strings or tokens.
"""
import os, subprocess, uuid, pathlib, time, urllib.request, urllib.parse, urllib.error, http.cookiejar, re, html, secrets
root = pathlib.Path(__file__).resolve().parent.parent
server = os.environ["SEOFLOW_SQL_SERVER"]
user = os.environ["SEOFLOW_SQL_USER"]
assert os.environ.get("SQLCMDPASSWORD"), "Set SQLCMDPASSWORD"
name = "SEOFlowAI_Smoke_" + uuid.uuid4().hex
args = ["sqlcmd", "-S", server, "-U", user, "-C", "-b", "-I"]
def sql(*more):
    subprocess.run(args+list(more), check=True, stdout=subprocess.DEVNULL)
process = None
outbox_files = []
log_path = root / "artifacts" / "mvc-smoke.log"
log_path.parent.mkdir(exist_ok=True)
email = "synthetic-smoke-" + uuid.uuid4().hex + "@example.com"
try:
    sql("-Q", f"CREATE DATABASE [{name}]")
    for file in sorted((root/"database"/"migrations").glob("*.sql")):
        sql("-d", name, "-i", str(file))
    env = dict(os.environ)
    env.update(ASPNETCORE_ENVIRONMENT="Development", ConnectionStrings__SeoFlow=f"Server={server};Database={name};User Id={user};Password={os.environ['SQLCMDPASSWORD']};Encrypt=True;TrustServerCertificate=True;", Auth__SigningKey=secrets.token_hex(48))
    log = log_path.open("w",encoding="utf-8")
    process = subprocess.Popen(["dotnet","run","--no-build","--project","src/SEOFlow.Web","--urls","http://localhost:5087"],cwd=root,env=env,stdout=log,stderr=subprocess.STDOUT,creationflags=getattr(subprocess,"CREATE_NO_WINDOW",0))
    opener = urllib.request.build_opener(urllib.request.HTTPCookieProcessor(http.cookiejar.CookieJar()))
    base = "http://localhost:5087"
    def get(path):
        with opener.open(base+path,timeout=10) as response: return response.read().decode()
    def post(path,fields):
        with opener.open(base+path,urllib.parse.urlencode(fields).encode(),timeout=20) as response: return response.read().decode()
    def csrf(page): return html.unescape(re.search(r'name="__RequestVerificationToken" type="hidden" value="([^"]+)"', page).group(1))
    for _ in range(120):
        try:
            page=get("/Account/Login"); break
        except (OSError, urllib.error.URLError):
            if process.poll() is not None: raise RuntimeError("MVC startup failed; inspect artifacts/mvc-smoke.log")
            time.sleep(.5)
    else: raise RuntimeError("MVC startup timeout")
    try:
        post("/Account/Login", {"Email":email,"Password":"Test-Strong-Password-123"})
        raise AssertionError("Missing antiforgery token was accepted")
    except urllib.error.HTTPError as error: assert error.code == 400
    page=get("/Account/Register")
    page=post("/Account/Register", {"__RequestVerificationToken":csrf(page),"Email":email,"Password":"Test-Strong-Password-123","Organization":"SYNTHETIC MVC SMOKE"})
    assert "Verify your email" in page
    token=None
    for file in (root/"src"/"SEOFlow.Web"/".secrets"/"outbox").glob("*.txt"):
        message=file.read_text(encoding="utf-8-sig")
        if email in message:
            token=message.split("Token: ")[1].strip(); outbox_files.append(file)
    assert token, "Development verification token not delivered"
    page=post("/Account/Verify",{"__RequestVerificationToken":csrf(page),"Token":token})
    assert "Email verified" in page
    page=post("/Account/Login",{"__RequestVerificationToken":csrf(page),"Email":email,"Password":"Test-Strong-Password-123"})
    assert "Your search workspace" in page and "SYNTHETIC MVC SMOKE" in page
    tenant=re.search(r'name="tenantId" value="([0-9a-f-]+)"',page).group(1)
    page=post("/Dashboard/AddWebsite",{"__RequestVerificationToken":csrf(page),"tenantId":tenant,"Name":"SYNTHETIC Website","Domain":"https://synthetic.example"})
    assert "SYNTHETIC Website" in page and "Not verified" in page
    page=post("/Account/Logout",{"__RequestVerificationToken":csrf(page)})
    assert "Welcome back" in page
    print("MVC smoke passed: antiforgery rejection, register, verify, cookie login, tenant dashboard, website creation, logout.")
finally:
    if process:
        subprocess.run(["taskkill","/PID",str(process.pid),"/T","/F"],stdout=subprocess.DEVNULL,stderr=subprocess.DEVNULL)
        process.wait(timeout=20)
    for file in outbox_files: file.unlink(missing_ok=True)
    assert re.fullmatch(r"SEOFlowAI_Smoke_[0-9a-f]{32}",name)
    sql("-Q",f"IF DB_ID(N'{name}') IS NOT NULL BEGIN ALTER DATABASE [{name}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{name}]; END")
