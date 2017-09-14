using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using static room.DrawScreen;

namespace room
{
    class Program
    {
        public static bool run = true;
        public static int width = 120;
        public static int height = 30;

        [STAThread]
        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            var floor = InitFloor(width, height);
            // Paint the floor

            // OK.  Actually generate the floor ;)
            var rooms = new List<SmallRect>();
            GenerateFloor(floor, rooms, 0, 0, null);

            var playerX = 1;
            var playerY = 1;

            // 30x30 to start.  No border?
            bool dirty = true;
            var toDraw = new CharInfo[width * height];
            //DrawRooms(floor, rooms);
            playerX = rooms[0].Left;
            playerY = rooms[0].Top;

            while(run)
            {
                if (Console.KeyAvailable)
                {
                    var inputKey = Console.ReadKey(true);
                    int targetX = playerX;
                    int targetY = playerY;
                    switch (inputKey.Key)
                    {
                        case ConsoleKey.A:
                            if (targetX > 0)
                                targetX--;
                            break;
                        case ConsoleKey.W:
                            if (targetY > 0)
                                targetY--;
                            break;
                        case ConsoleKey.S:
                            if (targetY < height)
                                targetY++;
                            break;
                        case ConsoleKey.D:
                            if (targetX < width)
                                targetX++;
                            break;
                    }

                    if (CanWalk(floor, targetX, targetY))
                    {
                        playerX = targetX;
                        playerY = targetY;
                    }
                    dirty = true;
                }

                if (dirty)
                {
                    Array.Copy(floor, toDraw, width * height);
                    //DrawRooms(toDraw, rooms);
                    toDraw[playerY * width + playerX].Char.AsciiChar = (byte)'S';
                    toDraw[playerY * width + playerX].Attributes = 0x2 | 0x8;
                    DrawFloor(toDraw);
                    dirty = false;
                }
                // Room collision
                // Moving AI
                // Efficient tile drawing and updating...
                // https://stackoverflow.com/questions/2754518/how-can-i-write-fast-colored-output-to-console
                //Draw(oldX, oldY, ConsoleColor.Black, ConsoleColor.Cyan, floor[oldY][oldX]);
                //Draw(playerX, playerY, ConsoleColor.Black, ConsoleColor.White, 'S');
                // Draw screen vs. deltas?  Start with a simple buffer write.
                Thread.Sleep(1);
            }
        }

        static CharInfo[] InitFloor(int width, int height)
        {
            CharInfo[] floor = new CharInfo[width * height];
            for (int i = 0; i < floor.Length; i++)
            {
                floor[i].Char.AsciiChar = (byte)' ';
                floor[i].Attributes = (short)(0x1 | 0x8);
            }

            return floor;
        }

        static bool CanWalk(CharInfo[] floor, int x, int y)
        {
            if (floor[y * width + x].Char.AsciiChar != '#')
                return true;
            return false;
        }

        static void DrawFloor(CharInfo[] floor)
        {
            bool b = WriteConsoleOutput(_drawHandle, floor,
                                      new Coord() { X = 120, Y = 30 },
                                      new Coord() { X = 0, Y = 0 },
                                      ref rect);
        }

        private static SafeFileHandle _drawHandle = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
        private static SmallRect rect = new SmallRect() { Left = 0, Top = 0, Right = 120, Bottom = 30 };


        static void Draw(int x, int y, ConsoleColor bg, ConsoleColor color, string s)
        {
            Console.CursorTop = y;
            Console.CursorLeft = x;
            Console.BackgroundColor = bg;
            Console.ForegroundColor = color;
            Console.Write(s);
        }

        static void Draw(int x, int y, ConsoleColor bg, ConsoleColor color, char s)
        {
            Console.CursorTop = y;
            Console.CursorLeft = x;
            Console.BackgroundColor = bg;
            Console.ForegroundColor = color;
            Console.Write(s);
        }

        private static int seed = 1;
        private static Random random = new Random(seed);

        static void DrawRooms(CharInfo[] floor, List<SmallRect> rooms)
        {
            for(int i=0; i<rooms.Count;i++)
            {
                var room = rooms[i];
                DrawRoom(floor, room);
            }
        }

        static void DrawRoom(CharInfo[] floor, SmallRect room)
        {
            FillRect(floor, new SmallRect { Left = (short)(room.Left - 1), Top = (short)(room.Top - 1), Bottom = (short)(room.Bottom + 1), Right = (short)(room.Right + 1) }, '#', ConsoleColor.DarkGray);
            FillRect(floor, room, '.', ConsoleColor.DarkYellow);
        }

        static void FillRect(CharInfo[] floor, SmallRect area, char value, ConsoleColor color)
        {
            for (int i = area.Top; i < area.Bottom; i++)
            {
                for (int j = area.Left; j < area.Right; j++)
                {
                    SetFloor(floor, j, i, value, color);
                }
            }
        }

        static void FillRectIf(CharInfo[] floor, SmallRect area, char value, ConsoleColor color, char ifChar)
        {
            for (int i = area.Top; i < area.Bottom; i++)
            {
                for (int j = area.Left; j < area.Right; j++)
                {
                    if (GetFloor(floor, j, i) == ifChar)
                        SetFloor(floor, j, i, value, color);
                }
            }
        }

