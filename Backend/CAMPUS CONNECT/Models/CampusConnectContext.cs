using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CAMPUS_CONNECT.Models;

public partial class CampusConnectContext : DbContext
{
    public CampusConnectContext()
    {
    }

    public CampusConnectContext(DbContextOptions<CampusConnectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BuySell> BuySells { get; set; }

    public virtual DbSet<LostFound> LostFounds { get; set; }

    public virtual DbSet<complaint_suggestion> Reports { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        { 
        
        }
    }
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        //=> optionsBuilder.UseSqlServer("server=localhost\\SQLEXPRESS; database=CAMPUS_CONNECT; trusted_connection=true; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BuySell>(entity =>
        {
            entity.HasKey(e => e.product_id);

            entity.ToTable("buy_sell");

            entity.Property(e => e.product_id)
                .ValueGeneratedNever()
                .HasColumnName("product_id");
            entity.Property(e => e.description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.email)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.image_path)
                .IsUnicode(false)
                .HasColumnName("image_path");
            entity.Property(e => e.item_title)
                .IsUnicode(false)
                .HasColumnName("item_title");
            entity.Property(e => e.mobile_no)
                .IsUnicode(false)
                .HasColumnName("mobile_no");
            entity.Property(e => e.name)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.price)
                .HasColumnType("decimal(5, 0)")
                .HasColumnName("price");
        });

        modelBuilder.Entity<LostFound>(entity =>
        {
            entity.HasKey(e => e.item_id);

            entity.ToTable("lost_found");

            entity.Property(e => e.item_id)
                .ValueGeneratedNever()
                .HasColumnName("item_id");
            entity.Property(e => e.category)
                .IsUnicode(false)
                .HasColumnName("category");
            entity.Property(e => e.contact_information)
                .IsUnicode(false)
                .HasColumnName("contact_information");
            entity.Property(e => e.datetime)
                .HasColumnType("datetime")
                .HasColumnName("datetime");
            entity.Property(e => e.description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.image_path)
                .IsUnicode(false)
                .HasColumnName("image_path");
            entity.Property(e => e.item_name)
                .IsUnicode(false)
                .HasColumnName("item_name");
            entity.Property(e => e.location)
                .IsUnicode(false)
                .HasColumnName("location");
            entity.Property(e => e.status)
                .IsUnicode(false)
                .HasColumnName("status");
        });

        modelBuilder.Entity<complaint_suggestion>(entity =>
        {
            entity.ToTable("report");

            entity.Property(e => e.report_id)
                .ValueGeneratedNever()
                .HasColumnName("report_id");
            entity.Property(e => e.contact_email)
                .IsUnicode(false)
                .HasColumnName("contact_email");
            entity.Property(e => e.description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.image_path)
                .IsUnicode(false)
                .HasColumnName("image_path");
            entity.Property(e => e.location)
                .IsUnicode(false)
                .HasColumnName("location");
            entity.Property(e => e.status)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.response_type)
                .IsUnicode(false)
                .HasColumnName("response_type");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.user_name);

            entity.ToTable("user");

            entity.Property(e => e.user_name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_name");
            entity.Property(e => e.password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.role)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
