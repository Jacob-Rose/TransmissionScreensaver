using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisScreenSaver
{
    public static class StaticMethods
    {
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_*/$%&@";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static double getBellCurveValue(double maxValue, double value)
        {
            double v = -1.08+ ((maxValue * (value * 2)) / (maxValue * maxValue));
            return 1- Math.Pow((1 / (20 * Math.Sqrt(2 * Math.PI))), Math.Exp(-((v * v) / (0.28))));
        }


    }
}
