using System;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class GameForm : Form
    {
        GameController controller;
        Form mainMenu;
        Leaderboard leaderboard;
        Leaderboard.Difficulties difficulty;
        public GameForm(int y, int x, int minesCount, Leaderboard leaderboard, Form mainMenu)
        {
            InitializeComponent();

            if (minesCount == 10)
                difficulty = Leaderboard.Difficulties.Easy;
            else if (minesCount == 40)
                difficulty = Leaderboard.Difficulties.Medium;
            else if (minesCount == 99)
                difficulty = Leaderboard.Difficulties.Hard;

            this.leaderboard = leaderboard;
            this.mainMenu = mainMenu;

            int size = panel1.Height / y;
            panel1.Size = new Size(size * x + 30, size * y + 30);
            Size = new Size(Size.Width + panel1.Width + 15, Size.Height + 45);
            controller = new GameController(y, x, minesCount, size, panel1, MinesCountLabel, TimeLabel, timer1);
            controller.Ending += EndGame;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            controller.Reset();
            controller.IsActive = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mainMenu.Visible = true;
            Close();
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!mainMenu.Visible)
                mainMenu.Close();
        }

        void EndGame(bool result, int time)
        {
            if (!result)
                MessageBox.Show("Better luck next time!");
            else
            {
                string name = Microsoft.VisualBasic.Interaction.InputBox(string.Format("Congratulations!" +
                    "Your time is {0:D2}:{1:D2}\n Type your name:", time / 60, time % 60), "Type your name", "Player");
                if (string.IsNullOrEmpty(name))
                    return;
                
                leaderboard.Add(name, time, difficulty);
            }
        }
    }
}
