using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using NAudio.Dsp;

namespace AudioProcessing
{
    /// <summary>
    /// Logica di interazione per SpectrumViwer.xaml
    /// </summary>
    public partial class SpectrumViwer : UserControl
    {
        private double xScale = 200;
        private int bins = 1024;

        private const int binsPerPoint = 1;
        private int updateCount;

        public SpectrumViwer()
        {
            InitializeComponent();
            CalculateXScale();
            this.SizeChanged += SpectrumViwer_SizeChanged;
        }

        void SpectrumViwer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalculateXScale();
        }

        private void CalculateXScale()
        {
            this.xScale = this.ActualWidth / (bins / binsPerPoint);
        }

        public void Update(Complex[] fftResults)
        {
            if (updateCount++ % 2 == 0)
            {
                return;
            }

            if (fftResults.Length / 2 != bins)
            {
                this.bins = fftResults.Length / 2;
                CalculateXScale();
            }
            
            for (int n = 0; n < fftResults.Length / 2; n += binsPerPoint)
            {
                double yPos = 0;
                for (int b = 0; b < binsPerPoint; b++)
                {
                    yPos += GetYPosLog(fftResults[n+b]);
                }
                AddResult(n / binsPerPoint, yPos / binsPerPoint);
            }
        }

        private double GetYPosLog(Complex c)
        {
            double intensityDB = 10 * Math.Log10(Math.Sqrt(c.X * c.X + c.Y * c.Y));
            double minDB = -90;
            if (intensityDB < minDB) intensityDB = minDB;
            double percent = intensityDB / minDB;
            double yPos = percent * this.ActualHeight;
            return yPos;
        }

        private void AddResult(int index, double power)
        {
            Point p = new Point(CalculateXPos(index), power);
            if (index >= polyline.Points.Count)
            {
                polyline.Points.Add(p);
            }
            else
            {
                polyline.Points[index] = p;
            }
        }

        private double CalculateXPos(int bin)
        {
            if (bin == 0) return 0;
            return bin * xScale;
        }
    }
}
