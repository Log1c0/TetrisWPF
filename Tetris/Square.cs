using System.Windows.Media;

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
}