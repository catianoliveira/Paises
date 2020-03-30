using Paises.Modelos;
using Paises.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Paises
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Attributes

        //private NetworkService new

        private List<Country> Countries;

        private NetworkService networkService;

        private ApiService apiService;

        private DialogService dialogService;

        //TODO private DataService dataService;

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            dialogService = new DialogService();
            networkService = new NetworkService();
            apiService = new ApiService();
            //TODO dataService = new DataService();
            LoadCountries();
        }

        private async void LoadCountries()
        {
            bool load;

            //TODO lblResultado.Text = "Loading countries...";

            var connection = networkService.CheckConnection();

            if (!connection.IsSuccess)
            {
                LoadLocalCountries();
                load = false;
            }

            else
            {
                await LoadApi();
                load = true;
            }

            if (Countries.Count == 0)
            {
                //lblResultado.Text = "Não há ligação à Internet" + Environment.NewLine
                //    + "e não foram previamente carregadas as taxas." + Environment.NewLine +
                //    "Tente mais tarde!";

                //lblStatus.Text = "Primeira inicialização deverá ter ligação à internet";
                return;
            }

            cbCountries.ItemsSource = Countries;
            cbCountries.DisplayMemberPath = "name";

            //lblResultado.Text = "Countries loaded";

            if (load)
            {
                //lblStatus.Text = string.Format($"Taxas carregadas da internet em {DateTime.Now}");
            }

            else
            {
                //lblStatus.Text = string.Format("Taxas carregadas da Base de Dados");
            }
        }

        private async Task LoadApi()
        {
            var response = await apiService.GetCountries("http://restcountries.eu/", "rest/v2/all");

            Countries = (List<Country>)response.Result;

            //dataService.DeleteData();
            //dataService.SaveData(Countries);
        }

        private void LoadLocalCountries()
        {
            //TODO Countries = dataService.GetData();
        }
        private void cbCountries_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            LoadCountryInfo();
        }

        private void LoadCountryInfo()
        {
            lblCapital.Content = $"Capital: {Countries[cbCountries.SelectedIndex].capital}";
            lblRegion.Content = $"Region: {Countries[cbCountries.SelectedIndex].region}";
            lblSubregion.Content = $"Subregion: {Countries[cbCountries.SelectedIndex].subregion}";
            lblPopulation.Content = $"Population: {Countries[cbCountries.SelectedIndex].population}";
            lblGini.Content = $"Gini: {Countries[cbCountries.SelectedIndex].gini}";
            //lblFlag.Flag = $"Capital: {Countries[cbCountries.SelectedIndex].flag}";
        }
    }
}
