﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class OrdersClient
{
    public int ClientId { get; set; }

    /// <summary>
    /// Логин
    /// </summary>
    public string ClientLogin { get; set; }

    /// <summary>
    /// Пароль (хеш)
    /// </summary>
    public string ClientPassword { get; set; }

    /// <summary>
    /// E-mail
    /// </summary>
    public string ClientMail { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    public string ClientName { get; set; }

    /// <summary>
    /// Фамилия
    /// </summary>
    public string ClientSurname { get; set; }

    /// <summary>
    /// Телефон
    /// </summary>
    public string ClientPhone { get; set; }

    /// <summary>
    /// Дата рождения
    /// </summary>
    public DateOnly? ClientBirthday { get; set; }

    /// <summary>
    /// Дата регистрации
    /// </summary>
    public DateTime ClientDateRegistration { get; set; }

    /// <summary>
    /// Метка удаления
    /// </summary>
    public bool Delete { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}