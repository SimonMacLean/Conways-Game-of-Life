using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Conways_game_of_life
{
    public partial class mainForm : Form
    {
        static bool X = true;
        static bool _ = false;
        bool[][] startingPattern = new bool[][] {
            new bool[] {_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,X,_,_,_,_,_,_,_,_,_,_,_},
            new bool[] {_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,X,_,X,_,_,_,_,_,_,_,_,_,_,_},
            new bool[] {_,_,_,_,_,_,_,_,_,_,_,_,X,X,_,_,_,_,_,_,X,X,_,_,_,_,_,_,_,_,_,_,_,_,X,X},
            new bool[] {_,_,_,_,_,_,_,_,_,_,_,X,_,_,_,X,_,_,_,_,X,X,_,_,_,_,_,_,_,_,_,_,_,_,X,X},
            new bool[] {X,X,_,_,_,_,_,_,_,_,X,_,_,_,_,_,X,_,_,_,X,X,_,_,_,_,_,_,_,_,_,_,_,_,_,_},
            new bool[] {X,X,_,_,_,_,_,_,_,_,X,_,_,_,X,_,X,X,_,_,_,_,X,_,X,_,_,_,_,_,_,_,_,_,_,_},
            new bool[] {_,_,_,_,_,_,_,_,_,_,X,_,_,_,_,_,X,_,_,_,_,_,_,_,X,_,_,_,_,_,_,_,_,_,_,_},
            new bool[] {_,_,_,_,_,_,_,_,_,_,_,X,_,_,_,X,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_},
            new bool[] {_,_,_,_,_,_,_,_,_,_,_,_,X,X,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_}
        };
        Point mouse;
        bool clicking = false;
        bool changesMade = true;
        static Size pointSize = new Size(2,2);
        Timer drawTimer = new Timer();
        bool addingCities = true;
        bool[][] aliveVals;
        public mainForm()
        {
            InitializeComponent();
            mouse = Cursor.Position;
            typeof(Form).InvokeMember("DoubleBuffered", BindingFlags.SetProperty 
            | BindingFlags.Instance | BindingFlags.NonPublic, null,               
            this, new object[] { true });
            drawTimer.Enabled = false;
            drawTimer.Interval = 1;
            drawTimer.Tick += drawTimer_Tick;
        }

        private void drawTimer_Tick(object sender, EventArgs e)
        {
            update();
        }
        private void update()
        {
            Invalidate();
            if (addingCities)
                return;
            if (pointSize.Width == 0 || pointSize.Height == 0)
                return;
            bool[][] prevCities = new bool[Height / pointSize.Height][];
            for (int i = 0; i < prevCities.Length; i++)
            {
                prevCities[i] = new bool[Width / pointSize.Width];
                for (int j = 0; j < prevCities[i].Length; j++)
                {
                    prevCities[i][j] = false;
                }
            }
            for (int i = 1; i < prevCities.Length - 1 && i < aliveVals.Length - 1; i++)
            {
                for (int j = 1; j < prevCities[i].Length - 1 && j < aliveVals[i].Length - 1; j++)
                {
                    int nearbyAlive = 0;
                    nearbyAlive += aliveVals[i - 1][j] ? 1 : 0;
                    nearbyAlive += aliveVals[i + 1][j] ? 1 : 0;
                    nearbyAlive += aliveVals[i][j - 1] ? 1 : 0;
                    nearbyAlive += aliveVals[i][j + 1] ? 1 : 0;
                    nearbyAlive += aliveVals[i - 1][j - 1] ? 1 : 0;
                    nearbyAlive += aliveVals[i - 1][j + 1] ? 1 : 0;
                    nearbyAlive += aliveVals[i + 1][j - 1] ? 1 : 0;
                    nearbyAlive += aliveVals[i + 1][j + 1] ? 1 : 0;
                    prevCities[i][j] = nearbyAlive == 3 || (aliveVals[i][j] && nearbyAlive == 2);
                }
            }
            changesMade = aliveVals != prevCities;
            aliveVals = prevCities;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            init();
        }
        void init()
        {
            if (pointSize.Width == 0 || pointSize.Height == 0)
                return;
            aliveVals = new bool[Height / pointSize.Height][];
            for (int i = 0; i < aliveVals.Length; i++)
            {
                aliveVals[i] = new bool[Width / pointSize.Width];
                for (int j = 0; j < aliveVals[i].Length; j++)
                {
                    aliveVals[i][j] = false;
                    int startY = aliveVals.Length / 2 - startingPattern.Length / 2;
                    int endY = aliveVals.Length / 2 + startingPattern.Length / 2;
                    int startX = aliveVals[0].Length / 2 - startingPattern[0].Length / 2;
                    int endX = aliveVals[0].Length / 2 + startingPattern[0].Length / 2;
                    if (i >= startY && i <= endY && j >= startX && j < endX)
                        aliveVals[i][j] = startingPattern[i - startY][j - startX];
                }
            }
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (pointSize.Width == 0 || pointSize.Height == 0)
                return;
            clicking = true;
                aliveVals[mouse.Y / pointSize.Height][mouse.X / pointSize.Width] = !aliveVals[mouse.Y / pointSize.Height][mouse.X / pointSize.Width];
                update();
                changesMade = true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            //Rectangle bounds = Screen.PrimaryScreen.Bounds;
            //Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            //using (Graphics g = Graphics.FromImage(bitmap))
            //{
            //    g.CopyFromScreen(Point.Empty, new Point(0, 5), bounds.Size);
            //}
            if(pointSize.Width != 0 && pointSize.Height != 0)
                e.Graphics.FillRectangle(Brushes.Gray, mouse.X/pointSize.Width*pointSize.Width, mouse.Y / pointSize.Height * pointSize.Height, pointSize.Width, pointSize.Height);
            if(changesMade)
            {
                //e.Graphics.Clear(/*addingCities ? */Color.FromArgb(254, 254, 254)/* : Color.White*/);
                for (int i = 0; i < aliveVals.Length; i++)
                {
                    for (int j = 0; j < aliveVals[i].Length; j++)
                    {
                        if (aliveVals[i][j])
                            e.Graphics.FillRectangle(new SolidBrush(/*addingCities ? */Color.Black/* : invertColor(bitmap.GetPixel(j * zoom, i * zoom))*/), j * pointSize.Width, i * pointSize.Height, pointSize.Width, pointSize.Height);
                    }
                }
                changesMade = false;
            }
        }

        private void mainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    clicking = false;
                    drawTimer.Enabled = !drawTimer.Enabled;
                    addingCities = !addingCities;
                    break;
                case Keys.Escape:
                    Application.Exit();
                    break;
                case Keys.Delete:
                    init();
                    break;
                case Keys.Back:
                    init();
                    break;
                case Keys.Down:
                    pointSize.Height--;
                    break;
                case Keys.Up:
                    pointSize.Height++;
                    break;
                case Keys.Left:
                    pointSize.Width--;
                    break;
                case Keys.Right:
                    pointSize.Width++;
                    break;
            }
            Invalidate();

        }

        private void mainForm_MouseUp(object sender, MouseEventArgs e)
        {
            clicking = false;
        }

        private void mainForm_MouseMove(object sender, MouseEventArgs e)
        {
            mouse = e.Location;
            if (clicking)
            {
                if (pointSize.Width == 0 || pointSize.Height == 0)
                    return;
                aliveVals[e.Y / pointSize.Width][e.X / pointSize.Height] = !aliveVals[e.Y / pointSize.Width][e.X / pointSize.Height];
                update();
            }
            changesMade = true;
            if(addingCities)Invalidate();
        }
        public static Color invertColor(Color c)
        {
            return Color.FromArgb(255 ^ c.R, 255 ^ c.G, 255 ^ c.B);
        }
    }
}
