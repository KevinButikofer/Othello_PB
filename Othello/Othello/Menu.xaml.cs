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
using System.Windows.Shapes;

namespace Othello
{
    public enum PartyType
    {
        AivAI,
        AivP,
        PvP,
        ResumeOld,
    };
    /// <summary>
    /// Logique d'interaction pour Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {        
        public PartyType _PartyType
        { get; set; }
        public int _Height
        { get; set;}
        public int _Width
        { get; set; }
        public Menu()
        {
            InitializeComponent();
        }
        private void TwoPlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            _PartyType = PartyType.PvP;
            CloseWindow();
        }

        private void OnePlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            _PartyType = PartyType.AivP;
            CloseWindow();
        }

        private void TwoAiBtn_Click(object sender, RoutedEventArgs e)
        {
            _PartyType = PartyType.AivAI;
            CloseWindow();
        }
        private void LoadSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            _PartyType = PartyType.ResumeOld;
            CloseWindow();
        }
        private void CloseWindow()
        {
            DialogResult = true;
            Height = ActualHeight;
            Width = ActualWidth;
            Close();
        }
    }
}
