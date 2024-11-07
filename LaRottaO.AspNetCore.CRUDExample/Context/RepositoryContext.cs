using LaRottaO.AspNetCore.CRUDExample.Models;
using Microsoft.EntityFrameworkCore;

namespace LaRottaO.AspNetCore.CRUDExample.Context
{
    public class RepositoryContext : DbContext
    {
        public DbSet<Collaborator> CollaboratrorTable { get; set; }
        public DbSet<CollaboratorData> CollaboratrorDataTable { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(GlobalVariables.MYSQL_CONNECTION_STRING, new MySqlServerVersion(new Version(8, 0, 26)));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Collaborator>()
              .ToTable("collaborator_table")
              .HasMany(c => c.CollaboratorDataEntries)
              .WithOne()  // No need to specify Collaborator property
              .HasForeignKey(cd => cd.PassportNumber)
              .HasPrincipalKey(c => c.PassportNumber)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CollaboratorData>()
                .ToTable("collaborator_data_table");
        }
    }
}

//*****************************************************************//
// Useful commands on Packet Manager Console:

// Add-Migration -Name <Mig1>
// Remove-Migration
// Update-Database
//*****************************************************************//