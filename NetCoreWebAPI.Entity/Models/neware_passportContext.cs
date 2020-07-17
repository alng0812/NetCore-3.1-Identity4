using System;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace NetCoreWebAPI.Entity.Models
{
    public partial class neware_passportContext : DbContext
    {
        public neware_passportContext()
        {
        }

        public neware_passportContext(DbContextOptions<neware_passportContext> options)
            : base(options)
        {

        }

        public virtual DbSet<AccountOauthMap> AccountOauthMap { get; set; }
        public virtual DbSet<Accounts> Accounts { get; set; }
        public virtual DbSet<Apps> Apps { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=192.168.1.100;userid=root;pwd=123456;port=3307;database=neware_passport;sslmode=none", x => x.ServerVersion("5.7.23-mysql"));
                //optionsBuilder.UseMySql(Configuration.GetConnectionString("DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountOauthMap>(entity =>
            {
                entity.ToTable("account_oauth_map");

                entity.HasComment("第三方登陆账号信息表");

                entity.HasIndex(e => e.PassportId)
                    .HasName("idx_user_id");

                entity.HasIndex(e => new { e.Openid, e.Unionid })
                    .HasName("idx_unionid");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("created_on")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.OpenType)
                    .HasColumnName("open_type")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'1'")
                    .HasComment("开放平台(1-weixin,2-微信小程序,3-qq,4-facebook,5-twitter)");

                entity.Property(e => e.Openid)
                    .IsRequired()
                    .HasColumnName("openid")
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.PassportId)
                    .HasColumnName("passport_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Unionid)
                    .IsRequired()
                    .HasColumnName("unionid")
                    .HasColumnType("varchar(32)")
                    .HasComment("union_id")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Accounts>(entity =>
            {
                entity.HasKey(e => e.PassportId)
                    .HasName("PRIMARY");

                entity.ToTable("accounts");

                entity.HasIndex(e => e.Email)
                    .HasName("idx_email");

                entity.HasIndex(e => e.Mobile)
                    .HasName("idx_mobi");

                entity.Property(e => e.PassportId)
                    .HasColumnName("passport_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Account)
                    .HasColumnName("account")
                    .HasColumnType("varchar(32)")
                    .HasComment("账号")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)")
                    .HasComment("创建来源app_id");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("created_on")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasColumnType("varchar(32)")
                    .HasComment("email")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.EmailVerify)
                    .HasColumnName("email_verify")
                    .HasColumnType("tinyint(4)")
                    .HasComment("email验证状态(0:未验证,1:已验证)");

                entity.Property(e => e.Gender)
                    .HasColumnName("gender")
                    .HasColumnType("tinyint(4)")
                    .HasComment("性别(0:未知,1:男,2:女)");

                entity.Property(e => e.HeadImage)
                    .HasColumnName("head_image")
                    .HasColumnType("varchar(255)")
                    .HasComment("头像图片")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasColumnType("varchar(16)")
                    .HasComment("手机")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Nickname)
                    .HasColumnName("nickname")
                    .HasColumnType("varchar(32)")
                    .HasComment("昵称")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("varchar(32)")
                    .HasComment("密码")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Realname)
                    .HasColumnName("realname")
                    .HasColumnType("varchar(32)")
                    .HasComment("实名")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'1'")
                    .HasComment("状态(0:删除,1:启用,2:停用)");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)")
                    .HasComment("兼容各APP已有用户ID");
            });

            modelBuilder.Entity<Apps>(entity =>
            {
                entity.ToTable("apps");

                entity.HasIndex(e => e.AppId)
                    .HasName("idx_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

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
