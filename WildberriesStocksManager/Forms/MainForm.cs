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
        /*try
        {*/
            var store = cbStore.SelectedItem switch
            {
                "ArtXL" => Stores.ArtXL,
                "RusDecor" => Stores.RusDecor,
                _ => throw new ArgumentException("Invalid store selected")
            };

            var productsInfoList = await ProductsDataService.GetProductsInfos(store);

            dgvProductsInfo.DataSource = productsInfoList;
        /*}
        catch (Exception ee)
        {
            MessageBox.Show(ee.Message);
        }*/
        
    }
}
