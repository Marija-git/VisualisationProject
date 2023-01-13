using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Project_Visualisation
{
    /// <summary>
    /// Interaction logic for AddTextWindow.xaml
    /// </summary>
    public partial class AddTextWindow : Window
    {
        private Point pt;
        private double textSize;
        private SolidColorBrush textColor;
        private Button btnAddText;
        public AddTextWindow()
        {
            InitializeComponent();
        }
        public AddTextWindow(Button btnAddText, Point position)
        {
            InitializeComponent();
            btnAddText = btnAddText;
            pt = position;
        }

        public AddTextWindow(TextBlock tb)
        {
            InitializeComponent();
        }

        public void ChangeText(UIElement element)
        {
            Double.TryParse(CB_TextSize.SelectedItem.ToString(), out textSize);
            ((TextBlock)element).FontSize = textSize;
            ((TextBlock)element).Foreground = textColor;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        private void DrawText_Click(object sender, RoutedEventArgs e)
        {
            TextBlock addtext = new TextBlock();
            addtext.Margin = new Thickness(pt.X, pt.Y, 0, 0);
            Canvas.SetZIndex(addtext, 4);

            string tt = "";
            if (textSize == 0 || textColor == null || TB_text.Text.Length == 0)
            {
                MessageBox.Show("This field is required!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                tt = TB_text.ToString();
                tt = tt.Substring(32);

                StringBuilder str_build = new StringBuilder();
                Random random = new Random();
                char letter;
                for (int i = 0; i < 15; i++)
                {
                    double flt = random.NextDouble();
                    int shift = Convert.ToInt32(Math.Floor(25 * flt));
                    letter = Convert.ToChar(shift + 65);
                    str_build.Append(letter);
                }
                addtext.Name = str_build.ToString();

                addtext.Text = tt;

                addtext.Foreground = textColor;

                addtext.FontSize = textSize;

                MainWindow.tbObj = addtext;
                this.Close();
            }
        }

        #region COLORS
        private void CBTextColor(object sender, SelectionChangedEventArgs e)
        {
            Color tc = (Color)(CB_TextColor.SelectedItem as PropertyInfo).GetValue(1, null);
            textColor = new SolidColorBrush(tc);
        }

        private void CBTextSize(object sender, SelectionChangedEventArgs e)
        {
            Double.TryParse(CB_TextSize.SelectedItem.ToString(), out textSize);
        }
        private void Colors_Loaded(object sender, RoutedEventArgs e)
        {
            CB_TextColor.ItemsSource = typeof(Colors).GetProperties();
            CB_TextSize.ItemsSource = new List<double>() { 6, 7, 8, 9, 10, 11, 12, 14, 16, 18, 21, 24, 30, 36, 48, 60, 72 };
        }
        #endregion

      
    }
}
