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
        Point mouse;
        bool clicking = false;
        bool changesMade = true;
        static int zoom = 1;
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
            drawTimer.Interval = 10;
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
            bool[][] prevCities = new bool[Height / zoom][];
            for (int i = 0; i < prevCities.Length; i++)
            {
                prevCities[i] = new bool[Width / zoom];
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
            aliveVals = new bool[Height / zoom][];
            for (int i = 0; i < aliveVals.Length; i++)
            {
                aliveVals[i] = new bool[Width / zoom];
                for (int j = 0; j < aliveVals[i].Length; j++)
                {
                    aliveVals[i][j] = false;
                }
            }
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            clicking = true;
                aliveVals[e.Y / zoom][e.X / zoom] = !aliveVals[e.Y / zoom][e.X / zoom];
                update();
                changesMade = true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }
            e.Graphics.FillRectangle(Brushes.Black, mouse.X, mouse.Y, zoom, zoom);
            if (changesMade)
            {
                e.Graphics.Clear(addingCities ? Color.FromArgb(254, 254, 254) : Color.White);
                for (int i = 0; i < aliveVals.Length; i++)
                {
                    for (int j = 0; j < aliveVals[i].Length; j++)
                    {
                        if (aliveVals[i][j])
                            e.Graphics.FillRectangle(new SolidBrush(addingCities ? Color.Black : invertColor(bitmap.GetPixel(j * zoom, i * zoom))), j * zoom, i * zoom, zoom, zoom);
                    }
                }
                changesMade = false;
            }
        }

        private void mainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                clicking = false;
                drawTimer.Enabled = !drawTimer.Enabled;
                addingCities = !addingCities;
                this.FormBorderStyle = !addingCities? FormBorderStyle.None : FormBorderStyle.Sizable;
            }
            if (e.KeyCode == Keys.Escape)
                Application.Exit();
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
                init();
            if (e.KeyCode == Keys.Down)
                zoom--;
            if (e.KeyCode == Keys.Up)
                zoom++;

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
                aliveVals[e.Y / zoom][e.X / zoom] = !aliveVals[e.Y / zoom][e.X / zoom];
                update();
                changesMade = true;
            }
        }
        public static Color invertColor(Color c)
        {
            return Color.FromArgb(255 ^ c.R, 255 ^ c.G, 255 ^ c.B);
        }
    }
}
