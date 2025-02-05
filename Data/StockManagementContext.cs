using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Data;

public partial class StockManagementContext : DbContext
{
    public StockManagementContext()
    {
    }

    public StockManagementContext(DbContextOptions<StockManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Delivery> Deliveries { get; set; }


    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public DbSet<TransfersHistory> TransfersHistories { get; set; }
    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<Capacity> Capacities { get; set; } // DbSet for Capacity table

    public virtual DbSet<ItemDetail> ItemDetails { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderedItem> OrderedItems { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SalesmanStock> SalesmanStocks { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }


    public virtual DbSet<Supplier> Suppliers { get; set; }


    public virtual DbSet<ItemSupplier> ItemSupplier { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=StockManagement;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemSupplier>(entity =>
        {
            entity.HasKey(e => e.ItemSupplierId);

            modelBuilder.Entity<ItemSupplier>()
                .HasOne(e => e.Item)
                .WithMany(i => i.ItemSuppliers)
                .HasForeignKey(e => e.ItemId);

            modelBuilder.Entity<ItemSupplier>()
                .HasOne(e => e.Supplier)
                .WithMany(s => s.ItemSuppliers)
                .HasForeignKey(e => e.SupplierId);

            base.OnModelCreating(modelBuilder);

        });


        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK__brand__5E5A8E276F139381");

            entity.ToTable("brand");

            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.BrandName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("brand_name");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__category__D54EE9B4F809BF32");

            entity.ToTable("category");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("category_name");
            entity.Property(e => e.Identifier)
               .HasMaxLength(10)
               .IsUnicode(false)
               .HasColumnName("Identifier");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__customer__CD65CB85C0327932");

            entity.ToTable("customers");

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone_number");
        });

        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.HasKey(e => e.DeliveryId).HasName("PK__deliveri__1C5CF4F59A1F4DEC");

            entity.ToTable("deliveries");

            entity.Property(e => e.DeliveryId).HasColumnName("delivery_id");
            entity.Property(e => e.CashCollected)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("cash_collected");
            entity.Property(e => e.DeliveryDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("delivery_date");
            entity.Property(e => e.Status_Id).HasColumnName("status_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            
            entity.HasOne(d => d.Employee).WithMany(p => p.Deliveries)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__deliverie__emplo__6A30C649");

            entity.HasOne(d => d.Order).WithMany(p => p.Deliveries)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__deliverie__order__693CA210");


            entity.HasOne(d => d.Status).WithMany(p => p.Deliveries)
    .HasForeignKey(d => d.Status_Id)
    .OnDelete(DeleteBehavior.ClientSetNull)
    .HasConstraintName("FK__deliverie__statu__6B24EA82");
        });

        

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__employee__C52E0BA887EC62D3");

            entity.ToTable("employees");

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Employees)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__employees__role___3B75D760");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__items__52020FDD3A5A8C0E");

            entity.ToTable("items");

