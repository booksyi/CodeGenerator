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

        public virtual DbSet<Constant> Constants { get; set; }
        public virtual DbSet<Generator> Generators { get; set; }
        public virtual DbSet<Template> Templates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Constant>(entity =>
            {
                entity.ToTable("CodeGeneratorConstants");

                entity.Property(e => e.Result).IsRequired();
            });

            modelBuilder.Entity<Generator>(entity =>
            {
                entity.ToTable("CodeGeneratorGenerators");
            });

            modelBuilder.Entity<Template>(entity =>
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
