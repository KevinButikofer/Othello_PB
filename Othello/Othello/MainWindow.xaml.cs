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


namespace Othello
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Playable board;
        DispatcherTimer dispatcherTimeToWait = new DispatcherTimer();

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("click");
            //board.cancellationToken.Cancel();
            //board.eventWait.Set();
            int turn = board.whiteTurn ? 1 : 0;
            try
            {
                Label lbl = e.Source as Label;
                
                if(board.IsPlayable(Grid.GetRow(lbl), Grid.GetColumn(lbl), board.whiteTurn))
                {
                    Console.WriteLine(lbl);
                    replaceImage(Grid.GetColumn(lbl), Grid.GetRow(lbl), turn);
                    board.PlayMove(Grid.GetColumn(lbl), Grid.GetRow(lbl), board.whiteTurn);
                    board.whiteTurn = !board.whiteTurn;
                    if (board.stopwatchP1.IsRunning)
                    {
                        board.stopwatchP1.Stop();
                        board.stopwatchP2.Start();
                        lblTurnInfo.Content = "Player 2 Turn";
                    }
                    else
                    {
                        board.stopwatchP2.Stop();
                        board.stopwatchP1.Start();
                        lblTurnInfo.Content = "Player 1 Turn";
                    }
                    if (board.possibleMoves(board.whiteTurn).Count == 0)
                    {
                        board.stopwatchP1.Stop();
                        board.stopwatchP2.Stop();
                        if (board.possibleMoves(!board.whiteTurn).Count == 0)
                        {
                            
                            int winner = board.getWinner();
                            lblTurnInfo.Content = $"End of the game \n Player {winner + 1} has win";
                        }
                        else
                        {
                            lblTurnInfo.Content = $"Player {(board.whiteTurn ? 1 : 2)} can't play";
                            dispatcherTimeToWait.Start();
                        }
                    }
                }
            }
            catch { };

            if (board.canPlay)
            {
                MessageBox.Show("click");
            }
        }
        
        public MainWindow():this(9, 7)
        {
            
           
        }

        public MainWindow(int gridWidth, int gridHeight)
        {            

            InitializeComponent();

            dispatcherTimeToWait.Tick += new EventHandler(dispatcherTimeToWait_tick);
            dispatcherTimeToWait.Interval = new TimeSpan(0, 0, 0, 2);

            /*for(int i = 0; i<gridWidth; i++)
            {
                playGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < gridHeight; i++)
            {
                playGrid.RowDefinitions.Add(new RowDefinition());
            }*/



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
                    
                    //BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Othello;component/Resources/whitePawn.png", UriKind.Absolute));
                    //Brush tBrush = new ImageBrush(image);
                    //lbl.Background = tBrush;

                    
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
            //https://stackoverflow.com/questions/14185102/how-can-i-get-the-content-of-a-cell-in-a-grid-in-c
            Label lbl = playGrid.Children.Cast<Label>().FirstOrDefault(e => Grid.GetColumn(e) == column && Grid.GetRow(e) == row);
            BitmapImage image;
            if (player == 0)
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
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }

        }

        private void LoadItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Othello save file (*.oth)|*.oth";
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (fileDialog.ShowDialog() == true)
            {
                board = ReadFromBinaryFile<Playable>(fileDialog.FileName);
                board.initDispatcher();
                board.setMainWindow(this);
                int[,] boardGame = board.GetBoard();
                for(int i = 0; i < boardGame.GetLength(0); i++)
                {
                    for(int j = 0; j < boardGame.GetLength(1); j++)
                    {
                        if(boardGame[i,j] >= 0)
                            replaceImage(i, j, boardGame[i, j]);
                        else
                            this.playGrid.Children.Cast<Label>().FirstOrDefault(x => Grid.GetColumn(x) == j && Grid.GetRow(x) == i).Background = new ImageBrush();
                    }
                }

            }            
        }

        private void NewItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "Othello save file (*.oth)|*.oth";
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (fileDialog.ShowDialog() == true)
            {
                WriteToBinaryFile<Playable>(fileDialog.FileName, board);
            }
        }
        private void dispatcherTimeToWait_tick(object sender, EventArgs e)
        {
            int turn = board.whiteTurn ? 1 : 2;
            lblTurnInfo.Content = $"Player{turn} Turn";
            if(turn == 1)
            {
                board.stopwatchP1.Start();
            }
            else
            {
                board.stopwatchP2.Start();
            }
        }
    }
}
