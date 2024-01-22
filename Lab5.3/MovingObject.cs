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

        int distance = 15;

        public List<Move> Moves = new List<Move>();

        public delegate void Move();

        
        public void MoveForward()
        {
            double x = position.X + distance * Math.Cos(currentRotation);
            double y = position.Y + distance * Math.Sin(currentRotation);
            if (x > 500) x = 484;
            if (x < 0) x = 14;
            if (y > 500) y = 484;
            if (y < 0) y = 14;

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
            int movescount = rnd.Next(25, 50);
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

        public void ResetPos(Point pos, double curRot)
        {
            this.position = pos;
            this.currentRotation = curRot;
        }

        public MovingObject(Point position, double currentRotation, Random rnd)
        {
            this.rnd = rnd;
            this.position = position;
            this.currentRotation = 0;
            this.Moves = CreateGenes();
            
        }
    }
}
