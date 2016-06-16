using NAudio.Wave;
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

namespace AudioProcessing
{
    /// <summary>
    /// Logica di interazione per WaveformViwer.xaml
    /// </summary>
    public partial class WaveformViwer : UserControl
    {
        //min 4410
        private int numToDisplay = 4410;
        private int count = 0;
        private Queue<int> queue = new Queue<int>();
        public WaveformViwer()
        {
            InitializeComponent();
            this.SizeChanged += WaveformViwer_SizeChanged;   
        }

        private void WaveformViwer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            polyline.MaxHeight = this.ActualHeight;
            polyline.MaxWidth = this.ActualWidth;
        }

        private Point Normalize(int x, int y)
        {
            Point point = new Point();
            point.X = 1.0 * x / numToDisplay * polyline.MaxWidth;
            point.Y = polyline.MaxHeight / 2.0 - y / (int.MaxValue * 1.0) * (polyline.MaxHeight / 2.0);
            return point;
        }

        public void Update(WaveIn sender, WaveInEventArgs e)
        {
            for (int i = 0; i < e.Buffer.Length - 1; i += sender.WaveFormat.BlockAlign)
            {
                if (count < numToDisplay)
                {
                    queue.Enqueue(BitConverter.ToInt32(e.Buffer, i));
                    ++count;
                }
                else
                {
                    queue.Dequeue();
                    queue.Enqueue(BitConverter.ToInt32(e.Buffer, i));
                }
            }
            polyline.Points.Clear();
            int[] points = queue.ToArray();
            for (int x = 0; x < points.Length; ++x)
            {
                polyline.Points.Add(Normalize(x, points[x]));
            }
        }
    }
}
