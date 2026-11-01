# Phase 1 REST API

Versioned prefix: `/api/v1`. Swagger/OpenAPI is enabled in Development at `/swagger`. Example requests are in examples/SEOFlow.http. API and MVC may run independently against the same SQL database. API does not enable cross-origin access by default.

Anonymous POST auth routes: register, login, refresh, revoke, request-verification, verify-email, forgot-password, reset-password. Refresh and revoke take a refreshToken. Register requires email, password (12–128 characters), organization. Verification/reset tokens are single use. Login returns accessToken, refreshToken, expiresUtc. Access JWTs last 10 minutes; refresh tokens last 7 days, rotate, and are stored as SHA-256 digests. Replaying a rotated token revokes its whole family. Logout/revoke does not invalidate an already-issued access JWT until its ten-minute expiration; password reset invalidates both immediately through security-stamp validation.

Authenticated routes:

- GET `/users/me`: current account identity. GET `/users?tenantId=...&page=1&pageSize=20`: paginated tenant team directory, restricted to managers.
- GET/POST `/tenants`: list memberships/create organization.
- GET `/tenants/{tenantId}/dashboard`: database counts and disconnected provider state.
- GET/POST `/tenants/{tenantId}/websites`: paginated search/create unverified site. Query: page, pageSize (1–100), search.
- POST `/tenants/{tenantId}/invitations`: email and role; returns a one-time token for secure manual sharing. Requires AgencyAdmin or SuperAdmin membership. SuperAdmin cannot be invited.
- POST `/tenants/invitations/accept`: token; requires matching account email.
- DELETE `/tenants/{tenantId}/members/{memberId}`: deactivate another member. Self-deactivation is refused.

Authenticated identity comes from the JWT, never a request body. Invalid DTOs return 400; unauthenticated requests 401; unauthorized tenant access 403; uniqueness/concurrency conflicts 409. Unhandled errors return sanitized problem details with traceId. API auth routes limit each source IP to 15 requests/minute. `/health` returns readiness without secrets. Provider, crawling, publishing, report and schedule routes are intentionally absent until implemented. No endpoint can publish a live change in Phase 1.
