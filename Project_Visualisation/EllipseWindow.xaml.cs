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
    /// Interaction logic for EllipseWindow.xaml
    /// </summary>
    public partial class EllipseWindow : Window
    {
        private Point pt;
        private Button btnEllipse;
        private Ellipse el;

        private SolidColorBrush strokeColor;
        private SolidColorBrush ellipseColorr;
        private SolidColorBrush textColorr;

        double dkl = 0;
        double opacity = 0;
        double v = 0;  
        double s = 0;

        private TextBlock textt = new TextBlock();

        public EllipseWindow()
        {
            InitializeComponent();
        }
        public EllipseWindow(Button btnEllipse, Point position)
        {
            InitializeComponent();
            btnEllipse = btnEllipse;
            pt = position;
        }

        public EllipseWindow(Ellipse ElipsaEdit)
        {
            InitializeComponent();
            el = ElipsaEdit;
        }

        public void ChangeEllipse()
        {
            TBheight.Text = el.Height.ToString();
            TB_width.Text = el.Width.ToString();
            TB_strokethickness.Text = el.StrokeThickness.ToString();
            TBheight.IsReadOnly = true;
            TB_width.IsReadOnly = true;

            btnDraw.Content = "Change";
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Draw_Click(object sender, RoutedEventArgs e)
        {
            if (btnDraw.Content == "Change")
            {
                double.TryParse(TB_strokethickness.Text, out dkl);
                double.TryParse(Opacity_TB.Text, out opacity);

                el.StrokeThickness = dkl;
                el.Stroke = strokeColor;
                ellipseColorr.Opacity = opacity;
                el.Fill = ellipseColorr;

                MainWindow.changedEllipse = el;
            }
            else
            {
                double.TryParse(TBheight.Text, out v);
                double.TryParse(TB_width.Text, out s);
                double.TryParse(TB_strokethickness.Text, out dkl);
                double.TryParse(Opacity_TB.Text, out opacity);


                // tako da im je gornji levi ugao pozicija gde je pokazivačem miša kliknuto
                //da bi se inicirala akcija crtanja
                Ellipse drawnEllipse = new Ellipse { Height = v, Width = s };
                double axisX = pt.X;   //(sirina/2) ako zelimo da tacka bude u centru
                double axisY = pt.Y;
                drawnEllipse.Margin = new Thickness(axisX, axisY, 0, 0);

                drawnEllipse.StrokeThickness = dkl;
                drawnEllipse.Stroke = strokeColor;
                ellipseColorr.Opacity = opacity;
                drawnEllipse.Fill = ellipseColorr;

                // TEXT U ELIPSI 
                textt.Margin = new Thickness(axisX, axisY + (v / 2), 0, 0);
                textt.FontSize = 10; //velicina teksta je fiksirana
                //text je 4. na redu na z ravni (an element that has a value of 5 will appear above an element that has a value of 4)
                Canvas.SetZIndex(textt, 4);
                string tt = "";
                if (TB_text.Text.Length != 0)       //opciono 
                {
                    tt = TB_text.ToString();
                    // Get everything else after 32. position (izbacuje mi system.windows..bla bla pa to cut)
                    tt = tt.Substring(32);
                }
                textt.Name = "tb1";
                textt.Text = tt;
                if (textColorr == null)
                {
                    textt.Foreground = Brushes.Black;
                }
                else
                {
                    textt.Foreground = textColorr;
                }

                ((MainWindow)Application.Current.MainWindow).canvasDisplay.Children.Add(textt);
                MainWindow.listOfSllDrawnObjectsOnCanvas.Add(textt);
                MainWindow.UndoObjects.Clear();
                MainWindow.ellipseObj = drawnEllipse;

            }
            this.Close();

        }
        public void ChangeText(UIElement element)
        {
            if (((TextBlock)element).Name == "tb1")
            {
                ((TextBlock)element).Text = TB_text.Text;

                if (textColorr == null)
                {
                    ((TextBlock)element).Foreground = Brushes.Black;
                }
                else
                {
                    ((TextBlock)element).Foreground = textColorr;
                }
            }

        }

        #region COLORS

        private void CBStrokeColor(object sender, SelectionChangedEventArgs e)
        {
            Color bcl = (Color)(CB_StrokeColor.SelectedItem as PropertyInfo).GetValue(1, null);
            strokeColor = new SolidColorBrush(bcl);
        }

        private void CBEllipseColor(object sender, SelectionChangedEventArgs e)
        {
            Color ellipseColor = (Color)(CB_EllipseColora.SelectedItem as PropertyInfo).GetValue(1, null);
            ellipseColorr = new SolidColorBrush(ellipseColor);
        }

        private void CBTextColor(object sender, SelectionChangedEventArgs e)
        {
            Color textColor = (Color)(CB_TextColor.SelectedItem as PropertyInfo).GetValue(1, null);
            textColorr = new SolidColorBrush(textColor);
        }
        private void Colors_Loaded(object sender, RoutedEventArgs e)
        {
            CB_StrokeColor.ItemsSource = typeof(Colors).GetProperties();
            CB_EllipseColora.ItemsSource = typeof(Colors).GetProperties();
            CB_TextColor.ItemsSource = typeof(Colors).GetProperties();
        }
        #endregion


    }
}
