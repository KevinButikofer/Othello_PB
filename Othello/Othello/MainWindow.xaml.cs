using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
                    Console.WriteLine(board.WhiteTurn);
                    playGifAnim();
                    hidePossibleMoves();
                    replaceImage(Grid.GetColumn(lbl), Grid.GetRow(lbl), turn);
                    board.PlayMove(Grid.GetRow(lbl), Grid.GetColumn(lbl), board.WhiteTurn);
                    board.WhiteTurn = !board.WhiteTurn;
                    if (board.stopwatchP1.IsRunning)
                    {
                        board.stopwatchP1.Stop();
                        board.stopwatchP2.Start();
                        lblTurnInfo.Content = "Black Player Turn";
                    }
                    else if(board.stopwatchP2.IsRunning)
                    {
                        board.stopwatchP2.Stop();
                        board.stopwatchP1.Start();
                        lblTurnInfo.Content = "White Player Turn";
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
            showPossibleMoves();
        }

        public void playGifAnim()
        {
            if(board.WhiteTurn)
            {
                player2Gif.Source = new Uri("Resources/player2Anim.gif", UriKind.Relative);
                timerAttackAnim.Interval = TimeSpan.FromMilliseconds(2000);
                timerAttackAnim.Start();
                timerAttackAnim.Tick += (o, args) =>
                {
                    timerAttackAnim.Stop();
                    player2Gif.Source = new Uri("Resources/player2.gif", UriKind.Relative);

                };
            }
            else
            {
                player1Gif.Source = new Uri("Resources/player1Anim.gif", UriKind.Relative);
                timerAttackAnim.Interval = TimeSpan.FromMilliseconds(1250);
                timerAttackAnim.Start();
                timerAttackAnim.Tick += (o, args) =>
                {
                    timerAttackAnim.Stop();
                    player1Gif.Source = new Uri("Resources/player1.gif", UriKind.Relative);
                };
            }
        }
        
        public MainWindow():this(9, 7)
        {     
           
        }

        public MainWindow(int gridWidth, int gridHeight)
        {            

            InitializeComponent();
            
            timerAttackAnim = new DispatcherTimer();
            dispatcherTimeToWait.Tick += new EventHandler(dispatcherTimeToWait_tick);
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

            replaceImage(3, 3, 1);
            replaceImage(4, 4, 1);
            replaceImage(3, 4, 0);
            replaceImage(4, 3, 0);

            board = new Playable(false, false, this);

            this.DataContext = board;
        }

        public void replaceImage(int column, int row, int player)
        {
            Console.WriteLine("replace");
            try
            {
                //https://stackoverflow.com/questions/14185102/how-can-i-get-the-content-of-a-cell-in-a-grid-in-c
                Label lbl = playGrid.Children.Cast<Label>().FirstOrDefault(e => Grid.GetColumn(e) == column && Grid.GetRow(e) == row);
                BitmapImage image;
                if (player == 1)
                {
                    image = new BitmapImage(new Uri(@"pack://application:,,,/Othello;component/Resources/blackPawn.png", UriKind.Absolute));
                }
                else
                {
                    image = new BitmapImage(new Uri(@"pack://application:,,,/Othello;component/Resources/whitePawn.png", UriKind.Absolute));
                }

                Brush tBrush = new ImageBrush(image);
                lbl.Background = tBrush;
            }
            catch
            {

            }
            
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
                                replaceImage(i, j, boardGame[i, j] == 1 ? 0 : 1);
                            else
                                this.playGrid.Children.Cast<Label>().FirstOrDefault(x => Grid.GetColumn(x) == j && Grid.GetRow(x) == i).Background = new ImageBrush();
                        }
                    }
                    //needed otherwise the timer count doesn't work
                    board.dispatcherTimeRemaining.Stop();
                    board = newBoard;
                    if (board.WhiteTurn)
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

        private void player1Gif_MediaEnded(object sender, RoutedEventArgs e)
        {                   
            player1Gif.Position = new TimeSpan(0, 0, 1);
            player1Gif.Play();
        }

        

        private void player2Gif_MediaEnded(object sender, RoutedEventArgs e)
        {
            player2Gif.Position = new TimeSpan(0, 0, 1);
            player2Gif.Play();
        }

        private void viewBoxPlayGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            showPossibleMoves();
        }

        public void showPossibleMoves()
        {
            List<Point>  listPossible = board.PossibleMoves(board.WhiteTurn);
            Color c = Color.FromArgb(170, 0, 0, 0);

            foreach (Point p in listPossible)
            {
                changeCellColor(p, c);
            }
        }

        public void hidePossibleMoves()
        {
            List<Point>  listPossible = board.PossibleMoves(board.WhiteTurn);
            Color c = Color.FromArgb(0, 0, 0, 0);
            foreach (Point p in listPossible)
            {
                changeCellColor(p, c);
            }
        }

        public void changeCellColor(Point p, Color color)
        {
            Label lbl = playGrid.Children.Cast<Label>().FirstOrDefault(e => Grid.GetColumn(e) == p.Y && Grid.GetRow(e) == p.X);
            lbl.Background = new SolidColorBrush(color);
        }

        private void viewBoxPlayGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            hidePossibleMoves();
        }
        private void dispatcherTimeToWait_tick(object sender, EventArgs e)
        {
            int turn = board.WhiteTurn ? 1 : 2;
            lblTurnInfo.Content = $"{(board.WhiteTurn ? "White" : "Black")} Player{turn} Turn";
            if(turn == 1)
            {
                board.stopwatchP1.Start();
            }
            else
            {
                board.stopwatchP2.Start();
            }
            board.WhiteTurn = !board.WhiteTurn;
        }
    }
}
