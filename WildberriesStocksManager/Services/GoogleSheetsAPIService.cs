using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WildberriesStocksManager.Enums;
using WildberriesStocksManager.Helpers;
using WildberriesStocksManager.Models;
using static System.Formats.Asn1.AsnWriter;

namespace WildberriesStocksManager.Services;

internal class GoogleSheetsAPIService
{
    static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
    static SheetsService service;
    private readonly string googleCredentialsJson;
    

    //get the list of products to check from a Google Sheet , there would be id on WB and the minimal stocks levels
    public static async Task<ProductToCheck[]> GetTargetedProductsList(Stores store)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.Development.json", true, true)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
            .Build();

        Console.WriteLine("Started!");

        GoogleCredential credential;


        // Construct the JSON string for the Google credentials section in user secrets
        string googleCredentialsJson = JsonSerializer.Serialize(config.GetSection("GoogleCredentials").GetChildren()
            .ToDictionary(item => item.Key, item => item.Value));

        // Load the JSON string into a GoogleCredential object
        credential = GoogleCredential.FromJson(googleCredentialsJson).CreateScoped(Scopes);

        //load settings from the appsettings.json file
        string ApplicationName = config["Google:ApplicationName"];
        string SpreadsheetId = config["Google:SpreadsheetId"];
        
        // Create Google Sheets API service.
        service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        IList<IList<object>>? range;
        switch (store)
        {
            case Stores.ArtXL:
                range = GetDataFromGoogleAPI("ArtXL", SpreadsheetId);
                break;
            case Stores.RusDecor:
                range = GetDataFromGoogleAPI("RusDecor", SpreadsheetId);
                break;
            default:
                throw new ArgumentException("Bad store name!");
        }

        var arrayOfProducts = ConvertToProductArray(range);
        return arrayOfProducts;
    }

    private static IList<IList<object>>? GetDataFromGoogleAPI(string sheet,string spreadsheetId)
    {
        // Fetch the sheet data
        string range = $"{sheet}!A:C"; 
        var request = service.Spreadsheets.Values.Get(spreadsheetId, range);

        ValueRange response = request.Execute();
        IList<IList<object>> values = response.Values;

        if (values is not null)
        {
            return values;
        }
        else
        {
            Console.WriteLine("No data found.");
            return null;  
        }

    }

    public static ProductToCheck[] ConvertToProductArray(IList<IList<object>>? input)
    {
        // Validate input
        if (input == null || input.Count <= 1) // No data or only header row
            return Array.Empty<ProductToCheck>();

        // List to hold the converted ProductToCheck objects
        List<ProductToCheck> productList = new List<ProductToCheck>();

        // Skip the first row (header row) and start from the second row
        for (int i = 1; i < input.Count; i++)
        {
            var item = input[i];

            if (item.Count == 3) // Ensure there are exactly three values
            {
                var product = new ProductToCheck
                {
                    NmID = item[0]?.ToString() ?? string.Empty, // Convert first item to string (NmID)
                    MinimumFBOStockLevel = Convert.ToInt32(item[1]), // Convert second item to int (MinimumFBOStockLevel)
                    StockValueForFBS = Convert.ToInt32(item[2]) // Convert third item to int (StockValueForFBS)
                };

                productList.Add(product);
            }
        }

        // Convert list to array and return
        return productList.ToArray();
    }
}
