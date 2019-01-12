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
        public MainWindow()
        {
            InitializeComponent();

            board = new Playable(false, false);
            
            
            board.GetNextMove(board.GetBoard(), 3, true);
        }        

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("click");
            //board.cancellationToken.Cancel();
            //board.eventWait.Set();
            if(board.canPlay)
            {
                MessageBox.Show("click");
            }
        }
    }
}
