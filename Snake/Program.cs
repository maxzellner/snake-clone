using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static System.Console;

namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            WindowWidth = 32;
            WindowHeight = 16;
            SetBufferSize(WindowWidth, WindowHeight);
         
            int startingLength = 5;

            var head = new Pixel(WindowWidth / 2, WindowHeight / 2, ConsoleColor.Green);
            var body = new List<Pixel> ();

            var berry = new Berry();

            Direction currentMovement = Direction.Right;

            Border.Draw();
            Intro.Draw();

            while (!Console.KeyAvailable) { }

            Intro.Clear();
            Border.Draw();
            berry.Spawn();

            while (true)
            {
                var headOld = new Pixel(head.XPos, head.YPos);


                var sw = Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds <= 100)
                {
                    currentMovement = ReadMovement(currentMovement);
                }

                body.Add(headOld);
                body.ForEach(x => x.Clear());

                switch (currentMovement)
                {
                    case Direction.Up:
                        head.MoveUp();
                        break;
                    case Direction.Down:
                        head.MoveDown();
                        break;
                    case Direction.Left:
                        head.MoveLeft();
                        break;
                    case Direction.Right:
                        head.MoveRight();
                        break;
                }      

                if (body.Count > Score.score + startingLength)
                {
                    body.RemoveAt(0);
                }

                headOld.Clear();
                body.ForEach(x => x.Draw());
                head.Draw();

                if (berry.WasEaten(head))
                {
                    Score.Increase();
                    berry.Spawn();
                }

                Score.Draw();

                if (body.Any(x => x.Touches(head) == true)) 
                {
                    break;
                }

                if (head.XPos == 0 || head.XPos == WindowWidth - 1 || head.YPos == 0 || head.YPos == WindowHeight - 1)
                {
                    break;
                }
            }

            Score.Gameover();

            head.Explode();

            SetCursorPosition(1, 1);
            ReadKey();
        }

        static Direction ReadMovement(Direction movement)
        {
            if (KeyAvailable)
            {
                var key = ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow && movement != Direction.Down)
                {
                    movement = Direction.Up;
                }
                else if (key == ConsoleKey.DownArrow && movement != Direction.Up)
                {
                    movement = Direction.Down;
                }
                else if (key == ConsoleKey.LeftArrow && movement != Direction.Right)
                {
                    movement = Direction.Left;
                }
                else if (key == ConsoleKey.RightArrow && movement != Direction.Left)
                {
                    movement = Direction.Right;
                }
            }

            return movement;
        }
    }

    class Pixel
    {
        public Pixel(int xPos, int yPos, ConsoleColor color = ConsoleColor.Gray)
        {
            XPos = xPos;
            YPos = yPos;
            ScreenColor = color;
        }
        public int XPos { get; set; }
        public int YPos { get; set; }
        public ConsoleColor ScreenColor { get; set; }

        public void Draw()
        {
            ForegroundColor = ScreenColor;
            SetCursorPosition(XPos, YPos);
            Write("■");
            SetCursorPosition(0, 0);
        }

        public void Clear()
        {
            SetCursorPosition(XPos, YPos);
            Write(" ");
            SetCursorPosition(0, 0);
        }

        public bool Touches(Pixel pixel)
        {
            return (this.XPos == pixel.XPos) && (this.YPos == pixel.YPos);
        }

        public void MoveUp() { YPos--; }
        public void MoveDown() { YPos++; }
        public void MoveLeft() { XPos--; }
        public void MoveRight() { XPos++; }

        public void Explode()
        {
            var random = new Random();
            var fx = new string[] {"▒", "▒", "▓", "╔", "╖", "═", "╘", "║" };
            var colors = new ConsoleColor[] { ConsoleColor.Gray, ConsoleColor.White, ConsoleColor.Cyan };
            for (int i = 0; i < 15; i++)
            {
                XPos = random.Next(XPos - 2, XPos + 2);
                YPos = random.Next(YPos - 2, YPos + 2);
                ForegroundColor = colors[random.Next(colors.Length)];
                SetCursorPosition(Math.Clamp(XPos, 0, WindowWidth - 1), Math.Clamp(YPos, 0, WindowHeight - 1));
                Write(fx[random.Next(fx.Length)]);
            }
            SetCursorPosition(0, 0);
        }
    }

    class Berry
    {
        public int XPos { get; set; }
        public int YPos { get; set; }

        public void Spawn()
        {
            var random = new Random();
            XPos = random.Next(1, WindowWidth - 1);
            YPos = random.Next(1, WindowHeight - 1);
            var berries = new string[] { "ó", "ò", "*", "@", "#", ")"};
            ForegroundColor = ConsoleColor.Magenta;
            SetCursorPosition(XPos, YPos);
            Write(berries[random.Next(berries.Length)]);
            SetCursorPosition(0, 0);
        }

        public void Clear()
        {
            SetCursorPosition(XPos, YPos);
            Write(" ");
            SetCursorPosition(0, 0);
        }

        public bool WasEaten(Pixel pixel)
        {
            return (this.XPos == pixel.XPos) && (this.YPos == pixel.YPos);
        }
    }

    static class Border
    {
        public static void Draw()
        {
            ForegroundColor = ConsoleColor.DarkRed;
            // Draw vertical borders
            for (int i = 0; i < WindowHeight; i++)
            {
                SetCursorPosition(0, i);
                Write("║");

                SetCursorPosition(WindowWidth - 1, i);
                Write("║");
            }
            // Draw horizontal borders
            for (int i = 0; i < WindowWidth; i++)
            {
                SetCursorPosition(i, 0);
                Write("═");

                SetCursorPosition(i, WindowHeight - 1);
                Write("═");
            }
            // Draw all 4 corners
            SetCursorPosition(0, 0);
            Write("╔");
            SetCursorPosition(WindowWidth - 1, 0);
            Write("╗");
            SetCursorPosition(0, WindowHeight - 1);
            Write("╚");
            SetCursorPosition(WindowWidth - 1, WindowHeight - 1);
            Write("╝");
        }
    }

    static class Score
    {
        public static int score = 0;

        public static void Increase()
        {
            score++;
        }

        public static void Draw()
        {
            ForegroundColor = ConsoleColor.Red;
            string scoreText = $"SCORE: {score}";
            SetCursorPosition(WindowWidth / 2 - scoreText.Length / 2, 0);
            Write(scoreText);
            SetCursorPosition(0, 0);
        }

        public static void Gameover()
        {
            ForegroundColor = ConsoleColor.Red;

            string gameoverText = "░░░░░▒▒▒▒▓▓GAME OVER▓▓▒▒▒▒░░░░";
            SetCursorPosition(WindowWidth / 2 - gameoverText.Length / 2, (WindowHeight / 2) - 1);
            Write(gameoverText);
            SetCursorPosition(0, 0);
        }
    }

    static class Intro
    {
        static string[] lines =
        {
            @"                   __     ",
            @"       _______    /*_>-<  ",
            @"   ___/ _____ \__/ /      ",
            @"  <____/     \____/       ",
            @"                          ",
            @"  Press any key to start  ",
        };

        public static void Draw()
        {
            ForegroundColor = ConsoleColor.Green;

            for (int i = 0; i < lines.Length; i++)
            {
                SetCursorPosition(WindowWidth / 2 - lines[i].Length / 2, WindowHeight / 2 - lines.Length / 2 + i);
                Write(lines[i]);
            }

            SetCursorPosition(0, 0);
        }

        public static void Clear()
        {
            for (int i = 0; i < lines.Length; i++)
            {
                SetCursorPosition(WindowWidth / 2 - lines[i].Length / 2 + 1, WindowHeight / 2 - lines.Length / 2 + i);
                Write(@"                                            ");
            }

            SetCursorPosition(0, 0);
        }
    }

    enum Direction
    {
        Up,
        Down,
        Right,
        Left
    }
}
