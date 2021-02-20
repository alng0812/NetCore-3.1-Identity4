using Microsoft.EntityFrameworkCore;
using NetCoreWebAPI.Common;

#nullable disable

namespace NetCoreWebAPI.Entity.Models
{
    public partial class blogContext : DbContext
    {
        public blogContext()
        {
        }

        public blogContext(DbContextOptions<blogContext> options)
            : base(options)
        {
        }

        public virtual DbSet<App> Apps { get; set; }
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Articletype> Articletypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                ServerVersion serverVersion = ServerVersion.AutoDetect(ConfigHelper.GetAppSettings().MysqlServerUrl);
                optionsBuilder.UseMySql(ConfigHelper.GetAppSettings().MysqlServerUrl, serverVersion);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<App>(entity =>
            {
                entity.ToTable("apps");

                entity.HasIndex(e => e.AppId, "idx_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AppId)
                    .HasColumnType("char(8)")
                    .HasColumnName("app_id")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.AppKey)
                    .HasColumnType("char(32)")
                    .HasColumnName("app_key")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("timestamp")
                    .HasColumnName("created_on")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Remark)
                    .HasColumnType("varchar(64)")
                    .HasColumnName("remark")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Article>(entity =>
            {
                entity.ToTable("article");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Content)
                    .HasColumnType("text")
                    .HasColumnName("content")
                    .HasComment("文章内容")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Createdate)
                    .HasColumnType("datetime")
                    .HasColumnName("createdate")
                    .HasComment("创建时间");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(255)")
                    .HasColumnName("description")
                    .HasComment("文章内容缩略")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.PictureUrl)
                    .HasColumnType("varchar(255)")
                    .HasColumnName("picture_url")
                    .HasComment("文章配图")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("文章状态 1显示");

                entity.Property(e => e.Title)
                    .HasColumnType("varchar(255)")
                    .HasColumnName("title")
                    .HasComment("文章标题")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Typeid)
                    .HasColumnName("typeid")
                    .HasComment("文章类型");

                entity.Property(e => e.ViewCount)
                    .HasColumnName("view_count")
                    .HasComment("浏览次数");
            });

            modelBuilder.Entity<Articletype>(entity =>
            {
                entity.ToTable("articletype");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Createdate)
                    .HasColumnType("datetime")
                    .HasColumnName("createdate");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(255)")
                    .HasColumnName("name")
                    .HasComment("类别名称")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
