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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SQLclass.OpenConnection(); // открытие соединения с базой данных
        }

        Notification ntf = new Notification();

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_hidden_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btn_auth_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StaticVars.Login = mail.Text; // сохранение глобальной переменной
                string existUser_Login = SQLclass.Select("SELECT COUNT(*) FROM [dbo].[Employees] WHERE login = '" + mail.Text + "'")[0]; // проверка на введенный логин в бд
                StaticVars.UserId = SQLclass.Select("SELECT MAX(id) FROM Employees WHERE login = '" + mail.Text + "'")[0]; // запись переменной id пользователя

                if (existUser_Login != "0") // Если логин существует в бд
                {
                    string existUser_Password = SQLclass.Select("SELECT COUNT(*) FROM [dbo].[Employees] WHERE password='" + password.Password + "'")[0]; // проверка на введенный пароль в бд

                    if (existUser_Password != "0") // Если пароль введен верно
                    {
                        if (StaticVars.BlockTime > 0) // Проверка на блокировку входа
                        {
                            ntf.Notify("Вход заблокирован", "До разблокировки осталось: " + StaticVars.BlockTime + " минут"); // уведомление
                        }
                        else
                        {
                            // вход в аккаунт, запись данных в переменные и открытие основной панели
                            List<string> userData = SQLclass.Select(
                    "SELECT Employees.name, Employees.surname, Employees.id_post, Post.name FROM Employees JOIN Post ON Employees.id_post = Post.id WHERE Employees.id = '" + StaticVars.UserId + "'");

                            StaticVars.UserName = userData[0];
                            StaticVars.UserSurname = userData[1];
                            StaticVars.UserIDStatus = userData[2];
                            StaticVars.UserStatus = userData[3];

                            ntf.Notify("Добро пожаловать", "Желаем вам хорошего рабочего дня!");

                            LogAuth(1); // запись истории входа

                            this.Hide();
                            User_Card user_Card = new User_Card();
                            user_Card.Owner = this;
                            user_Card.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                            user_Card.ShowDialog();                          
                        }                       
                    }
                    else
                    {
                        ntf.Notify("Произошла ошибка", "Вы указали неверный пароль!"); // уведомление
                        LogAuth(2); // запись истории входа
                        CheckBadLogin(); // Проверка на количество неудачных входов
                    }
                }
                else
                {
                    ntf.Notify("Добро пожаловать", "Желаем вам хорошего рабочего дня!"); // уведомление
                    CheckBadLogin(); // Проверка на количество неудачных входов
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void CheckBadLogin()
        {
            if (StaticVars.BadLogin != 2) // если время блокировки не равно 2 минутам
            {
                StaticVars.BadLogin++; // минуты прибавляются
            }
            else
            {
                // открытие формы капчи
                this.Hide();
                Captcha captcha = new Captcha();
                captcha.Owner = this;
                captcha.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                captcha.ShowDialog();
            }
        }

        public void LogAuth(int type)
        {
            // запись истории входа в бд
            SQLclass.Insert("INSERT INTO Logins_system(id_employees, times, id_sys_status) VALUES('" + StaticVars.UserId + "', GETDATE() ,'" + type + "')");
        }

        private void btn_show_Click(object sender, RoutedEventArgs e)
        {
            // просмотр пароля
            if (password.Password != "")
            {
                MessageBox.Show(password.Password, "Введенный пароль");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // событие при загрузке программы
            GetBlock(); 
            StaticVars.BadLogin = 0;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            GetBlock();
            StaticVars.BadLogin = 0;
        }

        public void GetBlock()
        {
            // Запуск таймера блокировки входа
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 1, 0);
            timer.Start();
        }

        private void timerTick(object sender, EventArgs e)
        {
            // проверка на блокировку входа
            if (StaticVars.BlockTime > 0)
            {
                StaticVars.BlockTime--;
            }
        }
    }
}
