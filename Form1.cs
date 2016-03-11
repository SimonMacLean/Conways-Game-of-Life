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
    public partial class Form1 : Form
    {
        const int zoom = 3; 
        Timer t = new Timer();
        bool addingCities = true;
        bool[][] aliveVals;
        public Form1()
        {
            InitializeComponent();
            typeof(Form).InvokeMember("DoubleBuffered", BindingFlags.SetProperty 
            | BindingFlags.Instance | BindingFlags.NonPublic, null,               
            this, new object[] { true });                       
            t.Enabled = false;
            t.Interval = 10;
            t.Tick += T_Tick;
        }

        private void T_Tick(object sender, EventArgs e)
        {
            Invalidate();
            if (addingCities)
                return;
            bool[][] prevCities = new bool[aliveVals.Length][];
            for (int i = 0; i < prevCities.Length; i++)
            {
                prevCities[i] = new bool[aliveVals[i].Length];
                for (int j = 0; j < prevCities[i].Length; j++)
                {
                    prevCities[i][j] = false;
                }
            }
            for (int i = 1; i < aliveVals.Length - 1; i++)
            {
                for (int j = 1; j < aliveVals[i].Length - 1; j++)
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
            aliveVals = prevCities;
        }

        private void Form1_Load(object sender, EventArgs e)
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

        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            addingCities = !addingCities;
            t.Enabled = !t.Enabled;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if(addingCities)
            {
                aliveVals[e.Y / zoom][e.X / zoom] = !aliveVals[e.Y / zoom][e.X / zoom];

            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            for(int i = 0; i < aliveVals.Length; i++)
            {
                for(int j = 0; j < aliveVals[i].Length; j++)
                {
                    e.Graphics.Clear(Color.White);
                    if(aliveVals[i][j])
                        e.Graphics.FillRectangle(Brushes.Black, j * zoom, i * zoom, zoom, zoom);
                }
            }
        }
    }
    public struct City
    {
        bool alive;
        Point location;
        City[] nearbyCities;
        public void update()
        {
            int nearbyAlive = 0;
            foreach (var t in nearbyCities)
            {
                nearbyAlive += t.alive ? 1 : 0;
            }
            alive = alive ? nearbyAlive == 2 || nearbyAlive == 3 : nearbyAlive == 3;
        }
    }
}
