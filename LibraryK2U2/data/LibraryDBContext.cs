using System;
using System.Collections.Generic;
using LibraryK2U2.models;
using Microsoft.EntityFrameworkCore;

namespace LibraryK2U2.data;

public partial class LibraryDBContext : DbContext
{
    public LibraryDBContext()
    {
    }

    public LibraryDBContext(DbContextOptions<LibraryDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActiveLoan> ActiveLoans { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Loan> Loans { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<ReturnedLoan> ReturnedLoans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActiveLoan>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ActiveLoans");

            entity.Property(e => e.BookTitle).HasMaxLength(200);
            entity.Property(e => e.MemberName).HasMaxLength(201);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Book__3DE0C207C4F83D70");

            entity.ToTable("Book");

            entity.Property(e => e.Author).HasMaxLength(200);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Isbn)
                .HasMaxLength(20)
                .HasColumnName("ISBN");
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.LoanId).HasName("PK__Loan__4F5AD4571D29D182");

            entity.ToTable("Loan", tb => tb.HasTrigger("trg_OnReturnBook"));

            entity.HasOne(d => d.Book).WithMany(p => p.Loans)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Loan_Book");

            entity.HasOne(d => d.Member).WithMany(p => p.Loans)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Loan_Member");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PK__Member__0CF04B184AC670C1");

            entity.ToTable("Member");

            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
        });

        modelBuilder.Entity<ReturnedLoan>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ReturnedLoans");

            entity.Property(e => e.BookTitle).HasMaxLength(200);
            entity.Property(e => e.MemberName).HasMaxLength(201);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
