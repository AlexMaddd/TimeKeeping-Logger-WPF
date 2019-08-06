using MySql.Data.MySqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFTimeKeeping
{
    /// <summary>
    /// Interaction logic for UserLog.xaml
    /// </summary>
    public partial class UserLog : Page
    {
        public static string connString = "server=localhost;database=timekeeping;username=root;password=";

        public UserLog()
        {
            InitializeComponent();
        }

        //public UserLog(DataRowView row) : this()
        //{
        //    //MessageBox.Show(row[1].ToString());
        //    this.DataContext = row;
        //}

        public UserLog(int data) : this()
        {
            IdNumber idnumber = new IdNumber();
            idnumber.IDNumber = data;
            //MessageBox.Show(row[1].ToString());
            idNum.DataContext = idnumber;
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connString);

            conn.Open();

            //int id = Convert.ToInt32(idN.Text);
            int idNumber = Convert.ToInt32(idNum.Text);
            //string status = txtBox.Text.ToUpper();


            string query = string.Format("UPDATE timetable SET status = @status WHERE id_number = @idnumber AND id = @id ");

            MySqlCommand cmd = new MySqlCommand(query, conn);

            //cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@idnumber", idNumber);

            cmd.ExecuteNonQuery();

            cmd.Dispose();

            conn.Close();

            Home home = new Home();
            this.NavigationService.Navigate(home);

        }

        private void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            string query = string.Format("INSERT INTO log (id_number, datetime, commentLog) VALUES (@idnumber, @datetime, @comment)");
            MySqlConnection conn = new MySqlConnection(connString);

            conn.Open();

            MySqlCommand cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@idnumber", Convert.ToInt32(idNum.Text));
            cmd.Parameters.AddWithValue("@datetime", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"));
            cmd.Parameters.AddWithValue("@comment", commentLog.Text);

            cmd.ExecuteNonQuery();

            cmd.Dispose();
            conn.Close();

            MessageBox.Show("Log Saved");
        }

        public class IdNumber
        {
            public int IDNumber { get; set; }
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            string query = string.Format("INSERT INTO timetable (id_number, datetime, status) VALUES (@idnumber, @datetime, @status)");

            MySqlConnection conn = new MySqlConnection(connString);

            conn.Open();

            MySqlCommand cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@idnumber", Convert.ToInt32(idNum.Text));
            cmd.Parameters.AddWithValue("@datetime", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"));
            cmd.Parameters.AddWithValue("@status", "OUT");

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            conn.Close();

            MessageBox.Show("Logging Out");

            Home home = new Home();
            this.NavigationService.Navigate(home);

        }

    }
}
