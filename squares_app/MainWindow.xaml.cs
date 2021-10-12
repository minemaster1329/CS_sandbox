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

namespace squares_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            if (rect.Fill == Brushes.Red) rect.Fill = Brushes.Green;
            else rect.Fill = Brushes.Red;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Children.Clear();
            MainGrid.ColumnDefinitions.Clear();
            MainGrid.RowDefinitions.Clear();

            int rows_count = Int32.Parse(RowsCount.Text);
            int cols_count = Int32.Parse(RowsCount.Text);

            for (int i = 0; i < rows_count; i++) MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1.0, GridUnitType.Star) });

            for (int i = 0; i < cols_count; i++) MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1.0, GridUnitType.Star) });

            for (int i = 0; i < rows_count; i++)
            {
                for (int j = 0; j < cols_count; j++)
                {
                    Rectangle rect = new Rectangle() { Fill = Brushes.Red, Margin = new Thickness(2) };
                    rect.MouseLeftButtonDown += Rect_MouseLeftButtonDown;
                    MainGrid.Children.Add(rect);
                    Grid.SetColumn(rect, j);
                    Grid.SetRow(rect, i);
                }
            }
        }
    }
}
