using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebApplication2.Models
{
    public partial class DBaseContext : DbContext
    {
        public DBaseContext()
        {
        }

        public DBaseContext(DbContextOptions<DBaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Budget> Budgets { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<FinishedProduct> FinishedProducts { get; set; } = null!;
        public virtual DbSet<Ingredient> Ingredients { get; set; } = null!;
        public virtual DbSet<Month> Months { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<Production> Productions { get; set; } = null!;
        public virtual DbSet<PurchaseOfRawMaterial> PurchaseOfRawMaterials { get; set; } = null!;
        public virtual DbSet<RawMaterial> RawMaterials { get; set; } = null!;
        public virtual DbSet<ReportOfProduction> ReportOfProductions { get; set; } = null!;
        public virtual DbSet<ReportOfSalesOfProduct> ReportOfSalesOfProducts { get; set; } = null!;
        public virtual DbSet<ReportPurchaseOfRawMaterial> ReportPurchaseOfRawMaterials { get; set; } = null!;
        public virtual DbSet<Salary> Salaries { get; set; } = null!;
        public virtual DbSet<SaleOfProduct> SaleOfProducts { get; set; } = null!;
        public virtual DbSet<Unit> Units { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<ViewEmployee> ViewEmployees { get; set; } = null!;
        public virtual DbSet<ViewEmployee1> ViewEmployees1 { get; set; } = null!;
        public virtual DbSet<ViewIngridient> ViewIngridients { get; set; } = null!;
        public virtual DbSet<ViewRawMaterial> ViewRawMaterials { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=DBase;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Budget>(entity =>
            {
                entity.ToTable("Budget");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Bonus).HasColumnName("bonus");

                entity.Property(e => e.PercentageOfPremium).HasColumnName("percentageOfPremium");

                entity.Property(e => e.SumOfBudget)
                    .HasColumnType("money")
                    .HasColumnName("sumOfBudget");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Address)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("address");

                entity.Property(e => e.Names)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("names");

                entity.Property(e => e.Phone).HasColumnName("phone");

                entity.Property(e => e.Position).HasColumnName("position");

                entity.Property(e => e.Salary)
                    .HasColumnType("money")
                    .HasColumnName("salary");

                entity.HasOne(d => d.PositionNavigation)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.Position)
                    .HasConstraintName("FK__Employees__posit__36B12243");
            });

            modelBuilder.Entity<FinishedProduct>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("amount");

                entity.Property(e => e.Summa)
                    .HasColumnType("money")
                    .HasColumnName("summa");

                entity.Property(e => e.Title)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("title");

                entity.Property(e => e.Unit).HasColumnName("unit");

                entity.HasOne(d => d.UnitNavigation)
                    .WithMany(p => p.FinishedProducts)
                    .HasForeignKey(d => d.Unit)
                    .HasConstraintName("FK_FinishedProducts_Units");
            });

            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("amount");

                entity.Property(e => e.Product).HasColumnName("product");

                entity.Property(e => e.RawMaterial).HasColumnName("rawMaterial");

                entity.HasOne(d => d.ProductNavigation)
                    .WithMany(p => p.Ingredients)
                    .HasForeignKey(d => d.Product)
                    .HasConstraintName("FK_Ingredients_FinishedProducts");

                entity.HasOne(d => d.RawMaterialNavigation)
                    .WithMany(p => p.Ingredients)
                    .HasForeignKey(d => d.RawMaterial)
                    .HasConstraintName("FK_Ingredients_RawMaterials");
            });

            modelBuilder.Entity<Month>(entity =>
            {
                entity.ToTable("Month");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Month1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("month");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Position)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("position");
            });

            modelBuilder.Entity<Production>(entity =>
            {
                entity.ToTable("Production");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("amount");

                entity.Property(e => e.Datee)
                    .HasColumnType("date")
                    .HasColumnName("datee");

                entity.Property(e => e.Employee).HasColumnName("employee");

                entity.Property(e => e.Product).HasColumnName("product");

                entity.HasOne(d => d.EmployeeNavigation)
                    .WithMany(p => p.Productions)
                    .HasForeignKey(d => d.Employee)
                    .HasConstraintName("FK__Productio__emplo__4222D4EF");

                entity.HasOne(d => d.ProductNavigation)
                    .WithMany(p => p.Productions)
                    .HasForeignKey(d => d.Product)
                    .HasConstraintName("FK__Productio__produ__412EB0B6");
            });

            modelBuilder.Entity<PurchaseOfRawMaterial>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("amount");

                entity.Property(e => e.Datee)
                    .HasColumnType("date")
                    .HasColumnName("datee");

                entity.Property(e => e.Employee).HasColumnName("employee");

                entity.Property(e => e.RawMaterial).HasColumnName("rawMaterial");

                entity.Property(e => e.Summa)
                    .HasColumnType("money")
                    .HasColumnName("summa");

                entity.HasOne(d => d.EmployeeNavigation)
                    .WithMany(p => p.PurchaseOfRawMaterials)
                    .HasForeignKey(d => d.Employee)
                    .HasConstraintName("FK__PurchaseO__emplo__45F365D3");

                entity.HasOne(d => d.RawMaterialNavigation)
                    .WithMany(p => p.PurchaseOfRawMaterials)
                    .HasForeignKey(d => d.RawMaterial)
                    .HasConstraintName("FK__PurchaseO__rawMa__44FF419A");
            });

            modelBuilder.Entity<RawMaterial>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("amount");

                entity.Property(e => e.Summa)
                    .HasColumnType("money")
                    .HasColumnName("summa");

                entity.Property(e => e.Title)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("title");

                entity.Property(e => e.Unit).HasColumnName("unit");

                entity.HasOne(d => d.UnitNavigation)
                    .WithMany(p => p.RawMaterials)
                    .HasForeignKey(d => d.Unit)
                    .HasConstraintName("FK__RawMateria__unit__2A4B4B5E");
            });

            modelBuilder.Entity<ReportOfProduction>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("ReportOfProduction");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("amount");

                entity.Property(e => e.Datee)
                    .HasColumnType("date")
                    .HasColumnName("datee");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Names)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("names");

                entity.Property(e => e.Title)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<ReportOfSalesOfProduct>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("ReportOfSalesOfProducts");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("amount");

                entity.Property(e => e.Datee)
                    .HasColumnType("date")
                    .HasColumnName("datee");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Names)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("names");

                entity.Property(e => e.Title)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<ReportPurchaseOfRawMaterial>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("ReportPurchaseOfRawMaterials");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("amount");

                entity.Property(e => e.Datee)
                    .HasColumnType("date")
                    .HasColumnName("datee");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Names)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("names");

                entity.Property(e => e.Summa)
                    .HasColumnType("money")
                    .HasColumnName("summa");

                entity.Property(e => e.Title)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<Salary>(entity =>
            {
                entity.ToTable("Salary");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Bonus).HasColumnName("bonus");

                entity.Property(e => e.Salary1)
                    .HasColumnType("money")
                    .HasColumnName("Salary");

                entity.Property(e => e.TotalAmount).HasColumnType("money");

                entity.HasOne(d => d.EmployeeNavigation)
                    .WithMany(p => p.Salaries)
                    .HasForeignKey(d => d.Employee)
                    .HasConstraintName("FK_Salary_Employees");

                entity.HasOne(d => d.MonthNavigation)
                    .WithMany(p => p.Salaries)
                    .HasForeignKey(d => d.Month)
                    .HasConstraintName("FK_Salary_Month");
            });

            modelBuilder.Entity<SaleOfProduct>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("amount");

                entity.Property(e => e.Datee)
                    .HasColumnType("date")
                    .HasColumnName("datee");

                entity.Property(e => e.Employee).HasColumnName("employee");

                entity.Property(e => e.Product).HasColumnName("product");

                entity.Property(e => e.Summa)
                    .HasColumnType("money")
                    .HasColumnName("summa");

                entity.HasOne(d => d.EmployeeNavigation)
                    .WithMany(p => p.SaleOfProducts)
                    .HasForeignKey(d => d.Employee)
                    .HasConstraintName("FK__SaleOfPro__emplo__3A81B327");

                entity.HasOne(d => d.ProductNavigation)
                    .WithMany(p => p.SaleOfProducts)
                    .HasForeignKey(d => d.Product)
                    .HasConstraintName("FK__SaleOfPro__produ__398D8EEE");
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Title)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.UserLogin)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("user_login");

                entity.Property(e => e.UserPassword)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("user_password");
            });

            modelBuilder.Entity<ViewEmployee>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_employee");

                entity.Property(e => e.Address)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("address");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Names)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("names");

                entity.Property(e => e.Phone).HasColumnName("phone");

                entity.Property(e => e.Position)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("position");

                entity.Property(e => e.Salary)
                    .HasColumnType("money")
                    .HasColumnName("salary");
            });

            modelBuilder.Entity<ViewEmployee1>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_Employees");

                entity.Property(e => e.Address)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("address");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Names)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("names");

                entity.Property(e => e.Phone).HasColumnName("phone");

                entity.Property(e => e.Position)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("position");

                entity.Property(e => e.Salary)
                    .HasColumnType("money")
                    .HasColumnName("salary");
            });

            modelBuilder.Entity<ViewIngridient>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("ViewIngridients");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("amount");

                entity.Property(e => e.Product).HasColumnName("product");

                entity.Property(e => e.Title)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<ViewRawMaterial>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_RawMaterials");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("amount");

                entity.Property(e => e.Expr1)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Summa)
                    .HasColumnType("money")
                    .HasColumnName("summa");

                entity.Property(e => e.Title)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("title");

                entity.Property(e => e.Unit).HasColumnName("unit");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
