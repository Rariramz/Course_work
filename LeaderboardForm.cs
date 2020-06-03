using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class LeaderboardForm : Form
    {
        public LeaderboardForm(Leaderboard leaderboard)
        {
            InitializeComponent();

            if (leaderboard.Easy != null)
            {
                richTextBox1.AppendText("------------EASY------------\n");
                foreach ((string, int) player in leaderboard.Easy)
                    richTextBox1.AppendText(ToString(player.Item1, player.Item2) + '\n');                
            }
            if (leaderboard.Medium.Count > 0)
            {
                richTextBox1.AppendText("-----------MEDIUM-----------\n");
                foreach ((string, int) player in leaderboard.Medium)
                    richTextBox1.AppendText(ToString(player.Item1, player.Item2) + '\n');
            }
            if (leaderboard.Hard.Count > 0)
            {
                richTextBox1.AppendText("------------HARD------------\n");
                foreach ((string, int) player in leaderboard.Hard)
                    richTextBox1.AppendText(ToString(player.Item1, player.Item2) + '\n');
            }
        }

        string ToString(string name, int time)
        {
            string str = string.Format("{0:D2}:{1:D2}", time / 60, time % 60);
            return name.PadRight(28 - str.Length, '.') + str;
        }
    }
}
