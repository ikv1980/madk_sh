﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class CarsType
{
    public int TypeId { get; set; }

    /// <summary>
    /// Наименование
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// Метка удаления
    /// </summary>
    public bool Delete { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}