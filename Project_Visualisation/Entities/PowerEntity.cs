using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Project_Visualisation.Entities
{
    public abstract class PowerEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public int Connections { get; set; }
        public Ellipse Ellipse { get; set; }
        public Image Image { get; set; }
        public Brush PreviousColor { get; set; }

    }
}
