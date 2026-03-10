using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace backend.Models;

public partial class SpovedContext : DbContext
{
    public SpovedContext()
    {
    }

    public SpovedContext(DbContextOptions<SpovedContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Fruit> Fruits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Fruit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("fruits_pkey");

            entity.ToTable("fruits");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Sweetness).HasColumnName("sweetness");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
