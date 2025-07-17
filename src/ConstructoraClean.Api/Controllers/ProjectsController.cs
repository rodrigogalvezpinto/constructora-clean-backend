using Microsoft.AspNetCore.Mvc;
using ConstructoraClean.Api.DTOs;
using ConstructoraClean.Application.Interfaces;
using ConstructoraClean.Application.Queries;

namespace ConstructoraClean.Api.Controllers
{
    [ApiController]
    [Route("api/v1/projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectCostService _projectCostService;
        public ProjectsController(IProjectCostService projectCostService)
        {
            _projectCostService = projectCostService ?? throw new ArgumentNullException(nameof(projectCostService));
        }

        /// <summary>
        /// Obtiene los costos de un proyecto, materiales principales y desglose mensual.
        /// </summary>
        /// <param name="projectId">ID del proyecto</param>
        /// <param name="from">Fecha inicial (yyyy-MM-dd)</param>
        /// <param name="to">Fecha final (yyyy-MM-dd)</param>
        /// <returns>Costos del proyecto, top materiales y desglose mensual</returns>
        [HttpGet("{projectId}/costs")]
        [ProducesResponseType(typeof(ProjectCostsDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProjectCosts(
            int projectId,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            if (from > to)
                return BadRequest("La fecha inicial no puede ser mayor que la final.");
            if (projectId <= 0)
                return BadRequest("El ID de proyecto debe ser mayor que cero.");
            try
            {
                var query = new GetProjectCostsQuery(projectId, from, to);
                var result = await _projectCostService.GetProjectCostsAsync(query);
                if (result == null)
                    return NotFound();
                
                // Mapear a DTO de API
                var dto = new ProjectCostsDto
                {
                    TotalCost = result.TotalCost,
                    TopMaterials = result.TopMaterials.Select(m => new TopMaterialDto { Material = m.Material, TotalCost = m.TotalCost }).ToList(),
                    MonthlyBreakdown = result.MonthlyBreakdown.Select(m => new MonthlyBreakdownDto { Month = m.Month, TotalCost = m.TotalCost }).ToList()
                };
                
                return Ok(dto);
            }
            catch (Exception ex)
            {
                // Aquí podrías loggear el error con ILogger
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}
