using BarcodeLib;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;

namespace rent_vyborov
{
    /// <summary>
    /// Логика взаимодействия для order_panel.xaml
    /// </summary>
    public partial class order_panel : Page
    {
        public order_panel()
        {
            InitializeComponent();
        }

        Notification ntf = new Notification();
        Random random = new Random();

        string recomended_code;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // событие при открытии страницы
            try
            {
                // загрузка данных
                GetDataNameUser();
                GetDataNameServices();               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void GetDataNameUser()
        {
            // формирование списка клиентов
            SqlDataAdapter a = new SqlDataAdapter("SELECT CONCAT(surname, ' ', name, ' ', patronymic) AS users, id FROM Customers", SQLclass.conn);

            DataTable table_p = new DataTable();
            a.Fill(table_p);

            customers.DisplayMemberPath = "users";
            customers.ItemsSource = table_p.AsDataView();
            customers.SelectedIndex = -1;
        }

        public void GetDataNameServices()
        {
            // формирование списка услуг
            SqlDataAdapter a = new SqlDataAdapter("SELECT CONCAT(name, ' (', service_code, ')') AS service, id FROM Services", SQLclass.conn);

            DataTable table_p = new DataTable();
            a.Fill(table_p);

            services.DisplayMemberPath = "service";
            services.ItemsSource = table_p.AsDataView();
            services.SelectedIndex = -1;
        }

        private void btn_code_Click(object sender, RoutedEventArgs e)
        {
            // генерация штрих-кода
            if (customers.Text != "")
            {
                string code_result = recomended_code.Split('/')[0] + DateTime.Now + random.Next(100000, 999999);
                string eth_1 = code_result.Replace(".", "");
                string eth_2 = eth_1.Replace(":", "");
                string eth_3 = eth_2.Replace(" ", "");

                GeneratorBar(eth_3);
            }
            else
            {
                ntf.Notify("Произошла ошибка", "Пожалуйста выберите клиента и запишите его");
            }
            
        }

        private System.Drawing.Image GeneratorBar(string msg)
        {
            MultiFormatWriter mutiWriter = new MultiFormatWriter();
            BitMatrix bm = mutiWriter.encode(msg, BarcodeFormat.CODE_39, 350, 256);
            Bitmap img = new BarcodeWriter().Write(bm);
            barcode_img.Source = BitmapToBitmapImage(img);
            return img;
        }

        // Bitmap --> BitmapImage
        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        // BitmapImage --> Bitmap
        public static Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }

        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            if (customers.Text != "")
            {
                client.Visibility = Visibility.Visible;
                client.Text = "Клиент: " + customers.Text;

                ex_number.Visibility = Visibility.Visible;
                ex_number.Text = "*Установлен рекомендуемый номер заказа";
                recomended_code = SQLclass.Select("SELECT id FROM [dbo].[Customers] WHERE surname = N'" + customers.Text.Split(' ')[0] + "'")[0] + "/" + DateTime.Today.ToString("d");
                code.Text = recomended_code;
            }          
        }

