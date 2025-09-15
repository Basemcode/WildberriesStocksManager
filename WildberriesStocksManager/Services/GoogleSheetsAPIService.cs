using System.Reflection;
using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;
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
        var googleCredentialsJson = JsonSerializer.Serialize(
            config
                .GetSection("GoogleCredentials")
                .GetChildren()
                .ToDictionary(item => item.Key, item => item.Value)
        );

        // Load the JSON string into a GoogleCredential object
        credential = GoogleCredential.FromJson(googleCredentialsJson).CreateScoped(_scopes);

        //load settings from the appsettings.json file
        var ApplicationName = config["Google:ApplicationName"];
        var SpreadsheetId = config["Google:SpreadsheetId"];

        // Create Google Sheets API service.
        _service = new SheetsService(
            new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            }
        );

        IList<IList<object>>? range;

        // Determine which store to use and fetch the data accordingly
        switch (store)
        {
            case Stores.ArtXL:
                range = await GetDataFromGoogleAPIAsync("ArtXL", SpreadsheetId);
                break;
            case Stores.RusDecor:
                range = await GetDataFromGoogleAPIAsync("RusDecor", SpreadsheetId);
                break;
            default:
                throw new ArgumentException("Bad store name!");
        }

        var arrayOfProducts = ConvertToProductArray(range);
        return arrayOfProducts;
    }

    private static async Task<IList<IList<object>>?> GetDataFromGoogleAPIAsync(
        string sheet,
        string spreadsheetId
    )
    {
        // Fetch the sheet data
        string range = $"{sheet}!A:C";
        var request = _service?.Spreadsheets.Values.Get(spreadsheetId, range);

        try
        {
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
        catch (Exception error)
        {
            Console.WriteLine($"can't get info from server! {error.Message}");
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

            ProductToCheck product;

            // performe all the neccesarry checks of the id field then convert it (NmID object) to string
            if (NmIDIndex < item.Count)
            {
                // ToDo : refactor this checking code by invert the logic so it will be shorter  (if > or null or empty => continue)  
                if (item[NmIDIndex] is not null)
                {
                    var nmid = item[NmIDIndex].ToString();
                    if (string.IsNullOrEmpty(nmid))
                    {
                        Console.WriteLine("NmID is empty, skipping this item.");
                        continue; // Skip this item if NmID is empty
                    }
                    else
                    {
                        product = new ProductToCheck()
                        {
                            NmID = nmid
                        };
                    }
                }
                else
                {
                    continue; // Skip this item if NmID is null
                }
            }
            else
            {
                continue; // Skip this item if NmID is not found
            }

            // Convert MinimumFBOStockLevel int
            if (
                MinimumFBOStockLevelIndex < item.Count
            )
            {
                try
                {
                    product.MinimumFBOStockLevel = Convert.ToInt32(item[MinimumFBOStockLevelIndex]);
                }
                catch (Exception error)
                {
                    product.MinimumFBOStockLevel = 0;
                    Console.WriteLine($"Can't parse data from googlesheets in value MinimumFBOStockLevel! {error.Message}");
                }
                
            }

            // Convert StockValueForFBS
            if (StockValueForFBSIndex < item.Count)
            {
                try
                {
                    product.StockValueForFBS = Convert.ToInt32(item[StockValueForFBSIndex]);
                }
                catch (Exception error)
                {
                    product.StockValueForFBS = 0;
                    Console.WriteLine($"Can't parse data from googlesheets in value StockValueForFBS! {error.Message}");
                }
            }

            productList.Add(product);
        }

        //convert list to array and return
        return productList.ToArray();
    }
}
