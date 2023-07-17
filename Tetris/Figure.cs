using System.Windows.Media;
using System.Windows;

namespace Tetris
{
    public class Figure
    {
        public Point[] Cordinates { get; set; }
        public SolidColorBrush Color { get; set; }
        public Figure(Point[] Cordinates, SolidColorBrush color)
        {
            this.Cordinates = Cordinates;
            this.Color = color;
        }

        public Figure Clone()
        {
            Figure clone = (Figure)this.MemberwiseClone();
            clone.Cordinates = new Point[this.Cordinates.Length];
            for (int i = 0; i < this.Cordinates.Length; i++)
            {
                clone.Cordinates[i] = new Point(this.Cordinates[i].X, this.Cordinates[i].Y);
            }
            return clone;
        }
    }
}