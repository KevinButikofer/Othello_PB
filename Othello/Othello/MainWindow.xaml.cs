using System;
using System.Collections.Generic;
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

namespace Othello
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Playable board;

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
                if(board.IsPlayable(Grid.GetColumn(lbl), Grid.GetRow(lbl), board.whiteTurn))
                {
                    replaceImage(Grid.GetColumn(lbl), Grid.GetRow(lbl), turn);
                    board.whiteTurn = !board.whiteTurn;
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


    }
}
