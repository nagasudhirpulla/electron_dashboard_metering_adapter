using AdapterUtils;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ElectronDashboardMeteringDataAdapter
{
    /// <summary>
    /// Interaction logic for MeasPickerWindow.xaml
    /// </summary>
    public partial class MeasPickerWindow : Window
    {
        public ConfigurationManager Config_ { get; set; } = new ConfigurationManager();
        private List<object> MeteringMeasList_ = new List<object>();
        public MeasPickerWindow()
        {
            InitializeComponent();
            Config_.Initialize();
            RefreshMeasurements();
        }

        private void RefreshMeasurements()
        {
            string connString = $"server={Config_.DataHost};user id={Config_.DataUserName};password={Config_.DataPassword};database={Config_.DataDbName};";
            // Connect to a PostgreSQL database
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            conn.Open();

            // Define a query
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT distinct location_id, description FROM public.fict_master_data", conn);

            // Execute a query
            NpgsqlDataReader dr = cmd.ExecuteReader();

            MeteringMeasList_ = new List<object>();
            // Read all rows and output the first column in each row
            while (dr.Read())
            {
                MeteringMeasList_.Add(new
                {
                    Name = dr.GetString(0),
                    Description = dr.GetString(1)
                });
                // Console.WriteLine($"{dr.GetString(0)}, {dr.GetString(1)}, {dr.GetString(2)}");
            }
            MeasListView.ItemsSource = MeteringMeasList_;
            // Close connection
            conn.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ShutdownApp();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            //todo console the selected measurement
            int selectedIndex = MeasListView.SelectedIndex;
            if (selectedIndex > -1)
            {
                object selObj = MeasListView.SelectedItems[0];
                string measId = (string)selObj.GetType().GetProperty("Name").GetValue(selObj, null);
                string measDesc = (string)selObj.GetType().GetProperty("Description").GetValue(selObj, null);
                // measId, measName, measDescription
                ConsoleUtils.FlushMeasData(measId, measId, measDesc);
                ShutdownApp();
            }
            else
            {
                MessageBox.Show("Please select a measurement...");
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            RefreshMeasurements();
        }

        private void FilterTxt_Changed(object sender, RoutedEventArgs e)
        {
            List<object> filteredMeasurements = MeteringMeasList_;
            if (!string.IsNullOrEmpty(NameFilter.Text))
            {
                filteredMeasurements = filteredMeasurements.Where(item =>
                {
                    string measId = (string)item.GetType().GetProperty("Name").GetValue(item, null);
                    return measId.ToUpper().Contains(NameFilter.Text.ToUpper());
                }).ToList();
            }
            if (!string.IsNullOrEmpty(DescFilter.Text))
            {

                filteredMeasurements = filteredMeasurements.Where(item =>
                {
                    string measDesc = (string)item.GetType().GetProperty("Description").GetValue(item, null);
                    return measDesc.ToUpper().Contains(DescFilter.Text.ToUpper());
                }).ToList();
            }
            MeasListView.ItemsSource = filteredMeasurements;
        }

        private void ShutdownApp()
        {
            Application.Current.Shutdown();
        }
    }
}
