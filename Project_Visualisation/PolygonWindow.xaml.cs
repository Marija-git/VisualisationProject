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
    /// Interaction logic for PolygonWindow.xaml
    /// </summary>
    public partial class PolygonWindow : Window
    {
        private List<Point> points;
        private Polygon po;
        private SolidColorBrush textColorr;
        private SolidColorBrush strokeColor;
        private SolidColorBrush polygonColor;
        double dkl = 0;
        double opacity = 0;
        private TextBlock textInPolygon = new TextBlock();


        public PolygonWindow()
        {
            InitializeComponent();
        }
        public PolygonWindow(Button btnPolygon, List<Point> polygonPoints)
        {
            InitializeComponent();
            btnPolygon = btnPolygon;
            points = polygonPoints;
        }
        public PolygonWindow(Polygon PolygonEdit)
        {
            InitializeComponent();
            po = PolygonEdit;

        }

        public void ChangePolygon()
        {
            TB_strokeThicknesse.Text = po.StrokeThickness.ToString();
            btnDraw.Content = "Change";
        }
        public void ChangeText(UIElement element)
        {
            if (((TextBlock)element).Name == "tbp")
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
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Draw_Click(object sender, RoutedEventArgs e)
        {
            if (btnDraw.Content == "Change")
            {
                double.TryParse(TB_strokeThicknesse.Text, out dkl);
                double.TryParse(Opacity_TB.Text, out opacity);

                po.StrokeThickness = dkl;
                po.Stroke = strokeColor;
                polygonColor.Opacity = opacity;
                po.Fill = polygonColor;
                MainWindow.changedPolygon = po;

            }
            else
            {
                double.TryParse(TB_strokeThicknesse.Text, out dkl);
                double.TryParse(Opacity_TB.Text, out opacity);

                Polygon p = new Polygon();
                p.Points = new PointCollection(points);
                p.StrokeThickness = dkl;
                p.Stroke = strokeColor;
                polygonColor.Opacity = opacity;
                p.Fill = polygonColor;

                //TEXT in POLYGON
                Point pointWithMinX;
                pointWithMinX = points[0];
                foreach (Point pt in points)
                {
                    if (pt.X < pointWithMinX.X)
                    {
                        pointWithMinX = pt;     
                    }
                }

                Point pointWithMaxY;
                pointWithMaxY = points[0];
                foreach (Point pt in points)
                {
                    if (pt.Y > pointWithMaxY.Y)
                    {
                        pointWithMaxY = pt;     
                    }
                }

                Point pointWithMinY;
                pointWithMinY = points[0];
                foreach (Point pt in points)
                {
                    if (pt.Y < pointWithMinY.Y)
                    {
                        pointWithMinY = pt;     
                    }
                }

                Point pointWithMaxX;
                pointWithMaxX = points[0];
                foreach (Point pt in points)
                {
                    if (pt.X > pointWithMaxX.X)
                    {
                        pointWithMaxX = pt;     
                    }
                }

                double height = (pointWithMaxY.Y - pointWithMinY.Y) / 2;
                Point centerInHeight = new Point((pointWithMaxY.X + pointWithMinY.X) / 2, (pointWithMaxY.Y + pointWithMinY.Y) / 2);
                Point centerInWidth = new Point((pointWithMaxX.X + pointWithMinX.X) / 2, (pointWithMaxX.Y + pointWithMinX.Y) / 2);
                //left top right bottom
                textInPolygon.Margin = new Thickness(centerInWidth.X, centerInHeight.Y, 0, 0);
                textInPolygon.FontSize = 10;
                Canvas.SetZIndex(textInPolygon, 4);
                string tt = "";
                if (TB_text.Text.Length != 0)       //opciono 
                {
                    tt = TB_text.ToString();
                    tt = tt.Substring(32);
                }
                textInPolygon.Name = "tbp";
                textInPolygon.Text = tt;
                if (textColorr == null)
                {
                    textInPolygon.Foreground = Brushes.Black;
                }
                else
                {
                    textInPolygon.Foreground = textColorr;
                }

              ((MainWindow)Application.Current.MainWindow).canvasDisplay.Children.Add(textInPolygon);
                MainWindow.listOfSllDrawnObjectsOnCanvas.Add(textInPolygon);
                MainWindow.UndoObjects.Clear();
                MainWindow.polygonObj = p;

            }
            this.Close();


        }


        #region COLORS
        private void CBStrokeColor(object sender, SelectionChangedEventArgs e)
        {
            Color bcl = (Color)(CB_StrokeColor.SelectedItem as PropertyInfo).GetValue(1, null);
            strokeColor = new SolidColorBrush(bcl);
        }

        private void CBPolygonColor(object sender, SelectionChangedEventArgs e)
        {
            Color pc = (Color)(CB_PolygonColor.SelectedItem as PropertyInfo).GetValue(1, null);
            polygonColor = new SolidColorBrush(pc);
        }

        private void CBTextColor(object sender, SelectionChangedEventArgs e)
        {
            Color ct = (Color)(CB_TextColor.SelectedItem as PropertyInfo).GetValue(1, null);
            textColorr = new SolidColorBrush(ct);
        }
        private void Colors_Loaded(object sender, RoutedEventArgs e)
        {
            CB_StrokeColor.ItemsSource = typeof(Colors).GetProperties();
            CB_PolygonColor.ItemsSource = typeof(Colors).GetProperties();
            CB_TextColor.ItemsSource = typeof(Colors).GetProperties();
        }

        #endregion
    }
}
