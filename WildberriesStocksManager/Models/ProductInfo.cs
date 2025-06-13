using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildberriesStocksManager.Models;

public class ProductInfo
{
    public int NmID { get; set; }
    public string SubjectName { get; set; }
    public string Name { get; set; }
    public string VendorCode { get; set; }
    public string BrandName { get; set; }
    public string MainPhoto { get; set; }
    public int StockCount { get; set; }
}


