using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Tetris
{

    

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private double squareSize;
        private int lineSize;
        
        private int nextFigureIndex = 0;

        private bool makeNewFigure = true;

        private Figure CurrentFigure;

        private readonly DispatcherTimer gameTimer = new DispatcherTimer();

        private readonly Square[,] gameField = new Square[10, 20]; //x, y

        private static readonly Figure[] FigureArray = new Figure[7]
        {
            new Figure(new Point[] { new Point(3, 0), new Point(4, 0), new Point(5, 0), new Point(6, 0)},
                (SolidColorBrush) (new BrushConverter().ConvertFrom("#00F0F0"))),

            new Figure(new Point[] { new Point(4, 0), new Point(5, 0), new Point(4, 1), new Point(5, 1)},
                (SolidColorBrush) (new BrushConverter().ConvertFrom("#F0F000"))),

            new Figure(new Point[] { new Point(3, 1), new Point(4, 1), new Point(5, 1), new Point(5, 0)},
                (SolidColorBrush) (new BrushConverter().ConvertFrom("#F0A000"))),

            new Figure(new Point[] { new Point(3, 0), new Point(3, 1), new Point(4, 1), new Point(5, 1)},
                (SolidColorBrush) (new BrushConverter().ConvertFrom("#0000F0"))),

            new Figure(new Point[] { new Point(4, 1), new Point(5, 1), new Point(5, 0), new Point(6, 0)},
                (SolidColorBrush) (new BrushConverter().ConvertFrom("#00F000"))),

            new Figure(new Point[] { new Point(4, 0), new Point(5, 0), new Point(5, 1), new Point(6, 1)},
                (SolidColorBrush) (new BrushConverter().ConvertFrom("#F00000"))),

            new Figure(new Point[] { new Point(3, 0), new Point(4, 1),  new Point(4, 0), new Point(5, 0)},
                (SolidColorBrush) (new BrushConverter().ConvertFrom("#A000F0"))),
        };

        private static List<Rectangle> _allRect = new List<Rectangle>();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int x = 0; x < gameField.GetLength(0); x++)
                for (int y = 0; y < gameField.GetLength(1); y++)
                    gameField[x, y] = new Square((SolidColorBrush) (new BrushConverter().ConvertFrom("#EFEFEF")), false);

            squareSize = Math.Floor(GameCanvas.Height / gameField.GetLength(1));

            GameCanvas.Width = 10 * squareSize;
            GameCanvas.Height = 20 * squareSize;

            lineSize = 1;

            Line line;
            for (int column = 0; column < gameField.GetLength(0) + 1; column++)
            {
                line = new Line
                {
                    Stroke = (SolidColorBrush) (new BrushConverter().ConvertFrom("#000000")),
                    X1 = column * squareSize,
                    X2 = column * squareSize,
                    Y1 = 0,                  // From start
                    Y2 = GameCanvas.Height, //  To end
                    StrokeThickness = lineSize
                };
                GameCanvas.Children.Add(line);
            }
            
            for (int row = 0; row < gameField.GetLength(1) + 1; row++)
            {
                line = new Line
                {
                    Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000")), 
                    X1 = 0,                 // From start
                    X2 = GameCanvas.Width, //  To end
                    Y1 = row * squareSize,
                    Y2 = row * squareSize,
                    StrokeThickness = lineSize
                };
                GameCanvas.Children.Add(line);
            }

            RemoveRows();
            FieldDrawing();
            gameTimer.Tick += GameTimer_Tick;
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            gameTimer.Start();
        }
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            foreach (Rectangle rect in _allRect)
            {
                GameCanvas.Children.Remove(rect);
            }

            // Create new figure
            if (makeNewFigure)
            {
                RemoveRows();

                // if figure can't be place, GAME OVER
                if (FigureArray[nextFigureIndex].Cordinates.Any(cord => gameField[(int)cord.X, (int)cord.Y].Filled))
                {
                    FinishGame();
                    return;
                }

                makeNewFigure = false;
                CurrentFigure = FigureArray[nextFigureIndex].Clone();

                // Place cords on field
                foreach (Point cord in CurrentFigure.Cordinates)
                {
                    ChangeSquareStatus((int)cord.X, (int)(cord.Y), CurrentFigure.Color);
                }
            }
            // Move figure
            else
            {
                bool move = true;
                for (int i = 0; i < CurrentFigure.Cordinates.Length; i++)
                {
                    if (CurrentFigure.Cordinates[i].Y + 1 == gameField.GetLength(1))
                        move = false;
                    else if (gameField[(int)CurrentFigure.Cordinates[i].X, (int)CurrentFigure.Cordinates[i].Y + 1].Filled)
                        move = IsPartOfFigure('Y', 1, i);

                    if(!move)
                    {
                        makeNewFigure = true;

                        if (nextFigureIndex + 1 == FigureArray.Length)
                            nextFigureIndex = 0;
                        else
                            nextFigureIndex++;

                        break;
                    }
                }
                if (!makeNewFigure)
                    MoveFigure('Y',1);
            }
            FieldDrawing();
            CheckFirstRow();
        }
        private void ChangeSquareStatus(int x, int y, SolidColorBrush color)
        {
            gameField[x, y].Filled = (color != GameCanvas.Background);
            gameField[x, y].Color = color;
        }
        private void CheckFirstRow()
        {
            for(int x = 0; x < gameField.GetLength(0); x++)
            {
                if (gameField[x, 0].Filled)
                {
                    bool isCurrentSquare = false;
                    foreach (Point cord in CurrentFigure.Cordinates)
                    {
                        if (cord.X == x)
                            isCurrentSquare = true;
                    }

                    if (!isCurrentSquare)
                        FinishGame();
                }
            }
        }
        private void RemoveRows()
        {
            for (int checkRow = 0; checkRow < gameField.GetLength(1); checkRow++)
            {
                int filledSquare = 0;
                for (int checkCol = 0; checkCol < gameField.GetLength(0); checkCol++)
                {
                    if (gameField[checkCol, checkRow].Filled)
                        filledSquare += 1;
                }

                if (filledSquare < 10) continue;
                
                for (int x = 0; x < gameField.GetLength(0); x++)
                    ChangeSquareStatus(x, checkRow, (SolidColorBrush)GameCanvas.Background);


                for(int y = checkRow; y >= 0; y--)
                {
                    for (int x = 0; x < gameField.GetLength(0); x++)
                    {
                        if (gameField[x, y].Filled)
                        {
                            ChangeSquareStatus(x, y + 1, gameField[x, y].Color);
                            ChangeSquareStatus(x, y, (SolidColorBrush)GameCanvas.Background);
                        }
                    }
                }
            }
        }
        private void FieldDrawing()
        {
            Rectangle rect;
            _allRect.Clear();
            for (int x = 0; x < gameField.GetLength(0); x++)
            {
                for (int y = 0; y < gameField.GetLength(1); y++)
                {
                    rect = new Rectangle
                    {
                        StrokeThickness = squareSize - 2,
                        Stroke = gameField[x, y].Color // - 2 px from both sides
                    };
                    Canvas.SetLeft(rect, x * squareSize + lineSize);
                    Canvas.SetTop (rect, y * squareSize + lineSize);
                    _allRect.Add(rect);
                    GameCanvas.Children.Add(rect);
                }
            }
        }
        private bool IsPartOfFigure(char coordinate, int direction, int currentIndex)
        {
            for (int figureCheckIndex = 0; figureCheckIndex < CurrentFigure.Cordinates.Length; figureCheckIndex++)
            {
                if(coordinate == 'Y')
                {
                    if (CurrentFigure.Cordinates[figureCheckIndex].Y == CurrentFigure.Cordinates[currentIndex].Y + direction &&
                        CurrentFigure.Cordinates[figureCheckIndex].X == CurrentFigure.Cordinates[currentIndex].X &&
                        currentIndex != figureCheckIndex)
                    {
                        return true;
                    }
                }
                else
                {
                    if (CurrentFigure.Cordinates[figureCheckIndex].Y == CurrentFigure.Cordinates[currentIndex].Y &&
                        CurrentFigure.Cordinates[figureCheckIndex].X == CurrentFigure.Cordinates[currentIndex].X + direction && 
                        currentIndex != figureCheckIndex)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void MoveFigure(char coordinate, int direction)
        {
            foreach (Point cord in CurrentFigure.Cordinates)
            {
                ChangeSquareStatus((int)cord.X, (int)cord.Y, (SolidColorBrush)GameCanvas.Background);
            }

            for (int i = 0; i < CurrentFigure.Cordinates.Length; i++)
            {
                if(coordinate == 'Y')
                    CurrentFigure.Cordinates[i].Y += direction;
                else
                    CurrentFigure.Cordinates[i].X += direction;

                ChangeSquareStatus((int)CurrentFigure.Cordinates[i].X, (int)CurrentFigure.Cordinates[i].Y, CurrentFigure.Color);
            }
        }

        private void RotateFigure()
        {
            Point pCord = CurrentFigure.Cordinates[0];
            List<Point> newFigure = new List<Point>();
            foreach (Point cord in CurrentFigure.Cordinates)
            {
                Point newPoint = new Point(pCord.X + pCord.Y - cord.Y, cord.X + (pCord.Y - pCord.X));

                if (newPoint.X > 0 && newPoint.X < gameField.GetLength(0) && 
                    newPoint.Y > 0 && newPoint.Y < gameField.GetLength(1) && !gameField[(int)newPoint.X, (int)newPoint.Y].Filled)
                    newFigure.Add(newPoint);
            }
            if(newFigure.Count == 3)
            {
                for (int index = 1; index < CurrentFigure.Cordinates.Length; index++)
                {
                    ChangeSquareStatus((int) CurrentFigure.Cordinates[index].X, (int) CurrentFigure.Cordinates[index].Y, (SolidColorBrush) GameCanvas.Background);

                    CurrentFigure.Cordinates[index] = newFigure[index - 1];

                    ChangeSquareStatus((int) CurrentFigure.Cordinates[index].X, (int) CurrentFigure.Cordinates[index].Y, CurrentFigure.Color);
                }
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(!makeNewFigure)
            {
                int direction = 0;

                if(e.Key == Key.Right || e.Key == Key.D)
                    direction = 1;
                else if(e.Key == Key.Left || e.Key == Key.A)
                    direction = -1;
                else if(e.Key == Key.Down || e.Key == Key.S)
                    direction = 2;
                else if (e.Key == Key.R)
                {
                    RotateFigure();
                    return;
                }

                if (direction == 2)
                {
                    gameTimer.Interval = new TimeSpan(0,0,0,0, 10);
                    return;
                }
                bool move = true;
                for (int index = 0; index < CurrentFigure.Cordinates.Length; index++)
                {
                    if ((direction == 1 && CurrentFigure.Cordinates[index].X + 1 == gameField.GetLength(0)) ||
                        (direction == -1 && CurrentFigure.Cordinates[index].X == 0))
                        move = false;
                    else if (gameField[(int)CurrentFigure.Cordinates[index].X + direction, (int)CurrentFigure.Cordinates[index].Y].Filled)
                    {
                        if (!IsPartOfFigure('X', direction, index))
                            move = false;
                    }

                    if (!move)
                        break;
                }
                if (move)
                    MoveFigure('X', direction);
            }
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S || e.Key == Key.Down)
                gameTimer.Interval = new TimeSpan(0,0,0,0, 100);
        }
        private void FinishGame()
        {
            MessageBox.Show("GAME OVER!");
            gameTimer.Stop();
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            for (int x = 0; x < gameField.GetLength(0); x++)
            {
                for (int y = 0; y < gameField.GetLength(1); y++)
                {
                    gameField[x, y].Filled = false;
                    gameField[x, y].Color = (SolidColorBrush) GameCanvas.Background;
                }
            }

            foreach (Rectangle rect in _allRect)
            {
                GameCanvas.Children.Remove(rect);
            }
            _allRect.Clear();
        }
    }
}