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
    public static async Task<List<ProductInfo>?> GetProductsInfos(Stores store, string stockType)
    {
        //determine which products IDs to use bases on the store
        ProductToCheck[] ProductsList;
        ProductsList = await GoogleSheetsAPIService.GetTargetedProductsListAsync(store);

        List<ProductInfo>? allProductsInfosList = new();
        bool allReturned = false;
        int pageOffset = 0;

        while (!allReturned)
        {
            // call the API to receive info about the products from wildberries
            var ProductsStocksJson = await WBAPIService.GetStockReportAsync(
                store,
                stockType,
                pageOffset
            );

            // check if the response is valid
            if (ProductsStocksJson.IsSuccessStatusCode && ProductsStocksJson.Content is not null)
            {
                var returnedList = ParseProductInfo(ProductsStocksJson.Content);
                if (returnedList != null)
                {
                    // add the returned part of data to final list
                    allProductsInfosList.AddRange(returnedList);
                    //check if all the data returned
                    if (returnedList.Count < 1000) //the 1000 is the maximum amount of products info that the api can return
                    {
                        allReturned = true;
                    }
                    else
                    {
                        pageOffset += 1000;
                    } 
                }
            }
            else
            {
                throw new Exception(
                    $"Error getting data from Wildberries server! | {ProductsStocksJson.Content} "
                );
                
            } 
        }
        return allProductsInfosList;
    }

    //convert the json response from WB API to a list of ProductInfo objects
    public static List<ProductInfo>? ParseProductInfo(string json)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var response = JsonSerializer.Deserialize<ApiResponse>(json, options);

        var products = response
            ?.Data.Items.Select(item => new ProductInfo
            {
                NmID = item.NmID,
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
