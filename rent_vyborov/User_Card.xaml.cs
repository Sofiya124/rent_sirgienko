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
    /// Логика взаимодействия для User_Card.xaml
    /// </summary>
    public partial class User_Card : Window
    {
        public User_Card()
        {
            InitializeComponent();
        }

        private void btn_resume_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainPanel mainPanel = new MainPanel();
            mainPanel.Owner = this;
            mainPanel.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            mainPanel.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                profile_photo.Source = (new ImageSourceConverter()).ConvertFromString("../../Resources/" + StaticVars.UserSurname + ".jpeg") as ImageSource;

                user_name.Text = StaticVars.UserName + " " + StaticVars.UserSurname;
                user_status.Text = StaticVars.UserStatus;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }          
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
