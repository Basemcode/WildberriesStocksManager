using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WildberriesStocksManager.Enums;
using WildberriesStocksManager.Models;

namespace WildberriesStocksManager.Services;

internal class GoogleSheetsAPIService
{
    public static async Task<string> GetGoogleSheetDataAsync(string sheetId, string range)
    {
        return $"Data from Google Sheet {sheetId} in range {range}";
    }

    //get the list of products to check from a Google Sheet , there would be id on WB and the minimal stocks levels
    public static async Task<ProductToCheck[]> GetTargetedProductsList(Stores store)
    {
        return new ProductToCheck[10];
    }
}
