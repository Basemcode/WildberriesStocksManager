using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;

using WildberriesStocksManager.Helpers;
using WildberriesStocksManager.Enums;

namespace WildberriesStocksManager.Services
{
    internal class APIService
    {
        public static async Task<RestResponse> GetStockReportAsync(int[] ProductsList,Stores store)
        {
            var config = new ConfigurationBuilder()
              .AddUserSecrets<APIService>()
              .Build();

            //set the api token for the chosen store from the secrets.json file
            var ApiToken=string.Empty;
            switch (store)
            {
                case Stores.ArtXL:
                    ApiToken = config["Wildberries:ArtXLAPIToken"];
                    break;
                case Stores.RusDecor:
                    ApiToken = config["Wildberries:RusDecorAPIToken"];
                    break;
                default:
                    throw new ArgumentException("Invalid store Argument in GetStockReportAsync()");
                    break;
            }
            

            var options = new RestClientOptions("https://seller-analytics-api.wildberries.ru")
            {
                MaxTimeout = -1
            };
            var client = new RestClient(options);

            var request = new RestRequest("/api/v2/stocks-report/products/products", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {ApiToken}");

            string today = DateTime.Today.ToString("yyyy-MM-dd");

            var requestBody = new
            {
                nmIDs = ProductsList,
                currentPeriod = new
                {
                    start = today,
                    end = today
                },
                stockType = "mp",
                skipDeletedNm = true,
                orderBy = new
                {
                    field = "avgOrders",
                    mode = "asc"
                },
                availabilityFilters = new[] {
                    "deficient", "actual", "balanced", "nonActual", "nonLiquid", "invalidData"
                },
                offset = 0
            };

            request.AddJsonBody(requestBody);

            RestResponse response = await client.ExecuteAsync(request);

           return response;
        }
    }
}
