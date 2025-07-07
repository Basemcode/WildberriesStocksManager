﻿using System;
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
    public static async Task<List<ProductInfo>> GetProductsInfos(Stores store, string stockType)
    {
        //determine which products IDs to use bases on the store
        ProductToCheck[] ProductsList;
        ProductsList =await GoogleSheetsAPIService.GetTargetedProductsList(store);
        /*switch (store)
        {
            case Stores.ArtXL:
                ProductsList = ProductsDetails.ArtXLProductsList;
                break;
            case Stores.RusDecor:
                ProductsList = ProductsDetails.RusDecorProductsList;
                break;
            default:
                throw new ArgumentException("Bad store name!");
        }*/

        int[] listOfIds = ProductsList.Select(p => Convert.ToInt32(p.NmID)).ToArray();
        var ProductsStocksJson = await WBAPIService.GetStockReportAsync(
            listOfIds,
            store,
            stockType
        );

        if (ProductsStocksJson.IsSuccessStatusCode && ProductsStocksJson.Content is not null)
        {
            return ParseProductInfo(ProductsStocksJson.Content);
        }
        else 
        {
            throw new Exception($"Error getting data from Wildberries server! | {ProductsStocksJson.Content} ");
        }
    }

    //convert the json response from WB API to a list of ProductInfo objects
    public static List<ProductInfo> ParseProductInfo(string json)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var response = JsonSerializer.Deserialize<ApiResponse>(json, options);

        var products = response
            .Data.Items.Select(item => new ProductInfo
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
