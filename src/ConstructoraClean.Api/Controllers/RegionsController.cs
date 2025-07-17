using Microsoft.AspNetCore.Mvc;
using ConstructoraClean.Api.DTOs;
using ConstructoraClean.Application.Interfaces;
using ConstructoraClean.Application.Queries;

namespace ConstructoraClean.Api.Controllers
{
    [ApiController]
    [Route("api/v1/regions")]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionOverrunService _regionOverrunService;
        public RegionsController(IRegionOverrunService regionOverrunService)
        {
            _regionOverrunService = regionOverrunService ?? throw new ArgumentNullException(nameof(regionOverrunService));
        }

        /// <summary>
        /// Obtiene los proyectos con mayor sobrecosto en una región.
        /// </summary>
        /// <param name="regionId">ID de la región</param>
        /// <param name="limit">Cantidad máxima de proyectos a retornar</param>
        /// <returns>Lista de proyectos con sobrecosto</returns>
        [HttpGet("{regionId}/top-overruns")]
        [ProducesResponseType(typeof(List<RegionOverrunDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTopOverruns(
            int regionId,
            [FromQuery] int limit = 10)
        {
            if (regionId <= 0)
                return BadRequest("El ID de región debe ser mayor que cero.");
            if (limit <= 0)
                return BadRequest("El límite debe ser mayor que cero.");
            try
            {
                var query = new GetRegionOverrunsQuery(regionId, limit);
                var result = await _regionOverrunService.GetTopOverrunsAsync(query);
                
                if (result == null || !result.Any())
                    return NotFound();
                
                // Mapear a DTO de API
                var dtos = result.Select(r => new RegionOverrunDto
                {
                    ProjectId = r.ProjectId,
                    Name = r.Name,
                    Budget = r.Budget,
                    TotalCost = r.TotalCost,
                    OverrunPct = r.OverrunPct
                }).ToList();
                
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                // Aquí podrías loggear el error con ILogger
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}
