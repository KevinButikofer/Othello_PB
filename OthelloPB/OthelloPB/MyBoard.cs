﻿using System;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Diagnostics;
using System.ComponentModel;

namespace OthelloPB
{
  
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
        class MyBoard : IPlayable.IPlayable, INotifyPropertyChanged
        {
            private int[,] board = new int[9, 7];

            public bool PlayerWhiteIsAI
            { get; }
            public bool PlayerBlackIsAI
            { get; }
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

            [field: NonSerialized]
            public event PropertyChangedEventHandler PropertyChanged;
            private void FirePropertyChanged(string name)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }

            public bool WhiteTurn
            { get; set; }
            public MyBoard() : this(false) { }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="_player0IsAI"></param>
            /// <param name="_player1IsAI"></param>
            /// <param name="_mainWindow"></param>
            public MyBoard(bool _playerWhiteIsAI)
            {
                WhiteTurn = false;
                PlayerWhiteIsAI = _playerWhiteIsAI;

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
                WhiteTurn = whiteTurn;
                int column = 0, row = 0;
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    for (int j = 0; j < board.GetLength(1); j++)
                    {
                        TreeNode node = new TreeNode(this);
                        Alphabeta(node, 4, 1, 0, out double optVal, out Point? optOp);
                        if (optOp.HasValue)
                        {
                            Console.WriteLine(optVal + " " + optOp.Value.X + ":" + optOp.Value.Y);
                            row = optOp.Value.X;
                            column = optOp.Value.Y;
                            return new Tuple<int, int>(column, row);
                        }
                    }
                }
                return new Tuple<int, int>(-1, -1);
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
                if (board[row, column] == -1)
                {
                    int player = isWhite ? 0 : 1;
                    int other = isWhite ? 1 : 0;

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
                            possiblesMoves.Add(new Point { X = j, Y = i });
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
                if (IsPlayable(row, column, isWhite))
                {
                    int player = isWhite ? 0 : 1;
                    int other = isWhite ? 1 : 0;
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
                                }
                                //we place the new piece
                                board[row, column] = player;
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
                TimeSpan time2 = stopwatchP2.Elapsed + timeP2;
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
                MyBoard p;
                public TreeNode(MyBoard playable, int[,] board = null)
                {
                    p = playable;
                    if (board != null)
                        Array.Copy(board, nodeBoard, board.Length);
                    else
                        Array.Copy(p.GetBoard(), nodeBoard, p.GetBoard().Length);
                }

                public double Eval()
                {
                        int myTiles = 0;
                        int oppTiles = 0;
                        double q = 0;

                        int myTurn;
                        int oppTurn;

                        int[,] V = new int[,]
                        {
                            {20, -3, 11, 8, 2, 8, 11, -3, 20 },
                            {-3, -7, -4, 1, -3, 1, -4, -7, -3},
                            {11, -4, 2, 2, -3, 2, 2, -4, 11},
                            {8, 1, 2, -3, -3, -3, 2, 1, 8},
                            {11, -4, 2, 2, -3, 2, 2, -4, 11},
                            {-3, -7, -4, 1, -3, 1, -4, -7, -3},
                            {20, -3, 11, 8, 2, 8, 11, -3, 20}

                        };

                        if (p.WhiteTurn)
                        {
                            myTurn = 0;
                            oppTurn = 1;
                        }
                        else
                        {
                            myTurn = 1;
                            oppTurn = 0;
                        }
                        double d = 0;

                        for (int i = 0; i < nodeBoard.GetLength(0); i++)
                        {
                            for (int j = 0; j < nodeBoard.GetLength(1); j++)
                            {

                                if (nodeBoard[i, j] == myTurn)
                                {
                                    myTiles++;
                                    d += V[i, j];
                                }

                                else if (nodeBoard[i, j] == oppTurn)
                                {
                                    oppTiles++;
                                    d -= V[i, j];
                                }


                            }
                        }

                        if (myTiles > oppTiles)
                            q = (100 * myTiles) / (myTiles + oppTiles);
                        else if (myTiles < oppTiles)
                            q = -(100.0 * oppTiles) / (myTiles + oppTiles);
                        else
                            q = 0;


                        //corner
                        myTiles = 0;
                        oppTiles = 0;
                        double c = 0;


                        if (nodeBoard[0, 0] == myTurn)
                            myTiles++;
                        else if (nodeBoard[0, 0] == oppTurn)
                            oppTiles++;
                        if (nodeBoard[0, 8] == myTurn)
                            myTiles++;
                        else if (nodeBoard[0, 8] == oppTurn)
                            oppTiles++;
                        if (nodeBoard[6, 0] == myTurn)
                            myTiles++;
                        else if (nodeBoard[6, 0] == oppTurn)
                            oppTiles++;
                        if (nodeBoard[6, 8] == myTurn)
                            myTiles++;
                        else if (nodeBoard[6, 8] == oppTurn)
                            oppTiles++;

                        c = 25 * (myTiles - oppTiles);

                        //corner closeness
                        myTiles = 0;
                        oppTiles = 0;

                        if (nodeBoard[0, 0] == -1)
                        {
                            if (nodeBoard[0, 1] == myTurn)
                                myTiles++;
                            else if (nodeBoard[0, 1] == oppTurn)
                                oppTiles++;
                            if (nodeBoard[1, 1] == myTurn)
                                myTiles++;
                            else if (nodeBoard[1, 1] == oppTurn)
                                oppTiles++;
                            if (nodeBoard[1, 0] == myTurn)
                                myTiles++;
                            else if (nodeBoard[1, 0] == oppTurn)
                                oppTiles++;

                        }
                        if (nodeBoard[0, 8] == -1)
                        {
                            if (nodeBoard[0, 7] == myTurn)
                                myTiles++;
                            else if (nodeBoard[0, 7] == oppTurn)
                                oppTiles++;
                            if (nodeBoard[1, 7] == myTurn)
                                myTiles++;
                            else if (nodeBoard[1, 7] == oppTurn)
                                oppTiles++;
                            if (nodeBoard[1, 8] == myTurn)
                                myTiles++;
                            else if (nodeBoard[1, 8] == oppTurn)
                                oppTiles++;

                        }
                        if (nodeBoard[6, 0] == -1)
                        {
                            if (nodeBoard[6, 1] == myTurn)
                                myTiles++;
                            else if (nodeBoard[6, 1] == oppTurn)
                                oppTiles++;
                            if (nodeBoard[5, 1] == myTurn)
                                myTiles++;
                            else if (nodeBoard[5, 1] == oppTurn)
                                oppTiles++;
                            if (nodeBoard[5, 0] == myTurn)
                                myTiles++;
                            else if (nodeBoard[5, 0] == oppTurn)
                                oppTiles++;

                        }
                        if (nodeBoard[6, 8] == -1)
                        {
                            if (nodeBoard[5, 7] == myTurn)
                                myTiles++;
                            else if (nodeBoard[5, 7] == oppTurn)
                                oppTiles++;
                            if (nodeBoard[6, 7] == myTurn)
                                myTiles++;
                            else if (nodeBoard[6, 7] == oppTurn)
                                oppTiles++;
                            if (nodeBoard[5, 8] == myTurn)
                                myTiles++;
                            else if (nodeBoard[5, 8] == oppTurn)
                                oppTiles++;

                        }
                        double l = -12.5 * (myTiles - oppTiles);

                        if (myTurn == 0)
                        {
                            myTiles = p.PossibleMoves(true).Count;
                            oppTiles = p.PossibleMoves(false).Count;

                        }
                        else
                        {
                            myTiles = p.PossibleMoves(false).Count;
                            oppTiles = p.PossibleMoves(true).Count;
                        }

                        double m = 0;
                        if (myTiles > oppTiles)
                            myTiles = (100 * myTiles) / (myTiles + oppTiles);
                        else if (myTiles < oppTiles)
                            m = -(100 * oppTiles) / (myTiles + oppTiles);
                        else m = 0;
                                                

                        return (10 * q) + (801.724 * c) + (382.026 * l) + (78.922 * m) + (10 * d);
                }
                public List<Point> GetOps()
                {
                    return p.PossibleMoves(p.WhiteTurn);
                }
                public bool Final()
                {
                    if (p.PossibleMoves(true).Count == 0 && p.PossibleMoves(false).Count == 0)
                        return true;
                    else
                        return false;
                }
                public TreeNode Apply(Point op)
                {
                    int player;
                    int other;
                    if (p.WhiteTurn)
                    {
                        player = 0;
                        other = 1;
                    }
                    else
                    {
                        player = 1;
                        other = 0;
                    }
                    //int player = p.WhiteTurn ? 1 : 0;
                    //int other = p.WhiteTurn ? 0 : 1;
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
            public void Alphabeta(TreeNode root, int depth, int minOrMax, double parentValue, out double optVal, out Point? optOp)
            {
                if (depth == 0 || root.Final())
                {
                    optVal = root.Eval();
                    optOp = null;
                    return;
                }
                optVal = minOrMax * -Int32.MaxValue;
                optOp = null;
                foreach (Point op in root.GetOps())
                {
                    TreeNode _new = root.Apply(op);
                    Alphabeta(_new, depth - 1, -minOrMax, optVal, out double val, out Point? dummy);
                    if (val * minOrMax > optVal * minOrMax)
                    {
                        optVal = val;
                        optOp = op;
                        if (optVal * minOrMax > parentValue * minOrMax)
                        {
                            break;
                        }                       
                    }
                }
                return;
            }


        }
    }

}
