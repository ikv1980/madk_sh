﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class OrdersStatus
{
    public int OrderStatusId { get; set; }

    public string OrderStatusName { get; set; }

    public string OrderStatusDescription { get; set; }

    public bool OrderStatusDelete { get; set; }

    public virtual ICollection<MmOrdersStatus> MmOrdersStatuses { get; set; } = new List<MmOrdersStatus>();
}