using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Othello
{
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    [Serializable]
    public class SerializableStopwatch : Stopwatch, ISerializable
    {
        public SerializableStopwatch(int _player)
        {
            Player = _player;
        }
        private int player;
        public int Player
        {
            get { return player; }
            set { player = value; }
        }
        private TimeSpan timeByLoad = new TimeSpan();
        public TimeSpan TimeByLoad
        {
            get { return timeByLoad; }
            set { timeByLoad = value; }
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Elapsed", Elapsed);
            info.AddValue("isRunning", IsRunning);
            info.AddValue("player", Player);
        }
        public SerializableStopwatch(SerializationInfo info, StreamingContext context)
        {
             Player = (int)info.GetValue("player", typeof(int));
             TimeByLoad = (TimeSpan)info.GetValue("Elapsed", typeof(TimeSpan));
             bool b = (bool)info.GetValue("isRunning", typeof(bool));
             if (b)
                this.Start();
        }
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
        public SerializableStopwatch stopwatchP1;
        public SerializableStopwatch stopwatchP2;

        [NonSerialized]
        DispatcherTimer dispatcherTimeRemaining = new DispatcherTimer();

        public bool canPlay = false;
        [NonSerialized]
        private MainWindow mainWindow;
        public void setMainWindow(MainWindow m)
        {
            mainWindow = m;
        }
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        private void FirePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                Console.WriteLine("update");
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

        public Playable(bool _player0IsAI, bool _player1IsAI, MainWindow _mainWindow)
        {
            whiteTurn = false;
            player0IsAI = _player0IsAI;
            player1IsAI = _player1IsAI;
            mainWindow = _mainWindow;

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

            Console.WriteLine("Moves : ");
            Console.WriteLine("White possible moves : " + possibleMoves(true).Count);
            Console.WriteLine("Black possible moves : " + possibleMoves(false).Count);
            stopwatchP1 = new SerializableStopwatch(0);
            stopwatchP2 = new SerializableStopwatch(1);


            stopwatchP1.Start();
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
        private void showPossibleMoves(List<Point> possiblesMoves)
        {
            foreach (Point P in possiblesMoves)
            {
                //Color case on board
            }
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
            return (-1 < i && i < 7 && -1 < j && j < 9);
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
                                Console.WriteLine(p.X + " " + p.Y);
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
            if (stopwatchP1.IsRunning)
            {
                if (stopwatchP1.Elapsed < TimeSpan.FromMinutes(5))
                {
                    TimeSpan time = TIME - stopwatchP1.Elapsed - stopwatchP1.TimeByLoad;
                    mainWindow.p1Time.Content = time.ToString(@"mm\:ss");
                }
                if(stopwatchP2.ElapsedMilliseconds == 0)
                {
                    mainWindow.p2Time.Content = TIME.ToString(@"mm\:ss");
                }
            }
            else if(stopwatchP2.IsRunning)
            {
                if (stopwatchP2.Elapsed < TimeSpan.FromMinutes(5))
                {
                    TimeSpan time = TIME - stopwatchP2.Elapsed - stopwatchP2.TimeByLoad;
                    mainWindow.p2Time.Content = time.ToString(@"mm\:ss");
                }
            }
        }
        public bool getWinner()
        {
            return GetBlackScore() > GetWhiteScore() ? false : true;
        }
    }
}
