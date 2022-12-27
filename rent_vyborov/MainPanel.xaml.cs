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

namespace rent_vyborov
{
    /// <summary>
    /// Логика взаимодействия для MainPanel.xaml
    /// </summary>
    public partial class MainPanel : Window
    {
        public MainPanel()
        {
            InitializeComponent();
            StaticVars.MainWnd = Frame;
        }

        Notification ntf = new Notification();

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        int hour, min;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 1, 0);
            timer.Start();

            hour = 0;
            min = 2;

            Frame.Content = new order_panel();
        }

        private void timerTick(object sender, EventArgs e)
        {
            if (hour == 2 && min == 0)
            {
                hour = 1;
                min = 59;
            }
            else if (hour == 1 && min == 0)
            {
                hour = 0;
                min = 59;
            }

            if (hour == 0 && min == 1)
            {
                ntf.Notify("Время сеанса заканчивается", "Пожалуйста успейте сохранить все данные!");
            }
            else if (hour == 0 && min == 0)
            {
                StaticVars.BlockTime = 3;
                BackMainWnd();
            }

            if (min < 10)
            {
                txt_timer.Text = (hour + ":0" + (min--)).ToString();
            }
            else
            {
                txt_timer.Text = (hour + ":" + (min--)).ToString();
            }          
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            BackMainWnd();
        }

        private void btn_hidden_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            BackMainWnd();
        }

        private void btn_create_Click(object sender, RoutedEventArgs e)
        {
            if (StaticVars.UserIDStatus == "1" || StaticVars.UserIDStatus == "3")
            {
                Frame.Content = new order_panel();
            }
            else
            {
                ntf.Notify("Произошла ошибка", "Извините, данный раздел вам недоступен!");
            }
        }

        private void btn_plus_Click(object sender, RoutedEventArgs e)
        {
            ntf.Notify("Произошла ошибка", "Извините, данный раздел находится в разработке!");
        }

        private void btn_report_Click(object sender, RoutedEventArgs e)
        {
            ntf.Notify("Произошла ошибка", "Извините, данный раздел находится в разработке!");
        }

        private void btn_history_Click(object sender, RoutedEventArgs e)
        {
            if (StaticVars.UserIDStatus == "2")
            {
                Frame.Content = new history_page();
            }
            else
            {
                ntf.Notify("Произошла ошибка", "Извините, данный раздел вам недоступен!");
            }
        }

        public void BackMainWnd()
        {
            this.Hide();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Owner = this;
            mainWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            mainWindow.ShowDialog();
        }
    }
}
