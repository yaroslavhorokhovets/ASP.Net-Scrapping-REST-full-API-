using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawyerScrapper.Model
{
    public class LawyerDbContext : DbContext
    {     

        public DbSet<CourtCaseAgenda> CourtCaseAgendas { get; set; }
        public DbSet<Lawyer> Lawyers { get; set; }
        public DbSet<Court> Courts { get; set; }
        public LawyerDbContext()
        {
        }

        public LawyerDbContext(DbContextOptions<LawyerDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CourtCaseAgenda>(entity =>
            {
                entity.HasKey(e => e.ID);
            });

            modelBuilder.Entity<Lawyer>(entity =>
            {
                entity.HasKey(e => e.ID);
            });

            modelBuilder.Entity<Court>(entity =>
            {
                entity.HasKey(e => e.ID);
            });

        }
    }
}
