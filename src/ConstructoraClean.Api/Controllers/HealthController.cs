using Microsoft.AspNetCore.Mvc;
using ConstructoraClean.Api.DTOs;
using ConstructoraClean.Infrastructure.Data;
using System.Data;

namespace ConstructoraClean.Api.Controllers
{
    [ApiController]
    [Route("api/v1/health")]
    public class HealthController : ControllerBase
    {
        private readonly DapperContext _context;
        
        public HealthController(DapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        /// <summary>
        /// Verifica el estado de salud de la API.
        /// </summary>
        /// <returns>OK si la API está activa.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(HealthResponse), 200)]
        public ActionResult<object> Get()
        {
            var apiStatus = "OK";
            var dbStatus = "OK";
            try
            {
                using var conn = _context.CreateConnection();
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteScalar();
            }
            catch
            {
                dbStatus = "FAIL";
            }
            return Ok(new {
                ApiStatus = apiStatus,
                DbStatus = dbStatus,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
