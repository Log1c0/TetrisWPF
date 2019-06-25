using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace Tetris
{
    public class Square
    {
        public SolidColorBrush Color { get; set; }
        public bool Filled { get; set; }
        public Square(SolidColorBrush color, bool filled)
        {
            this.Color = color;
            this.Filled = filled;
        }
    }

    public class Figure
    {
        public Point [] Cordinates { get; set; }
        public  SolidColorBrush Color { get; set; }
        public Figure(Point[] Cordinates, SolidColorBrush color)
        {
            this.Cordinates = Cordinates;
            this.Color = color;
        }

        public object Clone()
        {
            return new Figure(this.Cordinates, this.Color);
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private double SquareSize;
        private int LineSize;
        private int NextFigureIndex = 0;

        private bool stopFigure = true;

        private Figure CurrentFigure;

        private Square[,] _gameField = new Square[10, 20]; //x, y

        private static Figure[] _figureArray = new Figure[7]
        {

            new Figure(new Point[] {new Point(3, 0), new Point(4, 0), new Point(5, 0), new Point(6, 0)},
                (SolidColorBrush) (new BrushConverter().ConvertFrom("#F00000"))),

            new Figure(new Point[] {new Point(4, 0), new Point(5, 0), new Point(4, 1), new Point(5, 1)},
                (SolidColorBrush) (new BrushConverter().ConvertFrom("#0F0000"))),

            new Figure(new Point[] {new Point(3, 1), new Point(4, 1), new Point(5, 1), new Point(5, 0)},
                (SolidColorBrush) (new BrushConverter().ConvertFrom("#00F000"))),

            new Figure(new Point[] { new Point(3, 1), new Point(3, 0), new Point(4, 1), new Point(5, 1)},
                (SolidColorBrush) (new BrushConverter().ConvertFrom("#000F00"))),

            new Figure(new Point[] { new Point(4, 0), new Point(4, 1), new Point(5, 1), new Point(5, 2)},
                (SolidColorBrush) (new BrushConverter().ConvertFrom("#0000F0"))),

            new Figure(new Point[] { new Point(5, 0), new Point(5, 1), new Point(4, 1), new Point(4, 2)},
                (SolidColorBrush) (new BrushConverter().ConvertFrom("#00000F"))),

            new Figure(new Point[] { new Point(4, 0), new Point(3, 1), new Point(4, 1), new Point(5, 1)},
                (SolidColorBrush) (new BrushConverter().ConvertFrom("#FFFF00"))),
        };

        private static List<Rectangle> _allRect = new List<Rectangle>();
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int x = 0; x < _gameField.GetLength(0); x++)
                for (int y = 0; y < _gameField.GetLength(1); y++)
                    _gameField[x, y] = new Square((SolidColorBrush) (new BrushConverter().ConvertFrom("#EFEFEF")), false);

            SquareSize = Math.Floor(GameCanvas.Height / _gameField.GetLength(1));

            GameCanvas.Width = 10 * SquareSize;
            GameCanvas.Height = 20 * SquareSize;

            LineSize = 1;

            Line line;
            for (int column = 0; column < _gameField.GetLength(0) + 1; column++)
            {
                line = new Line
                {
                    Stroke = (SolidColorBrush) (new BrushConverter().ConvertFrom("#000000")),
                    X1 = column * SquareSize,
                    X2 = column * SquareSize,
                    Y1 = 0,                  // From start
                    Y2 = GameCanvas.Height, //  To end
                    StrokeThickness = LineSize
                };
                GameCanvas.Children.Add(line);
            }
            
            for (int row = 0; row < _gameField.GetLength(1) + 1; row++)
            {
                line = new Line
                {
                    Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000")), 
                    X1 = 0,                 // From start
                    X2 = GameCanvas.Width, //  To end
                    Y1 = row * SquareSize,
                    Y2 = row * SquareSize,
                    StrokeThickness = LineSize
                };
                GameCanvas.Children.Add(line);
            }

            FieldDrawing();
        }
        
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer gameTimer = new DispatcherTimer();
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            foreach (Rectangle rect in _allRect)
            {
                GameCanvas.Children.Remove(rect);
            }

            // Create new figure
            if (stopFigure)
            {
                stopFigure = false;
                CurrentFigure = (Figure)_figureArray[NextFigureIndex].Clone();

                // Place cords on field
                foreach (Point cord in CurrentFigure.Cordinates)
                {
                    _gameField[(int)(cord.X), (int)(cord.Y)].Filled = true;

                    _gameField[(int)(cord.X), (int)(cord.Y)].Color = CurrentFigure.Color;
                }
            }
            // Move figure
            else
            {
                bool let = false;
                for (int i = 0; i < CurrentFigure.Cordinates.Length; i++)
                {
                    if (CurrentFigure.Cordinates[i].Y + 1 == _gameField.GetLength(1))
                        let = true;
                    else if (_gameField[(int)CurrentFigure.Cordinates[i].X, (int)CurrentFigure.Cordinates[i].Y + 1].Filled)
                    {
                        let = true;
                        if (IsPartOfFigure(i))
                            let = false;
                    }

                    if(let)
                    {
                        stopFigure = true;

                        if (NextFigureIndex + 1 == _figureArray.Length)
                            NextFigureIndex = 0;
                        else
                            NextFigureIndex++;

                        break;
                    }
                }

                if (!stopFigure)
                {
                    for(int i = 0; i < CurrentFigure.Cordinates.Length; i++)
                    {
                        _gameField[(int)(CurrentFigure.Cordinates[i].X),
                            (int)(CurrentFigure.Cordinates[i].Y)].Filled = false;

                        _gameField[(int)(CurrentFigure.Cordinates[i].X),
                            (int)(CurrentFigure.Cordinates[i].Y)].Color = (SolidColorBrush)GameCanvas.Background;
                    }

                    for (int i = 0; i < CurrentFigure.Cordinates.Length; i++)
                    {
                        CurrentFigure.Cordinates[i].Y += 1;

                        _gameField[(int)(CurrentFigure.Cordinates[i].X),
                            (int)(CurrentFigure.Cordinates[i].Y)].Filled = true;

                        _gameField[(int)(CurrentFigure.Cordinates[i].X),
                            (int)(CurrentFigure.Cordinates[i].Y)].Color = CurrentFigure.Color;
                    }
                }
            }

            FieldDrawing();
        }

        private bool IsPartOfFigure(int currentIndex)
        {
            for (int figureCheckIndex = 0; figureCheckIndex < CurrentFigure.Cordinates.Length; figureCheckIndex++)
            {
                if (CurrentFigure.Cordinates[figureCheckIndex].Y == CurrentFigure.Cordinates[currentIndex].Y + 1 &&
                    CurrentFigure.Cordinates[figureCheckIndex].X == CurrentFigure.Cordinates[currentIndex].X     &&
                    currentIndex != figureCheckIndex)
                {
                    return true;
                }
            }
            return false;
        }

        private void FieldDrawing()
        {
            _allRect.Clear();
            Rectangle rect;
            for (int x = 0; x < _gameField.GetLength(0); x++)
            {
                for (int y = 0; y < _gameField.GetLength(1); y++)
                {
                    rect = new Rectangle
                    {
                        StrokeThickness = SquareSize - 2 // - 2 px from both sides
                    };

                    rect.Stroke = _gameField[x, y].Color;

                    Canvas.SetLeft(rect, x * SquareSize + LineSize);
                    Canvas.SetTop(rect,  y * SquareSize + LineSize);

                    _allRect.Add(rect);

                    GameCanvas.Children.Add(rect);
                }
            }
        }
    }
}
