using System;
using System.Windows;
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
        public int _Height
        { get; set; }
        public int _Width
        { get; set; }
        public Menu()
        {
            InitializeComponent();
        }
        private void TwoPlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            _PartyType = PartyType.PvP;
            DisableAllButton();
            PlayAnimation();
        }

        private void OnePlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            _PartyType = PartyType.AivP;
            DisableAllButton();
            PlayAnimation();
        }

        private void LoadSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            _PartyType = PartyType.ResumeOld;
            DisableAllButton();
            PlayAnimation();
        }
        private void CloseWindow()
        {
            this.DialogResult = true;
            Height = ActualHeight;
            Width = ActualWidth;
            Close();
        }
        private void DisableAllButton()
        {
            loadSaveBtn.IsEnabled = false;
            onePlayerBtn.IsEnabled = false;
            twoPlayerBtn.IsEnabled = false;
        }

        private void PlayAnimation()
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
                PlayExplosion();
                timerAttackAnim.Stop();
            };

        }

        private void PlayExplosion()
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
                CloseWindow();
            };
        }


    }
}
