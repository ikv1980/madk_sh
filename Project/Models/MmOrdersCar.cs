﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class MmOrdersCar
{
    public int Id { get; set; }

    /// <summary>
    /// id для заказа
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// id для авто
    /// </summary>
    public int CarId { get; set; }

    public virtual Car Car { get; set; }

    public virtual Order Order { get; set; }
}