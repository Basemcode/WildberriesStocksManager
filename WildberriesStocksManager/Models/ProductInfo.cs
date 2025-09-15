using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildberriesStocksManager.Models;

public class ProductInfo
{
    public string NmID { get; set; } = string.Empty; 
    public string SubjectName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string VendorCode { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string MainPhoto { get; set; } = string.Empty;
    public int ReceivedStockCount { get; set; }
    public int FPSStockCount { get; set; }
    public int FPOStockCount { get; set; }
}


