using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnalysisScreenSaver.CustomUIElements
{
    class DistortedCubes : PictureBox
    {
        float frequency = 0.2f;
        int maxSquareSize = 200;
        int minSquareSize = 10;
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Random ran = new Random();
            int squares = (int)(ran.NextDouble() * (500 * frequency));

            for (int i = 0; i < squares; i++)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb((int)(ran.NextDouble() * 100), (int)(ran.NextDouble() * 100), (int)(ran.NextDouble() * 50), (int)(ran.NextDouble() * 100) + 154)),new Rectangle((int)(ran.NextDouble() * Bounds.Width), (int)(ran.NextDouble() * Bounds.Height), (int)(ran.NextDouble() * maxSquareSize), (int)(ran.NextDouble() * maxSquareSize)));
            }
        }
    }
}
