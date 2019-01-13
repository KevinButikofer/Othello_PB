using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Othello
{
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    [Serializable]
    class Playable : IPlayable.IPlayable
    {
        private int[,] board = new int[7, 9];
        private bool player0IsAI;
        private bool player1IsAI;
        public bool canPlay = false;

        public bool whiteTurn { get; set; }

        public Playable(bool _player0IsAI, bool _player1IsAI)
        {
            MainWindow wnd = (MainWindow)App.Current.MainWindow;
            wnd.replaceImage(0, 0, 1);
            whiteTurn = false;
            player0IsAI = _player0IsAI;
            player1IsAI = _player1IsAI;

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


            Console.WriteLine("Moves : ");
            Console.WriteLine("White possible moves : " + possibleMoves(true).Count);
            Console.WriteLine("Black possible moves : " + possibleMoves(false).Count);
        }

        public int GetBlackScore()
        {
            return GetScore(1);
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
                if(whiteTurn && player0IsAI || !whiteTurn && player1IsAI)
                {
                    //AI CODE
                }
                else
                {
                    showPossibleMoves(possibleMoves(whiteTurn));
                    
                    canPlay = true;                    
                }
            }
            while (!IsPlayable(column, row, whiteTurn));
            return new Tuple<int, int>(column, row);
        }
        public int GetWhiteScore()
        {
           return GetScore(0);
        }

        public bool IsPlayable(int row, int column, bool isWhite)
        {
            int player = isWhite ? 0 : 1;
            int other = isWhite ? 1 : 0;

            for(int i = -1; i <= 1; i++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    if (i != 0 || j != 0)
                    {
                        if (checkDirection(row + i, column + j, player, other, i, j))
                            return true;
                    }
                }
            }
            #region
            //if (board[column, line] != -1 || column >= board.GetLength(0) || line >= board.GetLength(1))
            //     return false;
            // else
            // {
            //     //Check down
            //     int currentLine = line + 1;
            //     if (currentLine < board.GetLength(1) - 1)
            //     {
            //         if (board[currentLine, column] == other)
            //         {
            //             while (currentLine < board.GetLength(1) - 1)
            //             {
            //                 if (board[currentLine, column] == other)
            //                 {
            //                     if (board[currentLine + 1, column] == player)
            //                         return true;
            //                 }
            //                 else
            //                     break;
            //                 currentLine--;
            //             }
            //         }
            //     }

            //     //Check up
            //     currentLine = line - 1;
            //     if (currentLine > 0)
            //     {
            //         if (board[currentLine, column] == other)
            //         {
            //             while (currentLine > 1)
            //             {
            //                 if (board[currentLine, column] == other)
            //                 {
            //                     if (board[currentLine - 1, column] == player)
            //                         return true;
            //                 }
            //                 else
            //                     break;
            //                 currentLine--;
            //             }
            //         }
            //     }

            //     //Check right
            //     int currentColumn = column + 1;
            //     if (currentColumn < board.GetLength(0) - 1)
            //     {
            //         if (board[line, currentColumn] == other)
            //         {
            //             while (currentColumn < board.GetLength(0) - 1)
            //             {
            //                 if (board[line, currentColumn] == other)
            //                 {
            //                     if (board[line + 1, currentColumn] == player)
            //                         return true;
            //                 }
            //                 else
            //                     break;
            //                 currentColumn--;
            //             }
            //         }
            //     }

            //     //Check left
            //     currentColumn = column - 1;
            //     if (currentColumn > 1)
            //     {
            //         if (board[currentColumn, line] == other)
            //         {
            //             while (currentColumn > 1)
            //             {
            //                 if (board[line, currentColumn] == other)
            //                 {
            //                     if (board[line - 1, currentColumn] == player)
            //                         return true;
            //                 }
            //                 else
            //                     break;
            //                 currentColumn++;
            //             }
            //         }
            //     }

            //     currentLine = line + 1;
            //     currentColumn = column + 1;
            //     //check diagonnal
            //  }
            #endregion
            return false;
        }
        private void showPossibleMoves(List<Point> possiblesMoves)
        {
            foreach(Point P in possiblesMoves)
            {
                //Color case on board
            }
        }
        public List<Point> possibleMoves(bool isWhite)
        {
            List<Point> possiblesMoves = new List<Point>();
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for(int j = 0; j < board.GetLength(1); j++)
                {
                    if(IsPlayable(i, j, isWhite))
                    {
                        possiblesMoves.Add(new Point { X = i, Y = j });
                    }
                }
            }
            return possiblesMoves;
        }
        private bool checkDirection(int i, int j, int player, int other, int incI = 0, int incJ = 0)
        {
            if(boundsCheck(i, j))
            {
                if (board[i, j] == other)
                {
                    while (boundsCheck(i, j))
                    {
                        if (board[i, j] == other)
                        {
                            if (boundsCheck(i + incI, j + incJ) && board[i + incI, j + incJ] == player)
                                return true;
                        }
                        else
                        {
                            return false;
                        }
                        i += incI;
                        j += incJ;
                    }
                }
            }
            return false;
        }

        private bool boundsCheck(int i, int j)
        {
            return (-1 < i && i < 7 && -1 < j && j < 9);
        }

        public bool PlayMove(int column, int row, bool isWhite)
        {     
            if(IsPlayable(row, column, isWhite))
            {
                if (isWhite)
                    board[row, column] = 0;
                else
                    board[row, column] = 1;
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
       
    }
}
