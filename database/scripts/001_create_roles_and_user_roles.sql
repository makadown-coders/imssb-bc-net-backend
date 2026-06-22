BEGIN;

CREATE TABLE IF NOT EXISTS roles (
    code character varying(80) NOT NULL,
    descripcion character varying(200) NOT NULL,
    is_active boolean NOT NULL DEFAULT true,
    created_at timestamp with time zone NOT NULL DEFAULT now(),
    updated_at timestamp with time zone NULL,
    CONSTRAINT pk_roles PRIMARY KEY (code),
    CONSTRAINT ck_roles_code_format CHECK (code = upper(code) AND code ~ '^[A-Z0-9_]+$')
);

CREATE TABLE IF NOT EXISTS user_roles (
    user_id uuid NOT NULL,
    role_code character varying(80) NOT NULL,
    assigned_at timestamp with time zone NOT NULL DEFAULT now(),
    assigned_by_user_id uuid NULL,
    revoked_at timestamp with time zone NULL,
    is_active boolean NOT NULL DEFAULT true,
    CONSTRAINT pk_user_roles PRIMARY KEY (user_id, role_code),
    CONSTRAINT fk_user_roles_users_user_id
        FOREIGN KEY (user_id)
        REFERENCES "Users" ("Id")
        ON DELETE CASCADE,
    CONSTRAINT fk_user_roles_roles_role_code
        FOREIGN KEY (role_code)
        REFERENCES roles (code)
        ON DELETE RESTRICT,
    CONSTRAINT fk_user_roles_users_assigned_by_user_id
        FOREIGN KEY (assigned_by_user_id)
        REFERENCES "Users" ("Id")
        ON DELETE SET NULL,
    CONSTRAINT ck_user_roles_revoked_state CHECK (
        (is_active = true AND revoked_at IS NULL)
        OR (is_active = false)
    )
);

CREATE INDEX IF NOT EXISTS ix_user_roles_role_code
    ON user_roles (role_code);

CREATE INDEX IF NOT EXISTS ix_user_roles_active_user_id
    ON user_roles (user_id)
    WHERE is_active = true;

CREATE INDEX IF NOT EXISTS ix_user_roles_active_role_code
    ON user_roles (role_code)
    WHERE is_active = true;

INSERT INTO roles (code, descripcion)
VALUES
    ('ADMIN_TIC', 'Administrador TI'),
    ('FINANZAS', 'Operación finanzas'),
    ('ABASTO', 'Operación abasto'),
    ('JURIDICO', 'Operación juridico'),
    ('RECURSOS_HUMANOS', 'Operación Departamento de Personal'),
    ('RECURSOS_MATERIALES', 'Operación RM'),
    ('ATENCION_MEDICA', 'Operación Atencion Medica'),
    ('UNIDAD_MEDICA', 'Operación Unidad Medica'),
    ('PLANEACION_ESTRATEGICA', 'Operación Planeacion Estrategica'),
    ('ACCION_COMUNITARIA', 'Operación Accion Comunitaria'),
    ('CONSERVACION', 'Operación Conservacion y Mantenimiento'),
    ('SUPERVISION_REGIONAL', 'Operación Supervision Regional'),
    ('SUPERVISION_SALUD', 'Operación Supervision en Salud'),
    ('EPIDEMIOLOGIA', 'Operación Epidemiologia'),
    ('ENFERMERIA', 'Operación Enfermería'),
    ('CONDUCCION', 'Operación Conducción'),
    ('JURISDICCION', 'Operación Jurisdicción'),
    ('COORDINACION', 'Coordinación Estatal'),
    ('PROGRAMAS_PREVENTIVOS', 'Operación Programas Preventivos'),
    ('EDUCACION_INVESTIGACION', 'Operación Educación e investigación'),
    ('SOLICITUDES_ABASTO', 'Captura de solicitudes para abasto (piloto)')
ON CONFLICT (code) DO UPDATE
SET descripcion = EXCLUDED.descripcion,
    is_active = true,
    updated_at = now();

COMMIT;
