﻿using Microsoft.EntityFrameworkCore;
using BlobStorage.WebAPI.Entities;

namespace BlobStorage.WebAPI.Contexts
{
    public partial class BlobStorageContext : DbContext
    {
        public BlobStorageContext()
        {
        }

        public BlobStorageContext(DbContextOptions<BlobStorageContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Car> Cars { get; set; } = null!;
        public virtual DbSet<Blob> Blobs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost;Database=DB_Leame_Advance;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>(entity =>
            {
                entity.Property(e => e.Brand)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ImagePath)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Model)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TechnicalDataSheetPath)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ImageGuid)
                   .HasMaxLength(200)
                   .IsUnicode(false);
            });

            modelBuilder.Entity<Blob>(entity =>
            {

                entity.Property(e => e.ImagePath)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ImageGuid)
                   .HasMaxLength(200)
                   .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
