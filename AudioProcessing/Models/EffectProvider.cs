using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AudioProcessing.Models
{
    class EffectProvider : ISampleProvider
    {
        private Effect Effect;
        private readonly ISampleProvider SourceProvider;
        public WaveFormat WaveFormat { get { return SourceProvider.WaveFormat; } }

        public EffectProvider(ISampleProvider sourceProvider, List<Slider> sliders)
        {
            Effect = new SuperPitch(sliders);
            SourceProvider = sourceProvider;
            InitializeEffect(Effect);
        }

        private void InitializeEffect(Effect effect)
        {
            effect.SampleRate = WaveFormat.SampleRate;
            effect.Init();
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int read = SourceProvider.Read(buffer, offset, count);
            int samples = count;
            if (Effect.Enabled)
            {
                Effect.Block(samples);
            }

            for (int sample = 0; sample < samples; sample++)
            {
                float sampleLeft = buffer[offset];
                float sampleRight = sampleLeft;
                if (WaveFormat.Channels == 2)
                {
                    sampleRight = buffer[offset + 1];
                    sample++;
                }

                // run these samples through the effect
                if (Effect.Enabled)
                {
                    Effect.OnSample(ref sampleLeft, ref sampleRight);
                }

                // put them back
                buffer[offset++] = sampleLeft;
                if (WaveFormat.Channels == 2)
                {
                    buffer[offset++] = sampleRight;
                }
            }
            return read;
        }

        public void EffectOff()
        {
            Effect.Enabled = false;         
        }

        public void EffectOn()
        {
            Effect.Enabled = true;
        }
    }
}
