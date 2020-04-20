using Paises.Modelos;
using Paises.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Paises
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Attributes

        private List<Country> Countries;

        private NetworkService networkService;

        private ApiService apiService;

        private DialogService dialogService;

        private DataService dataService;

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            networkService = new NetworkService();
            apiService = new ApiService();
            dataService = new DataService();

            LoadCountries();
        }

        private async void LoadCountries()
        {
            dataService.CreateDataCountries();
            dataService.CreateDataTranslations();

            bool load;

            lblResult.Content = "Loading Countries...";

            var connection = networkService.CheckConnection();

            if (!connection.IsSuccess)
            {
                LoadLocalCountries();
                load = false;
            }

            else
            {
                await LoadApiCountries();
                load = true;
            }

            if (Countries.Count == 0)
            {
                lblResult.Content = "There's no internet connection and the countries were not previously loaded." +
                                   Environment.NewLine + "Try again later.";

                lblStatus.Content = "You need to have internet connection for the first boot.";
                return;
            }

            cbCountries.ItemsSource = Countries;
            cbCountries.DisplayMemberPath = "Name";

            lblResult.Content = "Countries loaded!";

            if (load)
            {
                lblStatus.Content = string.Format($"Countries loaded in {DateTime.Now}");
            }
            else
            {
                lblStatus.Content = "Countries loaded from database.";
            }
        }

        private async Task LoadApiCountries()
        {
            var response = await apiService.GetCountries("http://restcountries.eu/rest/v2/", "all");

            Countries = (List<Country>)response.Result;
            dataService.DeleteData();
            dataService.SaveDataCountries(Countries);
        }

        private void LoadLocalCountries()
        {
            Countries = dataService.GetData();
        }

        private void CbCountries_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            LoadCountryInfo();
        }

        private void LoadCountryInfo()
        {
            lblCapital.Content = $"Capital: {Countries[cbCountries.SelectedIndex].Capital}";
            lblRegion.Content = $"Region: {Countries[cbCountries.SelectedIndex].Region}";
            lblSubregion.Content = $"Subregion: {Countries[cbCountries.SelectedIndex].Subregion}";
            lblPopulation.Content = $"Population: {Countries[cbCountries.SelectedIndex].Population}";
            lblGini.Content = $"Gini: {Countries[cbCountries.SelectedIndex].Gini}";

            //Abrir numa nova janela com tradutor ou wtr
            lblDE.Content = $"German: {Countries[cbCountries.SelectedIndex].Translations.De}";
            lblES.Content = $"Spanish: {Countries[cbCountries.SelectedIndex].Translations.Es}";
            lblFR.Content = $"French: {Countries[cbCountries.SelectedIndex].Translations.Fr}";
            lblJA.Content = $"Japanese: {Countries[cbCountries.SelectedIndex].Translations.Ja}";
            lblIT.Content = $"Italian: {Countries[cbCountries.SelectedIndex].Translations.It}";
            lblBR.Content = $"Brazilian: {Countries[cbCountries.SelectedIndex].Translations.Br}";
            lblPT.Content = $"Portuguese: {Countries[cbCountries.SelectedIndex].Translations.Pt}";
            lblNL.Content = $"Dutch: {Countries[cbCountries.SelectedIndex].Translations.Nl}";
            lblHR.Content = $"Croatian: {Countries[cbCountries.SelectedIndex].Translations.Hr}";
            lblFA.Content = $"Arabian: {Countries[cbCountries.SelectedIndex].Translations.Fa}";

            try
            {
                imgFlag.Source = new BitmapImage(new Uri($@"\Images\Flags\{Countries.name.ToLower()}.png", UriKind.Relative));
            }
            catch (Exception)
            {
                imgFlag.Source = new BitmapImage(new Uri(@"\Images\Flags\portugal.jpg", UriKind.Relative));
            }



            //byte[] pngBytes = new HtmlToImageConverter().GenerateImageFromFile($"Images/Flags/{cbCountries.SelectedItem}.svg,");
            //var ms = new MemoryStream(pngBytes);
            //Bitmap bmp = new Bitmap(ms);


            //bmp.Save($"img{imgControl}.jpg");

            //BitmapImage bitmapImage = new BitmapImage();

            //bitmapImage.BeginInit();
            //bitmapImage.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory +
            //    $"img{imgControl}.jpg", UriKind.Absolute);
            //bitmapImage.EndInit();

            //imgFlag.Source = bitmapImage;
        }
    }
}
