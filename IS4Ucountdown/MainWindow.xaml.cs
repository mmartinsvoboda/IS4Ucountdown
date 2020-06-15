using System;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace IS4Ucountdown
{
    public partial class MainWindow : Window
    {
        private System.Timers.Timer timer;
        private TimeSpan time;
        private readonly int interval = 1000;

        private readonly SolidColorBrush greenColor = new SolidColorBrush(Colors.Green);
        private readonly SolidColorBrush yellowColor = new SolidColorBrush(Colors.Yellow);
        private readonly SolidColorBrush redColor = new SolidColorBrush(Colors.Red);

        public MainWindow()
        {
            InitializeComponent();
        }

        //Metoda zavolána pomocí tlačítka "Zahájit odpočet" 
        private void StartCountdown(object sender, RoutedEventArgs e)
        {
            SetTimeSpan();
            UpdateLayoutForStart();
            Start_Timer();
        }

        //Nastaví délku odpočtu podle RadioButtons
        private void SetTimeSpan()
        {
            if (min5.IsChecked == true) time = TimeSpan.FromMinutes(5);
            else if (min3.IsChecked == true) time = TimeSpan.FromMinutes(3);
            else if (min1.IsChecked == true) time = TimeSpan.FromMinutes(1);
        }

        //Nastavení UI pro zahájení odpočtu
        private void UpdateLayoutForStart()
        {
            settingsGrid.IsEnabled = false;
            settingsGrid.Visibility = Visibility.Hidden;
            counterGrid.Visibility = Visibility.Visible;
            finished.Content = "";
            CountdownHandler();
        }

        private void Start_Timer()
        {
            timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(TimerTick);
            timer.Interval = interval;
            timer.Start();
        }

        private void StopTimer()
        {
            timer.Stop();
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                finished.Content = "Odpočet dokončen.";
                settingsGrid.IsEnabled = true;
                settingsGrid.Visibility = Visibility.Visible;
            }));
        }

        //Metoda volána každou jednotku intervalu
        private void TimerTick(object sender, EventArgs e)
        {
            time = time.Add(TimeSpan.FromSeconds(-1));
            Application.Current.Dispatcher.Invoke(new Action(() => { CountdownHandler(); }));
            if (time <= TimeSpan.Zero)
            {
                StopTimer();
            }
        }

        private void CountdownHandler()
        {
            SetColors();
            PrintMinutes();
            PrintSecLeft();
            PrintSecRight();
            if (time.Seconds <= 10) HandleBeeps();
        }

        private void SetColors()
        {
            if (time.TotalSeconds == 0)
                secRight.Foreground = redColor;
            else if (10 >= time.Seconds && time.Seconds >= 1 && time.TotalSeconds <= 20)
            {
                secLeft.Foreground = yellowColor;
                secRight.Foreground = yellowColor;
            }
            else
            {
                secLeft.Foreground = greenColor;
                secRight.Foreground = greenColor;
            }
        }

        private void PrintMinutes()
        {
            if (time.Minutes == 0) minutes.Content = "";
            else minutes.Content = time.Minutes.ToString();
        }

        private void PrintSecLeft()
        {
            if (time.Seconds >= 10) secLeft.Content = (time.Seconds / 10);
            else secLeft.Content = "";
        }

        private void PrintSecRight()
        {
            secRight.Content = time.Seconds % 10;
        }

        //Ovladač zvuků
        private void HandleBeeps()
        {
            if (time.TotalSeconds == 0)
                PlayBeep(5000, 1500);
            else if (10 >= time.Seconds && time.Seconds >= 1 && time.TotalSeconds <= 10)
            {
                PlayBeep(1000, 10);
            }
            else if (time.Seconds == 0)
            {
                PlayBeep(500, 400);
            }
        }

        //Přehrává zvuky dle zadané frekvence, nezpožďuje timer
        private void PlayBeep(int frequency, int duration)
        {
            Action beep = () => Console.Beep(frequency, duration); beep.BeginInvoke((a) => { beep.EndInvoke(a); }, null);
        }
    }
}