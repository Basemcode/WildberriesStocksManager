using WildberriesStocksManager.Enums;
using WildberriesStocksManager.Helpers;
using WildberriesStocksManager.Services;

namespace WildberriesStocksManager;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    private async void btnGetData_Click(object sender, EventArgs e)
    {
        var productsInfoList = await ProductsDataService.GetProductsInfos();  
        dgvProductsInfo.DataSource = productsInfoList;
    }
}
