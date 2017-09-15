using System;
using System.Collections.Generic;
using static room.DrawScreen;

namespace room
{
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
}
