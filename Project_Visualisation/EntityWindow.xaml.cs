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
    /// Interaction logic for EntityWindow.xaml
    /// </summary>
    public partial class EntityWindow : Window
    {
        Ellipse el;
        Image image;
        private SolidColorBrush entitityColor;
        public EntityWindow()
        {
            InitializeComponent();
        }
        public EntityWindow(Ellipse EllipseEdit)
        {
            InitializeComponent();
            el = EllipseEdit;
        }
        public EntityWindow(Image img)
        {
            InitializeComponent();
            image = img;
        }


        private void ChangeToImages_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).DeleteEllipse_ShowImages();
            this.Close(); 
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ChangeToEllipse_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).DeleteImage_BackToEllipse();
            this.Close();
        }

        private void ColorEntity_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).ColorEntity(entitityColor, el);
            this.Close();
        }

        #region BOJENJE
        private void CBEntityColor(object sender, SelectionChangedEventArgs e)
        {

            Color colorEntity = (Color)(CB_entityColor.SelectedItem as PropertyInfo).GetValue(1, null);
            entitityColor = new SolidColorBrush(colorEntity);
        }

        private void Colors_Loaded(object sender, RoutedEventArgs e)
        {
            CB_entityColor.ItemsSource = typeof(Colors).GetProperties();
        }
        #endregion

    }
}