            entity.Property(e => e.ItemId).HasColumnName("item_id");
           
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.ModelNumber)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("model_number");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
          
            entity.HasOne(d => d.Brand).WithMany(p => p.Items)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("FK_Items_Brand"); 

           entity.HasOne(d => d.Category).WithMany(p => p.Items)
               .HasForeignKey(d => d.CategoryId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Items_Category");
             
            entity.HasMany(i => i.Capacities)
                  .WithMany(c => c.Items)
                  .UsingEntity(j => j.ToTable("ItemCapacity")); // Optional: Customize join table name

            entity.Property(e => e.Description)
              .HasMaxLength(500);

            entity.HasOne(e => e.Color)
                 .WithMany(c => c.Items)
                 .HasForeignKey(e => e.ColorId)
                 .OnDelete(DeleteBehavior.SetNull);

        });

        modelBuilder.Entity<ItemDetail>(entity =>
        {
            entity.HasKey(e => e.ItemDetailsId).HasName("PK__item_det__3213E83FC73542C3");

            entity.ToTable("item_details");

            entity.Property(e => e.ItemDetailsId).HasColumnName("item_details_id");
            entity.Property(e => e.Cost)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("cost");
            entity.Property(e => e.DateReceived).HasColumnName("date_received");
            entity.Property(e => e.Imei1)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("imei_1");
            entity.Property(e => e.Imei2)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("imei_2");
            entity.Property(e => e.Barcode)
               .HasMaxLength(100)
               .IsUnicode(false)
               .HasColumnName("barcode");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SalePrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("sale_price");
            entity.Property(e => e.SerialNumber)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("serial_number");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

            

            //entity.HasOne(d => d.Item).WithMany(p => p.ItemDetails)
               // .HasForeignKey(d => d.ItemId)
               // .OnDelete(DeleteBehavior.ClientSetNull)
               // .HasConstraintName("FK__item_deta__item___76969D2E");

            entity.HasOne(d => d.Supplier).WithMany(p => p.ItemDetails)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("fk_supplier_id");

        });

        // Optional: Configure Capacity table if needed
        modelBuilder.Entity<Capacity>(entity =>
        {
            entity.HasKey(c => c.CapacityID); // Configure primary key
            entity.Property(c => c.CapacityName)
                  .IsRequired() // Make CapacityName required
                  .HasMaxLength(100); // Set a max length for CapacityName
        });


        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__orders__4659622907CDD9A3");

            entity.ToTable("orders");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.OrderDate).HasColumnName("order_date");
            entity.Property(e => e.Status_Id).HasColumnName("status_id");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total amount");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__orders__customer__5812160E");

            entity.HasOne(d => d.Employee).WithMany(p => p.Orders)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__orders__employee__571DF1D5");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Status_Id)
                .HasConstraintName("fk_orders_status");
        });

        modelBuilder.Entity<OrderedItem>(entity =>
        {
            entity.HasKey(e => e.OrderedItemId).HasName("PK__ordered___F43DC48BD93B8A99");

            entity.ToTable("ordered_items");

            entity.Property(e => e.OrderedItemId).HasColumnName("ordered_item_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.Discount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("discount");
            entity.Property(e => e.ItemDetailsId).HasColumnName("item_details_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.ItemDetails).WithMany(p => p.OrderedItems)
                .HasForeignKey(d => d.ItemDetailsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_item_details_id");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderedItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ordered_i__order__6383C8BA");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__roles__760965CCCC34A350");

            entity.ToTable("roles");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<SalesmanStock>(entity =>
        {
            entity.HasKey(e => e.SalesmanStockId).HasName("PK__salesman__7D6D00AEBDF35074");

            entity.ToTable("salesman_stock");

            
            entity.Property(e => e.Cost)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("cost");
            entity.Property(e => e.DateReceived).HasColumnName("date_received");
            entity.Property(e => e.TransferDate).HasColumnName("transfer_date");

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.Imei1)
                .HasMaxLength(50)
                .HasColumnName("imei_1");
            entity.Property(e => e.Imei2)
                .HasMaxLength(50)
                .HasColumnName("imei_2");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.SalePrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("sale_price");
            entity.Property(e => e.SerialNumber)
                .HasMaxLength(50)
                .HasColumnName("serial_number");
            entity.Property(e => e.Barcode)
                .HasMaxLength(50)
                .HasColumnName("Barcode");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Item).WithMany(p => p.SalesmanStocks)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_salesman_stock_items");

            entity.HasOne(d => d.Status)
        .WithMany(p => p.SalesmanStocks)
        .HasForeignKey(d => d.StatusId)
        .OnDelete(DeleteBehavior.ClientSetNull)
        .HasConstraintName("fk_salesman_stock_status");

        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.status_id).HasName("PK__status__3213E83F9BA64F51");

            entity.ToTable("status");

            entity.Property(e => e.status_id).HasColumnName("id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .HasColumnName("status");
        });

        

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__supplier__6EE594E800C8141D");

            entity.ToTable("supplier");

            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("supplier_name");
        });


        modelBuilder.Entity<Color>(entity =>
        {
            // Optional: Set constraints for the Color model
            entity.Property(e => e.ColorName)
                  .HasMaxLength(100)  // Limit the length of ColorName
                  .IsRequired();  // Ensure ColorName is required
        });

        OnModelCreatingPartial(modelBuilder);

        modelBuilder.Entity<TransfersHistory>(entity =>
        {
            entity.HasKey(e => e.TransferHistoryId).HasName("PK_TransfersHistory");

            entity.ToTable("transfers_history");

            entity.Property(e => e.TransferHistoryId).HasColumnName("transfer_history_id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.IMEI1).HasColumnName("imei1").HasMaxLength(100);
            entity.Property(e => e.IMEI2).HasColumnName("imei2").HasMaxLength(100);
            entity.Property(e => e.SerialNumber).HasColumnName("serial_number").HasMaxLength(100);
            entity.Property(e => e.DateTransfered).HasColumnName("date_transfered").HasColumnType("datetime");
            entity.Property(e => e.Source).HasColumnName("source").HasMaxLength(255);
            entity.Property(e => e.Destination).HasColumnName("destination").HasMaxLength(255);
            entity.Property(e => e.Note).HasColumnName("note").HasMaxLength(500);

            entity.HasOne(d => d.Item)
                .WithMany()
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TransfersHistory_Items");
        });

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
