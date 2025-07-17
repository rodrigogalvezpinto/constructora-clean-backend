-- seed.sql: Carga masiva de datos para Constructora Clean
-- PostgreSQL 15+
-- 10 regiones, 100 proyectos, 500 materiales, 50 proveedores, 1.000.000 compras

-- 1. Regiones
INSERT INTO region (name)
SELECT 'Región ' || g FROM generate_series(1, 10) g;

-- 2. Proyectos
INSERT INTO project (name, region_id, budget, start_date, end_date)
SELECT 
    'Proyecto ' || p,
    ((p - 1) % 10) + 1, -- Distribuye proyectos entre regiones
    (random() * 900000 + 100000)::NUMERIC(18,2), -- presupuesto entre 100k y 1M
    (CURRENT_DATE - INTERVAL '24 months') + ((p-1) * INTERVAL '7 days'),
    NULL
FROM generate_series(1, 100) p;

-- 3. Materiales
INSERT INTO material (name)
SELECT 'Material ' || m FROM generate_series(1, 500) m;

-- 4. Proveedores
INSERT INTO supplier (name)
SELECT 'Proveedor ' || s FROM generate_series(1, 50) s;

-- 5. Compras (1.000.000 registros, distribuidos en 24 meses)
-- Usamos una tabla temporal para eficiencia
CREATE TEMP TABLE tmp_purchase AS
SELECT 
    (random() * 99 + 1)::INTEGER AS project_id,
    (random() * 499 + 1)::INTEGER AS material_id,
    (random() * 49 + 1)::INTEGER AS supplier_id,
    (CURRENT_DATE - (random() * INTERVAL '24 months'))::DATE AS purchase_date,
    (random() * 90 + 10)::NUMERIC(12,2) AS quantity,
    (random() * 9500 + 500)::NUMERIC(14,2) AS unit_cost
FROM generate_series(1, 1000000);

INSERT INTO purchase (project_id, material_id, supplier_id, purchase_date, quantity, unit_cost)
SELECT project_id, material_id, supplier_id, purchase_date, quantity, unit_cost FROM tmp_purchase;

DROP TABLE tmp_purchase;
