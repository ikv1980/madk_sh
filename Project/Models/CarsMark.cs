﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class CarsMark
{
    public int MarkId { get; set; }

    /// <summary>
    /// Наименование марки
    /// </summary>
    public string MarkName { get; set; }

    /// <summary>
    /// Метка удаления
    /// </summary>
    public bool Delete { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    public virtual ICollection<MmMarkModel> MmMarkModels { get; set; } = new List<MmMarkModel>();
}