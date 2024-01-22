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
using System.Windows.Threading;

namespace Lab5._3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<MovingObject> Objects = new List<MovingObject>();
        public List<Point> Points = new List<Point>();

        Random rnd;

        int pointCount = 5;
        int movingObjCount = 10;
        Point startPos = new Point(250,250);
        int startRotation = 0;

        int pointR = 10;
        int objR = 15;

        int iteration = 0;
        int maxiterations = 1000;
        DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            rnd = new Random();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += Timer_Tick;
        }

        public void CreatePoints()
        {
            Points.Clear();
            for(int i = 0; i<pointCount; i++)
            {
                Points.Add(new Point(rnd.Next(9,490), rnd.Next(9, 490)));
            }
        }

        public void initPopulation()
        {
            for(int i = 0; i<movingObjCount; i++)
            {
                Objects.Add(new MovingObject(startPos, startRotation, rnd));
            }
        }

        public void drawEllipse(Point p, double r, int type)
        {
            Ellipse el = new Ellipse();
            SolidColorBrush cb = new SolidColorBrush();
            if (type == 0) cb.Color = Color.FromArgb(255, 0, 0, 0);
            else cb.Color = Color.FromArgb(0, 255, 0, 0);
            el.Fill = cb;
            el.StrokeThickness = 1;
            el.Stroke = Brushes.Black;
            el.Width = r;
            el.Height = r;
            el.RenderTransform = new TranslateTransform(p.X - r/2, p.Y - r / 2);
            scene.Children.Add(el);
        }

        public void DrawScene()
        {
            scene.Children.Clear();
            foreach (Point p in Points)
            {
                drawEllipse(p, pointR, 1);
            }
            foreach (MovingObject obj in Objects)
            {
                drawEllipse(obj.position, objR, 0);
            }
        }

        private void StartB_Click(object sender, RoutedEventArgs e)
        {
            CreatePoints();
            DrawScene();
            timer.Start();
            
        }

        private void InitPopB_Click(object sender, RoutedEventArgs e)
        {
            initPopulation();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(iteration < maxiterations)
            {
                itcounter.Content = iteration;
                foreach(MovingObject obj in Objects)
                {
                    obj.Moves[obj.movecounter]();
                    obj.movecounter++;
                    if (obj.movecounter == obj.Moves.Count) obj.movecounter = 0;
                }
                DrawScene();
                iteration++;
            }
            else
            {
                iteration = 0;
                timer.Stop();
            }
        }
    }
}
