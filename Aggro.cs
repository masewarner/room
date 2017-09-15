namespace room
{
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
}
