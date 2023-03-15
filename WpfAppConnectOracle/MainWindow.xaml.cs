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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System.Data;

namespace WpfAppConnectOracle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OracleConnection con = null;
        public MainWindow()
        {
            this.setConnection();
            InitializeComponent();
        }

        private void setConnection()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            con = new OracleConnection(connectionString);
            try
            {
                con.Open();
                MessageBox.Show("Connection Success");
            }
            catch (Exception ex) {
                MessageBox.Show("Connection false: "+ex);
            }
        }

        private void updateDataGrid()
        {
            string query = "SELECT * FROM employees";
            OracleDataAdapter adapter = new OracleDataAdapter(query, con);
            DataSet empList = new DataSet();
            adapter.Fill(empList);
            if (empList.Tables.Count > 0)
            {
                myDataGrid.ItemsSource = empList.Tables[0].DefaultView;
            }
            else
            {
                MessageBox.Show("No data found.");
            }
            
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            this.updateDataGrid();
        }

        /* private void button_Click(object sender, RoutedEventArgs e)
        {
            *//*string connectionString = "Data Source=172.25.219.61:1521/xe;User Id=SYS;Password=Odc2023!;DBA Privilege=SYSDBA;";
            OracleConnection connection = new OracleConnection(connectionString);*//*
           
            try
            {
                setConnection();             
                string query = "SELECT * FROM employees";
                OracleDataAdapter adapter = new OracleDataAdapter(query, connection);
                DataSet empList = new DataSet();
                adapter.Fill(empList);

                if (empList.Tables.Count > 0)
                {
                    myDataGrid.ItemsSource = empList.Tables[0].DefaultView;
                }
                else
                {
                    MessageBox.Show("No data found.");
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to Oracle: " + ex.Message);
                Console.WriteLine("Connect failed: " + ex);
            }

        }*/

    }
}
