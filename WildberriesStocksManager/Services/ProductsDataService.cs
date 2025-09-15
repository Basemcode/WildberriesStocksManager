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
    public static async Task<List<ProductInfo>?> GetProductsInfos(Stores store)
    {
        //determine which products IDs to use bases on the store
        ProductToCheck[] ProductsList;
        ProductsList = await GoogleSheetsAPIService.GetTargetedProductsListAsync(store);

        // get the FBO stock infos
        var FBOStockList = await GetProductsInfosByStockType(store, StockTypes.FBO);

        //
        foreach (var product in FBOStockList)
        {
            product.FPOStockCount = product.ReceivedStockCount;
        }

        // Save results to final list of products infos in dictionary structure
        Dictionary<string, ProductInfo> finalStocksInfo = FBOStockList.ToDictionary(productInfo => productInfo.NmID);

        // Get the FBS stock infos
        var FBSStockList = await GetProductsInfosByStockType(store, StockTypes.FBS);

        // merge the results in one list
        foreach (var product in FBSStockList)
        {
            if (finalStocksInfo.TryGetValue(product.NmID, out var update))
            {
                update.FPSStockCount = product.ReceivedStockCount; // assume you have this method
            }
        }
        


        return await GetProductsInfosByStockType(store , StockTypes.FBO);
    }

    // get the products info from Wildberries API by stock type (FBS or FBO)
    private static async Task<List<ProductInfo>?> GetProductsInfosByStockType(Stores store, string stockType)
    {
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
                allReturned = true;
                Console.WriteLine(
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
                NmID = item.NmID.ToString(),
                SubjectName = item.SubjectName,
                Name = item.Name,
                VendorCode = item.VendorCode,
                BrandName = item.BrandName,
                MainPhoto = item.MainPhoto,
                ReceivedStockCount = item.Metrics?.StockCount ?? 0,
            })
            .ToList();

        return products;
    }
}
