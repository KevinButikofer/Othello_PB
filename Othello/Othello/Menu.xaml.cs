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
using System.Windows.Threading;


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
        public Menu()
        {
            InitializeComponent();
        }
        private void TwoPlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            playAnimation(PartyType.PvP);
            
        }

        private void OnePlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            playAnimation(PartyType.AivP);

        }

        private void TwoAiBtn_Click(object sender, RoutedEventArgs e)
        {
            playAnimation(PartyType.AivAI);

        }
        private void LoadSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            playAnimation(PartyType.ResumeOld);

        }
        private void CloseWindow()
        {
            DialogResult = true;
            Close();
        }

        public void playAnimation(PartyType p)
        {
            DispatcherTimer timerAttackAnim = new DispatcherTimer();

            timerAttackAnim.Interval = TimeSpan.FromMilliseconds(2100);
            timerAttackAnim.Start();
            punchImage.Visibility = Visibility.Hidden;
            punchGif.Visibility = Visibility.Visible;
            punchGif.Play();

            punchImageReverse.Visibility = Visibility.Hidden;
            punchGifReverse.Visibility = Visibility.Visible;
            punchGifReverse.Play();

            timerAttackAnim.Tick += (o, args) =>
            {
                playExplosion(p);
                timerAttackAnim.Stop();
                
            };

        }

        public void playExplosion(PartyType p)
        {
            DispatcherTimer timerAttackAnim = new DispatcherTimer();

            timerAttackAnim.Interval = TimeSpan.FromMilliseconds(500);
            timerAttackAnim.Start();
            punchImage.Visibility = Visibility.Visible;
            punchGif.Visibility = Visibility.Hidden;

            punchImageReverse.Visibility = Visibility.Visible;
            punchGifReverse.Visibility = Visibility.Hidden;
            explosionImage.Visibility = Visibility.Visible;

            timerAttackAnim.Tick += (o, args) =>
            {
                timerAttackAnim.Stop();
                _PartyType = p;
                CloseWindow();
            };
        }

        
    }
}
