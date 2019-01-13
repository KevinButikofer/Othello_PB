﻿using System;
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
using Microsoft.Win32;
using System.Windows.Threading;


namespace Othello
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timerAttackAnim;
        Playable board;
       
        List<Point> listPossible;


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("click");
            //board.cancellationToken.Cancel();
            //board.eventWait.Set();
            int turn = board.whiteTurn ? 1 : 0;
            try
            {

                Label lbl = e.Source as Label;
                Console.WriteLine(lbl);

                if (board.IsPlayable(Grid.GetColumn(lbl), Grid.GetRow(lbl), board.whiteTurn))
                {
                    playGifAnim();
                    hidePossibleMoves();

                    replaceImage(Grid.GetColumn(lbl), Grid.GetRow(lbl), turn);
                    board.whiteTurn = !board.whiteTurn;
                    showPossibleMoves();
                }



            }
            catch { };

            if (board.canPlay)
            {
                MessageBox.Show("click");
            }
        }

        public void playGifAnim()
        {
            if(board.whiteTurn)
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



            board = new Playable(false, false);           

            //board.GetNextMove(board.GetBoard(), 3, true);

        }

        public void replaceImage(int column, int row, int player)
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
                int[,] boardGame = board.GetBoard();
                for(int i = 0; i < boardGame.GetLength(0); i++)
                {
                    for(int j = 0; j < boardGame.GetLength(1); j++)
                    {
                        if(boardGame[i,j] > 0)
                            replaceImage(i, j, boardGame[i, j]);
                        else
                            playGrid.Children.Cast<Label>().FirstOrDefault(x => Grid.GetColumn(x) == i && Grid.GetRow(x) == j).Background = new ImageBrush();
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
            listPossible = board.possibleMoves(board.whiteTurn);
            Color c = Color.FromArgb(170, 0, 0, 0);

            foreach (Point p in listPossible)
            {
                changeCellColor(p, c);
            }
        }

        public void hidePossibleMoves()
        {
            listPossible = board.possibleMoves(board.whiteTurn);
            Color c = Color.FromArgb(0, 0, 0, 0);
            foreach (Point p in listPossible)
            {
                changeCellColor(p, c);
            }
        }

        public void changeCellColor(Point p, Color color)
        {
            Label lbl = playGrid.Children.Cast<Label>().FirstOrDefault(e => Grid.GetColumn(e) == p.X && Grid.GetRow(e) == p.Y);
            lbl.Background = new SolidColorBrush(color);

        }

        private void viewBoxPlayGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            hidePossibleMoves();
        }
    }
}
