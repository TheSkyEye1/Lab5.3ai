using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Lab5._3.MovingObject;

namespace Lab5._3
{
    public class MovingObject
    {
        Random rnd;
        public Point position;
        public double currentRotation;
        public int fitness = 0;
        public int movecounter = 0;
        public string name;

        int distance;
        public int lifeTime = 0;
        public int lifeSpan;
        public List<Move> Moves = new List<Move>();
        public List<int> collectedPoints = new List<int>();


        public delegate void Move();

        
        public void MoveForward()
        {
            double x = position.X + distance * Math.Cos(currentRotation);
            double y = position.Y + distance * Math.Sin(currentRotation);
            if (x > 500) x = x - 500;
            if (x < 0) x = 500 + x;
            if (y > 500) y = y - 500;
            if (y < 0) y = 500 + y;

            position = new Point(x, y);
        }

        public void RotateLeft()
        {
            if (currentRotation == 0) currentRotation = 315;
            else currentRotation -= 45;
        }

        public void RotateRight()
        {
            if (currentRotation == 315) currentRotation = 0;
            else currentRotation += 45;
        }
        public List<Move> CreateGenes()
        {
            List<Move> movlist = new List<Move>();
            int movescount = rnd.Next(25, 100);
            for(int i = 0; i<movescount; i++)
            {
                int m = rnd.Next(3);
                switch(m)
                {
                    case 0:
                        Move moveforward = MoveForward;
                        movlist.Add(moveforward);
                        break;
                    case 1:
                        Move moveleft = RotateLeft;
                        movlist.Add(moveleft);
                        break;
                    case 2:
                        Move moveright = RotateRight;
                        movlist.Add(moveright);
                        break;
                }
            }
            return movlist;
        }

        public void setMoves(List<Move> movlist)
        {
            this.Moves = movlist;
        }

        public void ResetPos(Point pos, double curRot)
        {
            this.position = pos;
            this.currentRotation = curRot;
        }

        public MovingObject(Point position, double currentRotation, Random rnd, int distance, int lifeSpan, string name)
        {
            this.rnd = rnd;
            this.position = position;
            this.currentRotation = 0;
            this.Moves = CreateGenes();
            this.distance = distance;
            this.lifeSpan = rnd.Next(1,lifeSpan);
            this.name = name;
            
        }

        public void Mutate()
        {
            for(int i = 0; i<Moves.Count;i++)
            {
                if(rnd.Next(100) > MainWindow.mutationChance)
                {
                    if (Moves[i] == MoveForward)
                    {
                        if (rnd.Next(2) == 0) Moves[i] = RotateLeft;
                        else Moves[i] = RotateRight;
                    }
                    else if (Moves[i] == RotateLeft)
                    {
                        if (rnd.Next(2) == 0) Moves[i] = MoveForward;
                        else Moves[i] = RotateRight;
                    }
                    else
                    {
                        if (rnd.Next(2) == 0) Moves[i] = RotateLeft;
                        else Moves[i] = MoveForward;
                    }
                }
            }   
        }

        public MovingObject crossover(MovingObject parent2)
        {
            List<Move> newMoves = new List<Move>();

            List<Move> MaxMoves = new List<Move>();
            int minmoves = 0;
            int maxmoves = 0;

            if (Moves.Count == parent2.Moves.Count)
            {
                minmoves = parent2.Moves.Count;
                maxmoves = parent2.Moves.Count;
            }
            else if (Moves.Count > parent2.Moves.Count)
            {
                minmoves = parent2.Moves.Count;
                maxmoves = Moves.Count;
                MaxMoves = Moves;
            }
            else
            {
                minmoves = Moves.Count;
                maxmoves = parent2.Moves.Count;
                MaxMoves = parent2.Moves;
            }


            for (int i = 0; i < maxmoves; i++)
            {
                if (i < minmoves)
                {
                    if (rnd.Next(2) == 0) newMoves.Add(Moves[i]);
                    else newMoves.Add(parent2.Moves[i]);
                }
                else
                {
                    if (rnd.Next(4) == 0)
                    {
                        break;
                    }
                    else
                    {
                        newMoves.Add(MaxMoves[i]);
                    }
                }
            }

            MovingObject children = new MovingObject(new Point(250,250), 0, rnd, distance, this.lifeSpan, "child" + name + "_" + parent2.name);
            children.setMoves(newMoves);
            return children;

        }
    }
}
