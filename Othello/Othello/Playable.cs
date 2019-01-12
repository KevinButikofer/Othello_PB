using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace Othello
{
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    class Playable : IPlayable.IPlayable
    {
        private int[,] board = new int[7, 9];
        private bool player0IsAI;
        private bool player1IsAI;
        public bool canPlay = false;

        public CancellationTokenSource cancellationToken = new CancellationTokenSource();
        public EventWaitHandle eventWait = new EventWaitHandle(false, EventResetMode.AutoReset);
        public  ManualResetEvent signalEvent
        { get; set; }

        public bool whiteTurn { get; set; }

        public Playable(bool _player0IsAI, bool _player1IsAI)
        {
            whiteTurn = false;
            player0IsAI = _player0IsAI;
            player1IsAI = _player1IsAI;
            signalEvent = new ManualResetEvent(false);
            PlayMove(3, 4, true);
            PlayMove(4, 4, true);
            PlayMove(3, 5, false);
            PlayMove(4, 5, false);
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
        private async void tes()
        {
            //Wait for user Click;
            Task t = new Task(parra);
            await t;
            //This thread will block here until the reset event is sent.
            signalEvent.Reset();            
        }
        private async void parra()
        {
            eventWait.WaitOne();
        }
        private Task WaitForClick()
        {
            return Task.Delay(new TimeSpan(1000), cancellationToken.Token);
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            int column = 0, line = 0;
            do
            {
                if(whiteTurn && player0IsAI || !whiteTurn && player1IsAI)
                {
                    //AI CODE
                }
                else
                {
                    showPossibleMoves(possibleMoves(whiteTurn));
                    Console.WriteLine("test");
                    canPlay = true;
                    WaitForClick();

                    
                }
            }
            while (!IsPlayable(column, line, whiteTurn));
            return new Tuple<int, int>(column, line);
        }
        public int GetWhiteScore()
        {
           return GetScore(0);
        }

        public bool IsPlayable(int column, int row, bool isWhite)
        {
            int player = isWhite ? 0 : 1;
            int other = isWhite ? 1 : 0;

            for(int i = row -1; i <= row+1; i++)
            {
                for(int j = column -1; j <= column+1; j++)
                {
                    checkDirection(row, column, player, other, i, j);
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
        private List<Point> possibleMoves(bool isWhite)
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

        public bool PlayMove(int column, int line, bool isWhite)
        {            
            if (IsPlayable(column, line, isWhite))
            {
                if (isWhite)
                    board[line, column] = 0;
                else
                    board[line, column] = 1;
                return true;                
            }
            else
                return false;
        }
        public int GetScore(int x)
        {
            int count = 0;
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(0); j++)
                {
                    if (board[i, j] == x) count++;
                }
            }
            return count;
        }
    }
}
