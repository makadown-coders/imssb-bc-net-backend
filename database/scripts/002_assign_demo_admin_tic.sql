BEGIN;

INSERT INTO user_roles (user_id, role_code)
SELECT users."Id", 'ADMIN_TIC'
FROM "Users" users
WHERE lower(users."Email") = lower('demo@example.com')
ON CONFLICT (user_id, role_code) DO UPDATE
SET is_active = true,
    revoked_at = NULL,
    assigned_at = now();

COMMIT;
