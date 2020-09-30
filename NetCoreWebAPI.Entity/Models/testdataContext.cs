using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NetCoreWebAPI.Entity.Models
{
    public partial class testdataContext : DbContext
    {
        public testdataContext()
        {
        }

        public testdataContext(DbContextOptions<testdataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Apps> Apps { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=localhost;userid=root;pwd=123456;port=3306;database=testdata;sslmode=none", x => x.ServerVersion("8.0.20-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Apps>(entity =>
            {
                entity.ToTable("apps");

                entity.HasIndex(e => e.AppId)
                    .HasName("idx_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AppId)
                    .HasColumnName("app_id")
                    .HasColumnType("char(8)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.AppKey)
                    .HasColumnName("app_key")
                    .HasColumnType("char(32)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("created_on")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Remark)
                    .HasColumnName("remark")
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
