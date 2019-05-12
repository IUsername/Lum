using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lum
{
    public static class Tools
    {
        public static double Lerp(double v0, double v1, double t) => (1 - t) * v0 + t * v1;

        public static double Clamp(double min, double max, double v) => Math.Min(Math.Max(min, v), max);

        public static double UnitValue(double min, double max, double v)
        {
            return (Clamp(min, max, v) - min) / (max - min);
        }
    }
}
