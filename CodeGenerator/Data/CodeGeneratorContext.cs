using System;
using CodeGenerator.Data.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CodeGenerator.Data.Models
{
    public partial class CodeGeneratorContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().ToTable("CodeGeneratorUsers");
            modelBuilder.Entity<IdentityRole<Guid>>().ToTable("CodeGeneratorRoles");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("CodeGeneratorRoleClaims");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("CodeGeneratorUserRoles");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("CodeGeneratorUserLogins");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("CodeGeneratorUserClaims");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("CodeGeneratorUserTokens");

            modelBuilder.Entity<Constant>(entity =>
            {
                entity.ToTable("CodeGeneratorConstants");

                entity.Property(e => e.Result).IsRequired();
            });

            modelBuilder.Entity<Generator>(entity =>
            {
                entity.ToTable("CodeGeneratorGenerators");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Template>(entity =>
            {
                entity.ToTable("CodeGeneratorTemplates");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            });
        }
    }
}