        static void GenerateFloor(CharInfo[] floor, List<SmallRect> rooms, int startX, int startY, Direction? roomDir)
        {
            int lastX = startX;
            int lastY = startY;
            for (int i = 0; i < 10; i++)
            {
                var roomWidth = random.Next(3, 6);
                var roomHeight = random.Next(3, 7);
                int x;
                int y;
                if (!roomDir.HasValue)
                {
                    x = random.Next(50, 70);
                    y = random.Next(10, 15);
                }
                else
                {
                    switch(roomDir.Value)
                    {
                        case Direction.North: x = ClampShortToWorldX(lastX - (roomWidth / 2)); y = ClampShortToWorldY(lastY - (roomHeight / 2)); break;
                        case Direction.South: x = ClampShortToWorldX(lastX - (roomWidth / 2)); y = lastY;  break;
                        case Direction.East: x = lastX; y = ClampShortToWorldY(lastY - (roomHeight / 2)); break;
                        case Direction.West: x = ClampShortToWorldX(lastX - roomWidth); y = ClampShortToWorldY(lastY - (roomHeight / 2)); break;
                        default:
                            throw new Exception("Unknown direction");
                    }
                }

                var room = new SmallRect()
                {
                    Left = ClampShortToWorldX(x),
                    Top = ClampShortToWorldY(y),
                    Bottom = ClampShortToWorldY(y + roomHeight),
                    Right = ClampShortToWorldX(x + roomWidth)
                };
                rooms.Add(room);
                DrawRoomIf(floor, room);

                Direction dir = (Direction)random.Next(0, 3);
                int xToFill = 0;
                int yToFill = 0;
                int cooridorLength = random.Next(4, 10);
                switch (dir)
                {
                    case Direction.North: xToFill = ClampShortToWorldX(random.Next(x, x + roomWidth)); yToFill = ClampShortToWorldY(y); break;
                    case Direction.South: xToFill = ClampShortToWorldX(random.Next(x, x + roomWidth)); yToFill = ClampShortToWorldY(y + roomHeight); break;
                    case Direction.East: xToFill = ClampShortToWorldX(x + roomWidth); yToFill = ClampShortToWorldY(random.Next(y, y + roomHeight)); break;
                    case Direction.West: xToFill = ClampShortToWorldX(x); yToFill = yToFill = ClampShortToWorldY(random.Next(y, y + roomHeight)); break;
                }

                SmallRect cooridorRect = new SmallRect() { Top = (short)yToFill, Left = (short)xToFill, Right = ClampShortToWorldX(xToFill + 1), Bottom = ClampShortToWorldY(yToFill + 1) };
                switch (dir)
                {
                    //case Direction.North: cooridorRect.Right = ; break;
                    case Direction.East: cooridorRect.Right = ClampShortToWorldX(xToFill + cooridorLength); break;
                    case Direction.West: cooridorRect.Left = ClampShortToWorldX(xToFill - cooridorLength); cooridorRect.Right = ClampShortToWorldX(xToFill); break;
                    case Direction.South: cooridorRect.Bottom = ClampShortToWorldY(yToFill + cooridorLength); break;
                    case Direction.North: cooridorRect.Top = ClampShortToWorldY(yToFill - cooridorLength); break;
                }
                roomDir = dir;

                // Break out of the corner
                if ((cooridorRect.Right - cooridorRect.Left) == 0)
                    cooridorRect.Left = ClampShortToWorldX(cooridorRect.Left - 1);
                if ((cooridorRect.Bottom - cooridorRect.Top) == 0)
                    cooridorRect.Top = ClampShortToWorldY(cooridorRect.Top - 1);

                lastX = cooridorRect.Left;
                lastY = cooridorRect.Top;
                DrawRoomIf(floor, cooridorRect);

                //SetFloor(floor, xToFill, yToFill, '.', ConsoleColor.DarkYellow);
            }
        }

        static short ClampShortToWorldX(int value)
        {
            // Leave space for 1 px border around the world for walls
            return ClampShort(value, 1, width-1);
        }

        static short ClampShortToWorldY(int value)
        {
            // Leave space for 1 px border around the world for walls
            return ClampShort(value, 1, height-1);
        }

        static short ClampShort(int value, int min, int max)
        {
            if (value < min)
                return (short)min;
            if (value > max)
                return (short)max;
            return (short)value;
        }

        static void DrawRoomIf(CharInfo[] floor, SmallRect room)
        {
            FillRectIf(floor, new SmallRect { Left = (short)(room.Left - 1), Top = (short)(room.Top - 1), Bottom = (short)(room.Bottom + 1), Right = (short)(room.Right + 1) }, '#', ConsoleColor.DarkGray, ' ');
            FillRect(floor, room, '.', ConsoleColor.DarkYellow);
        }

        enum Direction
        {
            North = 0,
            East = 1,
            South = 2,
            West = 3
        }

        static void SetFloor(CharInfo[] floor, int x, int y, char c, ConsoleColor color)
        {
            int index = y * width + x;
            floor[index].Char.AsciiChar = (byte)c;
            short colorShort = (short)color;
                /*
The different color codes are

0   BLACK
1   BLUE
2   GREEN
3   CYAN
4   RED
5   MAGENTA
6   BROWN
7   LIGHTGRAY
8   DARKGRAY
9   LIGHTBLUE
10  LIGHTGREEN
11  LIGHTCYAN
12  LIGHTRED
13  LIGHTMAGENTA
14  YELLOW
15  WHITE
*/
            floor[index].Attributes = colorShort;
        }

        static char GetFloor(CharInfo[] floor, int x, int y)
        {
            int index = y * width + x;
            return (char)floor[index].Char.AsciiChar;
        }
    }
}
