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
    [Route("api/tratamientos")]
    // [Authorize]  // Desactivado temporalmente para permitir pruebas GET sin token
    public class TratamientoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TratamientoController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TratamientoDto>>> ObtenerTodos([FromQuery] string? busqueda = null, [FromQuery] bool? activo = null)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var query = _context.Tratamientos
                .Where(t => t.UsuarioId == usuarioId);
                
            if (!string.IsNullOrEmpty(busqueda))
            {
                query = query.Where(t => 
                    t.Nombre.Contains(busqueda) || 
                    t.Descripcion.Contains(busqueda));
            }
            
            if (activo.HasValue)
            {
                query = query.Where(t => t.Activo == activo.Value);
            }
            
            var tratamientos = await query.ToListAsync();
            return Ok(_mapper.Map<List<TratamientoDto>>(tratamientos));
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<TratamientoDto>> ObtenerPorId(int id)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tratamiento = await _context.Tratamientos
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);
                
            if (tratamiento == null)
                return NotFound(new { Mensaje = "Tratamiento no encontrado" });
                
            return Ok(_mapper.Map<TratamientoDto>(tratamiento));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<TratamientoDto>> Crear(TratamientoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tratamiento = _mapper.Map<Tratamiento>(dto);
            
            tratamiento.UsuarioId = usuarioId;
            tratamiento.FechaCreacion = DateTime.UtcNow;
            
            _context.Tratamientos.Add(tratamiento);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(ObtenerPorId), new { id = tratamiento.Id }, _mapper.Map<TratamientoDto>(tratamiento));
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, TratamientoDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { Mensaje = "Los IDs no coinciden" });
                
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tratamiento = await _context.Tratamientos
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);
                
            if (tratamiento == null)
                return NotFound(new { Mensaje = "Tratamiento no encontrado" });
                
            _mapper.Map(dto, tratamiento);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TratamientoExiste(id))
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
            var tratamiento = await _context.Tratamientos
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);
                
            if (tratamiento == null)
                return NotFound(new { Mensaje = "Tratamiento no encontrado" });
                
            // Verificar si está asociado a consultas
            var tieneConsultas = await _context.ConsultasVeterinarias
                .AnyAsync(c => c.TratamientoId == id);
                
            if (tieneConsultas)
                return BadRequest(new { Mensaje = "No se puede eliminar el tratamiento porque está asociado a consultas" });
                
            // Verificar si está asociado a planes de salud
            var tienePlanes = await _context.PlanesSalud
                .AnyAsync(p => p.TratamientoId == id);
                
            if (tienePlanes)
                return BadRequest(new { Mensaje = "No se puede eliminar el tratamiento porque está asociado a planes de salud" });
                
            _context.Tratamientos.Remove(tratamiento);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        private bool TratamientoExiste(int id)
        {
            return _context.Tratamientos.Any(t => t.Id == id);
        }
    }
}
