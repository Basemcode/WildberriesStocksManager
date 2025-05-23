using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WildberriesStocksManager.Enums;
using WildberriesStocksManager.Helpers;
using WildberriesStocksManager.Models;

namespace WildberriesStocksManager.Services;

internal static class ProductsDataService
{
    public static async Task<List<ProductInfo>> GetProductsInfos()
    {
        var ProductsStocksJson = await APIService.GetStockReportAsync(
            ProductsDetails.ArtXLProductsList,
            Stores.ArtXL
        );

        if (ProductsStocksJson.IsSuccessStatusCode)
        {
            return await ParseProductInfoAsync(ProductsStocksJson.Content);
        }
        else 
        {
            throw new Exception("Error connection getting data from Wildberries server!");
        }
    }

    public static async Task<List<ProductInfo>> ParseProductInfoAsync(string json)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var response = JsonSerializer.Deserialize<ApiResponse>(json, options);

        var products = response
            .Data.Items.Select(item => new ProductInfo
            {
                NmID = item.NmID,
                IsDeleted = item.IsDeleted,
                SubjectName = item.SubjectName,
                Name = item.Name,
                VendorCode = item.VendorCode,
                BrandName = item.BrandName,
                MainPhoto = item.MainPhoto,
                StockCount = item.Metrics?.StockCount ?? 0,
            })
            .ToList();

        return products;
    }
}
