using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnalysisScreenSaver.CustomUIElements
{
    class FlyByLetter : PictureBox
    {
        float direction = 0;
        float speed = 0;
        Point rotations = new Point(1,1);

        protected override void OnPaint(PaintEventArgs e)
        {
            float scaleX = 1 / rotations.X;
            float scaleY = 1 / rotations.Y;

            Graphics g = e.Graphics;
            g.ScaleTransform(scaleX, scaleY);
            g.TranslateTransform(Bounds.Width / 2 - scaleX / 2, Bounds.Height / 2 - scaleY / 2);
            base.OnPaint(e);
            e.Graphics.ResetTransform();
        }

        public void MoveInDirBySpd()
        {
            
        }
    }
}
