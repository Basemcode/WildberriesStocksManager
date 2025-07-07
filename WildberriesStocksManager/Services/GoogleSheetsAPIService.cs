using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Text.Json;
using WildberriesStocksManager.Enums;
using WildberriesStocksManager.Models;


namespace WildberriesStocksManager.Services;

internal class GoogleSheetsAPIService
{
    private static readonly string[] _scopes = [SheetsService.Scope.SpreadsheetsReadonly];
    private static SheetsService? _service;

    //get the list of products to check from a Google Sheet , there would be id on WB and the minimal stocks levels
    public static async Task<ProductToCheck[]> GetTargetedProductsListAsync(Stores store)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.Development.json", true, true)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
            .Build();

        Console.WriteLine("Started!");

        GoogleCredential credential;


        // Construct the JSON string for the Google credentials section in user secrets
        var googleCredentialsJson = JsonSerializer.Serialize(config.GetSection("GoogleCredentials").GetChildren()
            .ToDictionary(item => item.Key, item => item.Value));

        // Load the JSON string into a GoogleCredential object
        credential = GoogleCredential.FromJson(googleCredentialsJson).CreateScoped(_scopes);

        //load settings from the appsettings.json file
        var ApplicationName = config["Google:ApplicationName"];
        var SpreadsheetId = config["Google:SpreadsheetId"];
        
        // Create Google Sheets API service.
        _service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        IList<IList<object>>? range;

        // Determine which store to use and fetch the data accordingly
        switch (store)
        {
            case Stores.ArtXL:
                range = await GetDataFromGoogleAPIAsync("ArtXL", SpreadsheetId);
                break;
            case Stores.RusDecor:
                range =await GetDataFromGoogleAPIAsync("RusDecor", SpreadsheetId);
                break;
            default:
                throw new ArgumentException("Bad store name!");
        }

        var arrayOfProducts = ConvertToProductArray(range);
        return arrayOfProducts;
    }

    private static async Task<IList<IList<object>>?> GetDataFromGoogleAPIAsync(string sheet,string spreadsheetId)
    {
        // Fetch the sheet data
        string range = $"{sheet}!A:C"; 
        var request = _service?.Spreadsheets.Values.Get(spreadsheetId, range);

        ValueRange response = await request.ExecuteAsync();
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
        //salidate input
        if (input == null || input.Count <= 1) // No data or only header row
            return Array.Empty<ProductToCheck>();

        // List to hold the converted ProductToCheck objects
        List<ProductToCheck> productList = new List<ProductToCheck>();

        //get the headers order 
        var headers = input[0];
        int NmIDIndex = headers.IndexOf("Артикул WB");
        int MinimumFBOStockLevelIndex = headers.IndexOf("Минимальный Остаток FBO");
        int StockValueForFBSIndex = headers.IndexOf("Остаток FPS");

        //skip the first row (header row) and start from the second row
        for (int i = 1; i < input.Count; i++)
        {
            var item = input[i];
         
            var product = new ProductToCheck
            {
                NmID = item[NmIDIndex]?.ToString() ?? string.Empty, // Convert first item to string (NmID)
                MinimumFBOStockLevel = Convert.ToInt32(item[MinimumFBOStockLevelIndex]), // Convert second item to int (MinimumFBOStockLevel)
                StockValueForFBS = Convert.ToInt32(item[StockValueForFBSIndex]) // Convert third item to int (StockValueForFBS)
            };

            productList.Add(product);
         
        }

        //convert list to array and return
        return productList.ToArray();
    }
}
