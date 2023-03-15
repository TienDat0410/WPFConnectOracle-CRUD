using System;
using System.Collections.Generic;
using System.Data;
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
using Oracle.ManagedDataAccess.Types;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;

namespace WpfAppConnectOracle
{
    /// <summary>
    /// Interaction logic for WindowCon.xaml
    /// </summary>
    public partial class WindowCon : Window
    {
        OracleConnection connection = null;
        public WindowCon()
        {
            this.setConnection();
            InitializeComponent();
        }

        private void updateDataGrid()
        {
            OracleCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT EMPLOYEE_ID, FIRST_NAME, LAST_NAME, HIRE_DATE FROM EMPLOYEES ORDER BY EMPLOYEE_ID ASC";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            myDataGrid.ItemsSource = dt.DefaultView;
            dr.Close();
        }

        private void setConnection()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            connection = new OracleConnection(connectionString);
            try
            {
                connection.Open();
                MessageBox.Show("Success to connect to Oracle");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection false: " + ex);
            }
        }

        private void CRUD(String sql_stmt, int state)
        {
            String msg = "";
            OracleCommand cmd = connection.CreateCommand();
            cmd.CommandText = sql_stmt;
            cmd.CommandType = CommandType.Text;

            try {
                switch (state)
                {
                    case 0:
                        msg = "Row Insertd Successfully!";
                        cmd.Parameters.Add("EMPLOYEE_ID", OracleDbType.Int32, 6).Value = Int32.Parse(txbEmpID.Text);
                        cmd.Parameters.Add("FIRST_NAME", OracleDbType.Varchar2, 25).Value = txbFirstName.Text;
                        cmd.Parameters.Add("LAST_NAME", OracleDbType.Varchar2, 25).Value = txblastName.Text;
                        cmd.Parameters.Add("HIRE_DATE", OracleDbType.Date, 7).Value = datePickerHireDate.SelectedDate;
                        break;
                    case 1:
                        msg = "Row Update Successfully!";
                        cmd.Parameters.Add("FIRST_NAME", OracleDbType.Varchar2, 25).Value = txbFirstName.Text;
                        cmd.Parameters.Add("LAST_NAME", OracleDbType.Varchar2, 25).Value = txblastName.Text;
                        cmd.Parameters.Add("HIRE_DATE", OracleDbType.Date, 7).Value = datePickerHireDate.SelectedDate;

                        cmd.Parameters.Add("EMPLOYEE_ID", OracleDbType.Int32, 6).Value = Int32.Parse(txbEmpID.Text);
                        break;
                    case 2:
                        msg = "Row Deleted Successfully!";
                        cmd.Parameters.Add("EMPLOYEE_ID", OracleDbType.Int32, 6).Value = Int32.Parse(txbEmpID.Text);
                        break;
                }
            } catch (Exception ex)
            {
                MessageBox.Show("Some error: "+ex);
            }

            
            try
            {
                int n = cmd.ExecuteNonQuery();
                if (n > 0)
                {
                    MessageBox.Show(msg);
                    this.updateDataGrid();
                }
                else
                {
                    MessageBox.Show("Failed to change data.");
                }
            }
            catch(Exception ex) {

            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string sql = "INSERT INTO employees (EMPLOYEE_ID, FIRST_NAME, LAST_NAME, HIRE_DATE) " +
                "VALUES (:EMPLOYEE_ID, :FIRST_NAME, :LAST_NAME, :HIRE_DATE)";
            this.CRUD(sql, 0);
            btnAdd.IsEnabled = false;
            btnUpdate.IsEnabled = true;
            btndelete.IsEnabled = true;

        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            string sql = "UPDATE employees SET FIRST_NAME=:FIRST_NAME, LAST_NAME=:LAST_NAME, HIRE_DATE=:HIRE_DATE WHERE EMPLOYEE_ID=:EMPLOYEE_ID";
            this.CRUD(sql, 1);

        }

        private void btndelete_Click(object sender, RoutedEventArgs e)
        {
            string sql = "DELETE FROM employees WHERE EMPLOYEE_ID=:EMPLOYEE_ID";
            this.CRUD(sql, 2);
            this.resetAll();
        }

        private void resetAll()
        {
            txbEmpID.Text = "";
            txbFirstName.Text = "";
            txblastName.Text = "";
            datePickerHireDate.SelectedDate = null;

            btnAdd.IsEnabled = true;
            btnUpdate.IsEnabled = false;
            btndelete.IsEnabled = false;
        }


        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            this.updateDataGrid();
        }

        private void myDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                txbEmpID.Text = dr["EMPLOYEE_ID"].ToString();
                txbFirstName.Text = dr["FIRST_NAME"].ToString();
                txblastName.Text = dr["LAST_NAME"].ToString() ;
                datePickerHireDate.SelectedDate = DateTime.Parse(dr["HIRE_DATE"].ToString());

                btnAdd.IsEnabled = false;
                btnUpdate.IsEnabled = true;
                btndelete.IsEnabled = true; 

            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            this.resetAll();
        }

        private void Canvas_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            connection.Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            connection.Close();
            Application.Current.Shutdown();
        }
    }
}
