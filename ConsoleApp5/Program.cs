﻿using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace ConsoleApp29
{
    class Program
    {
        static void Main(string[] args)
        {
            Player.cords = new Point(-2, 5);
            Player.POV = 0;
            Line_segment[] segments = new Line_segment[] { new Line_segment(new Point(1, 1), new Point(9, 1)),//1
                                                           new Line_segment(new Point(1, 3), new Point(1, 7)),//2
                                                           new Line_segment(new Point(1, 7), new Point(9, 7)),//3
                                                           new Line_segment(new Point(9, 7), new Point(9, 1)),//4
                                                           new Line_segment(new Point(2, 2), new Point(4, 2)),//5
                                                           new Line_segment(new Point(4, 1), new Point(4, 2)),//6
                                                           new Line_segment(new Point(3, 4), new Point(4, 3)),//7
                                                           new Line_segment(new Point(4, 3), new Point(5, 4)),//8
                                                           new Line_segment(new Point(3, 4), new Point(4, 5)),//9
                                                           new Line_segment(new Point(4, 5), new Point(5, 4)),//10
                                                           new Line_segment(new Point(7, 2), new Point(6, 5)),//11
                                                           new Line_segment(new Point(7, 5), new Point(8, 5)),//12
                                                           new Line_segment(new Point(8, 5), new Point(8, 6)),//13
                                                           new Line_segment(new Point(8, 6), new Point(7, 6)),//14
                                                           new Line_segment(new Point(7, 6), new Point(7, 5)) //15

            };
            Word.AddSegment(segments);
            Word.Bake();
            new Core().Start();
        }
    }
    class Core : RenderWindow
    {
        float[] distances;
        Point[] points;

        float Fovwid;
        double Rprrl;
        RenderWindow Window;

        //Windows Size
        private int Widthpic;
        private int Heightpic;
        private int Widthpic02;
        private int Heightpic02;
        private int Widthpic04;
        private int Heightpic04;

        //rend pl
        CircleShape pl;

        //texture
        Image texture = new Image("test.jpg");
        uint textureScale;
        Sprite[] sprites;
        public Core() : base(new VideoMode(1920, 1080), "lol", Styles.Close)
        {
            Window = this;
            Widthpic = (int)Window.Size.X;
            Heightpic = (int)Window.Size.Y;
            Widthpic02 = Widthpic / 2;
            Heightpic02 = Heightpic / 2;
            Widthpic04 = Widthpic / 4;
            Heightpic04 = Heightpic / 4;
            Fovwid = (float)Player.FOV / Widthpic;
            Rprrl = (Widthpic02 / 2) / Math.Tan(Math.PI * Player.FOV / 360);
            Window.Closed += Window_Closed;
            Window.KeyPressed += Window_KeyPressed;
            Window.MouseMoved += Window_MouseMoved;
            Window.LostFocus += Window_LostFocus;
            Window.GainedFocus += Window_GainedFocus;
            Window.SetMouseCursorVisible(false);
            pl = new CircleShape();
            pl.FillColor = Color.White;
            pl.Radius = 10;
            distances = new float[Widthpic];
            points = new Point[Widthpic];
            Window.SetVerticalSyncEnabled(true);
            textureScale = texture.Size.X;
            sprites = new Sprite[textureScale];
            for (uint i = 0; i < sprites.Length; i++)
            {
                Image spr = new Image(1, textureScale);
                for (uint j = 0; j < textureScale; j++)
                {
                    spr.SetPixel(1, j, texture.GetPixel(i, j));
                }
                sprites[i] = new Sprite(new Texture(spr));
            }
        }

        private void Window_GainedFocus(object? sender, EventArgs e)
        {
            Window.MouseMoved += Window_MouseMoved;
            Window.SetMouseCursorVisible(false);
        }
        private void Window_LostFocus(object? sender, EventArgs e)
        {
            Window.SetMouseCursorVisible(true);
            Window.MouseMoved -= Window_MouseMoved;
        }
        int first;
        private void Window_MouseMoved(object? sender, MouseMoveEventArgs e)
        {
            Player.POV -= (e.X - Widthpic02 + Window.Position.X + 8) * 0.1f;
            Mouse.SetPosition(new Vector2i(Widthpic02, Heightpic02));
        }

        private void Window_KeyPressed(object? sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.W:
                    Player.cords.X += (float)Math.Cos(Ray.CorrectAngle((int)Math.Round(Player.POV)) * Math.PI / 180) * 0.05f;
                    Player.cords.Y += (float)Math.Sin(Ray.CorrectAngle((int)Math.Round(Player.POV)) * Math.PI / 180) * 0.05f;
                    break;
                case Keyboard.Key.S:
                    Player.cords.X -= (float)Math.Cos(Ray.CorrectAngle((int)Math.Round(Player.POV)) * Math.PI / 180) * 0.05f;
                    Player.cords.Y -= (float)Math.Sin(Ray.CorrectAngle((int)Math.Round(Player.POV)) * Math.PI / 180) * 0.05f;
                    break;
                case Keyboard.Key.Escape:
                    Window.Close();
                    break;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Window.Close();
        }

        (float[] distances, Point[] points) Renddis(float pov, int fov, int NOR)
        {
            float angle = pov + fov / 2;
            float[] distances = new float[NOR];
            Point[] points = new Point[NOR];
            for (int i = 0; i < NOR; i++)
            {
                Ray ray = new Ray(angle);
                distances[i] = ray.LTP;
                points[i] = ray.Intersection_point;
                angle -= Fovwid;
            }
            return (distances, points);
        }
        public void Start()
        {
            while (true)
            {
                Window.DispatchEvents();
                Window.Clear();
                Window.SetTitle($"X: {Player.cords.X} Y: {Player.cords.Y}");
                var cort = Renddis(Player.POV, Player.FOV, Widthpic);
                distances = cort.distances; points = cort.points;
                pl.Position = new Vector2f(Player.cords.X * 50, Player.cords.Y * 50);
                for (int i = 0; i < Widthpic; i++)
                {

                    if (points[i] != null && distances[i] < 10000)
                    {
                        double H = 1 / (double)distances[i] * Rprrl;
                        if (H > Heightpic)
                        {
                            H = Heightpic;
                        }
                        int num = (int)(points[i].X * textureScale + points[i].Y * textureScale) % (int)textureScale;
                        sprites[num].Scale = new Vector2f(1, (float)H / Heightpic);
                        sprites[num].Position = new Vector2f(i, Heightpic02 - sprites[num].Scale.Y * Heightpic02);
                        Window.Draw(sprites[num]);
                        Window.Draw(new Vertex[] { new Vertex(new Vector2f(pl.Position.X + 10, pl.Position.Y + 10), new Color((byte)(points[i].X * 10),(byte)(points[i].Y * 10),0)),
                                               new Vertex(new Vector2f(points[i].X * 50+ 10, points[i].Y * 50+ 10), new Color((byte)(points[i].X * 10),(byte)(points[i].Y * 10),0)) }, PrimitiveType.Lines);
                    }
                }
                for (int i = 0; i < Word.Segments.Length; i++)
                {
                    Window.Draw(new Vertex[] { new Vertex(new Vector2f(Word.Segments[i].PointA.X * 50 + 10, Word.Segments[i].PointA.Y * 50 + 10), Color.White),
                                               new Vertex(new Vector2f(Word.Segments[i].PointB.X * 50 + 10, Word.Segments[i].PointB.Y * 50 + 10), Color.White) }, PrimitiveType.Lines);
                }
                Window.Draw(pl);
                Window.Draw(new Vertex[] { new Vertex(new Vector2f(pl.Position.X + 10, pl.Position.Y + 10), Color.White),
                                           new Vertex(new Vector2f((float)Math.Cos(Ray.CorrectAngle((int)Math.Round(Player.POV)) * Math.PI / 180) * 100 + pl.Position.X + 10, (float)Math.Sin(Ray.CorrectAngle((int)Math.Round(Player.POV)) * Math.PI / 180) * 100 +  pl.Position.Y + 10), Color.White) }, PrimitiveType.Lines);
                Window.Display();
            }
        }
    }
    static class Player
    {
        public const int FOV = 120;
        static public Point cords = new Point();
        static public float POV;
    }
    class Ray
    {
        float ang;
        public Point Intersection_point;
        const float beam_length = 50;
        public float LTP;
        float Angle { get { return ang; } set { ang = CorrectAngle(value); } }
        public static float CorrectAngle(float ang)
        {
            float limited_angle = ang % 360;
            if (limited_angle < 0)
                limited_angle += 360;
            return limited_angle;
        }
        public Ray(float ang)
        {
            Angle = ang;
            float[] LTPs = new float[Word.Segments.Length];
            Point[] intersection_point = new Point[Word.Segments.Length];
            for (int i = 0; i < Word.Segments.Length; i++)
            {
                Line_segment LineA = Word.Segments[i];
                Line_segment LineB = new Line_segment(Player.cords, new Point((float)Math.Cos(Angle * Math.PI / 180) * beam_length + Player.cords.X, (float)Math.Sin(Angle * Math.PI / 180) * beam_length + Player.cords.Y));
                float n;
                if (LineA.PointB.Y - LineA.PointA.Y != 0)
                {  // a(y)
                    float q = (LineA.PointB.X - LineA.PointA.X) / (LineA.PointA.Y - LineA.PointB.Y);
                    float sn = (LineB.PointA.X - LineB.PointB.X) + (LineB.PointA.Y - LineB.PointB.Y) * q;
                    float fn = (LineB.PointA.X - LineA.PointA.X) + (LineB.PointA.Y - LineA.PointA.Y) * q;   // b(x) + b(y)*q
                    n = fn / sn;
                }
                else
                    n = (LineB.PointA.Y - LineA.PointA.Y) / (LineB.PointA.Y - LineB.PointB.Y);   // c(y)/b(y)
                intersection_point[i] = new Point();
                intersection_point[i].X = LineB.PointA.X + (LineB.PointB.X - LineB.PointA.X) * n;  // x3 + (-b(x))*n
                intersection_point[i].Y = LineB.PointA.Y + (LineB.PointB.Y - LineB.PointA.Y) * n;  // y3 +(-b(y))*n
                Line_segment C = new Line_segment(Player.cords, new Point(intersection_point[i].X, intersection_point[i].Y));
                float x1 = intersection_point[i].X;
                float y1 = intersection_point[i].Y;
                float xa1 = LineA.PointA.X;
                float xa2 = LineA.PointB.X;
                float ya1 = LineA.PointA.Y;
                float ya2 = LineA.PointB.Y;
                float xb1 = LineB.PointA.X;
                float xb2 = LineB.PointB.X;
                float yb1 = LineB.PointA.Y;
                float yb2 = LineB.PointB.Y;

                bool condition = (IsWithinRange(x1, xa1, xa2) &&
                  IsWithinRange(y1, ya1, ya2) &&
                  IsWithinRange(x1, xb1, xb2) &&
                  IsWithinRange(y1, yb1, yb2));

                if (condition)
                    LTPs[i] = (float)Math.Sqrt(((C.PointB.X - C.PointA.X) * (C.PointB.X - C.PointA.X)) + ((C.PointB.Y - C.PointA.Y) * (C.PointB.Y - C.PointA.Y)));
                else
                    LTPs[i] = 10000;

            }
            int Bestid = 0;
            float best = 10000;
            for (int i = 0; i < LTPs.Length; i++)
            {
                if (LTPs[i] < best && LTPs[i] < beam_length)
                {
                    best = LTPs[i];
                    Bestid = i;
                }
            }
            LTP = best;
            Intersection_point = intersection_point[Bestid];
        }
        bool IsWithinRange(float value, float rangeStart, float rangeEnd)
        {
            float minValue = Math.Min(rangeStart, rangeEnd);
            float maxValue = Math.Max(rangeStart, rangeEnd);
            float product = (float)Math.Round((value - rangeStart) * (rangeEnd - value), 10);
            return (0 <= product) && (product <= (maxValue - minValue) * (maxValue - minValue));
        }
    }
    class Point
    {
        public float X, Y;
        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }
        public Point(float a)
        {
            X = a;
            Y = a;
        }

        public Point()
        {
        }
    }
    class Line_segment
    {
        public Point PointA;
        public Point PointB;
        public Line_segment(Point A, Point B)
        {
            PointA = A;
            this.PointB = B;
        }
    }
    static class Word
    {
        static List<Line_segment> list_segments = new List<Line_segment>();
        static public Line_segment[] Segments { get; private set; }
        static public void Bake()
        {
            Segments = list_segments.ToArray();
            list_segments.Clear();
            list_segments.AsReadOnly();
        }
        static public void AddSegment(Line_segment segment) => list_segments.Add(segment);
        static public void AddSegment(Line_segment[] segments)
        {
            foreach (var item in segments)
            {
                list_segments.Add(item);
            }
        }
    }
}