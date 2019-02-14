using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnalysisScreenSaver
{
    class RandomStringLabel : Label
    {
        public int stringLength = 0;
        public int xSpeed = 0;
        public int ySpeed = 0;
        public double scaleSpeed = 0.1;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        public void basePaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
    }
}
