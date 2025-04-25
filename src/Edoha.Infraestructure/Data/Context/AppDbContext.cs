using Edoha.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Edoha.Infraestructure.Data.Context;

public class AppDbContext: DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Institution> Institutions { get; set; }
    public DbSet<Lottery> Lotteries { get; set; }
    public DbSet<StatusTicketbook> StatusTicketbooks { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Ticketbook> Ticketbooks { get; set; }
    public DbSet<UserInstitution> UserInstitutions { get; set; }
    public DbSet<UserType> UserTypes { get; set; }
   
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserInstitution>()
            .HasKey(ui => new { ui.IdUser, ui.IdInstitution});  

        modelBuilder.Entity<UserInstitution>()
            .HasOne(ui => ui.User)
            .WithMany(u => u.UserInstitutions)
            .HasForeignKey(ui => ui.IdUser);

        modelBuilder.Entity<UserInstitution>()
            .HasOne(ui => ui.Institution)
            .WithMany(i => i.UserInstitutions)
            .HasForeignKey(ui => ui.IdUser);
    }
}
