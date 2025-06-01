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
using System;

namespace back.Controllers
{
    [ApiController]
    [Route("api/especies")]
    // Autorizar desactivado para pruebas
    public class EspecieAnimalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public EspecieAnimalController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Endpoint de prueba sin autenticación para verificar que el API funciona
        [HttpGet("test")]
        [AllowAnonymous]
        public ActionResult<string> Test()
        {
            Console.WriteLine("Endpoint de prueba sin autenticación llamado exitosamente");
            return Ok(new { mensaje = "API de especies está funcionando correctamente", hora = DateTime.Now });
        }
        
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EspecieAnimalDto>>> ObtenerTodos([FromQuery] string? busqueda = null)
        {
            // Obtener todas las especies sin filtrar por usuario
            var query = _context.EspeciesAnimales.AsQueryable();
            
            if (!string.IsNullOrEmpty(busqueda))
            {
                query = query.Where(e => e.Nombre.Contains(busqueda) || e.Descripcion.Contains(busqueda));
            }
            
            var especies = await query.ToListAsync();
            
            var dtos = _mapper.Map<List<EspecieAnimalDto>>(especies);
            
            // Contar consultas para cada especie (sin filtrar por usuario)
            foreach (var dto in dtos)
            {
                dto.NumeroConsultas = await _context.ConsultasVeterinarias
                    .CountAsync(c => c.EspecieAnimalId == dto.Id);
            }
            
            return Ok(dtos);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<EspecieAnimalDto>> ObtenerPorId(int id)
        {
            // Obtener la especie por ID sin filtrar por usuario
            var especie = await _context.EspeciesAnimales
                .FirstOrDefaultAsync(e => e.Id == id);
                
            if (especie == null)
                return NotFound(new { Mensaje = "Especie animal no encontrada" });
                
            var dto = _mapper.Map<EspecieAnimalDto>(especie);
            
            dto.NumeroConsultas = await _context.ConsultasVeterinarias
                .CountAsync(c => c.EspecieAnimalId == dto.Id);
                
            return Ok(dto);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<EspecieAnimalDto>> Crear(EspecieAnimalDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            // Mapear nueva especie y asignar usuario admin
            var especie = _mapper.Map<EspecieAnimal>(dto);
            var defaultUser = await _context.Users.FirstOrDefaultAsync();
            especie.UsuarioId = defaultUser?.Id ?? throw new Exception("No se encontró usuario admin");
            especie.FechaCreacion = DateTime.UtcNow;
            
            try
            {
                _context.EspeciesAnimales.Add(especie);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Retornar detalle de error para depuración
                return StatusCode(500, new { mensaje = "Error al crear especie", detalle = ex.Message });
            }
            
            dto = _mapper.Map<EspecieAnimalDto>(especie);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = especie.Id }, dto);
        }

        // Endpoint de prueba para POST (no interactúa con la base de datos)
        [HttpPost("test-create")]
        [AllowAnonymous]
        public ActionResult<object> TestCreate([FromBody] EspecieAnimalDto dto)
        {
            // Devolver el DTO recibido como confirmación
            return Ok(new { mensaje = "Prueba de creación exitosa", especie = dto });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, EspecieAnimalDto dto)
        {
            // Asegurar que dto.Id coincide con la ruta
            dto.Id = id;
            // Omitir validación de ModelState para pruebas
            
            var especie = await _context.EspeciesAnimales
                .FirstOrDefaultAsync(e => e.Id == id);
                
            if (especie == null)
                return NotFound(new { Mensaje = "Especie animal no encontrada" });
                
            _mapper.Map(dto, especie);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EspecieAnimalExiste(id))
                    return NotFound();
                throw;
            }
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var especie = await _context.EspeciesAnimales
                .FirstOrDefaultAsync(e => e.Id == id);
                
            if (especie == null)
                return NotFound(new { Mensaje = "Especie animal no encontrada" });
                
            // Verificar si hay consultas asociadas
            var tieneConsultas = await _context.ConsultasVeterinarias
                .AnyAsync(c => c.EspecieAnimalId == id);
                
            if (tieneConsultas)
                return BadRequest(new { Mensaje = "No se puede eliminar la especie porque tiene consultas asociadas" });
                
            _context.EspeciesAnimales.Remove(especie);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        private bool EspecieAnimalExiste(int id)
        {
            return _context.EspeciesAnimales.Any(e => e.Id == id);
        }
    }
}
