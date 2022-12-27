using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.ObjectModel;
using LiveCharts.Configurations;
using LiveCharts.Helpers;

namespace rent_vyborov
{
    /// <summary>
    /// Логика взаимодействия для report_panel.xaml
    /// </summary>
    public partial class report_panel : Page
    {
        public report_panel()
        {
            InitializeComponent();
            SQLclass.OpenConnection();
        }

        Notification ntf = new Notification();
        private SqlDataAdapter dataAdapter = null;
        private DataSet dataSet = null;
        private DataTable table = null;

        private void btn_create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (date_start.Text != "" && date_end.Text != "" && services.Text == "")
                {
                    DateTime _start = (DateTime)date_start.SelectedDate;
                    DateTime _end = (DateTime)date_end.SelectedDate;

                    if (_end > _start)
                    {
                        UpdateChart($"WHERE [dbo].[Order].time > N'{_start.ToString("yyyy-MM-dd HH:mm:ss")}' AND [dbo].[Order].time < N'{_end.ToString("yyyy-MM-dd HH:mm:ss")}'", 1);
                    }
                    else
                        ntf.Notify("Произошла ошибка", "Пожалуйста выберите корректный диапазон времени");
                }
                else if (date_start.Text != "" && date_end.Text != "" && services.Text != "")
                {
                    DateTime _start = (DateTime)date_start.SelectedDate;
                    DateTime _end = (DateTime)date_end.SelectedDate;

                    if (_end > _start)
                    {
                        UpdateChart($"WHERE [dbo].[Order].time > N'{_start.ToString("yyyy-MM-dd HH:mm:ss")}' AND [dbo].[Order].time < N'{_end.ToString("yyyy-MM-dd HH:mm:ss")}' AND [dbo].[Services].name = N'{services.Text}'", 2);
                    }
                    else
                        ntf.Notify("Произошла ошибка", "Пожалуйста выберите корректный диапазон времени");
                }
                else if (date_start.Text == "" && date_end.Text == "" && services.Text != "")
                {
                    UpdateChart($"WHERE [dbo].[Services].name = N'{services.Text}'", 2);
                }
                else
                {
                    UpdateChart("", 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }           
        }

        private void btn_graf_Click(object sender, RoutedEventArgs e)
        {
            chart1.Visibility = Visibility.Visible;
            chart2.Visibility = Visibility.Hidden;

            br_graf.Background = new SolidColorBrush(Color.FromRgb(45, 158, 55));
            br_table.Background = new SolidColorBrush(Color.FromRgb(148, 210, 78));
        }

        private void btn_table_Click(object sender, RoutedEventArgs e)
        {
            chart1.Visibility = Visibility.Hidden;
            chart2.Visibility = Visibility.Visible;

            br_table.Background = new SolidColorBrush(Color.FromRgb(45, 158, 55));
            br_graf.Background = new SolidColorBrush(Color.FromRgb(148, 210, 78));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateChart("", 1);
            GetDataServices();
        }

        public void UpdateChart(string setdate, int type)
        {
            string query = "";

            if (type == 1)
            {
                query = $"SELECT [dbo].[Order].time, SUM([Order_items].id_order) OVER (PARTITION BY [dbo].[Order].time) AS id_order FROM [dbo].[Order] INNER JOIN [dbo].[Order_items] ON [dbo].[Order_items].id = [dbo].[Order].id {setdate} ORDER BY [Order_items].id_order, [dbo].[Order_items].id";
            }
            else if (type == 2)
            {
                query = $"SELECT [dbo].[Order_items].[id], [dbo].[Order].[time], [dbo].[Services].name FROM [dbo].[Order_items] INNER JOIN [dbo].[Order] ON [dbo].[Order].id = [dbo].[Order_items].id_order INNER JOIN [dbo].[Services] ON [dbo].[Services].id = [dbo].[Order_items].id_services {setdate}";
            }

            dataAdapter = new SqlDataAdapter(query, SQLclass.conn);

            dataSet = new DataSet();

            dataAdapter.Fill(dataSet, "Order");

            table = dataSet.Tables["Order"];

            SeriesCollection series = new SeriesCollection();
            ChartValues<int> seriesValues = new ChartValues<int>();
            List<string> dates = new List<string>();

            foreach (DataRow row in table.Rows)
            {
                if (type == 1)
                {
                    seriesValues.Add(Convert.ToInt32(row["id_order"]));
                    dates.Add(Convert.ToDateTime(row["time"]).ToShortDateString());
                }
                else if (type == 2)
                {
                    seriesValues.Add(Convert.ToInt32(row["id"]));
                    dates.Add(Convert.ToDateTime(row["time"]).ToShortDateString());
                }               
            }

            chart1.AxisX.Clear();
            chart1.AxisX.Add(new Axis()
            {
                Labels = dates
            });

            LineSeries line = new LineSeries();
            line.Values = seriesValues;

            series.Add(line);
            chart1.Series = series;
        }

        public void GetDataServices()
        {
            SqlDataAdapter a = new SqlDataAdapter("SELECT name as name, id FROM [dbo].[Services]", SQLclass.conn); // запрос в базу данных

            DataTable table_p = new DataTable();
            a.Fill(table_p);

            services.DisplayMemberPath = "name";
            services.ItemsSource = table_p.AsDataView();
            services.SelectedIndex = -1;
        }
    }
}
