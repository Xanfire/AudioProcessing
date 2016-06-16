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
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Microsoft.Win32;
using AudioProcessing.Models;

namespace AudioProcessing
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Slider> sliders;
        private WaveIn waveIn;
        private WaveOut waveOut;
        private WaveInProvider waveProvider;
        private SampleAggregator sampleAggregator;
        private EffectProvider effectProvider;
        private bool hearing = false;
        private bool effect = false;

        public MainWindow()
        {
            InitializeComponent();
            sliders = new List<Slider>();
            sliders.Add(slider1);
            sliders.Add(slider2);
            sliders.Add(slider3);
            sliders.Add(slider4);
            sliders.Add(slider5);
            sliders.Add(slider6);
            sliders.Add(slider7);
            sliders.Add(slider8);
        }

        private void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            waveformViwer.Update(sender as WaveIn, e);
        }

        private void btnHearing_Click(object sender, RoutedEventArgs e)
        {
            if (!hearing)
            {
                try
                {
                    waveIn = new WaveIn();
                    waveIn.WaveFormat = new WaveFormat(44100, WaveIn.GetCapabilities(waveIn.DeviceNumber).Channels);
                    waveIn.DataAvailable += new EventHandler<WaveInEventArgs>(waveIn_DataAvailable);
                    waveProvider = new WaveInProvider(waveIn);

                    var sampleProvider = new Pcm16BitToSampleProvider(waveProvider);
                    effectProvider = new EffectProvider(sampleProvider, sliders);

                    sampleAggregator = new SampleAggregator(effectProvider);
                    sampleAggregator.PerformFFT = true;
                    sampleAggregator.NotificationCount = sampleProvider.WaveFormat.SampleRate / 100;
                    sampleAggregator.FftCalculated += new EventHandler<FftEventArgs>(sampleAggregator_OnFftCalculated);
                    sampleAggregator.MaximumCalculated += new EventHandler<MaxSampleEventArgs>(sampleAggregator_OnMaximumCalculated);

                    waveOut = new WaveOut();
                    waveOut.Init(sampleAggregator);
                    waveIn.StartRecording();
                    waveOut.Play();

                    hearing = true;
                    btnHearing.Content = "Stop Hearing";
                    btnEffect.IsEnabled = true;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
            else
            {
                waveIn.Dispose();
                waveOut.Dispose();

                hearing = false;
                btnHearing.Content = "Start Hearing";
                if (effect)
                {
                    effect = !effect;
                    btnEffect.Content = "Effect Off";
                    effectProvider.EffectOff();
                    btnEffect.IsEnabled = false;
                }
            }
        }

        private void sampleAggregator_OnMaximumCalculated(object sender, MaxSampleEventArgs e)
        {
            //Nothing to do
        }

        private void sampleAggregator_OnFftCalculated(object sender, FftEventArgs e)
        {
            spectrumAnalyserView.Update(e.Result);
        }

        private void btnEffect_Click(object sender, RoutedEventArgs e)
        {
            if (effect)
            {
                effect = !effect;
                btnEffect.Content = "Effect Off";
                effectProvider.EffectOff();
            }
            else
            {
                effect = !effect;
                btnEffect.Content = "Effect On";
                effectProvider.EffectOn();
            }
        }
    }
}