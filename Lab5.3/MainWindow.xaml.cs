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

        bool selectedMode = false;

        int MaxLifeTime = 4;
        int maxpopcount = 15;
        int simCount = 10;
        int curSim = 0;
        int pointCount = 5;
        int movingObjCount = 10;
        Point startPos = new Point(250,250);
        int startRotation = 0;

        int pointR = 30;
        int objR = 40;
        int iterationsToReroll = 100;
        int iteration = 0;
        int maxiterations = 1000;
        DispatcherTimer timer;
        public int NameIndex = 0;


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
                Objects.Add(new MovingObject(startPos, startRotation, rnd, objR, MaxLifeTime, NameIndex.ToString()));
                NameIndex++;
            }
        }

        public void loadFromForm()
        {
            maxpopcount = int.Parse(MaxPopTB.Text);
            simCount = int.Parse(SimulationsTB.Text);
            pointCount = int.Parse(ColectablesTB.Text);
            movingObjCount = int.Parse(StartPopTB.Text);
            maxiterations = int.Parse(IterationsTB.Text);
            iterationsToReroll = int.Parse(IterationsRerollTB.Text);
        }


        public void drawEllipse(Point p, double r, int type)
        {
            Ellipse el = new Ellipse();
            SolidColorBrush cb = new SolidColorBrush();
            if (type == 0) cb.Color = Color.FromArgb(255, 0, 0, 0);
            else if (type == 1) cb.Color = Color.FromArgb(255, 0, 255, 0);
            else cb.Color = Color.FromArgb(255, 255, 0, 0);
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
            for(int i = 0; i<Objects.Count; i++)
            {
                if (i == 0) drawEllipse(Objects[i].position, objR, 2);
                else drawEllipse(Objects[i].position, objR, 1);
            }
            foreach (CollectableObject obj in CObjects)
            {
                drawEllipse(obj.point, pointR, 0);
            }
        }

        List<Line> Lines = new List<Line>();
        public void CreateLine(Point a, Point b)
        {
            Line line = new Line();
            line.X1 = a.X;
            line.Y1 = a.Y;
            line.X2 = b.X;
            line.Y2 = b.Y;
            line.Stroke = Brushes.Red;
            line.StrokeThickness = 2;

            if (Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2)) < 50)
            {
                Lines.Add(line);
            }
        }

        public void DrawLines()
        {
            foreach(Line line in Lines)
            {
                scene.Children.Add(line);
            }
        }

        private void LoadListBox()
        {
            ObjectsList.SelectedItem = -1;
            ObjectsList.Items.Clear();
            foreach (MovingObject obj in Objects)
            {
                ObjectsList.Items.Add(obj.name.ToString());
            }
        }

        private void StartB_Click(object sender, RoutedEventArgs e)
        {
            if (curSim == 0)
            {
                loadFromForm();
                createCObjects();
                DrawScene();
                LoadListBox();
                timer.Start();

            }
            else if(curSim < simCount)
            {
                StartNewSimulation();
            }

        }

        private void InitPopB_Click(object sender, RoutedEventArgs e)
        {
            initPopulation();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (selectedMode == false && curSim != simCount)
            {
                curPop_counter.Content = Objects.Count;
                simCounter.Content = curSim + 1;
                if (iteration < maxiterations)
                {
                    itcounter.Content = iteration + 1;
                    foreach (MovingObject obj in Objects)
                    {
                        switch(obj.Moves[obj.movecounter])
                        {
                            case ("F"): { obj.MoveForward(); break; }
                            case ("L"): { obj.RotateLeft(); break; }
                            case ("R"): { obj.RotateRight(); break; }
                        }
                        obj.movecounter++;
                        if (obj.movecounter == obj.Moves.Count) obj.movecounter = 0;

                        foreach (CollectableObject cobj in CObjects)
                        {
                            if (!obj.collectedPoints.Contains(cobj.ID))
                                if (intersection(obj, cobj))
                                    obj.collectedPoints.Add(cobj.ID);
                        }
                    }

                    //if(iteration % iterationsToReroll == 0)
                    //{
                    //    
                    //}

                    DrawScene();
                    iteration++;
                }

                else if(ckbx.IsChecked == true)
                {
                    timer.Stop();
                    bestFitness.Content = CalculateFitness();
                    iteration = 0;
                    LoadListBox();
                    curSim++;
                    
                    if (curSim != simCount)
                    {
                        StartNewSimulation();
                    } 

                }
                else
                {
                    bestFitness.Content = CalculateFitness();
                    iteration = 0;
                    LoadListBox();
                    curSim++;
                    timer.Stop();
                }
            }
            else
            {
                scene.Children.Clear();

                MovingObject obj = Objects[ObjectsList.SelectedIndex];

                Point a = obj.position;

                drawEllipse(obj.position, objR, 2);

                switch (obj.Moves[obj.movecounter])
                {
                    case ("F"): { obj.MoveForward(); break; }
                    case ("L"): { obj.RotateLeft(); break; }
                    case ("R"): { obj.RotateRight(); break; }
                }

                obj.movecounter++;

                Point b = obj.position;
                CreateLine(a, b);
                DrawLines();

                if (obj.movecounter == obj.Moves.Count) obj.movecounter = 0;

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

            Objects.Sort((x, y) => y.fitness.CompareTo(x.fitness));
            return fitneses.Max();
        }

        private void createNewPop()
        {
            int n = Objects.Count;

            //for (int i = 1; i< n; i++)
            //{
            //    if (Objects[i].fitness > 0)
            //    {
            //        Objects.Add(Objects[0].crossover(Objects[i]));
            //    }
            //}
            List<MovingObject> parents = new List<MovingObject>();

            for(int i = 0; i<n/2+2; i++)
            {
                int j = rnd.Next(n);
                if (parents.Contains(Objects[j]))
                {
                    i--;
                }
                else
                {
                    parents.Add(Objects[j]);
                }
            }

            while(parents.Count>1)
            {
                int index1 = rnd.Next(parents.Count);
                int index2 = index1;

                while(index1 == index2)
                {
                    index2 = rnd.Next(parents.Count);
                }

                MovingObject parent1 = parents[index1];
                MovingObject parent2 = parents[index2];

                Objects.Add(parent1.crossover(parent2));

                parents.Remove(parent1);
                parents.Remove(parent2);
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

            Objects.Sort((x, y) => y.fitness.CompareTo(x.fitness));

            for(int i = 0; i<Objects.Count; i++)
            {
                if (Objects[i].lifeTime >= Objects[i].lifeSpan)
                {
                    Objects.Remove(Objects[i]);
                }
            }
        }
        
        private void StartNewSimulation()
        {
            foreach(MovingObject obj in Objects)
            {
                obj.lifeTime++;
            }
            CObjects.Clear();
            createNewPop();
            createCObjects();
            LoadListBox();
            DrawScene();
            timer.Start();
        }

        private void StartLocalB_Click(object sender, RoutedEventArgs e)
        {
            if (ObjectsList.SelectedItems.Count != -1 && selectedMode == false)
            {
                Objects[ObjectsList.SelectedIndex].position = startPos;
                Objects[ObjectsList.SelectedIndex].currentRotation = 0;
                Lines.Clear();
                StartLocalB.Content = "Stop Local";
                selectedMode = true;
                timer.Start();
            }
            else
            {
                StartLocalB.Content = "Start Local";
                selectedMode = false;
                timer.Stop();
            }
        }

        private void ObjectsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ObjectsList.SelectedItems.Count > 0)
            {
                MovesCountLB.Content = Objects[ObjectsList.SelectedIndex].Moves.Count;
                AgeLB.Content = Objects[ObjectsList.SelectedIndex].lifeTime + 1;
                LifeSpanLB.Content = Objects[ObjectsList.SelectedIndex].lifeSpan + 1;
                FitnessLB.Content = Objects[ObjectsList.SelectedIndex].fitness;
            }
        }
    }
}
