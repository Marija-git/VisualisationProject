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

namespace Project_Visualisation
{
    /// <summary>
    /// Interaction logic for EntityWindow.xaml
    /// </summary>
    public partial class EntityWindow : Window
    {
        Ellipse el;
        Image image;
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
    }
}
