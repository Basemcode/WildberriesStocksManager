using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildberriesStocksManager.Models
{
    public class ProductToCheck
    {
        public required string NmID { get; set; }  // Wildberries product ID (NmID)
        public int MinimumFBOStockLevel { get; set; } // Minimum stock level to check against
        public int StockValueForFBS { get; set; } // Stock value for FBS (Fulfillment by Seller) warehouse
    }
}
