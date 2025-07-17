-- queries.sql: Consultas SQL finales para los endpoints requeridos

-- 1. Costos por Proyecto (GET /api/v1/projects/{project_id}/costs?from=&to=)
-- Parámetros: :project_id, :from_date, :to_date
-- a) Costo total para el período
SELECT SUM(total_cost) AS total_cost
FROM purchase
WHERE project_id = :project_id
  AND purchase_date BETWEEN :from_date AND :to_date;

-- b) Top 10 materiales por costo en el período
SELECT m.name AS material, SUM(p.total_cost) AS total_cost
FROM purchase p
JOIN material m ON m.id = p.material_id
WHERE p.project_id = :project_id
  AND p.purchase_date BETWEEN :from_date AND :to_date
GROUP BY m.name
ORDER BY total_cost DESC
LIMIT 10;

-- c) Desglose mensual (YYYY-MM)
SELECT to_char(purchase_date, 'YYYY-MM') AS month, SUM(total_cost) AS total_cost
FROM purchase
WHERE project_id = :project_id
  AND purchase_date BETWEEN :from_date AND :to_date
GROUP BY month
ORDER BY month;

-- 2. Top Overruns por Región (GET /api/v1/regions/{region_id}/top-overruns?limit=)
-- Parámetros: :region_id, :limit
SELECT pr.id AS project_id, pr.name, pr.budget,
       COALESCE(SUM(p.total_cost),0) AS total_cost,
       CASE WHEN pr.budget > 0 THEN (COALESCE(SUM(p.total_cost),0) / pr.budget - 1) ELSE NULL END AS overrun_pct
FROM project pr
LEFT JOIN purchase p ON p.project_id = pr.id
WHERE pr.region_id = :region_id
GROUP BY pr.id, pr.name, pr.budget
ORDER BY overrun_pct DESC NULLS LAST
LIMIT :limit;

-- Índices recomendados:
-- idx_purchase_project_date, idx_purchase_material_project, idx_project_region
