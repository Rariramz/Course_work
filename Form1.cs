using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        Leaderboard leaderboard;
        public Form1()
        {
            InitializeComponent();

            leaderboard = LoadLeaderboard();
        }

        Leaderboard LoadLeaderboard()
        {
            Leaderboard leaderboard;

            if (!File.Exists("leaderboard"))
                leaderboard = null;            
            else
            {
                using(StreamReader sr = new StreamReader("leaderboard"))
                {
                    string str = sr.ReadToEnd();
                    leaderboard = JsonConvert.DeserializeObject<Leaderboard>(str);
                }
            }
            if (leaderboard == null)
                return new Leaderboard();

            return leaderboard;
        }

        void StartGame(int y, int x, int mines)
        {
            GameForm game = new GameForm(y, x, mines, leaderboard, this);
            game.Visible = true;
            Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StartGame(9, 9, 10);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StartGame(16, 16, 40);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            StartGame(16, 30, 99);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LeaderboardForm form = new LeaderboardForm(leaderboard);
            form.Show();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            using(StreamWriter sw = new StreamWriter("leaderboard"))
                sw.Write(JsonConvert.SerializeObject(leaderboard)); 
        }
    }
}
