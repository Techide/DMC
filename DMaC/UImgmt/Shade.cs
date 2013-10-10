using System;
using System.Windows.Media;

namespace DMaC.UImgmt
{
    public class Shade
    {
        public static Color Bright(Color color, int shade = 50, int alpha = 255)
        {
            var red = (color.R + shade) <= 255 ? (color.R + shade) : 255;
            var green = (color.G + shade) <= 255 ? (color.G + shade) : 255;
            var blue = (color.B + shade) <= 255 ? (color.B + shade) : 255;

            return  Color.FromArgb((Byte)alpha, (Byte)red, (Byte)green, (Byte)blue);
        }

        public static Color Dark(Color color, int shade = 50, Byte alpha = 255)
        {
            var red = (color.R - shade) >= 0 ? (color.R - shade) : 0;
            var green = (color.G - shade) >= 0 ? (color.G - shade) : 0;
            var blue = (color.B - shade) >= 0 ? (color.B - shade) : 0;

            return Color.FromArgb(alpha, (Byte)red, (Byte)green, (Byte)blue);
        }

    }
}
