CREATE TABLE IF NOT EXISTS public.onco_clases (
    id smallint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    codigo varchar(10) NOT NULL,
    nombre varchar(100) NOT NULL,
    descripcion text NULL,
    stock_factor numeric(6,2) NULL,
    activo boolean NOT NULL DEFAULT true,
    creado_en timestamptz NOT NULL DEFAULT now(),
    actualizado_en timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.onco_subclases (
    id smallint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    codigo varchar(20) NOT NULL,
    nombre varchar(150) NOT NULL,
    descripcion text NULL,
    activo boolean NOT NULL DEFAULT true,
    creado_en timestamptz NOT NULL DEFAULT now(),
    actualizado_en timestamptz NOT NULL DEFAULT now()
);

ALTER TABLE public.onco_clases
    ADD COLUMN IF NOT EXISTS descripcion text NULL,
    ADD COLUMN IF NOT EXISTS stock_factor numeric(6,2) NULL,
    ADD COLUMN IF NOT EXISTS activo boolean NOT NULL DEFAULT true,
    ADD COLUMN IF NOT EXISTS creado_en timestamptz NOT NULL DEFAULT now(),
    ADD COLUMN IF NOT EXISTS actualizado_en timestamptz NOT NULL DEFAULT now();

ALTER TABLE public.onco_subclases
    ADD COLUMN IF NOT EXISTS descripcion text NULL,
    ADD COLUMN IF NOT EXISTS activo boolean NOT NULL DEFAULT true,
    ADD COLUMN IF NOT EXISTS creado_en timestamptz NOT NULL DEFAULT now(),
    ADD COLUMN IF NOT EXISTS actualizado_en timestamptz NOT NULL DEFAULT now();

DO $$
DECLARE
    constraint_name text;
BEGIN
    FOR constraint_name IN
        SELECT constraint_definition.conname
        FROM pg_constraint constraint_definition
        JOIN pg_class source_table ON source_table.oid = constraint_definition.conrelid
        JOIN pg_namespace source_schema ON source_schema.oid = source_table.relnamespace
        JOIN pg_class target_table ON target_table.oid = constraint_definition.confrelid
        JOIN pg_namespace target_schema ON target_schema.oid = target_table.relnamespace
        WHERE constraint_definition.contype = 'f'
          AND source_schema.nspname = 'public'
          AND source_table.relname = 'onco_subclases'
          AND target_schema.nspname = 'public'
          AND target_table.relname = 'onco_clases'
    LOOP
        EXECUTE format(
            'ALTER TABLE public.onco_subclases DROP CONSTRAINT %I',
            constraint_name);
    END LOOP;

    IF EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_schema = 'public'
          AND table_name = 'onco_subclases'
          AND column_name = 'clase_codigo')
    THEN
        ALTER TABLE public.onco_subclases
            ALTER COLUMN clase_codigo DROP NOT NULL;
    END IF;
END
$$;

CREATE UNIQUE INDEX IF NOT EXISTS ux_onco_clases_codigo
    ON public.onco_clases (codigo);

CREATE UNIQUE INDEX IF NOT EXISTS ux_onco_subclases_codigo
    ON public.onco_subclases (codigo);

INSERT INTO public.onco_clases (codigo, nombre, descripcion, activo)
VALUES
    ('I', 'Clase I', NULL, true),
    ('II', 'Clase II', NULL, true),
    ('III', 'Clase III', NULL, true),
    ('IV', 'Clase IV', NULL, true)
ON CONFLICT (codigo) DO NOTHING;
