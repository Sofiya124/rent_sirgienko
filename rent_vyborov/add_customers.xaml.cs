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

namespace rent_vyborov
{
    /// <summary>
    /// Логика взаимодействия для add_customers.xaml
    /// </summary>
    public partial class add_customers : Page
    {
        public add_customers()
        {
            InitializeComponent();
        }

        Notification ntf = new Notification();

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mail.Text != "" || fio.Text != "" || adres.Text != "" || date.Text != "" || ser_pass.Text != "" || num_pass.Text != "" || phone.Text != "")
                {
                    string max_id = SQLclass.Select("SELECT MAX(id) FROM Customers")[0];
                    string photo = "Стандарт" + (max_id + 1) + ".jpeg";

                    string[] subs = fio.Text.Split(' ');

                    string family = subs[0];
                    string name = subs[1];
                    string patronymic = subs[2];


                    SQLclass.Insert("INSERT INTO Customers(surname, name, patronymic, pass_series, pass_number, birthday, addres, email, password, photo)" +
                        " VALUES(N'" + family + "'," +
                        "N'" + name + "'," +
                        "N'" + patronymic + "'," +
                        "'" + ser_pass.Text + "'," +
                        "'" + num_pass.Text + "'," +
                        "'" + date.Text + "'," +
                        "N'" + adres.Text + "'," +
                        "'" + mail.Text + "'," +
                        "'123123'," +
                        "N'" + photo + "')");

                    ntf.Notify("Пополнение в базе", "Вы успешно добавили нового пользователя");
                    StaticVars.MainWnd.Content = new order_panel();
                }
                else
                {
                    ntf.Notify("Произошла ошибка", "Пожалуйста заполните все данные");
                }
        }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
}

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            StaticVars.MainWnd.Content = new order_panel();
        }
    }
}
