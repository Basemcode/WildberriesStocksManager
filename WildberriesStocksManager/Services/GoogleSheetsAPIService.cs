using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildberriesStocksManager.Services;

internal class GoogleSheetsAPIService
{
    public static async Task<string> GetGoogleSheetDataAsync(string sheetId, string range)
    {
        return $"Data from Google Sheet {sheetId} in range {range}";
    }
}
