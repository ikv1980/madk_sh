﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class MmModelCountry
{
    public int Id { get; set; }

    /// <summary>
    /// id для модели
    /// </summary>
    public int ModelId { get; set; }

    /// <summary>
    /// id для страны
    /// </summary>
    public int CountryId { get; set; }

    public virtual CarsCountry Country { get; set; }

    public virtual CarsModel Model { get; set; }
}