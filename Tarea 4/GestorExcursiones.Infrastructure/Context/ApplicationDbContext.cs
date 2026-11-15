using GestorExcursiones.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace GestorExcursiones.Infrastructure.Context;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):DbContext(options){
 public DbSet<Cliente> Clientes=>Set<Cliente>(); public DbSet<Excursion> Excursiones=>Set<Excursion>(); public DbSet<Guia> Guias=>Set<Guia>(); public DbSet<Reserva> Reservas=>Set<Reserva>();
 protected override void OnModelCreating(ModelBuilder modelBuilder){base.OnModelCreating(modelBuilder);
 modelBuilder.Entity<Cliente>().HasIndex(x=>x.DocumentoIdentificacion).IsUnique();
 modelBuilder.Entity<Reserva>().HasOne(x=>x.Cliente).WithMany().HasForeignKey(x=>x.ClienteId).OnDelete(DeleteBehavior.Restrict);
 modelBuilder.Entity<Reserva>().HasOne(x=>x.Excursion).WithMany(x=>x.Reservas).HasForeignKey(x=>x.ExcursionId).OnDelete(DeleteBehavior.Restrict);
 modelBuilder.Entity<Excursion>().Property(x=>x.Precio).HasPrecision(10,2);modelBuilder.Entity<Reserva>().Property(x=>x.MontoTotal).HasPrecision(10,2);modelBuilder.Entity<Reserva>().Property(x=>x.MontoPagado).HasPrecision(10,2);
 }}
