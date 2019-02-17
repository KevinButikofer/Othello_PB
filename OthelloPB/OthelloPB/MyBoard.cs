using System;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;


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
        class MyBoard : IPlayable.IPlayable
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
                    }
                }
            }

            public bool WhiteTurn
            { get; set; }

            public MyBoard()
            {
                WhiteTurn = false;

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

                return "Pervert boy humiliation";
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
                TreeNode node = new TreeNode(this);
                Alphabeta(node, 5, 1, 0, out double optVal, out Point? optOp);
                if (optOp.HasValue)
                {
                    return new Tuple<int, int>(optOp.Value.X, optOp.Value.Y);
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
                            possiblesMoves.Add(new Point { X = i, Y = j});
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
                            {
                                return Cases;
                            }
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
            /// Return if white player has the best score
            /// </summary>
            /// <returns>if white has win</returns>
            public bool IsWhiteWinner()
            {
                return GetBlackScore() > GetWhiteScore() ? false : true;
            }
            public class TreeNode
            {
                int[,] nodeBoard = new int[9, 7];
                MyBoard p;
                public TreeNode(MyBoard playable, int[,] board = null)
                {
                    p = playable;
                    if (board != null)
                        Array.Copy(board, nodeBoard, board.Length);
                    else
                        Array.Copy(p.GetBoard(), nodeBoard, p.GetBoard().Length);
                }


                /// <summary>
                /// Compute value of current state
                /// Computed using these sources:
                /// https://stackoverflow.com/questions/13314288/need-heuristic-function-for-reversiothello-ideas
                /// https://kartikkukreja.wordpress.com/2013/03/30/heuristic-function-for-reversiothello/
                /// https://github.com/kartikkukreja/blog-codes
                /// http://home.datacomm.ch/t_wolf/tw/misc/reversi/html/index.html
                /// 
                /// </summary>
                /// <returns>Value of the state</returns>
                public double Eval()
                {

                    int value = 0;
                    int myTiles = 0;
                    int oppTiles = 0;

                    int myTurn;
                    int oppTurn;



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

                    //check each position and increase or decrease value based on position color

                    for (int i = 0; i < nodeBoard.GetLength(0); i++)
                    {
                        for (int j = 0; j < nodeBoard.GetLength(1); j++)
                        {

                            if (nodeBoard[i, j] == myTurn)
                            {
                                value += 50;
                            }

                            else if (nodeBoard[i, j] == oppTurn)
                            {
                                value -= 50;
                            }

                        }
                    }



                    //check corners.
                    // if oponent get them it's really bad. If it's us it's really good
                    myTiles = 0;
                    oppTiles = 0;

                    if (nodeBoard[0, 0] == myTurn)
                    {

                        value += 1000;
                    }
                        
                    else if (nodeBoard[0, 0] == oppTurn)
                        value -= 1000;
                    if (nodeBoard[0, 6] == myTurn)
                        value += 1000;
                    else if (nodeBoard[0, 6] == oppTurn)
                        value -= 1000;
                    if (nodeBoard[8, 0] == myTurn)
                        value += 1000;
                    else if (nodeBoard[8, 0] == oppTurn)
                        value -= 1000;
                    if (nodeBoard[8, 6] == myTurn)
                        value += 1000;
                    else if (nodeBoard[8, 6] == oppTurn)
                        value -= 1000;


                    //corner closeness
                    //If a corner is empty and we play next to it, it is really bad as oponent will maybe be able to take it
                    myTiles = 0;
                    oppTiles = 0;

                    if (nodeBoard[0, 0] == -1)
                    {

                        if (nodeBoard[0, 1] == myTurn)
                            value -= 80;
                        else if (nodeBoard[0, 1] == oppTurn)
                            value += 80;
                        if (nodeBoard[1, 1] == myTurn)
                            value -= 80;
                        else if (nodeBoard[1, 1] == oppTurn)
                            value += 80;
                        if (nodeBoard[1, 0] == myTurn)
                            value -= 80;
                        else if (nodeBoard[1, 0] == oppTurn)
                            value += 80;

                    }
                    if (nodeBoard[8, 0] == -1)
                    {
                        if (nodeBoard[7, 0] == myTurn)
                            value -= 80;
                        else if (nodeBoard[7, 0] == oppTurn)
                            value += 80;
                        if (nodeBoard[7, 1] == myTurn)
                            value -= 80;
                        else if (nodeBoard[7, 1] == oppTurn)
                            value += 80;
                        if (nodeBoard[7, 1] == myTurn)
                            value -= 80;
                        else if (nodeBoard[7, 1] == oppTurn)
                            value += 80;

                    }
                    if (nodeBoard[0, 6] == -1)
                    {
                        if (nodeBoard[1, 6] == myTurn)
                            value -= 80;
                        else if (nodeBoard[1, 6] == oppTurn)
                            value += 80;
                        if (nodeBoard[1, 5] == myTurn)
                            value -= 80;
                        else if (nodeBoard[1, 5] == oppTurn)
                            value += 80;
                        if (nodeBoard[0, 5] == myTurn)
                            value -= 80;
                        else if (nodeBoard[0, 5] == oppTurn)
                            value += 80;

                    }
                    if (nodeBoard[8, 6] == -1)
                    {
                        if (nodeBoard[7, 5] == myTurn)
                            value -= 80;
                        else if (nodeBoard[7, 5] == oppTurn)
                            value += 80;
                        if (nodeBoard[7, 6] == myTurn)
                            value -= 80;
                        else if (nodeBoard[7, 6] == oppTurn)
                            value += 80;
                        if (nodeBoard[8, 5] == myTurn)
                            value -= 80;
                        else if (nodeBoard[8, 5] == oppTurn)
                            value += 80;

                    }



                    //mobility
                    //how many possible moves from this state for current player vs oponent
                    if (myTurn == 0)
                    {
                        myTiles = p.PossibleMoves(true).Count;
                        oppTiles = p.PossibleMoves(false).Count;
                        value += (myTiles - oppTiles) * 40;

                    }
                    else
                    {
                        myTiles = p.PossibleMoves(false).Count;
                        oppTiles = p.PossibleMoves(true).Count;
                        value += (myTiles - oppTiles) * 40;

                    }

                    return value;
                }

                /// <summary>
                /// Return applicable operators
                /// </summary>
                /// <returns>List of possible moves</returns>
                public List<Point> GetOps()
                {
                    return p.PossibleMoves(p.WhiteTurn);
                }


                /// <summary>
                /// Check if game is over
                /// </summary>
                /// <returns>true if game is over, else false </returns>
                public bool Final()
                {
                    if (p.PossibleMoves(true).Count == 0 && p.PossibleMoves(false).Count == 0)
                        return true;
                    else
                        return false;
                }

                /// <summary>
                /// Apply the operator on this state
                /// </summary>
                /// <param name="op">Point representing board placement</param>
                /// <returns>The resulting TreeNode after operator</returns>
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

            /// <summary>
            /// Return the best move possible
            /// </summary>
            /// <param name="root">Tree's root</param>
            /// <param name="depth">Depth of algorithm</param>
            /// <param name="minOrMax">Value for min or max(1 for max, -1 for min)</param>
            /// <param name="parentValue">Value of the parent</param>
            /// <param name="optVal">Optimal value of score</param>
            /// <param name="optOp">Optimal move to play</param>
            public void Alphabeta(TreeNode root, int depth, int minOrMax, double parentValue, out double optVal, out Point? optOp)
            {
                if (depth == 0 || root.Final())
                {
                    optVal = root.Eval();
                    optOp = null;
                    return;
                }
                optVal = minOrMax * -Double.MaxValue;
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
