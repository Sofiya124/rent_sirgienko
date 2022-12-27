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

namespace rent_vyborov
{
    /// <summary>
    /// Логика взаимодействия для history_page.xaml
    /// </summary>
    public partial class history_page : Page
    {
        public history_page()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // событие при открытии страницы
            try
            {
                GetDataLogin(); // формирование списка истории входов
                GetDataAuth(""); // фильтрация списка по конкретному сотруднику
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }           
        }

        public void GetDataLogin()
        {
            SqlDataAdapter a = new SqlDataAdapter("SELECT login AS log, id FROM Employees", SQLclass.conn); // запрос в базу данных

            DataTable table_p = new DataTable();
            a.Fill(table_p);

            login.DisplayMemberPath = "log";
            login.ItemsSource = table_p.AsDataView();
            login.SelectedIndex = -1;
        }

        public void GetDataAuth(string type)
        {
            SqlCommand query = new SqlCommand($"SELECT Logins_system.id AS '№', Employees.login AS 'Логин', Logins_system.times AS 'Время', Status_system.name AS 'Дата' FROM Logins_system JOIN Employees ON Employees.id = Logins_system.id_employees JOIN Status_system ON Status_system.id = Logins_system.id_sys_status {type}", SQLclass.conn); // запрос в базу данных
            query.ExecuteNonQuery();

            SqlDataAdapter dataAdp = new SqlDataAdapter(query);
            DataTable dt = new DataTable();
            dataAdp.Fill(dt);
            data.ItemsSource = dt.DefaultView;
        }

        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            // событие при фильтрации списка
            GetDataAuth($"WHERE Employees.login = '{login.Text}'"); 
        }
    }
}
