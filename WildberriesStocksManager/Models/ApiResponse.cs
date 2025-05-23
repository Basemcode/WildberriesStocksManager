using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildberriesStocksManager.Models;

public class Metrics
{
    public int StockCount { get; set; }
}

public class Item
{
    public int NmID { get; set; }
    public bool IsDeleted { get; set; }
    public string SubjectName { get; set; }
    public string Name { get; set; }
    public string VendorCode { get; set; }
    public string BrandName { get; set; }
    public string MainPhoto { get; set; }
    public Metrics Metrics { get; set; }
}

public class Data
{
    public List<Item> Items { get; set; }
}

public class ApiResponse
{
    public Data Data { get; set; }
}
