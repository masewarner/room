using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static room.DrawScreen;

namespace room
{
    class Mob
    {

        public ConsoleColor Color { get; set; }
        public char Icon { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public int HomeX { get; set; }
        public int HomeY { get; set; }

        public Mob()
        {

        }

        public AI CurrentAI { get; set; }

        public void Think(World w)
        {
            if (CurrentAI != null)
            {
                CurrentAI.Think(w, this);
            }
        }

        public void Aggro()
        {
            CurrentAI = new Aggro();
            Color = ConsoleColor.Red;
        }

        public void LoseAggro()
        {
            CurrentAI = new GoHomeAI(HomeX, HomeY);
            Color = ConsoleColor.Yellow;
        }
    }

    interface AI
    {
        void Think(World w, Mob m);
    }

    class Wander : AI
    {
        // What to do about AI state?
        public void Think(World w, Mob m)
        {
            if (w.RandomGenerator.Next(0, 2) == 1)
            {
                int x = m.X;
                int y = m.Y;
                switch ((Direction)w.RandomGenerator.Next(0, 3))
                {
                    case Direction.North: y--; break;
                    case Direction.South: y++; break;
                    case Direction.East: x++; break;
                    case Direction.West: x--; break;
                }

                if (w.CanWalk(x, y))
                {
                    m.X = x;
                    m.Y = y;
                }

                if (w.DistanceToPlayer(x, y) < 3)
                {
                    m.Aggro();
                }
            }
        }
    }

    class Aggro : AI
    {
        public void Think(World w, Mob m)
        {
            if (w.RandomGenerator.Next(0, 2) == 1)
            {
                int x = m.X;
                int y = m.Y;

                // Get Player direction and walk one step toward.
                if (m.X > w.Player.X)
                    x--;
                if (m.X < w.Player.X)
                    x++;
                if (m.Y > w.Player.Y)
                    y--;
                if (m.Y < w.Player.Y)
                    y++;

                if (w.CanWalk(x, y))
                {
                    m.X = x;
                    m.Y = y;
                }

                if (w.DistanceToPlayer(x, y) > 10)
                {
                    m.LoseAggro();
                }
            }
        }
    }

    class GoHomeAI : AI
    {
        public int TargetX { get; set; }
        public int TargetY { get; set; }

        public GoHomeAI(int targetX, int targetY)
        {
            TargetX = targetX;
            TargetY = targetY;
        }

        public void Think(World w, Mob m)
        {
            int x = m.X;
            int y = m.Y;

            // Get Player direction and walk one step toward.
            if (m.X > TargetX)
                x--;
            if (m.X < TargetX)
                x++;
            if (m.Y > TargetY)
                y--;
            if (m.Y < TargetY)
                y++;

            if (w.CanWalk(x, y))
            {
                m.X = x;
                m.Y = y;
            }

            if (x == TargetX && y == TargetY)
            {
                m.CurrentAI = new Wander();
                m.Color = ConsoleColor.Green;
            }
            else if (w.DistanceToPlayer(x, y) < 3)
            {
                m.Aggro();
            }
        }
    }


    class World
    {
        public List<Mob> Mobs { get; set; }
        public Random RandomGenerator { get; set; }

        public Mob Player { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public World(Random randomGenerator)
        {
            Mobs = new List<Mob>();
            RandomGenerator = randomGenerator;
        }

        public CharInfo[] Floor { get; set; }

        public bool CanWalk(int x, int y)
        {
            if (x < 0 || y < 0)
                return false;
            if (x >= Width || y >= Height)
                return false;

            if (Floor[y * Width + x].Char.AsciiChar == '.' && !HasMob(x, y))
                return true;
            return false;
        }

        public bool HasMob(int x, int y)
        {
            for (int i = 0; i < Mobs.Count; i++)
            {
                var mob = Mobs[i];
                if (mob.X == x && mob.Y == y)
                {
                    return true;
                }
            }

            return false;
        }

        public int DistanceToPlayer(int x, int y)
        {
            int x1 = Player.X;
            int y1 = Player.Y;
            int x2 = x;
            int y2 = y;

            return (int)GetDistance(x1, y1, x2, y2);
        }

        private static double GetDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }
    }

    public enum Direction
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3
    }
}
