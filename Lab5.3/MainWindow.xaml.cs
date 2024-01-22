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
        public const int mutationChance = 25;

        public List<MovingObject> Objects = new List<MovingObject>();
        public List<Point> Points = new List<Point>();
        public List<CollectableObject> CObjects = new List<CollectableObject>();

        Random rnd;

        int maxpopcount = 15;
        int simCount = 10;
        int curSim = 0;
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
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
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

        public void createCObjects()
        {
            int id;
            if (CObjects.Count == 0) id = 0;
            else id = CObjects[CObjects.Count-1].ID+1;
            CObjects.Clear();
            for (int i = id; i < pointCount+id; i++)
            {
                CObjects.Add(new CollectableObject(i, new Point(rnd.Next(9,491), rnd.Next(9, 491)), pointR));
            }
        }

        public void initPopulation()
        {
            for(int i = 0; i<movingObjCount; i++)
            {
                Objects.Add(new MovingObject(startPos, startRotation, rnd, objR));
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
            foreach (CollectableObject p in CObjects)
            {
                drawEllipse(p.point, pointR, 1);
            }
            foreach (MovingObject obj in Objects)
            {
                drawEllipse(obj.position, objR, 0);
            }
        }

        private void StartB_Click(object sender, RoutedEventArgs e)
        {
            createCObjects();
            DrawScene();
            timer.Start();
        }

        private void InitPopB_Click(object sender, RoutedEventArgs e)
        {
            initPopulation();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            simCounter.Content = curSim + 1;
            if (iteration < maxiterations)
            {
                itcounter.Content = iteration+1;
                foreach(MovingObject obj in Objects)
                {
                    obj.Moves[obj.movecounter]();
                    obj.movecounter++;
                    if (obj.movecounter == obj.Moves.Count) obj.movecounter = 0;

                    foreach(CollectableObject cobj in CObjects)
                    {
                        if(!obj.collectedPoints.Contains(cobj.ID)) 
                            if(intersection(obj,cobj)) 
                                obj.collectedPoints.Add(cobj.ID);
                    }
                }

                if(iteration % 100 == 0)
                {
                    createCObjects();
                }



                DrawScene();
                iteration++;
            }
            else
            {
                curSim++;
                if(curSim < simCount)
                {
                    StartNewSimulation();
                }
                else
                {
                    timer.Stop();
                }
            }
        }

        private bool intersection(MovingObject obj, CollectableObject cobj)
        {
            double distanceBetweenCenters = Math.Sqrt(Math.Pow(cobj.point.X - obj.position.X, 2) + Math.Pow(cobj.point.Y - obj.position.Y, 2));

            
            return distanceBetweenCenters <= pointR + objR;
        }

        private int CalculateFitness()
        {
            List<int> fitneses = new List<int>();
            foreach(MovingObject obj in Objects)
            {
                int fit = 0;
                foreach(int id in  obj.collectedPoints)
                {
                    fit += 100;
                }

                obj.fitness = fit;
                obj.collectedPoints = new List<int>();
                fitneses.Add(fit);
            }

            Objects.Sort((x, y) => x.fitness.CompareTo(y.fitness));

            return fitneses.Max();
        }

        private void createNewPop()
        {
            int n = Objects.Count;
            for (int i = 1; i< n; i++)
            {
                if (Objects[i].fitness > 0)
                {
                    Objects.Add(Objects[0].crossover(Objects[i]));
                }
            }

            foreach(MovingObject obj in Objects)
            {
                if(rnd.Next(100)>mutationChance)
                {
                    obj.Mutate();
                }
                obj.position = startPos;
                obj.movecounter = 0;
            }

            Objects.Sort((x, y) => x.fitness.CompareTo(y.fitness));

            if (Objects.Count > maxpopcount)
            {
                for (int i = Objects.Count-1; i > maxpopcount-1; i--)
                {
                    Objects.Remove(Objects[i]);
                }
            }

        }
        
        private void StartNewSimulation()
        {
            CObjects.Clear();
            bestFitness.Content = CalculateFitness();
            iteration = 0;
            createNewPop();
            createCObjects();
            DrawScene();
        }


    }
}
