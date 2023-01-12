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
using System.Xml;
using Project_Visualisation.Entities;
using Project_Visualisation.RowColumns;

namespace Project_Visualisation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SolidColorBrush btnColor;
        public double numberOfMiniGrids_Height;           //number of mini grids along the Y axis
        public double numberOfMiniGrids_Width;           //numebr of mini grids along the X axis
        public int numberOfPxInMiniGrid_ROW;        //number of pixels in mini grid along the row
        public int numberOfPxInMiniGrid_COLUMN;    //number of pixels in mini grid along the column

        Dictionary<double, PowerEntity> Entities = new Dictionary<double, PowerEntity>();
        Dictionary<long, LineEntity> LineEntities = new Dictionary<long, LineEntity>();

        double minX, maxX, minY, maxY;

        List<Row> rows;

        List<LineEntity> drawnLines = new List<LineEntity>();
        Dictionary<long, List<Line>> allXMLLines = new Dictionary<long, List<Line>>();

        public int numberOfRows, numberOfColumns;

        BitmapImage node, subs, switchh;

        PowerEntity startNode, endNode;

        public MainWindow()
        {           
            InitializeComponent();
            slider.IsEnabled = true;
            btnColor = new SolidColorBrush();
            btnColor.Color = Colors.LightBlue;

            numberOfPxInMiniGrid_ROW = 100;
            numberOfPxInMiniGrid_COLUMN = 100;

            numberOfRows = 100;
            numberOfColumns = 100;
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Load_entities_Click(object sender, RoutedEventArgs e)
        {
            btnLoadModel.Background = btnColor;
            if(btnLoadModel != sender)
            {
                btnLoadModel.Background = null;
            }

            InitGrid();
            LoadEntitiesFromXML();
            FindEdges();
            MakeMiniGrids();
            LoadEntitiesIntoGrids();
            LoadLineEntitiesIntoGrids();

        }

        #region GRID
        //define number of rows and columns in grid
        private void InitGrid()
        {
            numberOfMiniGrids_Height = ((MainWindow)App.Current.MainWindow).Height / numberOfPxInMiniGrid_ROW;     // 20 = 2000 / 100
            numberOfMiniGrids_Width = ((MainWindow)App.Current.MainWindow).Width / numberOfPxInMiniGrid_COLUMN;     // 10 = 1000 / 100

            //Initialize row-list  and column-list
            RowDefinition rowList;
            ColumnDefinition columnList;

            //splitting into mini grids (window)

            for (int i = 0; i < numberOfPxInMiniGrid_ROW; i++)
            {
                rowList = new RowDefinition  //element of rowList
                {
                    Height = new GridLength(numberOfMiniGrids_Height)
                };

                GridPanel.RowDefinitions.Add(rowList);  //create mulitiple rows in gridpanel
            }

            for (int i = 0; i < numberOfPxInMiniGrid_COLUMN; i++)
            {
                columnList = new ColumnDefinition  //element of columnList
                {
                    Width = new GridLength(numberOfMiniGrids_Width)
                };

                GridPanel.ColumnDefinitions.Add(columnList);    //create mulitiple columns in gridpanel
            }

            Grid.SetRowSpan(canvasDisplay, this.numberOfPxInMiniGrid_ROW);
            Grid.SetColumnSpan(canvasDisplay, this.numberOfPxInMiniGrid_COLUMN);
        }

        #region LOAD XML
        private void LoadEntitiesFromXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Geographic.xml");

            XmlNodeList nodeList;

            #region SUBSTITUTIONS

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            List<SubstationEntity> substations = new List<SubstationEntity>();
            SubstationEntity substationEntity;
            double x, y;

            foreach (XmlNode node in nodeList)
            {
                substationEntity = new SubstationEntity
                {
                    Id = long.Parse(node.SelectSingleNode("Id").InnerText),
                    Name = node.SelectSingleNode("Name").InnerText
                };
                x = double.Parse(node.SelectSingleNode("X").InnerText) + 40;
                y = double.Parse(node.SelectSingleNode("Y").InnerText) + 80;
                ToLatLon(x, y, 34, out double newX, out double newY);
                substationEntity.X = newX;
                substationEntity.Y = newY;

                if (!Entities.ContainsKey(substationEntity.Id))
                    Entities.Add(substationEntity.Id, substationEntity);
            }
            #endregion

            #region NODES
            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");
            List<NodeEntity> nodes = new List<NodeEntity>();
            NodeEntity nodeEntity;

            foreach (XmlNode node in nodeList)
            {
                nodeEntity = new NodeEntity
                {
                    Id = long.Parse(node.SelectSingleNode("Id").InnerText),
                    Name = node.SelectSingleNode("Name").InnerText
                };
                x = double.Parse(node.SelectSingleNode("X").InnerText) + 40;
                y = double.Parse(node.SelectSingleNode("Y").InnerText) + 80;
                ToLatLon(x, y, 34, out double newX, out double newY);
                nodeEntity.X = newX;
                nodeEntity.Y = newY;

                if (!Entities.ContainsKey(nodeEntity.Id))
                    Entities.Add(nodeEntity.Id, nodeEntity);
            }
            #endregion

            #region SWITCHES
            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            //  ParseSwitchEntities(nodeList);
            List<SwitchEntity> switches = new List<SwitchEntity>();
            SwitchEntity switchEntity;

            foreach (XmlNode node in nodeList)
            {
                switchEntity = new SwitchEntity
                {
                    Id = long.Parse(node.SelectSingleNode("Id").InnerText),
                    Name = node.SelectSingleNode("Name").InnerText,
                    Status = node.SelectSingleNode("Status").InnerText
                };
                x = double.Parse(node.SelectSingleNode("X").InnerText) + 40;
                y = double.Parse(node.SelectSingleNode("Y").InnerText) + 80;
                ToLatLon(x, y, 34, out double newX, out double newY);
                switchEntity.X = newX;
                switchEntity.Y = newY;

                if (!Entities.ContainsKey(switchEntity.Id))
                    Entities.Add(switchEntity.Id, switchEntity);
            }
            #endregion

            #region LINES
            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");
            LineEntity line;

            foreach (XmlNode node in nodeList)
            {
                line = new LineEntity
                {
                    Id = long.Parse(node.SelectSingleNode("Id").InnerText),
                    Name = node.SelectSingleNode("Name").InnerText,
                    FirstEnd = long.Parse(node.SelectSingleNode("FirstEnd").InnerText),
                    SecondEnd = long.Parse(node.SelectSingleNode("SecondEnd").InnerText),
                    Resistance = double.Parse(node.SelectSingleNode("R").InnerText)
                };
                try
                {
                    line.CondMaterial = (ConductorMaterial)Enum.Parse(typeof(ConductorMaterial), node.SelectSingleNode("ConductorMaterial").InnerText);

                }
                catch
                {
                    line.CondMaterial = ConductorMaterial.Other;

                }

                LineEntities.Add(line.Id, line);

                foreach (var entitet in Entities.Values)
                {
                    if (entitet.Id == line.FirstEnd)
                    {
                        entitet.Connections++;
                    }
                }
                foreach (var entitet in Entities.Values)
                {
                    if (entitet.Id == line.SecondEnd)
                    {
                        entitet.Connections++;
                    }
                }

            }
            #endregion
        }

        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
        {
            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
        }
        #endregion

        //find the edges of the new coordinate system
        private void FindEdges()
        {
            minX = Entities.Values.Min(entity => entity.X);
            maxX = Entities.Values.Max(entity => entity.X);

            minY = Entities.Values.Min(entity => entity.Y);
            maxY = Entities.Values.Max(entity => entity.Y);
        }

        //make mini grids and find the way to access them (g11,g12.. -matrix)
        private void MakeMiniGrids()
        {
            rows = new List<Row>(this.numberOfPxInMiniGrid_ROW);
            List<Column> columns = new List<Column>(this.numberOfPxInMiniGrid_COLUMN);

            //the size of the new coordinate system 
            var newCoordinateSystem_Width = maxX - minX;         
            var newCoordinateSystem_Height = maxY - minY;         

            //the size of one mini grid
            var miniGrid_Width = newCoordinateSystem_Width / this.numberOfPxInMiniGrid_ROW;        
            var miniGrid_Height = newCoordinateSystem_Height / this.numberOfPxInMiniGrid_COLUMN;

            //foreach element in column -> add his index and displacemnt relative to the original y axis 
            columns.Add(new Column(0, minY));               
            for (int column = 1; column < this.numberOfPxInMiniGrid_COLUMN - 1; column++) //make new elements and add them to the list
            {
                columns.Add(new Column(column, minY + column * miniGrid_Height)); //idx,displacement
            }
            columns.Add(new Column(this.numberOfPxInMiniGrid_COLUMN - 1, maxY));


            rows.Add(new Row(0, maxX, new List<Column>(columns.Count))); // first element of the row -> idx,displacement, columnList under that element
            columns.ForEach(x => rows[0].Columns.Add((Column)x.Clone()));
            for (int row = 1; row < this.numberOfPxInMiniGrid_ROW - 1; row++)
            {
                //make list - of the lists - in which every - element od the row - conatains the list of the - whole column under itself -
                rows.Add(new Row(row, maxX - row * miniGrid_Width, new List<Column>(columns.Count)));
                columns.ForEach(x => rows[row].Columns.Add((Column)x.Clone()));
            }
            //the last element of the row
            rows.Add(new Row(this.numberOfPxInMiniGrid_ROW - 1, minX, new List<Column>(columns.Count)));
            columns.ForEach(x => rows[this.numberOfPxInMiniGrid_ROW - 1].Columns.Add((Column)x.Clone()));

        }

        //so far we have grids which are list of lists but they are empty bcs they dont have enitities in them
        #endregion

        #region LOAD ENTITIES
        private void LoadEntitiesIntoGrids()
        {
            Ellipse ellipse;
            foreach (var entity in Entities.Values)
            {
                ellipse = MakeEllipseForEntity(entity);
                
                Tuple<int, int> coordinates = FindPositionForEntitiesInGrid(entity); //return coordinates (where to load into grid -> num of row and column)
                entity.Row = coordinates.Item1;
                entity.Column = coordinates.Item2;
                entity.Ellipse = ellipse;

                Canvas.SetTop(ellipse, coordinates.Item1 * numberOfMiniGrids_Height - (ellipse.Height / 2)); //where to draw ellipse in grid , x-left, y-up (negative)
                Canvas.SetLeft(ellipse, coordinates.Item2 * numberOfMiniGrids_Width - (ellipse.Width / 2));
                Canvas.SetZIndex(ellipse, 2);

                ((Canvas)GridPanel.Children[0]).Children.Add(ellipse);
                //canvasDisplay.Children.Add(ellipse);
            }
        }

        private Ellipse MakeEllipseForEntity(PowerEntity entity)
        {
            Ellipse ellipse = new Ellipse
            {
                Width = 6,
                Height = 6,
                ToolTip = entity.ToString(),
            };

            if(entity.GetType().Name == "SubstationEntity")
            {
                ellipse.Fill = Brushes.Red;
            }
            else if (entity.GetType().Name == "NodeEntity")
            {
                ellipse.Fill = Brushes.Green;
            }
           else if (entity.GetType().Name == "SwitchEntity")
           {
                ellipse.Fill = Brushes.Blue;
           }
            ellipse.MouseLeftButtonDown += new MouseButtonEventHandler(Left_Click_Entity);
            return ellipse;
        }

        private Tuple<int, int> FindPositionForEntitiesInGrid(PowerEntity powerEntity)
        {
            bool found = false;
            //find mini grid foreach enitity and put that enitity right there

            //compare x of original coordinate system to x of entity from xml file (same for y)
            int row = rows.First(x => x.Value <= powerEntity.X).Id;
            int column = rows[row].Columns.First(x => x.Value >= powerEntity.Y).Id;

            if (rows[row].Columns[column].Taken) //is that mini grid taken
            {
                int kruznica = 0; //which circle

                while (!found)
                {
                    kruznica++;
                    //left column                    
                    if (column - kruznica >= 0) //check if there is concetric circle of sequential number of index (zero,first..)
                    {
                        List<Row> rows = new List<Row>();
                        // gornji deo reda
                        if (row - kruznica + 1 >= 0) //7
                        {

                            this.rows
                                .FindAll(x => x.Id >= row - kruznica + 1 && x.Id <= row).OrderByDescending(x => x.Id).ToList().ForEach(x => rows.Add(x));
                        }
                        // donji deo reda
                        if (row + kruznica - 1 < this.numberOfMiniGrids_Height) //1
                        {
                            this.rows
                                .FindAll(x => x.Id <= row + kruznica - 1 && x.Id >= row)
                                .ForEach(x => rows.Add(x));
                        }

                        foreach (var rowEl in rows) //4
                        {
                            if (!rowEl.Columns[column - kruznica].Taken)
                            {
                                column -= kruznica;
                                row = rowEl.Id;
                                found = true;
                                break;
                            }
                        }

                        if (found)
                            break;
                    }
                    // right column
                    if (column + kruznica < this.numberOfMiniGrids_Width)
                    {
                        List<Row> rows = new List<Row>();
                        // gornji deo reda
                        if (row - kruznica + 1 >= 0) //9
                        {
                            this.rows
                                .FindAll(x => x.Id >= row - kruznica + 1 && x.Id <= row)
                                .OrderByDescending(x => x.Id).ToList()
                                .ForEach(x => rows.Add(x));
                        }
                        // donji deo reda
                        if (row + kruznica - 1 < this.numberOfMiniGrids_Height) //3
                        {
                            this.rows
                                .FindAll(x => x.Id <= row + kruznica - 1 && x.Id >= row)
                                .ForEach(x => rows.Add(x));
                        }

                        foreach (var rowEl in rows) //6
                        {
                            if (!rowEl.Columns[column + kruznica].Taken)
                            {
                                column += kruznica;
                                row = rowEl.Id;
                                found = true;
                                break;
                            }
                        }

                        if (found)
                            break;
                    }

                    // gornji red
                    if (row - kruznica >= 0)
                    {
                        List<Column> columns = new List<Column>();
                        // levi deo kolone
                        if (column - kruznica > 0)//8
                            rows[row - kruznica].Columns
                                .FindAll(x => x.Id <= column && x.Id >= column - kruznica)
                                .OrderByDescending(x => x.Id).ToList()
                                .ForEach(x => columns.Add(x));
                        // desni deo kolone
                        if (column + kruznica < this.numberOfMiniGrids_Width)
                            rows[row - kruznica].Columns
                                .FindAll(x => x.Id >= column && x.Id <= column + kruznica)
                                .ForEach(x => columns.Add(x));

                        foreach (var col in columns) //za drugi krug elementi kooji se nalaze iznad 8
                        {
                            if (!col.Taken)
                            {
                                column = col.Id;
                                row -= kruznica;
                                found = true;
                                break;
                            }
                        }

                        if (found)
                            break;
                    }
                    // donji red
                    if (row + kruznica < this.numberOfMiniGrids_Height)
                    {
                        List<Column> columns = new List<Column>();
                        // levi deo kolone
                        if (column - kruznica > 0)
                            rows[row + kruznica].Columns
                                .FindAll(x => x.Id <= column && x.Id >= column - kruznica)
                                .OrderByDescending(x => x.Id).ToList()
                                .ForEach(x => columns.Add(x));
                        // desni deo kolone
                        if (column + kruznica < this.numberOfMiniGrids_Width)
                            rows[row + kruznica].Columns
                                .FindAll(x => x.Id >= column && x.Id <= column + kruznica)
                                .ForEach(x => columns.Add(x));

                        foreach (var col in columns)
                        {
                            if (!col.Taken)
                            {
                                column = col.Id;
                                row += kruznica;
                                found = true;
                                break;
                            }
                        }

                        if (found)
                            break;
                    }
                }
            }
            else
                rows[row].Columns[column].Taken = true;

            return new Tuple<int, int>(row, column);
        }

        #endregion

        #region LOAD LINE ENTITIES
        private void LoadLineEntitiesIntoGrids()
        {
            Line newLine;
            foreach (var line in LineEntities.Values)
            {
                //is there enitities which have coordinates like startNode and endNode of the line (first,second)
                if (Entities.TryGetValue(line.FirstEnd, out PowerEntity startNode) &&
                    Entities.TryGetValue(line.SecondEnd, out PowerEntity endNode) &&
                    //has line (taken from LineEnitite) already been drawn (dont draw it two times) 
                    drawnLines.Find(x => x.FirstEnd == line.FirstEnd && x.SecondEnd == line.SecondEnd ||
                                          x.FirstEnd == line.SecondEnd && x.SecondEnd == line.FirstEnd) == null)
                {
                    //if it hasnt ,add it to help-list
                    allXMLLines.Add(line.Id, new List<Line>());

                    //list of mini grids = rowcolum
                    //List<RowColumn> rowColumn = NadjiPutanju(startNode, endNode);
                    List<RowColumn> rowColumn = BreadthFirstSearch(startNode, endNode);
                    foreach (var rc in rowColumn)
                    {
                        if (rc.Parent != null)
                        {
                            newLine = CreateLine(line, rc);
                            Canvas.SetZIndex(newLine, 0);
                            ((Canvas)GridPanel.Children[0]).Children.Add(newLine);
                            drawnLines.Add(line);
                            allXMLLines[line.Id].Add(newLine);
                            rows[rc.Row].Columns[rc.Column].Taken = true;
                        }
                    }
                }
            }
        }

        private Line CreateLine(LineEntity lineEntity, RowColumn rowColumn)
        {
            Line line = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                X1 = rowColumn.Column * numberOfMiniGrids_Width,
                Y1 = rowColumn.Row * numberOfMiniGrids_Height,
                X2 = rowColumn.Parent.Column * numberOfMiniGrids_Width,
                Y2 = rowColumn.Parent.Row * numberOfMiniGrids_Height,
                ToolTip = lineEntity.ToString()
            };
            line.MouseRightButtonDown += new MouseButtonEventHandler(Right_Click_Line);
            return line;
        }

        private List<RowColumn> BreadthFirstSearch(PowerEntity powerEntityFirst, PowerEntity powerEntitySecond)
        {
            bool found = false;
            Queue<RowColumn> queue = new Queue<RowColumn>();
            List<RowColumn> checkedCoordinates = new List<RowColumn>();
            List<RowColumn> shortestPath = new List<RowColumn>();

            int rowFirst = powerEntityFirst.Row;
            int columnFirst = powerEntityFirst.Column;
            int rowSecond = powerEntitySecond.Row;
            int columnSecond = powerEntitySecond.Column;

            RowColumn rowColumn = new RowColumn();

            RowColumn rc = new RowColumn(rowFirst, columnFirst);
            queue.Enqueue(rc);
            checkedCoordinates.Add(rc);

            while (queue.Count > 0)
            {
                rowColumn = queue.Dequeue();

                if (rowColumn.Row == rowSecond && rowColumn.Column == columnSecond)
                {
                    found = true;
                    break;
                }
                else
                {
                    // find childrens of rowColumn
                    // same row, left column
                    if (rowColumn.Column - 1 >= 0)
                    {
                        if (checkedCoordinates.Find(x => x.Row == rowColumn.Row && x.Column == rowColumn.Column - 1) == null)
                        {
                            rc = new RowColumn(rowColumn.Row, rowColumn.Column - 1, rowColumn);
                            checkedCoordinates.Add(rc);
                            queue.Enqueue(rc);
                        }
                    }
                    // same row, right column
                    if (rowColumn.Column + 1 < numberOfColumns)
                    {
                        if (checkedCoordinates.Find(x => x.Row == rowColumn.Row && x.Column == rowColumn.Column + 1) == null)
                        {
                            rc = new RowColumn(rowColumn.Row, rowColumn.Column + 1, rowColumn);
                            checkedCoordinates.Add(rc);
                            queue.Enqueue(rc);
                        }
                    }

                    // same column, upper row
                    if (rowColumn.Row - 1 >= 0)
                    {
                        if (checkedCoordinates.Find(x => x.Row == rowColumn.Row - 1 && x.Column == rowColumn.Column) == null)
                        {
                            rc = new RowColumn(rowColumn.Row - 1, rowColumn.Column, rowColumn);
                            checkedCoordinates.Add(rc);
                            queue.Enqueue(rc);
                        }
                    }
                    // same column, lower row
                    if (rowColumn.Row + 1 < numberOfRows)
                    {
                        if (checkedCoordinates.Find(x => x.Row == rowColumn.Row + 1 && x.Column == rowColumn.Column) == null)
                        {
                            rc = new RowColumn(rowColumn.Row + 1, rowColumn.Column, rowColumn);
                            checkedCoordinates.Add(rc);
                            queue.Enqueue(rc);
                        }
                    }
                }
            }

            bool firstCrossroad = true;

            if (found)
            {
                while (rowColumn != null)
                {
                    if (rowColumn.Row == rowFirst && rowColumn.Column == columnFirst ||
                       rowColumn.Row == rowSecond && rowColumn.Column == columnSecond)
                    {
                        shortestPath.Add(rowColumn);
                    }
                    else
                    {
                        if (!rows[rowColumn.Row].Columns[rowColumn.Column].Taken)
                        {
                            shortestPath.Add(rowColumn);
                        }
                        else
                        {
                            if (firstCrossroad)
                            {
                                firstCrossroad = false;

                                Ellipse ellipse = new Ellipse();
                                ellipse.Fill = Brushes.Yellow;
                                ellipse.Width = 4;
                                ellipse.Height = 4;

                                Canvas.SetTop(ellipse, rowColumn.Row * numberOfMiniGrids_Height - (ellipse.Height / 2));
                                Canvas.SetLeft(ellipse, rowColumn.Column * numberOfMiniGrids_Width - (ellipse.Width / 2));
                                Canvas.SetZIndex(ellipse, 1);
                                ((Canvas)GridPanel.Children[0]).Children.Add(ellipse);
                            }

                            if (!rows[rowColumn.Parent.Row].Columns[rowColumn.Parent.Column].Taken)
                            {
                                shortestPath.Add(rowColumn);

                                Ellipse ellipse = new Ellipse();
                                ellipse.Fill = Brushes.Yellow;
                                ellipse.Width = 4;
                                ellipse.Height = 4;

                                Canvas.SetTop(ellipse, rowColumn.Row * numberOfMiniGrids_Height - (ellipse.Height / 2));
                                Canvas.SetLeft(ellipse, rowColumn.Column * numberOfMiniGrids_Width - (ellipse.Width / 2));
                                Canvas.SetZIndex(ellipse, 1);
                                ((Canvas)GridPanel.Children[0]).Children.Add(ellipse);
                            }
                        }
                    }

                    rowColumn = rowColumn.Parent;
                }
            }

            return shortestPath;
        }

        //first path-finding method (before bfs)
        /*
        private List<RowColumn> NadjiPutanju(PowerEntity startNode, PowerEntity endNode)
        {
            //lista gridica
            List<RowColumn> putanja = new List<RowColumn>(); //redja cvorove koji ce da cine konacnu putanju

            int rowFirst = startNode.Row;
            int columnFirst = startNode.Column;
            int rowSecond = endNode.Row;
            int columnSecond = endNode.Column;


            RowColumn rcStart = new RowColumn(rowFirst, columnFirst);
            RowColumn rcEnd = new RowColumn(rowSecond, columnSecond);

            if (rcStart != null)
            {
                putanja.Add(rcStart); //DODAMO GA U PUTANJU

                if (rowFirst < rowSecond)
                {
                    int tempRow = rowFirst;
                    while (tempRow != rowSecond)
                    {
                        tempRow = tempRow + 1;
                        putanja.Add(new RowColumn(tempRow, rcStart.Column));
                    }
                }
                else if (rowFirst > rowSecond)
                {
                    int tempRow = rowFirst;
                    while (tempRow != rowSecond)
                    {
                        tempRow = tempRow - 1;
                        putanja.Add(new RowColumn(tempRow, rcStart.Column));
                    }
                }
                else
                {
                    //nista 
                }

                if (columnFirst < columnSecond)
                {
                    int tempCol = columnFirst;
                    while (tempCol != columnFirst)
                    {
                        tempCol = tempCol + 1;
                        putanja.Add(new RowColumn(rowSecond, tempCol));
                    }
                }
                else if (columnFirst > columnSecond)
                {
                    int tempCol = columnFirst;
                    while (tempCol != columnFirst)
                    {
                        tempCol = tempCol - 1;
                        putanja.Add(new RowColumn(rowSecond, tempCol));
                    }
                }
                else
                {
                    //nista 
                }

                putanja.Add(rcEnd);
            }


            return putanja;
        }
        */

        #endregion


        public void DeleteEllipse_ShowImages()
        {
            LoadImages();
            foreach (var entity in Entities.Values)
            {
                if (entity.Ellipse != null)
                {
                    ((Canvas)GridPanel.Children[0]).Children.Remove(entity.Ellipse);

                    entity.Image = MakeImageForEntities(entity);

                    Canvas.SetTop(entity.Image, entity.Row * numberOfMiniGrids_Height - (entity.Image.Height / 2));
                    Canvas.SetLeft(entity.Image, entity.Column * numberOfMiniGrids_Width - (entity.Image.Width / 2));
                    Canvas.SetZIndex(entity.Image, 2);

                    ((Canvas)GridPanel.Children[0]).Children.Add(entity.Image);

                }
            }
        }

        private Image MakeImageForEntities(PowerEntity entity)
        {
            Image image = new Image
            {
                Width = 6,
                Height = 6,
                ToolTip = entity.ToString(),
            };
            if (entity.GetType().Name == "SubstationEntity")
            {
                image.Source = subs;
            }
            else if (entity.GetType().Name == "NodeEntity")
            {
                image.Source = node;
            }
            else if (entity.GetType().Name == "SwitchEntity")
            {
                image.Source = switchh;
            }
            image.MouseLeftButtonDown += new MouseButtonEventHandler(Left_Click_Entity);
            return image;
        }
      

        public void Left_Click_Entity(object sender, RoutedEventArgs e)
        {
            if (sender is Image)
            {
                EntityWindow ew = new EntityWindow((Image)sender);
                ew.ShowDialog();
            }
            else
            {
                EntityWindow ew1 = new EntityWindow((Ellipse)sender);
                ew1.ShowDialog();
            }

        }

        private void LoadImages()
        {
            //SUBSTITUTUON
            subs = new BitmapImage();
            subs.BeginInit();
            subs.UriSource = new Uri(@"C:\Users\Marija\Desktop\VisualisationProject\Project_Visualisation\Project_Visualisation\Images\image_substitution.png"); 
            //  node.UriSource = new Uri("https://play-lh.googleusercontent.com/ZyWNGIfzUyoajtFcD7NhMksHEZh37f-MkHVGr5Yfefa-IX7yj9SMfI82Z7a2wpdKCA=w240-h480-rw");
            subs.EndInit();

            //NODES
            node = new BitmapImage();
            node.BeginInit();
            node.UriSource = new Uri(@"C:\Users\Marija\Desktop\VisualisationProject\Project_Visualisation\Project_Visualisation\Images\node.png");
            node.EndInit();

            //SWITCH
            switchh = new BitmapImage();
            switchh.BeginInit();
            switchh.UriSource = new Uri(@"C:\Users\Marija\Desktop\PROJEKAT materijali\pz 1\PROJEKAT\PROJEKAT\Images\switch.png");
            switchh.EndInit();
        }

        public void DeleteImage_BackToEllipse()
        {
            foreach (var entity in Entities.Values)
            {
                if (entity.Image != null)
                {
                    ((Canvas)GridPanel.Children[0]).Children.Remove(entity.Image);

                    entity.Ellipse = MakeEllipseForEntity(entity);

                    Canvas.SetTop(entity.Ellipse, entity.Row * numberOfMiniGrids_Height - (entity.Ellipse.Height / 2));
                    Canvas.SetLeft(entity.Ellipse, entity.Column * numberOfMiniGrids_Width - (entity.Ellipse.Width / 2));
                    Canvas.SetZIndex(entity.Ellipse, 2);

                    ((Canvas)GridPanel.Children[0]).Children.Add(entity.Ellipse);
                }
            }

        }

        public void ColorEntity(SolidColorBrush entityColor, Ellipse el)
        {
            foreach (var entity in Entities.Values)
            {
                if (entity.Ellipse == el)
                {
                    foreach (var e2 in Entities.Values)
                    {
                        if (e2.GetType() == entity.GetType())
                        {
                            e2.Ellipse.Fill = entityColor;
                        }
                    }
                }
            }
        }

        private void ColorEntity_Connection_Click(object sender, RoutedEventArgs e)
        {
            foreach (var entity in Entities.Values)
            {
                if (entity.Connections >= 0 && entity.Connections < 3)
                {
                    if (entity.GetType().Name.Equals("SubstationEntity") && entity.Ellipse.Fill == Brushes.IndianRed)
                    {
                        entity.Ellipse.Fill = Brushes.Red;
                    }
                    else if (entity.GetType().Name.Equals("NodeEntity") && entity.Ellipse.Fill == Brushes.IndianRed)
                    {
                        entity.Ellipse.Fill = Brushes.Green;
                    }
                    else if (entity.GetType().Name.Equals("SwitchEntity") && entity.Ellipse.Fill == Brushes.IndianRed)
                    {
                        entity.Ellipse.Fill = Brushes.Blue;
                    }
                    else
                    {
                        entity.Ellipse.Fill = Brushes.IndianRed;
                    }


                }
                else if (entity.Connections >= 3 && entity.Connections < 5)
                {
                    if (entity.GetType().Name.Equals("SubstationEntity") && entity.Ellipse.Fill == Brushes.Red)
                    {
                        entity.Ellipse.Fill = Brushes.Red;
                    }
                    else if (entity.GetType().Name.Equals("NodeEntity") && entity.Ellipse.Fill == Brushes.Red)
                    {
                        entity.Ellipse.Fill = Brushes.Green;
                    }
                    else if (entity.GetType().Name.Equals("SwitchEntity") && entity.Ellipse.Fill == Brushes.Red)
                    {
                        entity.Ellipse.Fill = Brushes.Blue;
                    }
                    else
                    {
                        entity.Ellipse.Fill = Brushes.Red;
                    }
                }
                else if (entity.Connections > 5)
                {
                    if (entity.GetType().Name.Equals("SubstationEntity") && entity.Ellipse.Fill == Brushes.DarkRed)
                    {
                        entity.Ellipse.Fill = Brushes.Red;
                    }
                    else if (entity.GetType().Name.Equals("NodeEntity") && entity.Ellipse.Fill == Brushes.DarkRed)
                    {
                        entity.Ellipse.Fill = Brushes.Green;
                    }
                    else if (entity.GetType().Name.Equals("SwitchEntity") && entity.Ellipse.Fill == Brushes.DarkRed)
                    {
                        entity.Ellipse.Fill = Brushes.Blue;
                    }
                    else
                    {
                        entity.Ellipse.Fill = Brushes.DarkRed;
                    }
                }
                else
                {
                    //nothing
                }
            }
        }

        public void Right_Click_Line(object sender, RoutedEventArgs e)
        {
            Line line = (Line)sender;
            long IDLine;
            LineEntity lineEntity = new LineEntity();

            foreach (KeyValuePair<long, List<Line>> kp in allXMLLines)
            {
                if (kp.Value.Contains(line))
                {
                    IDLine = kp.Key;
                    lineEntity = LineEntities[IDLine];

                }
            }

            //return the previously clicked ones to the previous color
            if (startNode != null && endNode != null)
            {
                startNode.Ellipse.Fill = startNode.PreviousColor;
                endNode.Ellipse.Fill = endNode.PreviousColor;
            }

            //coloring entities whose line is clicked
            startNode = Entities[lineEntity.FirstEnd];
            endNode = Entities[lineEntity.SecondEnd];

            startNode.PreviousColor = startNode.Ellipse.Fill;
            endNode.PreviousColor = endNode.Ellipse.Fill;

            startNode.Ellipse.Fill = Brushes.Purple;
            endNode.Ellipse.Fill = Brushes.Purple;

        }

        private void Color_Line_Resistance_Click(object sender, RoutedEventArgs e)
        {
            foreach (var line in LineEntities.Values)
            {
                foreach (var el in allXMLLines)
                {
                    if (line.Id == el.Key)
                    {
                        foreach (Line lineOnCanvas in allXMLLines[line.Id])
                        {
                            if (line.Resistance < 1)
                            {
                                if (lineOnCanvas.Stroke == Brushes.Black)
                                {
                                    lineOnCanvas.Stroke = Brushes.Red;
                                }
                                else
                                {
                                    lineOnCanvas.Stroke = Brushes.Black;
                                }

                            }
                            else if (line.Resistance > 1 && line.Resistance <= 2)
                            {
                                if (lineOnCanvas.Stroke == Brushes.Black)
                                {
                                    lineOnCanvas.Stroke = Brushes.Orange;
                                }
                                else
                                {
                                    lineOnCanvas.Stroke = Brushes.Black;
                                }
                            }
                            else if (line.Resistance > 2)
                            {
                                if (lineOnCanvas.Stroke == Brushes.Black)
                                {
                                    lineOnCanvas.Stroke = Brushes.Yellow;
                                }
                                else
                                {
                                    lineOnCanvas.Stroke = Brushes.Black;
                                }
                            }
                            else
                            {
                                //nothing
                            }

                        }
                    }
                }
            }
        }

        private void Color_Line_Material_Click(object sender, RoutedEventArgs e)
        {
            foreach (var line in LineEntities.Values)
            {
                foreach (var el in allXMLLines)
                {
                    if (line.Id == el.Key)
                    {
                        foreach (Line lineOnCanvas in allXMLLines[line.Id])
                        {
                            if (line.CondMaterial == ConductorMaterial.Copper)
                            {
                                if (lineOnCanvas.Stroke == Brushes.Black)
                                {
                                    lineOnCanvas.Stroke = Brushes.Brown;
                                }
                                else
                                {
                                    lineOnCanvas.Stroke = Brushes.Black;
                                }

                            }
                            else if (line.CondMaterial == ConductorMaterial.Steel)
                            {
                                if (lineOnCanvas.Stroke == Brushes.Black)
                                {
                                    lineOnCanvas.Stroke = Brushes.DarkGray;
                                }
                                else
                                {
                                    lineOnCanvas.Stroke = Brushes.Black;
                                }
                            }
                            else if (line.CondMaterial == ConductorMaterial.Acsr)
                            {
                                if (lineOnCanvas.Stroke == Brushes.Black)
                                {
                                    lineOnCanvas.Stroke = Brushes.DarkMagenta;
                                }
                                else
                                {
                                    lineOnCanvas.Stroke = Brushes.Black;
                                }
                            }
                            else if (line.CondMaterial == ConductorMaterial.Other)
                            {
                                if (lineOnCanvas.Stroke == Brushes.Black)
                                {
                                    lineOnCanvas.Stroke = Brushes.DarkGreen;
                                }
                                else
                                {
                                    lineOnCanvas.Stroke = Brushes.Black;
                                }
                            }
                            else
                            {
                                //nothing
                            }

                        }
                    }
                }
            }

        }


    }
}
