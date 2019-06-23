using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Tetris
{
    class Figure
    {

    }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private const double CubeSize = 20.0;

        private readonly int[,] gameField = new int[10, 20] //x, y
        {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GameGrid.Width = (MainGrid.Height - 31) / 2;

            Line l;
            for (int column = 0; column < gameField.GetLength(0) + 1; column++)
            {
                l = new Line
                {
                    Stroke = (SolidColorBrush) (new BrushConverter().ConvertFrom("#000000")),
                    X1 = column * GameGrid.Height / CubeSize,
                    X2 = column * GameGrid.Height / CubeSize,
                    Y1 = 0,
                    Y2 = GameGrid.Height,
                    StrokeThickness = 1,
                };
                GameGrid.Children.Add(l);
            }

            for (int row = 0; row < gameField.GetLength(1) + 1; row++)
            {
                l = new Line
                {
                    Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000")), 
                    X1 = 0,
                    X2 = GameGrid.Width,
                    Y1 = row * GameGrid.Height / CubeSize,
                    Y2 = row * GameGrid.Height / CubeSize,
                    StrokeThickness = 1
                };
                GameGrid.Children.Add(l);
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer gameTimer = new DispatcherTimer();
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Interval = new TimeSpan(0, 0, 1);
            gameTimer.Start();
        }

        private static void GameTimer_Tick(object sender, EventArgs e)
        {

        }
    }
}
