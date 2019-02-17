using System;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Diagnostics;
using System.ComponentModel;

namespace Othello
{
    /// <summary>
    /// Point class used to store two int represeting a box of 2d array
    /// </summary>
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    class Playable : IPlayable.IPlayable, INotifyPropertyChanged
    {
        private int[,] board = new int[7, 9];
       
        public bool PlayerWhiteIsAI
        { get;}
        public bool PlayerBlackIsAI
        { get;}
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
        [NonSerialized]
        private MainWindow _mainWindow;
        public MainWindow MainWindow
        {
            get { return _mainWindow; }
            set { _mainWindow = value; }
        }

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        private void FirePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public bool WhiteTurn
        { get; set; }
        public Playable(): this(false, null) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_player0IsAI"></param>
        /// <param name="_player1IsAI"></param>
        /// <param name="_mainWindow"></param>
        public Playable(bool _playerWhiteIsAI, MainWindow _mainWindow)
        {
            WhiteTurn = false;
            PlayerWhiteIsAI = _playerWhiteIsAI;
            MainWindow = _mainWindow;

            InitDispatcher();

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
        public void SavePlayerTime()
        {
            if (stopwatchP1.IsRunning)
                stopwatchP1.Stop();
            if (stopwatchP2.IsRunning)
                stopwatchP2.Stop();
            timeP1 = stopwatchP1.Elapsed;
            timeP2 = stopwatchP2.Elapsed;
        }
        /// <summary>
        /// Initialization of the dispatcher, register the event, set the interval, and start it
        /// this method is used because dispatcher can't be serialize
        /// </summary>
        public void InitDispatcher()
        {
            dispatcherTimeRemaining = new DispatcherTimer();
            dispatcherTimeRemaining.Tick += new EventHandler(DispatcherTimeRemaining_Tick);
            dispatcherTimeRemaining.Interval = new TimeSpan(0, 0, 0, 0, 1);
            dispatcherTimeRemaining.Start();
        }
        /// <summary>
        /// Return black player score
        /// </summary>
        /// <returns>Black player score</returns>
        public int GetBlackScore()
        {
            return GetScore(0);
        }
        /// <summary>
        /// return the 2d array representing the game board
        /// </summary>
        /// <returns>2d board</returns>
        public int[,] GetBoard()
        {
            return board;
        }

        /// <summary>
        /// return game name 
        /// </summary>
        /// <returns>game name</returns>
        public string GetName()
        {
            return "Le meilleur Othello of the world of all times ever";
        }
        /// <summary>
        /// Ask the AI to play, and return the move receive
        /// </summary>
        /// <param name="game">2d game board</param>
        /// <param name="level">levl of AI</param>
        /// <param name="whiteTurn">is white player turn</param>
        /// <returns>a tuple with two int represing a box in the game board</returns>
        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            int column = 0, row = 0;
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (IsPlayable(i, j, whiteTurn))
                    {
                        return new Tuple<int, int>(j, i);
                    }
                }
            }
            return new Tuple<int, int>(column, row);
        }
        /// <summary>
        /// Return white player score
        /// </summary>
        /// <returns>White player score</returns>
        public int GetWhiteScore()
        {
            return GetScore(1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        public bool IsPlayable(int row, int column, bool isWhite)
        {
            if(board[row, column] == -1)
            {
                int player = isWhite ? 1 : 0;
                int other = isWhite ? 0 : 1;

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i != 0 || j != 0)
                        {
                            if (CheckDirection(row + i, column + j, player, other, board, i, j).Count > 0)
                                return true;
                        }
                    }
                }
            }          
            return false;
        }
        /// <summary>
        /// return a list of the possibles moves 
        /// </summary>
        /// <param name="isWhite"></param>
        /// <returns>a list of point with represeting all the boxes of the board where the player can play</returns>
        public List<Point> PossibleMoves(bool isWhite)
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

        private List<Point> CheckDirection(int i, int j, int player, int other, int[,] myBoard, int incI = 0, int incJ = 0)
        {
            List<Point> Cases = new List<Point>();
            if (BoundsCheck(i, j) && board[i, j] == other)
            {
                while (BoundsCheck(i, j))
                {
                    if (board[i, j] == other)
                    {
                        Cases.Add(new Point { X = i, Y = j });
                        if (BoundsCheck(i + incI, j + incJ) && board[i + incI, j + incJ] == player)
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
            return new List<Point>();
        }
        /// <summary>
        /// return if the given index aren't out of bound
        /// </summary>
        private bool BoundsCheck(int i, int j)
        {
            return (-1 < i && i < board.GetLength(0) && -1 < j && j < board.GetLength(1));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="row">row no to play</param>
        /// <param name="column">column no to play</param>
        /// <param name="isWhite">is white player playing</param>
        /// <returns>if move can be played</returns>
        public bool PlayMove(int row, int column, bool isWhite)
        {
            TreeNode node = new TreeNode(this);
            Alphabeta(node, 3, 1, 0, out int optVal, out Point? optOp);
            if (optOp.HasValue)
            {
                Console.WriteLine(optVal + " " + optOp.Value.X + ":" + optOp.Value.Y);
                row = optOp.Value.X;
                column = optOp.Value.Y;
            }
            else
                Console.WriteLine("BUG");
            if (IsPlayable(row, column, isWhite))
            {
                int player = isWhite ? 1 : 0;
                int other = isWhite ? 0 : 1;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        //we need to skip the one with no deplacement
                        if (i != 0 || j != 0)
                        {
                            //we update every retourned piece
                            foreach (Point p in CheckDirection(row + i, column + j, player, other, board, i, j))
                            {
                                board[p.X, p.Y] = player;
                                MainWindow.ReplaceImage(p.Y, p.X, player, true);                                
                            }
                            //we place the new piece
                            board[row, column] = player;
                            MainWindow.ReplaceImage(column, row, player, true);
                        }
                    }
                }
                //Update the score
                BlackScore = GetBlackScore();
                WhiteScore = GetWhiteScore();
                return true;
            }
            return false;
        }
        /// <summary>
        /// count the number of occurence of the given int on the board
        /// </summary>
        private int GetScore(int x)
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
        /// <summary>
        /// Called every 250 millisecondes by the dispatcher to update the time on window 
        /// </summary>
        private void DispatcherTimeRemaining_Tick(object sender, EventArgs e)
        {
            dispatcherTimeRemaining.Interval = new TimeSpan(0, 0, 0, 0, 250);
            TimeSpan time = stopwatchP1.Elapsed + timeP1;
            MainWindow.p1Time.Content = time.ToString(@"mm\:ss");
            TimeSpan time2 = stopwatchP2.Elapsed + timeP2;
            MainWindow.p2Time.Content = time2.ToString(@"mm\:ss");
        }
        /// <summary>
        /// Return if white player has the best score
        /// </summary>
        /// <returns>if white has win</returns>
        public bool IsWhiteWinner()
        {
            return GetBlackScore() > GetWhiteScore() ? false : true;
        }
        public class TreeNode
        {
            int[,] nodeBoard = new int[7, 9];
            Playable p;
            List<Point> ops;
            public TreeNode(Playable playable, int[,]board=null)
            {
                p = playable;
                if (board != null)
                    Array.Copy(board, nodeBoard, board.Length);
                else
                    Array.Copy(p.GetBoard(), nodeBoard, p.GetBoard().Length);
            }
            
            public int Eval()
            {
                return 0;
            }
            public List<Point> GetOps()
            {
                List<Point> ops = p.PossibleMoves(p.WhiteTurn);
                return ops;
            }
            public bool Final()
            {
                if(p.PossibleMoves(true).Count == 0 && p.PossibleMoves(false).Count == 0)
                    return true;
                else
                    return false;
            }
            public TreeNode Apply(Point op)
            {
                int player = p.WhiteTurn ? 1 : 0;
                int other = p.WhiteTurn ? 0 : 1;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        //we need to skip the one with no deplacement
                        if (i != 0 || j != 0)
                        {
                            foreach (Point p in p.CheckDirection(op.X + i, op.Y + j, player, other, nodeBoard, i, j))
                            {
                                nodeBoard[p.X, p.Y] = player;
                            }
                            //we place the new piece
                            nodeBoard[op.X, op.Y] = player;
                        }
                    }
                }
                TreeNode next = new TreeNode(p, nodeBoard);
                
                return next;
            }
          
        }
        public void Alphabeta(TreeNode root, int depth, int minOrMax, int parentValue, out int optVal, out Point? optOp)
        {
            if(depth == 0 || root.Final())
            {
                optVal = root.Eval();
                optOp = null;
                return;
            }
            optVal = minOrMax * -Int32.MaxValue;
            optOp = null;
            foreach(Point op in root.GetOps())
            {
                TreeNode _new = root.Apply(op);
                Alphabeta(_new, depth - 1, -minOrMax, optVal, out int val, out Point? dummy);
                if(val * minOrMax > optVal * minOrMax)
                {
                    optVal = val;
                    optOp = op;
                    if (optVal * minOrMax > parentValue * minOrMax)
                    {
                        break;
                    }
                    return;
                }
            }

        }
       

    }
}
