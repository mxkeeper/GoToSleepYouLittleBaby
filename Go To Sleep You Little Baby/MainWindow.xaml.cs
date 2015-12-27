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
using MahApps.Metro.Controls;
using System.Windows.Threading;
using NAudio;
using NAudio.Wave;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Go_To_Sleep_You_Little_Baby
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        WaveOut waveOutDevice = new WaveOut();

        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        DispatcherTimer dispatcherTimer2 = new DispatcherTimer();

        int remain = 15;
        string content;
        public MainWindow()
        {
            InitializeComponent();

            SystemEvents.PowerModeChanged += OnPowerChange;

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();


            dispatcherTimer2.Tick += new EventHandler(dispatcherTimer2_Tick);
            dispatcherTimer2.Interval = new TimeSpan(0, 0, 1);

            this.Visibility = Visibility.Hidden;
            this.Width = 460;
            this.Height = 250;

            content = (string)lblRemaining.Content;
        }

        private void OnPowerChange(object s, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    {
                        remain = 15;
                        break;
                    }
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            lblTime.Content = time;
            if (time.Equals("21:30:00"))
            {
                this.Visibility = Visibility.Visible;
                this.Activate();
                PlayMp3Resources(Properties.Resources.Song);
                dispatcherTimer2.Start();
            }
        }

        private void dispatcherTimer2_Tick(object sender, EventArgs e)
        {
            if (remain == 0)
            {
                waveOutDevice.Stop();
                dispatcherTimer2.Stop();
                SetSuspendState(false, true, true);
                return;
            }
            remain--;
            UpdateRemainingTime(remain);
        }

        private void UpdateRemainingTime(int i)
        {
            lblRemaining.Content = content + i.ToString() + " секунд";
        }

        private void PlayMp3Resources(byte[] audio)
        {
            MemoryStream ms = new MemoryStream(audio);
            WaveStream mp3Reader = new Mp3FileReader(ms);
            WaveChannel32 inputStream = new WaveChannel32(mp3Reader);

            waveOutDevice.Init(inputStream);
            waveOutDevice.Volume = 0.3f;
            waveOutDevice.Play();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
        }
    }
}
