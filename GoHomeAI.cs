using System;

namespace room
{
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
}
