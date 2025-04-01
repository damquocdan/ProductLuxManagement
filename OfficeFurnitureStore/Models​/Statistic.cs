using System;
using System.Collections.Generic;

namespace OfficeFurnitureStore.Models​;

public partial class Statistic
{
    public int StatisticId { get; set; }

    public DateOnly ReportDate { get; set; }

    public string? Period { get; set; }

    public DateOnly? PeriodStartDate { get; set; }

    public DateOnly? PeriodEndDate { get; set; }

    public int TotalOrders { get; set; }

    public decimal TotalRevenue { get; set; }

    public int? TotalProductsSold { get; set; }

    public int? TotalCustomers { get; set; }

    public int? TotalProductsInStock { get; set; }

    public decimal? CategoryRevenue { get; set; }

    public decimal? ProductRevenue { get; set; }

    public decimal? CustomerRevenue { get; set; }
}
