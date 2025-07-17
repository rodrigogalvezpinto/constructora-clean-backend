# Controllers incluidos en la API

- ProjectsController: Endpoint de costos por proyecto
- RegionsController: Endpoint de top overruns por región
- HealthController: Endpoint de salud para monitoreo

Todos los endpoints siguen el prefijo /api/v1/ y retornan JSON.

Endpoints principales:
- GET /api/v1/projects/{projectId}/costs?from=YYYY-MM-DD&to=YYYY-MM-DD
- GET /api/v1/regions/{regionId}/top-overruns?limit=N
- GET /api/v1/health
