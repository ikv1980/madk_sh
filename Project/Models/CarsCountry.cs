﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class CarsCountry
{
    public int CountryId { get; set; }

    public string CountryName { get; set; }

    public bool CountryDelete { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    public virtual ICollection<MmModelCountry> MmModelCountries { get; set; } = new List<MmModelCountry>();
}