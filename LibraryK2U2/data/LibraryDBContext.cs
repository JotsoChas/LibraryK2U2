using System;
using System.Collections.Generic;
using LibraryK2U2.models;
using Microsoft.EntityFrameworkCore;

namespace LibraryK2U2.data
{
    // EF Core DbContext for the Library database (Database First)
    public partial class LibraryDBContext : DbContext
    {
        public LibraryDBContext()
        {
        }

        public LibraryDBContext(DbContextOptions<LibraryDBContext> options)
            : base(options)
        {
        }

        // Views
        public virtual DbSet<ActiveLoan> ActiveLoans { get; set; }
        public virtual DbSet<ReturnedLoan> ReturnedLoans { get; set; }

        // Tables
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Loan> Loans { get; set; }
        public virtual DbSet<Member> Members { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        #warning Connection string should be moved out of source code for production use
            => optionsBuilder.UseSqlServer(
                "Server=(localdb)\\MSSQLLocalDB;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True"
            );

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // View: Active loans with book and member information
            modelBuilder.Entity<ActiveLoan>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("ActiveLoans");

                entity.Property(e => e.BookTitle)
                    .HasMaxLength(200);

                entity.Property(e => e.MemberName)
                    .HasMaxLength(201);
            });

            // Table: Book
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.BookId)
                    .HasName("PK__Book__3DE0C207C4F83D70");

                entity.ToTable("Book");

                entity.Property(e => e.Title)
                    .HasMaxLength(200);

                entity.Property(e => e.Author)
                    .HasMaxLength(200);

                entity.Property(e => e.Category)
                    .HasMaxLength(100);

                entity.Property(e => e.Isbn)
                    .HasMaxLength(20)
                    .HasColumnName("ISBN");
            });

            // Table: Loan with trigger on return
            modelBuilder.Entity<Loan>(entity =>
            {
                entity.HasKey(e => e.LoanId)
                    .HasName("PK__Loan__4F5AD4571D29D182");

                entity.ToTable("Loan", tb =>
                    tb.HasTrigger("trg_OnReturnBook")
                );

                // Relationship: Loan -> Book
                entity.HasOne(d => d.Book)
                    .WithMany(p => p.Loans)
                    .HasForeignKey(d => d.BookId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Loan_Book");

                // Relationship: Loan -> Member
                entity.HasOne(d => d.Member)
                    .WithMany(p => p.Loans)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Loan_Member");
            });

            // Table: Member
            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(e => e.MemberId)
                    .HasName("PK__Member__0CF04B184AC670C1");

                entity.ToTable("Member");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .HasMaxLength(100);
            });

            // View: Returned loans history
            modelBuilder.Entity<ReturnedLoan>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("ReturnedLoans");

                entity.Property(e => e.BookTitle)
                    .HasMaxLength(200);

                entity.Property(e => e.MemberName)
                    .HasMaxLength(201);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}