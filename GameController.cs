using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Minesweeper
{
    class GameController
    {
        public bool IsActive { get; set;  } //идет ли игра

        public Mine[,] Mines { get; set; } //массив всех клеток поля
        public int MinesCount { get; set; }

        int cellsLeft; //количество оставшихся пустых клеток (без мин)
        
        public delegate void End(bool isSuccess, int time);
        public event End Ending;

        int time;
        int Time { get => time; set => SetTime(value); }

        int flagsCount;
        int FlagsCount { get => flagsCount;  set => SetFlags(value); }

        Timer timer;
        Label minesLeft, timeLabel;

        public GameController(int y, int x, int minesCount, int buttonSize,
            Control control, Label minesLeft, Label timeLabel, Timer timer)
        {
            MinesCount = minesCount; //передаем свойству количество мин в зависимости от выбранного уровня сложности
            cellsLeft = x * y - minesCount;

            this.minesLeft = minesLeft;
            this.timeLabel = timeLabel;

            FlagsCount = 0;
            Mines = new Mine[y, x];

            this.timer = timer;
            timer.Tick += (sender, e) => Time++;

            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    Mine mine = new Mine(i, j, buttonSize, control);
                    Mines[i, j] = mine;

                    mine.MineDisarmed += MineOpened;
                    mine.Exploded += Explosion;
                    Ending += mine.EndGame;
                    mine.CellOpened += CellOpened;
                    mine.FlagSet += (modifier) => FlagsCount += modifier;
                }
            }
        }

        void Explosion(int y, int x)
        {
            IsActive = false;
            timer.Enabled = false;
            Ending?.Invoke(false, Time);
        }

        void MineOpened(int posY, int posX)
        {

            if (!IsActive) //игра начинается после первого клика по полю
            {
                IsActive = true;     //передаем в метод начала игры позицию первого клика по полю,
                NewGame(posY, posX);//чтобы исключить возможность генерирования бомбы под этой клеткой
                timer.Enabled = true;
            }
            cellsLeft--;

            if (Mines[posY, posX].MinesAround == 0)
            {
                for (int y = Math.Max(0, posY - 1); y < Math.Min(Mines.GetLength(0), posY + 2); y++)
                {       /*Math.Max-Min для корректной работы функции с клетками по периметру*/
                    for (int x = Math.Max(0, posX - 1); x < Math.Min(Mines.GetLength(1), posX + 2); x++)
                    {
                        if (y == posY && x == posX)
                            continue;

                        if (!Mines[y, x].IsDisarmed && !Mines[y, x].HasFlag)
                            Mines[y, x].Disarm();
                    }
                }
            }

            if (cellsLeft == 0 && IsActive) //если все клетки раскрыты, то заканчиваем игру
            {
                timer.Enabled = false;
                IsActive = false;
                FlagsCount = MinesCount;
                Ending?.Invoke(true, Time);
            }
        }

        void CellOpened(int posY, int posX)
        {
            int flags = CountFlags(posY, posX);
            if (flags == Mines[posY, posX].MinesAround)
            {
                for (int y = Math.Max(0, posY - 1); y < Math.Min(Mines.GetLength(0), posY + 2); y++)
                {
                    for (int x = Math.Max(0, posX - 1); x < Math.Min(Mines.GetLength(1), posX + 2); x++)
                    {
                        if (y == posY && x == posX)
                            continue;

                        if (!Mines[y, x].HasFlag && !Mines[y, x].IsDisarmed)
                            Mines[y, x].Disarm();
                    }
                }
            }
        }
        int CountFlags(int posY, int posX)
        {
            int count = 0;
            for (int y = Math.Max(0, posY - 1); y < Math.Min(Mines.GetLength(0), posY + 2); y++)
            {
                for (int x = Math.Max(0, posX - 1); x < Math.Min(Mines.GetLength(1), posX + 2); x++)
                {
                    if (y == posY && x == posX)
                        continue;

                    if (Mines[y, x].HasFlag)
                        count++;
                }
            }
            return count;
        }
        void NewGame(int posY, int posX)
        {
            //создаем список координат всех клеток поля, чтобы потом перемешать пары и получить случайное местоположение бомб
            List<(int, int)> cells = new List<(int, int)>();
            for (int i = 0; i < Mines.GetLength(0); i++)
            {
                for (int j = 0; j < Mines.GetLength(1); j++)
                {
                    if (Math.Abs(posY - i) < 2 && Math.Abs(posX - j) < 2) //если клетка находится в "округе" кликнутой,
                        continue;                                        //не добавляем ее в список, чтобы с первого клика
                                                                        //открывалась какая-то свободная от бомб область
                    cells.Add((i, j)); //иначе добавляем координаты клетки в список
                }
            }
            //перемешиваем пары координат списка n раз
            Random r = new Random();
            for (int i = 0; i < cells.Count * 10; i++)
            {
                int a = r.Next(0, cells.Count), b = r.Next(0, cells.Count);
                (cells[a], cells[b]) = (cells[b], cells[a]);
            }
            for (int i = 0; i < MinesCount; i++) //определенному количеству клеток присваиваем значение "бомба"
            {
                Mines[cells[i].Item1, cells[i].Item2].IsMine = true;
            }

            //в оставшиеся безвредные клетки вписываем количество бомб вокруг них
            for (int i = 0; i < Mines.GetLength(0); i++)
            {
                for (int j = 0; j < Mines.GetLength(1); j++)
                {
                    int count = 0;
                    for (int y = Math.Max(0, i - 1); y < Math.Min(Mines.GetLength(0), i + 2); y++)
                    {
                        for  (int x = Math.Max(0, j - 1); x < Math.Min(Mines.GetLength(1), j + 2); x++)
                        {
                            if (i == y && j == x)
                                continue;

                            if (Mines[y, x].IsMine)
                                count++;
                        }
                    }
                    Mines[i, j].MinesAround = count;
                }
            }
        }
        public void Reset()
        {
            foreach (Mine mine in Mines)
                mine.Reset();

            timer.Enabled = false;
            cellsLeft = Mines.GetLength(0) * Mines.GetLength(1) - MinesCount;
            FlagsCount = 0;
            Time = 0;
        }
        public void SetFlags(int value)
        {
            minesLeft.Text = $"{MinesCount - value}/{MinesCount}";
            flagsCount = value;
        }

        void SetTime(int value)
        {
            timeLabel.Text = string.Format("{0:D2}:{1:D2}", value / 60, value % 60);
            time = value;
        }
    } 
}
