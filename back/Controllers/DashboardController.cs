using back.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace back.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalEspecies = await _context.EspeciesAnimales.CountAsync();
            var totalTratamientos = await _context.Tratamientos.CountAsync();
            var totalConsultas = await _context.ConsultasVeterinarias.CountAsync();
            var totalPlanes = await _context.PlanesSalud.CountAsync();

            return Ok(new {
                totalEspecies,
                totalTratamientos,
                totalConsultas,
                totalPlanes
            });
        }
    }
}
