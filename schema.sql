-- schema.sql: Modelo relacional para Constructora Clean
-- PostgreSQL 15+

CREATE TABLE region (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE project (
    id SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    region_id INTEGER NOT NULL REFERENCES region(id) ON DELETE CASCADE,
    budget NUMERIC(18,2) NOT NULL CHECK (budget >= 0),
    start_date DATE NOT NULL,
    end_date DATE
);

CREATE TABLE material (
    id SERIAL PRIMARY KEY,
    name VARCHAR(120) NOT NULL UNIQUE
);

CREATE TABLE supplier (
    id SERIAL PRIMARY KEY,
    name VARCHAR(120) NOT NULL UNIQUE
);

CREATE TABLE purchase (
    id BIGSERIAL PRIMARY KEY,
    project_id INTEGER NOT NULL REFERENCES project(id) ON DELETE CASCADE,
    material_id INTEGER NOT NULL REFERENCES material(id),
    supplier_id INTEGER NOT NULL REFERENCES supplier(id),
    purchase_date DATE NOT NULL,
    quantity NUMERIC(12,2) NOT NULL CHECK (quantity > 0),
    unit_cost NUMERIC(14,2) NOT NULL CHECK (unit_cost >= 0),
    total_cost NUMERIC(16,2) GENERATED ALWAYS AS (quantity * unit_cost) STORED
);

-- Índices para performance de consultas agregadas
CREATE INDEX idx_purchase_project_date ON purchase(project_id, purchase_date);
CREATE INDEX idx_purchase_material_project ON purchase(material_id, project_id);
CREATE INDEX idx_project_region ON project(region_id);
