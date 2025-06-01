using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back.Data;
using back.Models;
using back.DTOs;
using AutoMapper;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace back.Controllers
{
    [ApiController]
    [Route("api/planes")]
    // [Authorize] // Desactivado temporalmente para pruebas sin token
    public class PlanSaludController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PlanSaludController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanSaludDto>>> ObtenerTodos(
            [FromQuery] string? busqueda = null,
            [FromQuery] bool? activo = null)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var query = _context.PlanesSalud
                .Include(p => p.Tratamiento)
                .Where(p => p.UsuarioId == usuarioId);
                
            if (!string.IsNullOrEmpty(busqueda))
            {
                query = query.Where(p => 
                    p.Nombre.Contains(busqueda) || 
                    p.Descripcion.Contains(busqueda));
            }
            
            if (activo.HasValue)
            {
                query = query.Where(p => p.Activo == activo.Value);
            }
            
            var planes = await query.ToListAsync();
            var dtos = _mapper.Map<List<PlanSaludDto>>(planes);
            
            // Agregar nombre del tratamiento para UI
            foreach (var dto in dtos)
            {
                var plan = planes.First(p => p.Id == dto.Id);
                dto.NombreTratamiento = plan.Tratamiento?.Nombre;
            }
            
            return Ok(dtos);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<PlanSaludDto>> ObtenerPorId(int id)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var plan = await _context.PlanesSalud
                .Include(p => p.Tratamiento)
                .FirstOrDefaultAsync(p => p.Id == id && p.UsuarioId == usuarioId);
                
            if (plan == null)
                return NotFound(new { Mensaje = "Plan de salud no encontrado" });
                
            var dto = _mapper.Map<PlanSaludDto>(plan);
            dto.NombreTratamiento = plan.Tratamiento?.Nombre;
                
            return Ok(dto);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<PlanSaludDto>> Crear(PlanSaludDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            // Verificar que el tratamiento existe (sin filtrar por usuario)
            var tratamiento = await _context.Tratamientos
                .FirstOrDefaultAsync(t => t.Id == dto.TratamientoId);
                
            if (tratamiento == null)
                return BadRequest(new { Mensaje = "El tratamiento especificado no existe" });
                
            // Verificar fechas
            if (dto.FechaInicio >= dto.FechaFin)
                return BadRequest(new { Mensaje = "La fecha de inicio debe ser anterior a la fecha de fin" });
                
            // Mapear plan y asignar usuario si hay
            var plan = _mapper.Map<PlanSalud>(dto);
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            plan.UsuarioId = usuarioId;
            
            plan.FechaInicio = DateTime.SpecifyKind(plan.FechaInicio, DateTimeKind.Utc);
            plan.FechaFin = DateTime.SpecifyKind(plan.FechaFin, DateTimeKind.Utc);

            _context.PlanesSalud.Add(plan);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Loguear error completo
                Console.Error.WriteLine($"Error al crear PlanSalud: {ex}");
                var detalle = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { mensaje = "Error al crear plan de salud", detalle });
            }
            
            // Cargar relaciones para devolver datos completos
            await _context.Entry(plan).Reference(p => p.Tratamiento).LoadAsync();
            
            var resultado = _mapper.Map<PlanSaludDto>(plan);
            resultado.NombreTratamiento = plan.Tratamiento?.Nombre;
            
            return CreatedAtAction(
                nameof(ObtenerPorId), 
                new { id = plan.Id }, 
                resultado);
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, PlanSaludDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { Mensaje = "Los IDs no coinciden" });
                
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Verificar existencia del plan
            var plan = await _context.PlanesSalud
                .FirstOrDefaultAsync(p => p.Id == id && p.UsuarioId == usuarioId);
                
            if (plan == null)
                return NotFound(new { Mensaje = "Plan de salud no encontrado" });
                
            // Verificar que el tratamiento existe
            var tratamiento = await _context.Tratamientos
                .FirstOrDefaultAsync(t => t.Id == dto.TratamientoId && t.UsuarioId == usuarioId);
                
            if (tratamiento == null)
                return BadRequest(new { Mensaje = "El tratamiento especificado no existe" });
                
            // Verificar fechas
            if (dto.FechaInicio >= dto.FechaFin)
                return BadRequest(new { Mensaje = "La fecha de inicio debe ser anterior a la fecha de fin" });
                
            _mapper.Map(dto, plan);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlanSaludExiste(id))
                    return NotFound();
                throw;
            }
            
            return NoContent();
        }

        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var plan = await _context.PlanesSalud
                .FirstOrDefaultAsync(p => p.Id == id && p.UsuarioId == usuarioId);
                
            if (plan == null)
                return NotFound(new { Mensaje = "Plan de salud no encontrado" });
                
            _context.PlanesSalud.Remove(plan);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        private bool PlanSaludExiste(int id)
        {
            return _context.PlanesSalud.Any(p => p.Id == id);
        }

        // Endpoint de prueba para POST (no persiste en BD)
        [AllowAnonymous]
        [HttpPost("test-create")]
        public ActionResult<object> TestCreate([FromBody] PlanSaludDto dto)
        {
            Console.WriteLine("TestCreate PlanSalud: " + System.Text.Json.JsonSerializer.Serialize(dto));
            return Ok(new { mensaje = "Prueba de creación exitosa", plan = dto });
        }
    }
}
