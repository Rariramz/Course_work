using System.Windows.Forms;
using System.Drawing;
using Minesweeper.Properties;
using System.Resources;

namespace Minesweeper
{
    class Mine
    {
        public bool IsMine { get; set; }
        public bool HasFlag { get; set; }
        public bool IsDisarmed { get; set; }
        public bool isActive { get; set; }
        public int posY { get; set; }
        public int posX { get; set; }
        int minesAround;
        public int MinesAround { get => minesAround; set => SetPicture(value); }

        Button Button;
        PictureBox PictureBox;

        public delegate void Disarmed(int y, int x);
        public delegate void Flag(int modifier);

        public event Disarmed MineDisarmed;
        public event Disarmed CellOpened;
        public event Disarmed Exploded;
        public event Flag FlagSet;

        public Mine(int posY, int posX, int size, Control control)
        {
            isActive = true;
            HasFlag = false;
            IsMine = false;
            this.posY = posY;
            this.posX = posX;

            Initialize(size, posY, posX);
            control.Controls.Add(PictureBox);
            PictureBox.BringToFront();
            control.Controls.Add(Button);
            Button.BringToFront();
            Button.MouseUp += Disarm;
            PictureBox.MouseUp += Disarm;
        }

        void Initialize(int size, int posY, int posX)
        {                       //инициализируем кнопку каждой клетки и место для картинки под ней
            Button = new Button();
            Button.Location = new Point(size * posX + 15, size * posY + 15);
            Button.BackColor = Color.DarkGray;
            Button.BackgroundImageLayout = ImageLayout.Zoom;
            Button.Size = new Size(size, size);
            Button.TabIndex = 0;
            Button.TabStop = false;

            PictureBox = new PictureBox();
            PictureBox.Location = new Point(size * posX + 15, size * posY + 15);
            PictureBox.BackColor = Color.Gainsboro;
            PictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            PictureBox.Size = new Size(size, size);
            PictureBox.TabIndex = 0;
            PictureBox.TabStop = false;
        }

        void Disarm(object sender, MouseEventArgs e) //раскрыть бомбу с установлением картинки
        {
            Button.Parent.Focus();

            if (!isActive)
                return;

            if (e.Button == MouseButtons.Right && !IsDisarmed) //клик правой кнопкой мыши - установка флага
            {
                if (!HasFlag)
                    Button.BackgroundImage = Minesweeper.Properties.Resources.flag;
                else
                    Button.BackgroundImage = null;

                HasFlag = !HasFlag;
                FlagSet?.Invoke(HasFlag ? 1 : -1);
            }
            else if (e.Button == MouseButtons.Left && !IsDisarmed && !HasFlag)
                Disarm();
            else if (e.Button == MouseButtons.Left && IsDisarmed) //если клетка уже раскрыта -   
                CellOpened?.Invoke(posY, posX);                  //раскрыть другие безопасные вокруг неё
        }

        public void EndGame(bool isSuccess, int time)
        {
            isActive = false;

            if (!isSuccess && !HasFlag && IsMine)
                Button.BackgroundImage = Resources.mine;

            if (!isSuccess && HasFlag && !IsMine)            
                Button.BackgroundImage = Resources.wrong_flag;

            else if (isSuccess && IsMine && !HasFlag)
                Button.BackgroundImage = Resources.flag;
        }

        public void Disarm()
        {
            if (IsMine)
            {
                Button.BackgroundImage = Resources.mine;
                Exploded?.Invoke(posY, posX); //если мина, ставим соотв. картинку и вызываем событие взрыва
            }
            else
            {
                IsDisarmed = true;
                Button.Visible = false; //иначе просто раскрываем клетку, сделав кнопку невидимой
                MineDisarmed?.Invoke(posY, posX);
            }
        }

        public void Reset()
        {
            Button.BackgroundImage = null;
            Button.Visible = true;
            PictureBox.Image = null;
            HasFlag = false;
            IsDisarmed = false;
            IsMine = false;
            MinesAround = 0;
            isActive = true;
        }
    
        void SetPicture(int value)
        {
            if (value == 1)
                PictureBox.Image = Resources._1;

            if (value == 2)
                PictureBox.Image = Resources._2;

            if (value == 3)
                PictureBox.Image = Resources._3;

            if (value == 4)
                PictureBox.Image = Resources._4;

            if (value == 5)
                PictureBox.Image = Resources._5;

            if (value == 6)
                PictureBox.Image = Resources._6;

            if (value == 7)
                PictureBox.Image = Resources._7;

            if (value == 8)
                PictureBox.Image = Resources._8;

            minesAround = value;
        }
    }
}
