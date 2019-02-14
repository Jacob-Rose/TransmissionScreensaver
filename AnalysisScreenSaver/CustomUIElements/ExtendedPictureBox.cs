using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnalysisScreenSaver.CustomUIElements
{
    class ExtendedPictureBox : PictureBox
    {
        public string text;
        public bool basePaint = true;
        public float rotation = 0;
        public Image customImage;
        public double posX = 100;
        public double posY;

        public ExtendedPictureBox()
        {
            
        }

        protected override void OnPaint(PaintEventArgs e)
        {         
            if (basePaint)
                base.OnPaint(e);
            e.Graphics.ResetTransform();
        }
    }
}
