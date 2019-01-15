using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Diagnostics;

namespace Othello
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timerAttackAnim;
        Playable board;       
        DispatcherTimer dispatcherTimeToWait = new DispatcherTimer();

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int turn = board.WhiteTurn ? 1 : 0;
            try
            {
                Label lbl = e.Source as Label;
                if(board.IsPlayable(Grid.GetRow(lbl), Grid.GetColumn(lbl), board.WhiteTurn))
                {
                    PlayGifAnim();
                    HidePossibleMoves();

                    ReplaceImage(Grid.GetColumn(lbl), Grid.GetRow(lbl), turn, true);
                    board.PlayMove(Grid.GetRow(lbl), Grid.GetColumn(lbl), board.WhiteTurn);
                    board.WhiteTurn = !board.WhiteTurn;

                    if (board.stopwatchP1.IsRunning)
                    {
                        board.stopwatchP1.Stop();
                        board.stopwatchP2.Start();
                        lblTurnInfo.Content = "White Player Turn";
                    }
                    else if(board.stopwatchP2.IsRunning)
                    {
                        board.stopwatchP2.Stop();
                        board.stopwatchP1.Start();
                        lblTurnInfo.Content = "Black Player Turn";
                    }
                    if (board.PossibleMoves(board.WhiteTurn).Count == 0)
                    {
                        board.stopwatchP1.Stop();
                        board.stopwatchP2.Stop();
                        if (board.PossibleMoves(!board.WhiteTurn).Count == 0)
                        {                            
                            bool isWhiteWinner = board.IsWhiteWinner();
                            lblTurnInfo.Content = $"End of the game \n{(isWhiteWinner? "White" : "Black")} Player has win";
                        }
                        else
                        {
                            lblTurnInfo.Content = $"{(board.WhiteTurn ? "White" : "Black")} Player can't play";
                            dispatcherTimeToWait.Start();
                        }
                    }
                }
            }
            catch
            { };
            if(board.WhiteTurn && board.PlayerWhiteIsAI || !board.WhiteTurn && board.PlayerBlackIsAI)
                board.GetNextMove(board.GetBoard(), 0, board.WhiteTurn);
            else
                ShowPossibleMoves();
        }

        public void PlayGifAnim()
        {
            if(board.WhiteTurn)
            {
                timerAttackAnim.Interval = TimeSpan.FromMilliseconds(2000);
                timerAttackAnim.Start();
                player2Gif.Source = new Uri("Resources/player2Anim.gif", UriKind.Relative);

                timerAttackAnim.Tick += (o, args) =>
                {
                    player2Gif.Source = new Uri("Resources/player2.gif", UriKind.Relative);
                    timerAttackAnim.Stop();
                };
            }
            else
            {
                timerAttackAnim.Interval = TimeSpan.FromMilliseconds(1300);
                timerAttackAnim.Start();
                player1Gif.Source = new Uri("Resources/player1Anim.gif", UriKind.Relative);

                timerAttackAnim.Tick += (o, args) =>
                {
                    player1Gif.Source = new Uri("Resources/player1.gif", UriKind.Relative);
                    timerAttackAnim.Stop();
                };
            }
        }
        
        public MainWindow():this(9, 7){}

        public MainWindow(int gridWidth, int gridHeight)
        {
            Menu menu = new Menu();
            bool result = (bool)menu.ShowDialog();

            InitializeComponent();
            
            timerAttackAnim = new DispatcherTimer();
            dispatcherTimeToWait.Tick += new EventHandler(DispatcherTimeToWait_tick);
            dispatcherTimeToWait.Interval = new TimeSpan(0, 0, 0, 2);
            
            Image imagePlayer1 = new Image();
            Image imagePlayer2 = new Image();

            windowGrid.Children.Add(imagePlayer1);
            windowGrid.Children.Add(imagePlayer2);


            Grid.SetColumn(imagePlayer1, 0);
            Grid.SetRow(imagePlayer1, 0);

            playGrid.Columns = gridWidth;
            playGrid.Rows = gridHeight;


            //fill play grid with labels in which we will add an image when a user play.
            for (int i = 0; i < gridHeight; i++)
            {
                for (int j = 0; j < gridWidth; j++)
                {
                    Label lbl = new Label();
                    
                    lbl.BorderBrush = Brushes.Black;
                    lbl.BorderThickness = new Thickness(0.2);
                    
                    playGrid.Children.Add(lbl);          
                    
                    Grid.SetColumn(lbl, j);
                    Grid.SetRow(lbl, i);
                }
            }

            ReplaceImage(3, 3, 1, false);
            ReplaceImage(4, 4, 1, false);
            ReplaceImage(3, 4, 0, false);
            ReplaceImage(4, 3, 0, false);

            if (result)
            {
                PartyType p = menu._PartyType;
                switch (p)
                {
                    case PartyType.PvP:
                        board = new Playable(false, false, this);
                        break;
                    case PartyType.AivP:
                        board = new Playable(true, false, this);
                        break;
                    case PartyType.AivAI:
                        board = new Playable(true, true, this);
                        break;
                    case PartyType.ResumeOld:
                        board = null;
                        LoadItem_Click(null, null);
                        break;
                }
                Width = menu.Width;
                Height = menu.Height;
            }
            else
            {
                Close();
            }

            this.DataContext = board;
        }

        public void ReplaceImage(int column, int row, int player, bool fade)
        {
            try
            {
                //https://stackoverflow.com/questions/14185102/how-can-i-get-the-content-of-a-cell-in-a-grid-in-c
                Label lbl = playGrid.Children.Cast<Label>().FirstOrDefault(e => Grid.GetColumn(e) == column && Grid.GetRow(e) == row);

                BitmapImage image;
                if (player == 1)
                {
                    whiteTurnLabel.Visibility = Visibility.Hidden;
                    blackTurnLabel.Visibility = Visibility.Visible;
                    image = new BitmapImage(new Uri(@"pack://application:,,,/Othello;component/Resources/blackPawn.png", UriKind.Absolute));

                    if (fade)
                    {
                        ReplaceImageWithFade(lbl, image);
                    }
                    else
                    {
                        Brush tBrush = new ImageBrush(image);
                        lbl.Background = tBrush;
                    }
                }
                else
                {
                    whiteTurnLabel.Visibility = Visibility.Visible;
                    blackTurnLabel.Visibility = Visibility.Hidden;
                    image = new BitmapImage(new Uri(@"pack://application:,,,/Othello;component/Resources/whitePawn.png", UriKind.Absolute));

                    if (fade)
                    {
                        ReplaceImageWithFade(lbl, image);
                    }
                    else
                    {
                        Brush tBrush = new ImageBrush(image);
                        lbl.Background = tBrush;
                    }
                }
            }
            catch
            { }
        }

        public void ReplaceImageWithFade(Label lbl, ImageSource img)
        {
            DispatcherTimer timerFlip = new DispatcherTimer();
            timerFlip.Interval = TimeSpan.FromMilliseconds(10);
            timerFlip.Start();
            bool isShrinking = true;

            timerFlip.Tick += (o, args) =>
            {
                if (isShrinking)
                    lbl.Opacity -= 0.1;
                else
                    lbl.Opacity += 0.1;

                if (lbl.Opacity <= 0)
                {
                    Brush tBrush = new ImageBrush(img);
                    lbl.Background = tBrush;
                    isShrinking = false;
                }
                if (lbl.Opacity >= 1)
                {
                    timerFlip.Stop();
                    timerFlip = null;
                }
            };
        }      

        private void LoadItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Othello save file (*.oth)|*.oth";
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (fileDialog.ShowDialog() == true)
            {
                Playable newBoard = ReadFromBinaryFile<Playable>(fileDialog.FileName);
                if (newBoard != null)
                {
                    newBoard.InitDispatcher();
                    newBoard.MainWindow = this;
                    
                    this.DataContext = newBoard;
                    newBoard.BlackScore = newBoard.GetBlackScore();
                    newBoard.WhiteScore = newBoard.GetWhiteScore();
                    newBoard.stopwatchP1 = new Stopwatch();
                    newBoard.stopwatchP2 = new Stopwatch();
                    int[,] boardGame = newBoard.GetBoard();
                    for (int i = 0; i < boardGame.GetLength(0); i++)
                    {
                        for (int j = 0; j < boardGame.GetLength(1); j++)
                        {
                            if (boardGame[i, j] != -1)
                                ReplaceImage(j, i, boardGame[i, j] == 1 ? 0 : 1, false);
                            else
                                this.playGrid.Children.Cast<Label>().FirstOrDefault(x => Grid.GetColumn(x) == j && Grid.GetRow(x) == i).Background = new ImageBrush();
                        }
                    }
                    //needed otherwise the timer count doesn't work
                    board.dispatcherTimeRemaining.Stop();
                    board = null;
                    board = newBoard;
                    if (!board.WhiteTurn)
                    {
                        lblTurnInfo.Content = "White Player Turn";
                        board.stopwatchP1.Start();
                    }
                    else
                    {
                        lblTurnInfo.Content = "Black Player Turn";
                        board.stopwatchP2.Start();
                    }
                }
                else
                {
                    MessageBox.Show("file loading failed");
                }
            }            
        }

        private void NewItem_Click(object sender, RoutedEventArgs e)
        {
            //needed otherwise the timer count doesn't work
            board.dispatcherTimeRemaining.Stop();
            board = new Playable(board.PlayerWhiteIsAI, board.PlayerBlackIsAI, this);
        }

        private void SaveItem_Click(object sender, RoutedEventArgs e)
        {
            if (board.stopwatchP1.IsRunning)
                board.stopwatchP1.Stop();
            else
                board.stopwatchP2.Stop();

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "Othello save file (*.oth)|*.oth";
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (fileDialog.ShowDialog() == true)
            {
                board.SavePlayerTime();
                WriteToBinaryFile<Playable>(fileDialog.FileName, board);
                if (board.WhiteTurn)
                    board.stopwatchP1.Start();
                else
                    board.stopwatchP2.Start();
            }
            if (board.WhiteTurn)
                board.stopwatchP1.Start();
            else
                board.stopwatchP2.Start();
        }

        private void Player1Gif_MediaEnded(object sender, RoutedEventArgs e)
        {                   
            player1Gif.Position = new TimeSpan(0, 0, 1);
            player1Gif.Play();
        }

        private void Player2Gif_MediaEnded(object sender, RoutedEventArgs e)
        {
            player2Gif.Position = new TimeSpan(0, 0, 1);
            player2Gif.Play();
        }

        private void ViewBoxPlayGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowPossibleMoves();
        }

        private void ShowPossibleMoves()
        {
            List<Point>  listPossible = board.PossibleMoves(board.WhiteTurn);
            Color c = Color.FromArgb(170, 255, 255, 255);

            foreach (Point p in listPossible)
                ChangeCellColor(p, c);
        }

        private void HidePossibleMoves()
        {
            List<Point>  listPossible = board.PossibleMoves(board.WhiteTurn);
            Color c = Color.FromArgb(0, 255, 255, 255);

            foreach (Point p in listPossible)
                ChangeCellColor(p, c);
        }

        private void ChangeCellColor(Point p, Color color)
        {
            Label lbl = playGrid.Children.Cast<Label>().FirstOrDefault(e => Grid.GetColumn(e) == p.Y && Grid.GetRow(e) == p.X);
            lbl.Background = new SolidColorBrush(color);
        }

        private void ViewBoxPlayGrid_MouseLeave(object sender, MouseEventArgs e) => HidePossibleMoves();
        private void DispatcherTimeToWait_tick(object sender, EventArgs e)
        {
            int turn = board.WhiteTurn ? 1 : 2;
            lblTurnInfo.Content = $"{(board.WhiteTurn ? "White" : "Black")} Player{turn} Turn";

            if(turn == 1)
                board.stopwatchP1.Start();
            else
                board.stopwatchP2.Start();

            board.WhiteTurn = !board.WhiteTurn;
        }
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }
        public static T ReadFromBinaryFile<T>(string filePath)
        {
            try
            {
                using (Stream stream = File.Open(filePath, FileMode.Open))
                {

                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    return (T)binaryFormatter.Deserialize(stream);
                }
            }
            catch
            {
                return default(T);
            }
        }
    }
}
