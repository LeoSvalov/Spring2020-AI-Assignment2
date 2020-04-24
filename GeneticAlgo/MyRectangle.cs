using System.Drawing;

namespace GeneticAlgo
{
    /* Structure using which the output image is building up*/
    public class MyRectangle
    {
        public Point Pivot; /*left-upper point of rectangle*/
        public readonly int Width;
        public readonly int Height;
        public Color Color;
        public MyRectangle(Point pivot, int width, int height, Color color)
        {
            this.Pivot = pivot;
            this.Width = width;
            this.Height = height;
            this.Color = color;
        }
    }
}