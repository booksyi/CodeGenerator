using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CodeGenerator.Data.Models
{
    public partial class CodeGeneratorContext : DbContext
    {
        public CodeGeneratorContext()
        {
        }

        public CodeGeneratorContext(DbContextOptions<CodeGeneratorContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DbApiConstant> ApiConstants { get; set; }
        public virtual DbSet<DbCodeTemplate> CodeTemplates { get; set; }
        public virtual DbSet<DbTemplate> Templates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbApiConstant>(entity =>
            {
                entity.ToTable("CodeGeneratorApiConstants");

                entity.Property(e => e.Result).IsRequired();
            });

            modelBuilder.Entity<DbCodeTemplate>(entity =>
            {
                entity.ToTable("CodeGeneratorCodeTemplates");
            });

            modelBuilder.Entity<DbTemplate>(entity =>
            {
                entity.ToTable("CodeGeneratorTemplates");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            });
        }
    }
}
