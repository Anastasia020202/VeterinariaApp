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
using Microsoft.AspNetCore.Authorization;

namespace back.Controllers
{
    [ApiController]
    [Route("api/consultas")]
    public class ConsultaVeterinariaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ConsultaVeterinariaController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConsultaVeterinariaDto>>> ObtenerTodas(
            [FromQuery] string? busqueda = null,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null,
            [FromQuery] int? especieId = null)
        {
            var query = _context.ConsultasVeterinarias
                .Include(c => c.EspecieAnimal)
                .Include(c => c.Tratamiento)
                .AsQueryable();

            if (!string.IsNullOrEmpty(busqueda))
            {
                query = query.Where(c =>
                    c.NombreMascota.Contains(busqueda) ||
                    c.NombrePropietario.Contains(busqueda) ||
                    c.Descripcion.Contains(busqueda));
            }

            if (fechaInicio.HasValue)
            {
                query = query.Where(c => c.FechaConsulta >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                query = query.Where(c => c.FechaConsulta <= fechaFin.Value);
            }

            if (especieId.HasValue)
            {
                query = query.Where(c => c.EspecieAnimalId == especieId.Value);
            }

            var consultas = await query.ToListAsync();
            var dtos = _mapper.Map<List<ConsultaVeterinariaDto>>(consultas);

            foreach (var dto in dtos)
            {
                var consulta = consultas.First(c => c.Id == dto.Id);
                dto.NombreEspecieAnimal = consulta.EspecieAnimal?.Nombre;
                dto.NombreTratamiento = consulta.Tratamiento?.Nombre;
            }

            return Ok(dtos);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ConsultaVeterinariaDto>> ObtenerPorId(int id)
        {
            var consulta = await _context.ConsultasVeterinarias
                .Include(c => c.EspecieAnimal)
                .Include(c => c.Tratamiento)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (consulta == null)
                return NotFound(new { mensaje = "Consulta veterinaria no encontrada" });

            var dto = _mapper.Map<ConsultaVeterinariaDto>(consulta);
            dto.NombreEspecieAnimal = consulta.EspecieAnimal?.Nombre;
            dto.NombreTratamiento = consulta.Tratamiento?.Nombre;

            return Ok(dto);
        }

[AllowAnonymous]
[HttpPost]
public async Task<ActionResult<ConsultaVeterinariaDto>> Crear(ConsultaVeterinariaDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var especieAnimal = await _context.EspeciesAnimales
        .FirstOrDefaultAsync(e => e.Id == dto.EspecieAnimalId);
    if (especieAnimal == null)
        return BadRequest(new { mensaje = "La especie animal especificada no existe" });

    var tratamiento = await _context.Tratamientos
        .FirstOrDefaultAsync(t => t.Id == dto.TratamientoId);
    if (tratamiento == null)
        return BadRequest(new { mensaje = "El tratamiento especificado no existe" });

    var consulta = new ConsultaVeterinaria
    {
        EspecieAnimalId = dto.EspecieAnimalId,
        TratamientoId = dto.TratamientoId,
        NombreMascota = dto.NombreMascota,
        NombrePropietario = dto.NombrePropietario,
        InformacionContacto = dto.InformacionContacto,
        Costo = dto.Costo,
        Descripcion = dto.Descripcion,
        NotasTratamiento = dto.NotasTratamiento,
        Prescripcion = dto.Prescripcion,
        FechaConsulta = dto.FechaConsulta,
    };

    var defaultUser = await _context.Users.FirstOrDefaultAsync();
    consulta.UsuarioId = defaultUser?.Id ?? throw new Exception("No se encontró usuario admin");

    _context.ConsultasVeterinarias.Add(consulta);

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error al guardar:");
        Console.WriteLine(ex.Message);
        if (ex.InnerException != null)
            Console.WriteLine($"➡️ Inner: {ex.InnerException.Message}");
        return StatusCode(500, new { mensaje = "Error interno al guardar", detalle = ex.Message });
    }

    await _context.Entry(consulta).Reference(c => c.EspecieAnimal).LoadAsync();
    await _context.Entry(consulta).Reference(c => c.Tratamiento).LoadAsync();

    var resultado = _mapper.Map<ConsultaVeterinariaDto>(consulta);
    resultado.NombreEspecieAnimal = consulta.EspecieAnimal?.Nombre;
    resultado.NombreTratamiento = consulta.Tratamiento?.Nombre;

    return CreatedAtAction(nameof(ObtenerPorId), new { id = consulta.Id }, resultado);
}


        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, ConsultaVeterinariaDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { mensaje = "Los IDs no coinciden" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var consulta = await _context.ConsultasVeterinarias
                .FirstOrDefaultAsync(c => c.Id == id);
            if (consulta == null)
                return NotFound(new { mensaje = "Consulta veterinaria no encontrada" });

            var especieAnimal = await _context.EspeciesAnimales
                .FirstOrDefaultAsync(e => e.Id == dto.EspecieAnimalId);
            if (especieAnimal == null)
                return BadRequest(new { mensaje = "La especie animal especificada no existe" });

            var tratamiento = await _context.Tratamientos
                .FirstOrDefaultAsync(t => t.Id == dto.TratamientoId);
            if (tratamiento == null)
                return BadRequest(new { mensaje = "El tratamiento especificado no existe" });

            _mapper.Map(dto, consulta);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConsultaVeterinariaExiste(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var consulta = await _context.ConsultasVeterinarias
                .FirstOrDefaultAsync(c => c.Id == id);
            if (consulta == null)
                return NotFound(new { mensaje = "Consulta veterinaria no encontrada" });

            _context.ConsultasVeterinarias.Remove(consulta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ConsultaVeterinariaExiste(int id)
        {
            return _context.ConsultasVeterinarias.Any(c => c.Id == id);
        }
    }
}
