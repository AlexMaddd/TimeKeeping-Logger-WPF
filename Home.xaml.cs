using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace WPFTimeKeeping
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public string connString = "server=localhost;database=timekeeping;username=root;password=";

        public Home()
        {
            InitializeComponent();

            Load_data();
        }

        public void Load_data()
        {
            string query = "SELECT * FROM timetable";

            MySqlConnection conn = new MySqlConnection(connString);

            conn.Open();

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);

                cmd.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable("timetable");

                da.Fill(dt);

                dataGrid.ItemsSource = dt.DefaultView;
                da.Update(dt);

                conn.Close();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }


        }

        private void BtnLog_Click(object sender, RoutedEventArgs e)
        {
            string querySelect = string.Format("SELECT id FROM idnumbers WHERE id_number = @input");
            string queryInsert = string.Format("INSERT INTO timetable (id_number, datetime, status) VALUES (@idnumber, @datetime, @status)");
            int input = Convert.ToInt32(txtBox.Text);

            MySqlConnection conn = new MySqlConnection(connString);

            conn.Open();

            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    MySqlCommand cmd = new MySqlCommand(querySelect, conn);
                    
                    cmd.Parameters.AddWithValue("@input", input);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (Convert.ToInt32(reader.HasRows) == 1)
                    {
                        reader.Dispose();
                        reader.Close();
                        cmd.Dispose();

                        if (btnLog.Content.ToString() == "Login")
                        {
                            cmd.CommandText = queryInsert;

                            cmd.Parameters.AddWithValue("@idnumber", input);
                            cmd.Parameters.AddWithValue("@datetime", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"));
                            cmd.Parameters.AddWithValue("@status", "IN");

                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            MessageBox.Show("Logged In");

                            UserLog ul = new UserLog(input);
                            this.NavigationService.Navigate(ul);
                            
                        }
                        else if (btnLog.Content.ToString() == "Logout")
                        {
                            cmd.CommandText = queryInsert;

                            cmd.Parameters.AddWithValue("@idnumber", input);
                            cmd.Parameters.AddWithValue("@datetime", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"));
                            cmd.Parameters.AddWithValue("@status", "OUT");

                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            MessageBox.Show("Logged Out");

                            btnLog.Content = "Login";

                            txtBox.Text = "";
                        }

                        conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("No existing ID#");
                    }

                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }

            Load_data();

        }

        private void TxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string querySelect = string.Format("SELECT idnumbers.id_number, timetable.status, timetable.datetime " +
                                               "FROM idnumbers INNER JOIN timetable " +
                                               "ON idnumbers.id_number = timetable.id_number " +
                                               "WHERE idnumbers.id_number = @input " +
                                               "ORDER BY timetable.datetime DESC " +
                                               "LIMIT 1"
                                               );   // LIMIT 1 to select top result out of list of results


            MySqlConnection conn = new MySqlConnection(connString);

            conn.Open();

            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    string input = Convert.ToString(txtBox.Text);

                    if (input.Length == 6)
                    {
                        MySqlCommand cmd = new MySqlCommand(querySelect, conn);

                        cmd.Parameters.AddWithValue("@input", input);

                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())   // use .Read() when going through multiple data sets
                        {
                            if (input == reader["id_number"].ToString())
                            {

                                if (reader["status"].ToString() == "IN")
                                {
                                    DateTime time1 = DateTime.ParseExact(reader["datetime"].ToString(), "MM/dd/yyyy hh:mm:ss tt", null); // last recorded time
                                    DateTime time2 = DateTime.Now; // 24 hour limit
                                    TimeSpan span = time2.Subtract(time1);
                                    double hours = span.TotalHours;

                                    if (hours > 24)
                                    {
                                        btnLog.Content = "Login";
                                    }
                                    else
                                    {
                                        btnLog.Content = "Logout";
                                    }
                                }
                                else if (reader["status"].ToString() == "OUT")
                                {
                                    btnLog.Content = "Login";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }

        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //DataGrid dg = sender as DataGrid;
            //DataRowView row = dg.SelectedItem as DataRowView;

            //UserLog ul = new UserLog(row);
            //this.NavigationService.Navigate(ul);
        }

        
    }

}