        int count_service = 0;

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            if (count_service == 0)
            {
                service_1.Text = "Услуга: " + services.Text;
                service_1.Visibility = Visibility.Visible;
                btn_del_1.Visibility = Visibility.Visible;
                count_service++;
            }
            else if (count_service == 1)
            {
                service_2.Text = "Услуга: " + services.Text;
                service_2.Visibility = Visibility.Visible;
                btn_del_2.Visibility = Visibility.Visible;
                count_service++;
            }
            else if (count_service == 2)
            {
                service_3.Text = "Услуга: " + services.Text;
                service_3.Visibility = Visibility.Visible;
                btn_del_3.Visibility = Visibility.Visible;
                count_service++;
            }
            else if (count_service == 3)
            {
                service_4.Text = "Услуга: " + services.Text;
                service_4.Visibility = Visibility.Visible;
                btn_del_4.Visibility = Visibility.Visible;
                count_service++;
            }
            else if (count_service == 4)
            {
                service_5.Text = "Услуга: " + services.Text;
                service_5.Visibility = Visibility.Visible;
                btn_del_5.Visibility = Visibility.Visible;
                count_service++;
            }
            else
            {
                ntf.Notify("Произошла ошибка", "К сожалению более 5 услуг нельзя добавлять");
            }
        }

        private void btn_del_1_Click(object sender, RoutedEventArgs e)
        {
            if (count_service == 1)
            {
                service_1.Text = null;
                service_1.Visibility = Visibility.Hidden;
                btn_del_1.Visibility = Visibility.Hidden;
                count_service = 0;
            }
            else if (count_service == 2)
            {
                service_2.Visibility = Visibility.Hidden;
                btn_del_2.Visibility = Visibility.Hidden;
                count_service = 1;

                service_1.Text = service_2.Text;
                service_2.Text = null;
            }
            else if (count_service == 3)
            {
                service_3.Visibility = Visibility.Hidden;
                btn_del_3.Visibility = Visibility.Hidden;
                count_service = 2;

                service_1.Text = service_2.Text;
                service_2.Text = service_3.Text;
                service_3.Text = null;
            }
            else if (count_service == 4)
            {
                service_4.Visibility = Visibility.Hidden;
                btn_del_4.Visibility = Visibility.Hidden;
                count_service = 3;

                service_1.Text = service_2.Text;
                service_2.Text = service_3.Text;
                service_3.Text = service_4.Text;
                service_4.Text = null;
            }
            else if (count_service == 5)
            {
                service_5.Visibility = Visibility.Hidden;
                btn_del_5.Visibility = Visibility.Hidden;
                count_service = 4;

                service_1.Text = service_2.Text;
                service_2.Text = service_3.Text;
                service_3.Text = service_4.Text;
                service_4.Text = service_5.Text;
                service_5.Text = null;
            }
        }

        private void btn_del_2_Click(object sender, RoutedEventArgs e)
        {
            if (count_service == 2)
            {
                service_2.Visibility = Visibility.Hidden;
                btn_del_2.Visibility = Visibility.Hidden;
                count_service = 1;
            }
            else if (count_service == 3)
            {
                service_3.Visibility = Visibility.Hidden;
                btn_del_3.Visibility = Visibility.Hidden;
                count_service = 2;

                service_2.Text = service_3.Text;
                service_3.Text = null;
            }
            else if (count_service == 4)
            {
                service_4.Visibility = Visibility.Hidden;
                btn_del_4.Visibility = Visibility.Hidden;
                count_service = 3;

                service_2.Text = service_3.Text;
                service_3.Text = service_4.Text;
                service_4.Text = null;
            }
            else if (count_service == 5)
            {
                service_5.Visibility = Visibility.Hidden;
                btn_del_5.Visibility = Visibility.Hidden;
                count_service = 4;

                service_2.Text = service_3.Text;
                service_3.Text = service_4.Text;
                service_4.Text = service_5.Text;
                service_5.Text = null;
            }
        }

        private void btn_del_3_Click(object sender, RoutedEventArgs e)
        {
            if (count_service == 3)
            {
                service_3.Visibility = Visibility.Hidden;
                btn_del_3.Visibility = Visibility.Hidden;
                count_service = 2;
                service_3.Text = null;
            }
            else if (count_service == 4)
            {
                service_4.Visibility = Visibility.Hidden;
                btn_del_4.Visibility = Visibility.Hidden;
                count_service = 3;

                service_3.Text = service_4.Text;
                service_4.Text = null;
            }
            else if (count_service == 5)
            {
                service_5.Visibility = Visibility.Hidden;
                btn_del_5.Visibility = Visibility.Hidden;
                count_service = 4;

                service_3.Text = service_4.Text;
                service_4.Text = service_5.Text;
                service_5.Text = null;
            }
        }

        private void btn_del_4_Click(object sender, RoutedEventArgs e)
        {
            if (count_service == 4)
            {
                service_4.Visibility = Visibility.Hidden;
                btn_del_4.Visibility = Visibility.Hidden;
                count_service = 3;
                service_4.Text = null;
            }
            else if (count_service == 5)
            {
                service_5.Visibility = Visibility.Hidden;
                btn_del_5.Visibility = Visibility.Hidden;
                count_service = 4;

                service_4.Text = service_5.Text;
                service_5.Text = null;
            }
        }

        private void btn_del_5_Click(object sender, RoutedEventArgs e)
        {
            service_5.Visibility = Visibility.Hidden;
            btn_del_5.Visibility = Visibility.Hidden;
            count_service = 4;
            service_5.Text = null;
        }

        private void code_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (code.Text == recomended_code)
            {
                ex_number.Visibility = Visibility.Visible;
                ex_number.Text = "*Установлен рекомендуемый номер заказа";
            }
            else
            {
                ex_number.Visibility = Visibility.Hidden;
            }
        }

        private void add_client_Click(object sender, RoutedEventArgs e)
        {
            StaticVars.MainWnd.Content = new add_customers();          
        }
    }
}
