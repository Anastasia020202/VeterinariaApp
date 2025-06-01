using Microsoft.EntityFrameworkCore;
using back.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace back.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Entidades de la clínica veterinaria
        public DbSet<EspecieAnimal> EspeciesAnimales { get; set; } = null!;
        public DbSet<Tratamiento> Tratamientos { get; set; } = null!;
        public DbSet<ConsultaVeterinaria> ConsultasVeterinarias { get; set; } = null!;
        public DbSet<PlanSalud> PlanesSalud { get; set; } = null!;
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuración de EspecieAnimal
            modelBuilder.Entity<EspecieAnimal>()
                .HasMany(e => e.Consultas)
                .WithOne(c => c.EspecieAnimal)
                .HasForeignKey(c => c.EspecieAnimalId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<EspecieAnimal>()
                .HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
                
            modelBuilder.Entity<EspecieAnimal>()
                .Property(e => e.UsuarioId)
                .IsRequired(false);
                
            // Configuración de Tratamiento
            modelBuilder.Entity<Tratamiento>()
                .HasMany(t => t.Consultas)
                .WithOne(c => c.Tratamiento)
                .HasForeignKey(c => c.TratamientoId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<Tratamiento>()
                .HasMany(t => t.PlanesSalud)
                .WithOne(p => p.Tratamiento)
                .HasForeignKey(p => p.TratamientoId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<Tratamiento>()
                .HasOne(t => t.Usuario)
                .WithMany()
                .HasForeignKey(t => t.UsuarioId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
                
            modelBuilder.Entity<Tratamiento>()
                .Property(t => t.UsuarioId)
                .IsRequired(false);
                
            // Configuración de propiedades decimales para evitar problemas con precisión
            modelBuilder.Entity<Tratamiento>()
                .Property(t => t.CostoEstandar)
                .HasColumnType("decimal(18,2)");
                
            modelBuilder.Entity<ConsultaVeterinaria>()
                .Property(c => c.Costo)
                .HasColumnType("decimal(18,2)");
                
            modelBuilder.Entity<PlanSalud>()
                .Property(p => p.Costo)
                .HasColumnType("decimal(18,2)");
                
            modelBuilder.Entity<PlanSalud>()
                .Property(p => p.PorcentajeDescuento)
                .HasColumnType("decimal(5,2)");
                
            modelBuilder.Entity<PlanSalud>()
                .HasOne(p => p.Usuario)
                .WithMany()
                .HasForeignKey(p => p.UsuarioId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<PlanSalud>()
                .Property(p => p.UsuarioId)
                .IsRequired(false);
        }
    }
}
