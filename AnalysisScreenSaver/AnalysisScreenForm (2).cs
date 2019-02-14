using AnalysisScreenSaver.CustomUIElements;
using AnalysisScreenSaver.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnalysisScreenSaver
{
    public partial class AnalysisScreenForm : Form
    {
        private Point mouseLocation;
        private Random random;
        private Graphics g;

        #region Timers
        public Timer smoothTimer = new Timer();
        public const int smoothTimerInterval = 20; //in miliseconds

        public Timer randomTextTimer = new Timer();
        public const int randomTextTimerInterval = 100; //in miliseconds

        public Timer screenEventTimer = new Timer();
        public const int screenEventTimeMin = 750;
        public const int screenEventTimeMax = 1500;

        public Timer screenRotateTimer = new Timer();
        #endregion

        #region Screen Picture Boxes
        private PictureBox currentScreenPictureBox = new PictureBox();
        private PictureBox newScreenPictureBox;
        public Func<List<Control>>  [] screens;

        public int currentScreenIndex;
        public int newScreenIndex;
        #endregion

        #region Background Color List
        public Color[] bgColors = new Color[] {
            ColorTranslator.FromHtml("#FF3F3F"),
            ColorTranslator.FromHtml("#FF8D3F"),
            ColorTranslator.FromHtml("#FFCB3F"),
            ColorTranslator.FromHtml("#BCFF3F"),
            ColorTranslator.FromHtml("#59FF3F"),
            ColorTranslator.FromHtml("#3FFF85"),
            ColorTranslator.FromHtml("#3FFFDF"),
            ColorTranslator.FromHtml("#3FC2FF"),
            ColorTranslator.FromHtml("#4A3FFF"),
            ColorTranslator.FromHtml("#F33FFF"),
            ColorTranslator.FromHtml("#FF3FAA"),
            ColorTranslator.FromHtml("#FF3F76")
        };
        #endregion

        public Action[] transitionTypes;
        public int currentTransitionType; //-1: transition ended
        public const int transitionSpeed = 1900 / smoothTimerInterval;

        PrivateFontCollection pfc;
        public bool previewMode = false;

        public int monitorNum;
        public bool isPrimaryMonitor;

        public bool isAdmin = true; //used to check for added usage, fonts and such

        public AnalysisScreenForm(Rectangle bounds , int screen, bool primary)
        {
            InitializeComponent();
            mouseLocation = MousePosition; 
            Bounds = bounds;
            monitorNum = screen;
            isPrimaryMonitor = primary;

            #region Active Transitions
            transitionTypes = new Action[]
            {
                ()=>screenSwitch(1), //scroll right
                ()=>screenSwitch(2), //scroll left
                ()=>screenSwitch(3), //scroll up
                ()=>screenSwitch(4), //scroll down
            };
            #endregion
            #region Active Screens
            screens = new Func<List<Control>>[] {
            ()=>getScreen(0),
            ()=>getScreen(1),
            ()=>getScreen(2),
            ()=>getScreen(3),
            ()=>getScreen(4),
            ()=>getScreen(5),
            };
            #endregion

            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            random = new Random();

            currentScreenIndex = (int)(random.NextDouble() * screens.Length);

            g = CreateGraphics();

            #region Timers
            smoothTimer.Tick += smoothTimer_Tick;
            smoothTimer.Interval = smoothTimerInterval;
            smoothTimer.Start();

            randomTextTimer.Tick += RandomTextTimer_Tick; ;
            randomTextTimer.Interval = randomTextTimerInterval;
            randomTextTimer.Start();

            int screenEventTime = (int)(screenEventTimeMin + (random.NextDouble() * (screenEventTimeMax - screenEventTimeMin)));
            screenEventTimer.Tick += screenEventTimer_Tick;
            screenEventTimer.Interval = screenEventTime;
            screenEventTimer.Start();

            int rotateTimeMin = 7500;
            int rotateTimeMax = 10000;
            int screenRotateTime = (int)(rotateTimeMin + (random.NextDouble() * (rotateTimeMax - rotateTimeMin)));
            screenRotateTimer.Tick += screenRotateTimer_Tick;
            screenRotateTimer.Interval = screenRotateTime;
            screenRotateTimer.Start();
            #endregion

            #region Fonts
            pfc = new PrivateFontCollection();
            foreach (byte[] fbs in new byte[][] 
            {
              Resources.Amputa_Bangiz,
              Resources.AunchantedXspace, 
              Resources.Digitall,
              Resources.isocp2_IV25,
              Resources.radio1875_Bold, 
            })

            {
                

                IntPtr fontPtr = Marshal.AllocCoTaskMem(fbs.Length);

                Marshal.Copy(fbs, 0, fontPtr, fbs.Length);

                pfc.AddMemoryFont(fontPtr, fbs.Length);
                

                Marshal.FreeCoTaskMem(fontPtr);
            }
            #endregion
        }

        #region DLL imports
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);
        #endregion
        public AnalysisScreenForm(IntPtr PreviewWndHandle)
        {
            return;
        }
        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                    .IsInRole(WindowsBuiltInRole.Administrator);
        }
        private const List<Control> getScreen(int screenIndex)
        {
            switch (screenIndex)
            {
                #region Screen 0: HexApeature
                case 0:
                    int rotateTimeMax = 10000;
                    int rotateTimeMin = 7500;
                    screenRotateTimer.Interval = (int)(rotateTimeMin + (random.NextDouble() * (rotateTimeMax - rotateTimeMin)));
                    newScreenIndex = 0;
                    Font font;
                    if (isAdmin)
                    {
                        font = new Font(pfc.Families[3], Bounds.Width / 60, FontStyle.Regular);
                    }
                    else
                    {
                        font = new Font("Century Gothic", Bounds.Width / 60, FontStyle.Italic);
                    }
                    
                    int strLength = 18;
                    string textMeasure = StaticMethods.RandomString(20);
                    List<Control> scr = new List<Control>() //screen 0
                {
                    new ExtendedPictureBox()
                    {
                        Name = "hexApeture",
                        Bounds = new Rectangle((int)(Bounds.Width * 0.5) - (int)(Bounds.Height * 0.2), (int)(Bounds.Height * 0.25), (int)(Bounds.Height * 0.5),(int)(Bounds.Height * 0.5)),
                        customImage = new Bitmap(Resources.hexApetureUncropped,new Size((int)(Bounds.Height * 0.4),(int)(Bounds.Height * 0.4))),
                        rotation = 45
                    },
                    new RandomStringLabel() { //Control 0: Label
                        stringLength = strLength,
                        Text = StaticMethods.RandomString(strLength),
                        Font = font,
                        AutoSize = false,
                        Width = TextRenderer.MeasureText(textMeasure,font).Width,
                        Height = TextRenderer.MeasureText(textMeasure,font).Height,
                        TextAlign = ContentAlignment.MiddleRight,
                        ForeColor = Color.White,
                        Location = new Point((int)(Bounds.Width * 0.5) - (int)(Bounds.Height * 0.28)  - (int)(TextRenderer.MeasureText(textMeasure,font).Width * 1.1) , (int)(Bounds.Height * 0.25)),
                        BackColor = Color.Transparent
                    },
                    new RandomStringLabel() { //Control 1: Label
                        stringLength = strLength,
                        Text = StaticMethods.RandomString(strLength),
                        Font = font,
                        AutoSize = false,
                        Width = TextRenderer.MeasureText(textMeasure,font).Width,
                        Height = TextRenderer.MeasureText(textMeasure,font).Height,
                        TextAlign = ContentAlignment.MiddleLeft,
                        ForeColor = Color.White,
                        Location = new Point((int)(Bounds.Width * 0.5) + (int)(Bounds.Height * 0.28) + (int)(TextRenderer.MeasureText(textMeasure,font).Width * 0.1), (int)(Bounds.Height * 0.25)),
                        BackColor = Color.Transparent
                    },
                    new RandomStringLabel() { //Control 0: Label
                        stringLength = strLength,
                        Text = StaticMethods.RandomString(strLength),
                        Font = font,
                        AutoSize = false,
                        Width = TextRenderer.MeasureText(textMeasure,font).Width,
                        Height = TextRenderer.MeasureText(textMeasure,font).Height,
                        TextAlign = ContentAlignment.MiddleRight,
                        ForeColor = Color.White,
                        Location = new Point((int)(Bounds.Width * 0.5) - (int)(Bounds.Height * 0.28) -(int)(TextRenderer.MeasureText(textMeasure,font).Width * 1.2) , (int)(Bounds.Height * 0.35)),
                        BackColor = Color.Transparent
                    },
                    new RandomStringLabel() { //Control 0: Label
                        stringLength = strLength,
                        Text = StaticMethods.RandomString(strLength),
                        Font = font,
                        AutoSize = false,
                        Width = TextRenderer.MeasureText(textMeasure,font).Width,
                        Height = TextRenderer.MeasureText(textMeasure,font).Height,
                        TextAlign = ContentAlignment.MiddleLeft,
                        ForeColor = Color.White,
                        Location = new Point((int)(Bounds.Width * 0.5) + (int)(Bounds.Height * 0.28) + (int)(TextRenderer.MeasureText(textMeasure,font).Width * 0.2) , (int)(Bounds.Height * 0.35)),
                        BackColor = Color.Transparent
                    },
                    new RandomStringLabel() { //Control 0: Label
                        stringLength = strLength,
                        Text = StaticMethods.RandomString(strLength),
                        Font = font,
                        AutoSize = false,
                        Width = TextRenderer.MeasureText(textMeasure,font).Width,
                        Height = TextRenderer.MeasureText(textMeasure,font).Height,
                        TextAlign = ContentAlignment.MiddleRight,
                        ForeColor = Color.White,
                        Location = new Point((int)(Bounds.Width * 0.5) - (int)(Bounds.Height * 0.28) - (int)(TextRenderer.MeasureText(textMeasure,font).Width * 1.2) , (int)(Bounds.Height * 0.45)),
                        BackColor = Color.Transparent
                    },
                    new RandomStringLabel() { //Control 0: Label
                        stringLength = strLength,
                        Text = StaticMethods.RandomString(strLength),
                        Font = font,
                        AutoSize = false,
                        Width = TextRenderer.MeasureText(textMeasure,font).Width,
                        Height = TextRenderer.MeasureText(textMeasure,font).Height,
                        TextAlign = ContentAlignment.MiddleLeft,
                        ForeColor = Color.White,
                        Location = new Point((int)(Bounds.Width * 0.5) + (int)(Bounds.Height * 0.28) + (int)(TextRenderer.MeasureText(textMeasure,font).Width * 0.2), (int)(Bounds.Height * 0.45)),
                        BackColor = Color.Transparent
                    },
                    new RandomStringLabel() { //Control 0: Label
                        stringLength = strLength,
                        Text = StaticMethods.RandomString(strLength),
                        Font = font,
                        AutoSize = false,
                        Width = TextRenderer.MeasureText(textMeasure,font).Width,
                        Height = TextRenderer.MeasureText(textMeasure,font).Height,
                        TextAlign = ContentAlignment.MiddleRight,
                        ForeColor = Color.White,
                        Location = new Point((int)(Bounds.Width * 0.5) - (int)(Bounds.Height * 0.28) - (int)(TextRenderer.MeasureText(textMeasure,font).Width * 1.1) , (int)(Bounds.Height * 0.55)),
                        BackColor = Color.Transparent
                    },
                    new RandomStringLabel() { //Control 0: Label
                        stringLength = strLength,
                        Text = StaticMethods.RandomString(strLength),
                        Font = font,
                        AutoSize = false,
                        Width = TextRenderer.MeasureText(textMeasure,font).Width,
                        Height = TextRenderer.MeasureText(textMeasure,font).Height,
                        TextAlign = ContentAlignment.MiddleLeft,
                        ForeColor = Color.White,
                        Location = new Point((int)(Bounds.Width * 0.5) + (int)(Bounds.Height * 0.28) + (int)(TextRenderer.MeasureText(textMeasure,font).Width * 0.1), (int)(Bounds.Height * 0.55)),
                        BackColor = Color.Transparent
                    }
                };
                    
                    scr[0].Paint += ((sender, e) => {
                        float moveX = ((ExtendedPictureBox)sender).customImage.Width / 2f;
                        float moveY = ((ExtendedPictureBox)sender).customImage.Height / 2f;
                        e.Graphics.TranslateTransform(moveX, moveY);
                        e.Graphics.RotateTransform(((ExtendedPictureBox)sender).rotation);
                        e.Graphics.TranslateTransform(-moveX, -moveY);
                        e.Graphics.DrawImage(((ExtendedPictureBox)sender).customImage, 0, 0);
                    });
                    scr[0].SendToBack();
                    return scr;
                #endregion
                #region Screen 1: Terminal
                case 1:
                    rotateTimeMax = 10000;
                    rotateTimeMin = 7500;
                    screenRotateTimer.Interval = (int)(rotateTimeMin + (random.NextDouble() * (rotateTimeMax - rotateTimeMin)));
                    newScreenIndex = 1;
                    if (isAdmin)
                    {
                        font = new Font(pfc.Families[3], Bounds.Width / 50, FontStyle.Regular);
                    }
                    else
                    {
                        font = new Font("Century Gothic", Bounds.Width / 50, FontStyle.Italic);
                    }
                    scr = new List<Control> //screen 1
                    {
                    new RandomStringLabel() { //Control 1: Label
                        stringLength = 20,
                        Name="RandomString",
                        Text = StaticMethods.RandomString(20),
                        Font = font,
                        AutoSize = true,
                        ForeColor = Color.White,
                        Location = new Point(Bounds.Width / 8, Bounds.Height / 12),
                        TextAlign = ContentAlignment.MiddleCenter
                    },
                    new Label() { //Control 1: Label
                        Name= "Text Cursor",
                        Text = ">>: ",
                        Font = new Font(new FontFamily("Arial"),Bounds.Width/60,FontStyle.Regular),
                        AutoSize = true,
                        ForeColor = Color.White,
                        Location = new Point(Bounds.Width / 12, (int)(Bounds.Height / 10.3)),
                        TextAlign = ContentAlignment.MiddleCenter
                    },
                    new PictureBox()
                    {
                        Name="Border",
                        Bounds = new Rectangle (Bounds.Width / 14, Bounds.Height/14, (Bounds.Width / 14)*12,(Bounds.Height/14)*12)
                    }
                };
                scr[2].Paint += new PaintEventHandler((sender, e) =>
                {
                    Graphics g = e.Graphics;
                    int thickness = 5;

                    g.DrawRectangle(new Pen(Color.White, thickness), new Rectangle(thickness, thickness, (((PictureBox)sender).Bounds.Width - (thickness * 2)), (((PictureBox)sender).Bounds.Height - (thickness * 2))));
                });
                return scr;
                #endregion
                #region Screen 2: RandomArray
                case 2:

                    rotateTimeMax = 10000;
                    rotateTimeMin = 7500;
                    screenRotateTimer.Interval = (int)(rotateTimeMin + (random.NextDouble() * (rotateTimeMax - rotateTimeMin)));
                    FontFamily fontFam;
                    FontStyle fontSty;
                    if (isAdmin)
                    {
                        fontFam = pfc.Families[3];
                        fontSty = FontStyle.Regular;
                    }
                    else
                    {
                        fontFam = new FontFamily("Century Gothic");
                        fontSty = FontStyle.Italic;
                    }
                    newScreenIndex = 2;
                    scr = new List<Control> //screen 2
                {
                    new RandomStringLabel() { //Control 0: Label
                        stringLength = 30,
                        Text = StaticMethods.RandomString(30),
                        Font = new Font(fontFam,Bounds.Width/60,fontSty),
                        xSpeed = 1,
                        AutoSize = true,
                        ForeColor = Color.White,
                        Location = new Point((int)(Bounds.Width * 0.2),(int)(Bounds.Height * 0.1))
                    },
                    new RandomStringLabel() { //Control 1: Label
                        stringLength = 30,
                        Text = StaticMethods.RandomString(30),
                        Font = new Font(fontFam,Bounds.Width/30,fontSty),
                        AutoSize = true,
                        ForeColor = Color.White,
                        Location = new Point((int)(Bounds.Width * 0.8),(int)(Bounds.Height * 0.1))
                    },
                    new RandomStringLabel() { //Control 2: Label
                        stringLength = 30,
                        Text = StaticMethods.RandomString(30),
                        Font = new Font(fontFam,Bounds.Width/20,fontSty),
                        AutoSize = true,
                        ForeColor = Color.White,
                        xSpeed = -1,
                        Location = new Point((int)(Bounds.Width * 0.2),(int)(Bounds.Height * 0.5))
                    },
                    new RandomStringLabel() { //Control 3: Label
                        stringLength = 30,
                        Text = StaticMethods.RandomString(30),
                        Font = new Font(fontFam,Bounds.Width/30,fontSty),
                        AutoSize = true,
                        ForeColor = Color.White,
                        Location = new Point((int)(Bounds.Width * -0.1),(int)(Bounds.Height * 0.3))
                    },
                    new RandomStringLabel() { //Control 4: Label
                        stringLength = 30,
                        Text = StaticMethods.RandomString(30),
                        Font = new Font(fontFam,Bounds.Width/40,fontSty),
                        AutoSize = true,
                        ForeColor = Color.White,
                        Location = new Point((int)(Bounds.Width * 0.3),(int)(Bounds.Height * 0.9))
                    },
                    new RandomStringLabel() { //Control 5: Label
                        stringLength = 30,
                        Text = StaticMethods.RandomString(30),
                        Font = new Font(fontFam,Bounds.Width/100,fontSty),
                        AutoSize = true,
                        ForeColor = Color.White,
                        Location = new Point((int)(Bounds.Width * 0.1),(int)(Bounds.Height * 0.8))
                    }
                };
                    return scr;
                #endregion
                #region Screen 3: Clock
                case 3:
                    rotateTimeMax = 10000;
                    rotateTimeMin = 7500;
                    screenRotateTimer.Interval = (int)(rotateTimeMin + (random.NextDouble() * (rotateTimeMax - rotateTimeMin)));
                    newScreenIndex = 3;

                    scr = new List<Control>() //screen 3
                    {
                        new ExtendedPictureBox()
                        {
                            Name="Clock",
                            Bounds = new Rectangle(0, Bounds.Height / 3, Bounds.Width, Bounds.Height / 3),
                            BackColor = Color.Transparent,
                            text = DateTime.Now.ToString("hh:mm:ss"),
                        },
                    };
                    scr[0].Paint += new PaintEventHandler((sender, e) =>
                    {
                        Graphics g = e.Graphics;

                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
                        if (isAdmin)
                        {
                            font = new Font(pfc.Families[2], (int)(Bounds.Height * 0.3));
                        }
                        else
                        {
                            font = new Font("Century Gothic", (int)(Bounds.Height * 0.3));
                        }
                        
                        string text = ((ExtendedPictureBox)sender).text;

                        SizeF textSize = e.Graphics.MeasureString( text.Substring(0, 7) + "0", font);
                        PointF locationToDraw = new PointF();
                        locationToDraw.X = (((ExtendedPictureBox)sender).Bounds.Width / 2) - (textSize.Width / 2);
                        locationToDraw.Y = (((ExtendedPictureBox)sender).Bounds.Height / 2) - (textSize.Height / 2) + 20;

                        e.Graphics.DrawString(text, font, Brushes.White, locationToDraw.X,locationToDraw.Y);

                    });
                    rotateTimeMax = 10000;
                    rotateTimeMin = 7500;
                    screenRotateTimer.Interval = (int)(rotateTimeMin + (random.NextDouble() * (rotateTimeMax - rotateTimeMin)));
                    return scr;
                #endregion
                #region Screen 4: Title
                case 4:
                    rotateTimeMax = 10000;
                    rotateTimeMin = 7500;
                    screenRotateTimer.Interval = (int)(rotateTimeMin + (random.NextDouble() * (rotateTimeMax - rotateTimeMin)));
                    newScreenIndex = 4;
                    Font titleFont;
                    Font nameFont;
                    if (isAdmin)
                    {
                        titleFont = new Font(pfc.Families[1], Bounds.Height / 12);
                        nameFont = new Font(pfc.Families[4], (float)(Bounds.Height * 0.025), FontStyle.Bold);

                    }
                    else
                    {
                        titleFont = new Font(new FontFamily("Bauhaus 93"), Bounds.Height / 10);
                        nameFont = new Font(new FontFamily("Century Gothic"), (float)(Bounds.Height * 0.035), FontStyle.Italic);
                    }
                    scr = new List<Control>()
                    {
                        new Label()
                        {
                            Name = "Title",
                            Text = "Transmission",
                            Font = titleFont,
                            AutoSize = true,
                            ForeColor = Color.White,
                            Location = new Point(200, (int)(Bounds.Height * 0.4))
                        },
                        new Label()
                        {
                            Name="Author",
                            Text = "By: Jake Rose",
                            Font = nameFont,
                            AutoSize = true,
                            ForeColor = Color.White,
                            Location = new Point(200, (int)(Bounds.Height * 0.55))
                        }
                    };
                    return scr;
                #endregion
                #region Screen 5: Map System
                case 5:
                    rotateTimeMax = 15000;
                    rotateTimeMin = 10000;
                    screenRotateTimer.Interval = (int)(rotateTimeMin + (random.NextDouble() * (rotateTimeMax - rotateTimeMin)));
                    newScreenIndex = 5;
                    scr = new List<Control>()
                    {
                        new ExtendedPictureBox()
                        {
                            Name = "Map",
                            Bounds = new Rectangle(0,0,Bounds.Width, Bounds.Height),
                            Image = new Bitmap(Resources.maptransparent, Bounds.Size),
                        },
                        

                    };
                    scr[0].Controls.Add(new ExtendedPictureBox()
                    {
                        Name = "radar",
                        Bounds = new Rectangle(Bounds.Location, new Size(4, Bounds.Height)),
                    });
                    scr[0].Controls[0].Paint += ((sender, e) => {
                        ExtendedPictureBox picBox = ((ExtendedPictureBox)sender);
                        
                        Graphics g = e.Graphics;
                        g.DrawLine(new Pen(Color.White, 5), new Point((int)picBox.Bounds.Width - 2, 0), new Point((int)picBox.Bounds.Width - 2, picBox.Bounds.Height));
                        
                    });
                    rotateTimeMax = 15000;
                    rotateTimeMin = 12500;
                    screenRotateTimer.Interval = (int)(rotateTimeMin + (random.NextDouble() * (rotateTimeMax - rotateTimeMin)));
                    return scr;
                    #endregion
            }
            return null;
        }

        private void smoothTimer_Tick(object sender, EventArgs e)
        {
            #region Transitions
            if (newScreenPictureBox != null && currentTransitionType >= 0)
            {
                int offsetX = 0;
                int offsetY = 0;

                switch (currentTransitionType)
                {
                    case 0:
                        break;
                    case 1:
                        offsetX = -transitionSpeed;
                        break;
                    case 2:
                        offsetX = transitionSpeed;
                        break;
                    case 3:
                        offsetY = transitionSpeed;
                        break;
                    case 4:
                        offsetY = -transitionSpeed;
                        break;
                    default:
                        break;
                }
                bool transitionFinished = false;
                if (0 < currentTransitionType && currentTransitionType <= 4)
                {
                    int bellOffsetX = (int)(offsetX * StaticMethods.getBellCurveValue(Bounds.Width, Math.Abs(newScreenPictureBox.Location.X)));
                    int bellOffsetY = (int)(offsetY * StaticMethods.getBellCurveValue(Bounds.Height, Math.Abs(newScreenPictureBox.Location.Y)));
                    currentScreenPictureBox.Location = new Point(currentScreenPictureBox.Location.X + bellOffsetX, currentScreenPictureBox.Location.Y + bellOffsetY);
                    newScreenPictureBox.Location = new Point(newScreenPictureBox.Location.X + bellOffsetX, newScreenPictureBox.Location.Y + bellOffsetY);


                    switch (currentTransitionType)
                    {
                        case 0:
                            break;
                        case 1:
                            if (newScreenPictureBox.Location.X <= 0)
                            {
                                transitionFinished = true;
                            }
                            break;
                        case 2:
                            if (newScreenPictureBox.Location.X >= 0)
                            {
                                transitionFinished = true;
                            }
                            break;
                        case 3:
                            if (newScreenPictureBox.Location.Y >= 0)
                            {
                                transitionFinished = true;
                            }
                            break;
                        case 4:
                            if (newScreenPictureBox.Location.Y <= 0)
                            {
                                transitionFinished = true;
                            }
                            break;
                        default:
                            break;
                    }

                }

                if (transitionFinished)
                {
                    Controls.Remove(currentScreenPictureBox);
                    currentScreenIndex = newScreenIndex;
                    newScreenIndex = -1;
                    currentScreenPictureBox = newScreenPictureBox;
                    newScreenPictureBox = null;
                    currentScreenPictureBox.Location = new Point(0, 0);
                    
                    screenRotateTimer.Start();
                }

            }
            #endregion
            #region Screen Updates
            else
            {
                Control.ControlCollection c = currentScreenPictureBox.Controls;
                switch (currentScreenIndex)
                {
                    case 0:
                        ((ExtendedPictureBox)c.Find("hexApeture", true)[0]).rotation += 0.3f;
                        ((ExtendedPictureBox)c.Find("hexApeture", true)[0]).Refresh();
                        break;
                    case 1:
                        break;
                    case 2:
                        foreach(RandomStringLabel r in c)
                        {
                            r.Location = new Point(r.Location.X + r.xSpeed, r.Location.Y + r.ySpeed);
                        }
                        break;
                    case 3:
                        ((ExtendedPictureBox)c.Find("Clock", true)[0]).text = DateTime.Now.ToString("hh:mm:ss");
                        ((ExtendedPictureBox)c.Find("Clock", true)[0]).Refresh();
                        break;
                    case 4:
                        ((Label)c.Find("Title", true)[0]).Location = new Point(((Label)c.Find("Title", true)[0]).Location.X + 1, ((Label)c.Find("Title", true)[0]).Location.Y);
                        ((Label)c.Find("Author", true)[0]).Location = new Point(((Label)c.Find("Author", true)[0]).Location.X + 2, ((Label)c.Find("Author", true)[0]).Location.Y);
                        break;
                    case 5:
                        ExtendedPictureBox d = ((ExtendedPictureBox)c.Find("radar", true)[0]);
                        d.Location = new Point(d.Location.X + 10, d.Location.Y);
                        if(d.Location.X > Bounds.Width + 10)
                        {
                            d.Location = new Point(-20, 0);
                        }
                        d.Invalidate();
                        break;
                }
            }
            #endregion
        }

        private void screenEventTimer_Tick(object sender, EventArgs e)
        {
            if (newScreenPictureBox == null)
            {
                Control.ControlCollection c = currentScreenPictureBox.Controls;
                switch (currentScreenIndex)
                {
                    case 0:
                        break;
                    case 1:
                        RandomStringLabel ranStr = (RandomStringLabel)c.Find("RandomString", true).First<Control>();
                        Label pointer = (Label)c.Find("Text Cursor", true).First<Control>();

                        c.Add(new Label()
                        {
                            Text = ranStr.Text,
                            Font = new Font(pfc.Families[3], Bounds.Width / 50, FontStyle.Regular),
                            AutoSize = true,
                            ForeColor = Color.White,
                            Location = ranStr.Location,
                            TextAlign = ContentAlignment.MiddleCenter
                        });
                        c.Find("Border", true)[0].SendToBack();

                        ranStr.Location = new Point(ranStr.Location.X, ranStr.Location.Y + (int)(Bounds.Width * 0.04));
                        pointer.Location = new Point(pointer.Location.X, pointer.Location.Y + (int)(Bounds.Width * 0.04));
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        /*
                        for(int i = 0; i < 100; i ++)
                        {
                            int randomY = (int)(random.NextDouble() * Bounds.Height);
                            ExtendedPictureBox line = ((ExtendedPictureBox)c.Find("radar", true)[0]);
                            ExtendedPictureBox map = ((ExtendedPictureBox)c.Find("Map", true)[0]);

                            Color pixelColor = ((Bitmap)map.Image).GetPixel(line.Location.X, randomY);


                            if (pixelColor.ToArgb() != Color.Transparent.ToArgb())
                            {
                                PictureBox dot = new PictureBox() {
                                    Location = new Point(line.Location.X - 25, randomY),
                                    Size = new Size(50, 50)
                                };
                                dot.Paint += ((a, b) => {
                                    Graphics g = b.Graphics;

                                    g.FillEllipse(new SolidBrush(Color.White), new Rectangle(0,0,dot.Width,dot.Height));

                                });
                                map.Controls.Add(dot);
                                break;
                            }
                        }
                        */
                        break;
                }
                screenEventTimer.Interval = (int)(screenEventTimeMin + (random.NextDouble() * (screenEventTimeMax - screenEventTimeMin)));
            }

        }

        private void RandomTextTimer_Tick(object sender, EventArgs e)
        {
            if(newScreenPictureBox == null)
            {
                foreach (Control c in currentScreenPictureBox.Controls)
                {
                    if (c is RandomStringLabel)
                    {
                        c.Text = StaticMethods.RandomString(((RandomStringLabel)c).stringLength);
                    }
                }
            }
        }

        private void screenRotateTimer_Tick(object sender, EventArgs e)
        {
            transitionTypes[(int)(random.NextDouble() * transitionTypes.Length)]();

            screenRotateTimer.Stop();
        }

        private void screenSwitch(int transitionType)
        {
            Color newBGColor = bgColors[(int)(random.NextDouble() * bgColors.Length)];
            while (newBGColor == currentScreenPictureBox.BackColor)
            {
                newBGColor = bgColors[(int)(random.NextDouble() * bgColors.Length)];
            }
            int nextScreenIndex = (int)(random.NextDouble() * screens.Length);
            while (nextScreenIndex == currentScreenIndex)
            {
                nextScreenIndex = (int)(random.NextDouble() * screens.Length);
            }
            newScreenPictureBox = new PictureBox()
            {
                Bounds = new Rectangle(0, 0, Bounds.Width, Bounds.Height),
                BackColor = newBGColor
            };
            List<Control> controls = screens[nextScreenIndex].Invoke();
            foreach (Control c in controls)
            {
                newScreenPictureBox.Controls.Add(c);
            }
            switch (transitionType)
            {
                case 0:
                    newScreenPictureBox.Location = new Point(0, 0);
                    this.Controls.Add(newScreenPictureBox);
                    this.Controls.Remove(currentScreenPictureBox);
                    currentScreenPictureBox = newScreenPictureBox;
                    newScreenPictureBox = null;
                    currentScreenIndex = newScreenIndex;
                    currentTransitionType = -1;
                    break;
                case 1:
                    newScreenPictureBox.Location = new Point(Bounds.Width, 0);
                    currentTransitionType = 1;
                    break;
                case 2:
                    newScreenPictureBox.Location = new Point(-Bounds.Width, 0);
                    currentTransitionType = 2;
                    break;
                case 3:
                    newScreenPictureBox.Location = new Point(0, -Bounds.Height);
                    currentTransitionType = 3;
                    break;
                case 4:
                    newScreenPictureBox.Location = new Point(0, Bounds.Height);
                    currentTransitionType = 4;
                    break;
            }
            System.Diagnostics.Debug.WriteLine(newScreenIndex);
            if(transitionType != 0) // since transition 0 is different from rest
            {
                this.Controls.Add(newScreenPictureBox);
            }
            
        }

        private void AnalysisScreenForm_Load(object sender, EventArgs e)
        {
            screenSwitch(0);
            this.BackColor = Color.Black;
            this.WindowState = FormWindowState.Normal;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            Cursor.Hide();
        }

        private void ScreenSaverForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!previewMode)
            {
                if (!mouseLocation.IsEmpty)
                {
                    // Terminate if mouse is moved a significant distance
                    if (Math.Abs(mouseLocation.X - e.X) > 5 ||
                        Math.Abs(mouseLocation.Y - e.Y) > 5)
                        Application.Exit();
                }

                // Update current mouse location
                mouseLocation = e.Location;
            }
        }

        private void ScreenSaverForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (!previewMode)
                Application.Exit();
        }

        private void ScreenSaverForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!previewMode)
                Application.Exit();
        }

        private void AnalysisScreenForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        
    }
}
