﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Project.Models;

public partial class Db : DbContext
{
    public Db()
    {
    }

    public Db(DbContextOptions<Db> options)
        : base(options)
    {
    }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<CarsColor> CarsColors { get; set; }

    public virtual DbSet<CarsCountry> CarsCountries { get; set; }

    public virtual DbSet<CarsMark> CarsMarks { get; set; }

    public virtual DbSet<CarsModel> CarsModels { get; set; }

    public virtual DbSet<CarsType> CarsTypes { get; set; }

    public virtual DbSet<MmMarkModel> MmMarkModels { get; set; }

    public virtual DbSet<MmModelCountry> MmModelCountries { get; set; }

    public virtual DbSet<MmOrdersCar> MmOrdersCars { get; set; }

    public virtual DbSet<MmOrdersStatus> MmOrdersStatuses { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrdersClient> OrdersClients { get; set; }

    public virtual DbSet<OrdersDelivery> OrdersDeliveries { get; set; }

    public virtual DbSet<OrdersPayment> OrdersPayments { get; set; }

    public virtual DbSet<OrdersStatus> OrdersStatuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UsersDepartmment> UsersDepartmments { get; set; }

    public virtual DbSet<UsersFunction> UsersFunctions { get; set; }

    public virtual DbSet<UsersStatus> UsersStatuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Проверьте, был ли уже установлен optionsBuilder
        if (!optionsBuilder.IsConfigured)
        {
            // Настройка подключения с использованием MariaDB
            var connectionString = "server=213.171.25.72;port=3306;database=madk;uid=madk;pwd=Kostik80";
            var serverVersion = new MariaDbServerVersion("11.5.2");

            // Здесь вы можете указать строку подключения напрямую
            optionsBuilder.UseMySql(connectionString, serverVersion);
            // Для этапа разработки. В production-среде отключить!!! ikv1980
            optionsBuilder
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_uca1400_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.CarId).HasName("PRIMARY");

            entity
                .ToTable("cars")
                .UseCollation("utf8mb3_uca1400_ai_ci");

            entity.HasIndex(e => e.CarColor, "car_color");

            entity.HasIndex(e => e.CarCountry, "car_country");

            entity.HasIndex(e => e.CarMark, "car_mark");

            entity.HasIndex(e => e.CarModel, "car_model");

            entity.HasIndex(e => e.CarPts, "car_pts").IsUnique();

            entity.HasIndex(e => e.CarVin, "car_vin").IsUnique();

            entity.HasIndex(e => e.CarType, "cars_type");

            entity.Property(e => e.CarId)
                .HasColumnType("int(10)")
                .HasColumnName("car_id");
            entity.Property(e => e.CarBlock)
                .HasComment("Блок")
                .HasColumnName("car_block");
            entity.Property(e => e.CarColor)
                .HasComment("Цвет")
                .HasColumnType("int(10)")
                .HasColumnName("car_color");
            entity.Property(e => e.CarCountry)
                .HasComment("Страна производства")
                .HasColumnType("int(10)")
                .HasColumnName("car_country");
            entity.Property(e => e.CarDate)
                .HasComment("Дата производства")
                .HasColumnName("car_date");
            entity.Property(e => e.CarDelete)
                .HasComment("Метка удаления")
                .HasColumnType("tinyint(4)")
                .HasColumnName("car_delete");
            entity.Property(e => e.CarMark)
                .HasComment("Производитель(марка)")
                .HasColumnType("int(10)")
                .HasColumnName("car_mark");
            entity.Property(e => e.CarModel)
                .HasComment("Модель")
                .HasColumnType("int(10)")
                .HasColumnName("car_model");
            entity.Property(e => e.CarPhoto)
                .IsRequired()
                .HasMaxLength(255)
                .HasComment("Ссылка на фотографии")
                .HasColumnName("car_photo")
                .UseCollation("utf8mb4_uca1400_ai_ci");
            entity.Property(e => e.CarPrice)
                .HasComment("Цена")
                .HasColumnType("float(10,2)")
                .HasColumnName("car_price");
            entity.Property(e => e.CarPts)
                .IsRequired()
                .HasMaxLength(20)
                .HasComment("ПТС авто")
                .HasColumnName("car_pts")
                .UseCollation("utf8mb4_uca1400_ai_ci");
            entity.Property(e => e.CarType)
                .HasComment("Тип")
                .HasColumnType("int(4)")
                .HasColumnName("car_type");
            entity.Property(e => e.CarVin)
                .IsRequired()
                .HasMaxLength(20)
                .HasComment("VIN-код")
                .HasColumnName("car_vin")
                .UseCollation("utf8mb4_uca1400_ai_ci");

            entity.HasOne(d => d.CarColorNavigation).WithMany(p => p.Cars)
                .HasForeignKey(d => d.CarColor)
                .HasConstraintName("cars_ibfk_4");

            entity.HasOne(d => d.CarCountryNavigation).WithMany(p => p.Cars)
                .HasForeignKey(d => d.CarCountry)
                .HasConstraintName("cars_ibfk_11");

            entity.HasOne(d => d.CarMarkNavigation).WithMany(p => p.Cars)
                .HasForeignKey(d => d.CarMark)
                .HasConstraintName("cars_ibfk_9");

            entity.HasOne(d => d.CarModelNavigation).WithMany(p => p.Cars)
                .HasForeignKey(d => d.CarModel)
                .HasConstraintName("cars_ibfk_10");

            entity.HasOne(d => d.CarTypeNavigation).WithMany(p => p.Cars)
                .HasForeignKey(d => d.CarType)
                .HasConstraintName("cars_ibfk_12");
        });

        modelBuilder.Entity<CarsColor>(entity =>
        {
            entity.HasKey(e => e.ColorId).HasName("PRIMARY");

            entity
                .ToTable("cars_color")
                .UseCollation("utf8mb3_uca1400_ai_ci");

            entity.HasIndex(e => e.ColorName, "color_name").IsUnique();

            entity.Property(e => e.ColorId)
                .HasColumnType("int(10)")
                .HasColumnName("color_id");
            entity.Property(e => e.ColorDelete)
                .HasComment("Метка удаления")
                .HasColumnName("color_delete");
            entity.Property(e => e.ColorName)
                .IsRequired()
                .HasMaxLength(30)
                .HasComment("Наименование")
                .HasColumnName("color_name")
                .UseCollation("utf8mb4_uca1400_ai_ci");
        });

        modelBuilder.Entity<CarsCountry>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PRIMARY");

            entity
                .ToTable("cars_country")
                .UseCollation("utf8mb3_uca1400_ai_ci");

            entity.HasIndex(e => e.CountryName, "country_name").IsUnique();

            entity.Property(e => e.CountryId)
                .HasColumnType("int(10)")
                .HasColumnName("country_id");
            entity.Property(e => e.CountryName)
                .IsRequired()
                .HasMaxLength(30)
                .HasComment("Наименование")
                .HasColumnName("country_name")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.CuuntryDelete)
                .HasComment("Метка удаления")
                .HasColumnName("cuuntry_delete");
        });

        modelBuilder.Entity<CarsMark>(entity =>
        {
            entity.HasKey(e => e.MarkId).HasName("PRIMARY");

            entity.ToTable("cars_mark");

            entity.HasIndex(e => e.MarkName, "model_name").IsUnique();

            entity.Property(e => e.MarkId)
                .HasColumnType("int(10)")
                .HasColumnName("mark_id");
            entity.Property(e => e.MarkDelete)
                .HasComment("Метка удаления")
                .HasColumnName("mark_delete");
            entity.Property(e => e.MarkName)
                .IsRequired()
                .HasMaxLength(30)
                .HasComment("Наименование марки")
                .HasColumnName("mark_name");
        });

        modelBuilder.Entity<CarsModel>(entity =>
        {
            entity.HasKey(e => e.ModelId).HasName("PRIMARY");

            entity
                .ToTable("cars_model")
                .UseCollation("utf8mb3_uca1400_ai_ci");

            entity.HasIndex(e => e.ModelName, "model_name").IsUnique();

            entity.Property(e => e.ModelId)
                .HasColumnType("int(10)")
                .HasColumnName("model_id");
            entity.Property(e => e.ModelDelete)
                .HasComment("Метка удаления")
                .HasColumnName("model_delete");
            entity.Property(e => e.ModelName)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Наименование")
                .HasColumnName("model_name")
                .UseCollation("utf8mb4_uca1400_ai_ci");
        });

        modelBuilder.Entity<CarsType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PRIMARY");

            entity
                .ToTable("cars_type")
                .UseCollation("utf8mb3_uca1400_ai_ci");

            entity.Property(e => e.TypeId)
                .HasColumnType("int(4)")
                .HasColumnName("type_id");
            entity.Property(e => e.TypeDelete)
                .HasComment("Метка удаления")
                .HasColumnName("type_delete");
            entity.Property(e => e.TypeName)
                .IsRequired()
                .HasMaxLength(30)
                .HasComment("Наименование")
                .HasColumnName("type_name")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<MmMarkModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("mm_mark_model")
                .UseCollation("utf8mb3_uca1400_ai_ci");

            entity.HasIndex(e => e.MarkId, "mark_id");

            entity.HasIndex(e => e.ModelId, "model_id");

            entity.Property(e => e.Id)
                .HasColumnType("int(10)")
                .HasColumnName("id");
            entity.Property(e => e.MarkId)
                .HasComment("id для марки")
                .HasColumnType("int(10)")
                .HasColumnName("mark_id");
            entity.Property(e => e.ModelId)
                .HasComment("id для модели")
                .HasColumnType("int(10)")
                .HasColumnName("model_id");

            entity.HasOne(d => d.Mark).WithMany(p => p.MmMarkModels)
                .HasForeignKey(d => d.MarkId)
                .HasConstraintName("mm_mark_model_ibfk_1");

            entity.HasOne(d => d.Model).WithMany(p => p.MmMarkModels)
                .HasForeignKey(d => d.ModelId)
                .HasConstraintName("mm_mark_model_ibfk_2");
        });

        modelBuilder.Entity<MmModelCountry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("mm_model_country")
                .UseCollation("utf8mb3_uca1400_ai_ci");

            entity.HasIndex(e => e.CountryId, "mm_model_country_ibfk_2");

            entity.HasIndex(e => e.ModelId, "model_id");

            entity.Property(e => e.Id)
                .HasColumnType("int(10)")
                .HasColumnName("id");
            entity.Property(e => e.CountryId)
                .HasComment("id для страны")
                .HasColumnType("int(10)")
                .HasColumnName("country_id");
            entity.Property(e => e.ModelId)
                .HasComment("id для модели")
                .HasColumnType("int(10)")
                .HasColumnName("model_id");

            entity.HasOne(d => d.Country).WithMany(p => p.MmModelCountries)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("mm_model_country_ibfk_2");

            entity.HasOne(d => d.Model).WithMany(p => p.MmModelCountries)
                .HasForeignKey(d => d.ModelId)
                .HasConstraintName("mm_model_country_ibfk_1");
        });

        modelBuilder.Entity<MmOrdersCar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("mm_orders_cars")
                .UseCollation("utf8mb3_uca1400_ai_ci");

            entity.HasIndex(e => e.CarId, "car_id");

            entity.HasIndex(e => e.OrderId, "order_id");

            entity.Property(e => e.Id)
                .HasColumnType("int(10)")
                .HasColumnName("id");
            entity.Property(e => e.CarId)
                .HasComment("id для авто")
                .HasColumnType("int(10)")
                .HasColumnName("car_id");
            entity.Property(e => e.OrderId)
                .HasComment("id для заказа")
                .HasColumnType("int(10)")
                .HasColumnName("order_id");

            entity.HasOne(d => d.Car).WithMany(p => p.MmOrdersCars)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mm_orders_cars_ibfk_2");

            entity.HasOne(d => d.Order).WithMany(p => p.MmOrdersCars)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mm_orders_cars_ibfk_1");
        });

        modelBuilder.Entity<MmOrdersStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("mm_orders_status");

            entity.HasIndex(e => e.StatusId, "mm_orders_status_ibfk_4");

            entity.HasIndex(e => e.OrderId, "order_id");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Дата смены статуса")
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.OrderId)
                .HasComment("id заказа")
                .HasColumnType("int(10)")
                .HasColumnName("order_id");
            entity.Property(e => e.StatusId)
                .HasComment("id статуса")
                .HasColumnType("int(4)")
                .HasColumnName("status_id");

            entity.HasOne(d => d.Order).WithMany(p => p.MmOrdersStatuses)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("mm_orders_status_ibfk_3");

            entity.HasOne(d => d.Status).WithMany(p => p.MmOrdersStatuses)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("mm_orders_status_ibfk_4");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrdersId).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.OrdersClient, "orders_client");

            entity.HasIndex(e => e.OrdersDelivery, "orders_delivery");

            entity.HasIndex(e => e.OrdersUser, "orders_ibfk_5");

            entity.HasIndex(e => e.OrdersPayment, "orders_payment");

            entity.Property(e => e.OrdersId)
                .HasColumnType("int(10)")
                .HasColumnName("orders_id");
            entity.Property(e => e.OrdersAddress)
                .HasComment("Адрес доставки")
                .HasColumnType("text")
                .HasColumnName("orders_address");
            entity.Property(e => e.OrdersClient)
                .HasComment("id клиента")
                .HasColumnType("int(10)")
                .HasColumnName("orders_client");
            entity.Property(e => e.OrdersData)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Дата создания заказа")
                .HasColumnType("datetime")
                .HasColumnName("orders_data");
            entity.Property(e => e.OrdersDelete)
                .HasComment("Метка удаления")
                .HasColumnName("orders_delete");
            entity.Property(e => e.OrdersDelivery)
                .HasComment("Тип доставки")
                .HasColumnType("int(4)")
                .HasColumnName("orders_delivery");
            entity.Property(e => e.OrdersPayment)
                .HasComment("Тип оплаты")
                .HasColumnType("int(4)")
                .HasColumnName("orders_payment");
            entity.Property(e => e.OrdersUser)
                .HasComment("id сотрудника")
                .HasColumnType("int(10)")
                .HasColumnName("orders_user");

            entity.HasOne(d => d.OrdersClientNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrdersClient)
                .HasConstraintName("orders_ibfk_1");

            entity.HasOne(d => d.OrdersDeliveryNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrdersDelivery)
                .HasConstraintName("orders_ibfk_3");

            entity.HasOne(d => d.OrdersPaymentNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrdersPayment)
                .HasConstraintName("orders_ibfk_4");

            entity.HasOne(d => d.OrdersUserNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrdersUser)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("orders_ibfk_5");
        });

        modelBuilder.Entity<OrdersClient>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PRIMARY");

            entity
                .ToTable("orders_client")
                .UseCollation("utf8mb3_uca1400_ai_ci");

            entity.HasIndex(e => e.ClientLogin, "client_login").IsUnique();

            entity.HasIndex(e => e.ClientMail, "client_mail").IsUnique();

            entity.Property(e => e.ClientId)
                .HasColumnType("int(10)")
                .HasColumnName("client_id");
            entity.Property(e => e.ClientBirthday)
                .HasComment("Дата рождения")
                .HasColumnName("client_birthday");
            entity.Property(e => e.ClientDateRegistration)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("Дата регистрации")
                .HasColumnType("datetime")
                .HasColumnName("client_date_registration");
            entity.Property(e => e.ClientFlagDelete)
                .HasComment("Метка удаления")
                .HasColumnName("client_flag_delete");
            entity.Property(e => e.ClientLogin)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Логин")
                .HasColumnName("client_login")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.ClientMail)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("E-mail")
                .HasColumnName("client_mail")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.ClientName)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Имя")
                .HasColumnName("client_name")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.ClientPassword)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Пароль (хеш)")
                .HasColumnName("client_password")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.ClientPhone)
                .HasMaxLength(50)
                .HasComment("Телефон")
                .HasColumnName("client_phone")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.ClientSurname)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Фамилия")
                .HasColumnName("client_surname")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<OrdersDelivery>(entity =>
        {
            entity.HasKey(e => e.DeliveryId).HasName("PRIMARY");

            entity.ToTable("orders_delivery");

            entity.HasIndex(e => e.DeliveryName, "delivery_name").IsUnique();

            entity.Property(e => e.DeliveryId)
                .HasColumnType("int(4)")
                .HasColumnName("delivery_id");
            entity.Property(e => e.DeliveryDelete)
                .HasComment("Метка удаления")
                .HasColumnName("delivery_delete");
            entity.Property(e => e.DeliveryName)
                .IsRequired()
                .HasMaxLength(30)
                .HasComment("Наименование")
                .HasColumnName("delivery_name");
        });

        modelBuilder.Entity<OrdersPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PRIMARY");

            entity.ToTable("orders_payment");

            entity.HasIndex(e => e.PaymentName, "payment_name").IsUnique();

            entity.Property(e => e.PaymentId)
                .HasColumnType("int(4)")
                .HasColumnName("payment_id");
            entity.Property(e => e.PaymentDelete)
                .HasComment("Метка удаления")
                .HasColumnName("payment_delete");
            entity.Property(e => e.PaymentName)
                .IsRequired()
                .HasMaxLength(30)
                .HasComment("Наименование")
                .HasColumnName("payment_name");
        });

        modelBuilder.Entity<OrdersStatus>(entity =>
        {
            entity.HasKey(e => e.OrderStatusId).HasName("PRIMARY");

            entity.ToTable("orders_status");

            entity.Property(e => e.OrderStatusId)
                .HasColumnType("int(4)")
                .HasColumnName("order_status_id");
            entity.Property(e => e.OrderStatusDelete)
                .HasComment("Метка удаления")
                .HasColumnName("order_status_delete");
            entity.Property(e => e.OrderStatusDescription)
                .HasComment("Описание")
                .HasColumnType("text")
                .HasColumnName("order_status_description");
            entity.Property(e => e.OrderStatusName)
                .IsRequired()
                .HasMaxLength(30)
                .HasComment("Наименование")
                .HasColumnName("order_status_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UsersId).HasName("PRIMARY");

            entity
                .ToTable("users")
                .UseCollation("utf8mb3_uca1400_ai_ci");

            entity.HasIndex(e => e.UsersDepartment, "user_department");

            entity.HasIndex(e => e.UsersFunction, "user_function");

            entity.HasIndex(e => e.UsersLogin, "users_login").IsUnique();

            entity.HasIndex(e => e.UsersMail, "users_mail").IsUnique();

            entity.HasIndex(e => e.UsersStatus, "users_status");

            entity.Property(e => e.UsersId)
                .HasColumnType("int(10)")
                .HasColumnName("users_id");
            entity.Property(e => e.UsersBirthday)
                .HasComment("Дата рождения")
                .HasColumnName("users_birthday");
            entity.Property(e => e.UsersDepartment)
                .HasComment("Отдел")
                .HasColumnType("int(4)")
                .HasColumnName("users_department");
            entity.Property(e => e.UsersFlagDelete)
                .HasComment("Метка удаления")
                .HasColumnName("users_flag_delete");
            entity.Property(e => e.UsersFunction)
                .HasComment("Должность")
                .HasColumnType("int(4)")
                .HasColumnName("users_function");
            entity.Property(e => e.UsersLogin)
                .IsRequired()
                .HasComment("Логин")
                .HasColumnName("users_login")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.UsersMail)
                .HasComment("E-mail")
                .HasColumnName("users_mail")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.UsersName)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Имя")
                .HasColumnName("users_name")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.UsersPassword)
                .IsRequired()
                .HasMaxLength(255)
                .HasComment("Пароль (хеш)")
                .HasColumnName("users_password")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.UsersPatronymic)
                .HasMaxLength(100)
                .HasComment("Отчество")
                .HasColumnName("users_patronymic")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.UsersPermissions)
                .HasComment("Разрешения")
                .HasColumnType("text")
                .HasColumnName("users_permissions")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.UsersPhone)
                .HasMaxLength(50)
                .HasComment("Телефон")
                .HasColumnName("users_phone")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.UsersStartWork)
                .HasComment("Статус")
                .HasColumnName("users_start_work");
            entity.Property(e => e.UsersStatus)
                .HasDefaultValueSql("'1'")
                .HasComment("Статус")
                .HasColumnType("int(4)")
                .HasColumnName("users_status");
            entity.Property(e => e.UsersSurname)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Фамилия")
                .HasColumnName("users_surname")
                .HasCharSet("utf8mb3");

            entity.HasOne(d => d.UsersDepartmentNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.UsersDepartment)
                .HasConstraintName("users_ibfk_1");

            entity.HasOne(d => d.UsersFunctionNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.UsersFunction)
                .HasConstraintName("users_ibfk_4");

            entity.HasOne(d => d.UsersStatusNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.UsersStatus)
                .HasConstraintName("users_ibfk_3");
        });

        modelBuilder.Entity<UsersDepartmment>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PRIMARY");

            entity
                .ToTable("users_departmment")
                .UseCollation("utf8mb3_uca1400_ai_ci");

            entity.Property(e => e.DepartmentId)
                .HasColumnType("int(4)")
                .HasColumnName("department_id");
            entity.Property(e => e.DepartmentDescription)
                .HasComment("Описание")
                .HasColumnType("text")
                .HasColumnName("department_description")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.DepartmentFlagDelete)
                .HasComment("Метка удаления")
                .HasColumnName("department_flag_delete");
            entity.Property(e => e.DepartmentMail)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("E-mail отдела")
                .HasColumnName("department_mail")
                .UseCollation("utf8mb4_bin");
            entity.Property(e => e.DepartmentName)
                .IsRequired()
                .HasMaxLength(255)
                .HasComment("Наименование")
                .HasColumnName("department_name")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<UsersFunction>(entity =>
        {
            entity.HasKey(e => e.FunctionId).HasName("PRIMARY");

            entity.ToTable("users_function");

            entity.HasIndex(e => e.FunctionName, "function_name").IsUnique();

            entity.Property(e => e.FunctionId)
                .HasColumnType("int(4)")
                .HasColumnName("function_id");
            entity.Property(e => e.FunctionDelete)
                .HasComment("Метка удаления")
                .HasColumnName("function_delete");
            entity.Property(e => e.FunctionDescription)
                .HasComment("Описание")
                .HasColumnType("text")
                .HasColumnName("function_description");
            entity.Property(e => e.FunctionName)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Наименование")
                .HasColumnName("function_name");
        });

        modelBuilder.Entity<UsersStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PRIMARY");

            entity
                .ToTable("users_status")
                .UseCollation("utf8mb3_uca1400_ai_ci");

            entity.HasIndex(e => e.StatusName, "status_name").IsUnique();

            entity.Property(e => e.StatusId)
                .HasColumnType("int(4)")
                .HasColumnName("status_id");
            entity.Property(e => e.StatusDelete)
                .HasColumnType("tinyint(4)")
                .HasColumnName("status_delete");
            entity.Property(e => e.StatusName)
                .IsRequired()
                .HasMaxLength(30)
                .HasColumnName("status_name")
                .HasCharSet("utf8mb3");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}