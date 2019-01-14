using System;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Othello
{
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    [Serializable]
    class Playable : IPlayable.IPlayable, INotifyPropertyChanged
    {
        private int[,] board = new int[7, 9];
        private bool player0IsAI;
        private bool player1IsAI;
        private int blackScore;
        public int BlackScore
        {
            get { return this.blackScore; }
            set
            {
                if (blackScore != value)
                {
                    blackScore = value;
                    FirePropertyChanged("BlackScore");
                }
            }
        }
        private int whiteScore;
        public int WhiteScore
        {
            get { return whiteScore; }
            set
            {
                if (whiteScore != value)
                {
                    whiteScore = value;
                    FirePropertyChanged("WhiteScore");
                }
            }
        }
        private TimeSpan TIME = new TimeSpan(0, 5, 0);
        private TimeSpan timeP1;
        private TimeSpan timeP2;

        [NonSerialized]
        public Stopwatch stopwatchP1;
        [NonSerialized]
        public Stopwatch stopwatchP2;

        [NonSerialized]
        public DispatcherTimer dispatcherTimeRemaining = new DispatcherTimer();

        public bool canPlay = false;
        [NonSerialized]
        private MainWindow mainWindow;
        public MainWindow MainWindow
        {
            get { return mainWindow; }
            set
            {
                mainWindow = value;
            }
        }
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        private void FirePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private bool whiteTurn;
        public bool WhiteTurn
        {
            get { return whiteTurn; }
            set
            {
                if (whiteTurn != value)
                {
                    whiteTurn= value;
                }
            }
        }
        public Playable()
        {
        }
        public Playable(bool _player0IsAI, bool _player1IsAI, MainWindow _mainWindow)
        {
            whiteTurn = true;
            player0IsAI = _player0IsAI;
            player1IsAI = _player1IsAI;
            mainWindow = (MainWindow)App.Current.MainWindow;

            initDispatcher();

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    board[i, j] = -1;
                }
            }

            board[3, 3] = 0;
            board[4, 4] = 0;
            board[3, 4] = 1;
            board[4, 3] = 1;

            BlackScore = GetBlackScore();
            WhiteScore = GetWhiteScore();

            stopwatchP1 = new Stopwatch();
            stopwatchP2 = new Stopwatch();

            stopwatchP1.Start();
            stopwatchP2.Stop();
        }
        public void saveTime()
        {
            if (stopwatchP1.IsRunning)
                stopwatchP1.Stop();
            if (stopwatchP2.IsRunning)
                stopwatchP2.Stop();
            timeP1 = stopwatchP1.Elapsed;
            timeP2 = stopwatchP2.Elapsed;
        }
        public void initDispatcher()
        {
            dispatcherTimeRemaining = new DispatcherTimer();
            dispatcherTimeRemaining.Tick += new EventHandler(dispatcherTimeRemaining_Tick);
            dispatcherTimeRemaining.Interval = new TimeSpan(0, 0, 0, 0, 1);
            dispatcherTimeRemaining.Start();
        }

        public int GetBlackScore()
        {
            return GetScore(0);
        }

        public int[,] GetBoard()
        {
            return board;
        }

        public string GetName()
        {
            return "Le meilleur Othello of the world of all times ever";
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            int column = 0, row = 0;
            do
            {
                if (whiteTurn && player0IsAI || !whiteTurn && player1IsAI)
                {
                    //AI CODE
                }
                else
                {
                    canPlay = true;
                }
            }
            while (!IsPlayable(column, row, whiteTurn));
            return new Tuple<int, int>(column, row);
        }
        public int GetWhiteScore()
        {
            return GetScore(1);
        }

        public bool IsPlayable(int row, int column, bool isWhite)
        {
            if(board[row, column] == -1)
            {
                int player = isWhite ? 0 : 1;
                int other = isWhite ? 1 : 0;

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i != 0 || j != 0)
                        {
                            if (checkDirection(row + i, column + j, player, other, i, j).Count > 0)
                                return true;
                        }
                    }
                }
            }          
            return false;
        }
        public List<Point> possibleMoves(bool isWhite)
        {
            List<Point> possiblesMoves = new List<Point>();
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (IsPlayable(i, j, isWhite))
                    {
                        possiblesMoves.Add(new Point { X = i, Y = j });
                    }
                }
            }
            return possiblesMoves;
        }
        private List<Point> checkDirection(int i, int j, int player, int other, int incI = 0, int incJ = 0)
        {
            List<Point> Cases = new List<Point>();
            if (boundsCheck(i, j))
            {
                if (board[i, j] == other)
                {
                    while (boundsCheck(i, j))
                    {
                        if (board[i, j] == other)
                        {
                            Cases.Add(new Point { X = i, Y = j });
                            if (boundsCheck(i + incI, j + incJ) && board[i + incI, j + incJ] == player)
                                return Cases;
                        }
                        else
                        {
                            return new List<Point>();
                        }
                        i += incI;
                        j += incJ;
                    }
                }
            }
            return new List<Point>();
        }

        private bool boundsCheck(int i, int j)
        {
            return (-1 < i && i < board.GetLength(0) && -1 < j && j < board.GetLength(1));
        }

        public bool PlayMove(int row, int column, bool isWhite)
        {
            if (IsPlayable(row, column, isWhite))
            {
                int player = isWhite ? 0 : 1;
                int other = isWhite ? 1 : 0;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i != 0 || j != 0)
                        {
                            foreach (Point p in checkDirection(row + i, column + j, player, other, i, j))
                            {
                                board[p.X, p.Y] = player;
                                mainWindow.replaceImage(p.Y, p.X, other);
                            }
                            board[row, column] = player;
                        }
                    }
                }
                BlackScore = GetBlackScore();
                WhiteScore = GetWhiteScore();
                return true;
            }
            return false;
        }
        public int GetScore(int x)
        {
            int count = 0;
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == x) count++;
                }
            }

            return count;
        }
        private void dispatcherTimeRemaining_Tick(object sender, EventArgs e)
        {
            dispatcherTimeRemaining.Interval = new TimeSpan(0, 0, 0, 0, 250);
            TimeSpan time = TIME - stopwatchP1.Elapsed - timeP1;
            mainWindow.p1Time.Content = time.ToString(@"mm\:ss");
            TimeSpan time2 = TIME - stopwatchP2.Elapsed - timeP2;
            mainWindow.p2Time.Content = time2.ToString(@"mm\:ss");
        }
        public bool getWinner()
        {
            return GetBlackScore() > GetWhiteScore() ? false : true;
        }
    }
}
