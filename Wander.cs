namespace room
{
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
}
