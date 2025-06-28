using Microsoft.EntityFrameworkCore;
using prototipo2.Models;
using prototipo2.Controllers;
using Microsoft.Identity.Client;
namespace prototipo2.Data
{
    public class FerreteriaContext: DbContext
    {
        public FerreteriaContext(DbContextOptions<FerreteriaContext> options): base(options)
        {
           
        }
        public DbSet<Empleado> Empleado { get; set; }
        public DbSet<MovimientoFinanciero> Finanza { get; set; }
        public DbSet<Pedido> Pedido { get; set; }
        public DbSet<Producto> Producto { get; set; }
        public DbSet<Reparacion> Reparacion { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Venta> Venta { get; set; }
        public DbSet<ItemVendido> ItemsVendidos { get; set; }
        public DbSet<MetodoPago> Pagos { get; set; }
        public DbSet<Devolucion> Devoluciones { get; set; }
        public DbSet<NotaCredito> NotaCredito { get; set; }
        public DbSet<ItemDevuelto> ItemsDevueltos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ItemVendido>().ToTable("ItemsVendidos");
            modelBuilder.Entity<MetodoPago>().ToTable("MetodosPago");
            modelBuilder.Entity<Devolucion>().ToTable("Devoluciones");
            modelBuilder.Entity<NotaCredito>().ToTable("NotaCredito");
            modelBuilder.Entity<ItemDevuelto>().ToTable("ItemsDevueltos");
            modelBuilder.Entity<ItemVendido>()
        .HasOne(iv => iv.Venta)
        .WithMany(v => v.Productos)
        .HasForeignKey(iv => iv.VentaId);

            modelBuilder.Entity<MetodoPago>()
                .HasOne(mp => mp.Venta)
                .WithMany(v => v.Pagos)
                .HasForeignKey(mp => mp.VentaId);

            modelBuilder.Entity<Venta>().ToTable("Venta");
        }
    }
}
