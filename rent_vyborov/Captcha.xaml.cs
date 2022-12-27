using System;
using System.Windows;
using System.Windows.Input;

namespace rent_vyborov
{
    /// <summary>
    /// Логика взаимодействия для Captcha.xaml
    /// </summary>
    public partial class Captcha : Window
    {
        public Captcha()
        {
            InitializeComponent();
        }

        Notification ntf = new Notification();
        Random rnd = new Random();

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateCaptcha();
        }

        string[] symbol = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "B", "c", "D", "e", "F", "g", "H", "i", "J", "!", "@", "#", "$", "%", "№", "*", "+", "-", "=" };

        public void CreateCaptcha()
        {
            sybol_one.Text = symbol[rnd.Next(symbol.Length)];
            sybol_two.Text = symbol[rnd.Next(symbol.Length)];
            sybol_three.Text = symbol[rnd.Next(symbol.Length)];

            int random = rnd.Next(0,2);

            if (random == 0)
            {
                sybol_one.TextDecorations = TextDecorations.Strikethrough;
            }
            else if (random == 1)
            {
                sybol_two.TextDecorations = TextDecorations.Strikethrough;
            }
            else
            {
                sybol_three.TextDecorations = TextDecorations.Strikethrough;
            }           
        }

        private void btn_recaptcha_Click(object sender, RoutedEventArgs e)
        {
            CreateCaptcha();
        }

        private void btn_check_Click(object sender, RoutedEventArgs e)
        {
            string result = sybol_one.Text + sybol_two.Text + sybol_three.Text;

            if (tb_result.Text != "")
            {
                if (tb_result.Text != result)
                {
                    StaticVars.BlockTime = 1;
                }

                this.Hide();
                MainWindow mainWindow = new MainWindow();
                mainWindow.Owner = this;
                mainWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                mainWindow.ShowDialog();
            }
            else
            {
                ntf.Notify("Произошла ошибка", "Пожалуйста введите ответ");
            }
        }
    }
}
