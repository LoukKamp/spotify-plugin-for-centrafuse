﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Un4seen.Bass;

namespace Spotify
{
    public class BASSPlayer
    {
        public BASSPlayer()
        {
            Paused = true;
        }
        
        private int channel = -1;
        public int EnqueueSamples(int channels, int rate, byte[] samples, int frames)
        {
            if (stopped)
            {
                return frames; //should we return 0? this means frames will be actively dropped
            }

            if (channel == -1)
            {
                channel = Bass.BASS_StreamCreate(rate, channels, BASSFlag.BASS_DEFAULT, BASSStreamProc.STREAMPROC_PUSH);
                Bass.BASS_ChannelPlay(channel, false);
            }

            if (channel != -1)
                Bass.BASS_StreamPutData(channel, samples, samples.Length); //data will always be queued up, never dropped

            return frames;
        }

        private bool stopped = true;
        public void Stop()
        {
            if (channel != -1)
            {
                Bass.BASS_ChannelStop(channel);
                Bass.BASS_StreamFree(channel);
                channel = -1;
                stopped = true;
            }
        }

        public void ReadyPlay()
        {
            stopped = false;
        }

        private bool _paused = false;
        public bool Paused
        {
            get
            {
                return _paused;
            }
            set
            {
                _paused = value;
                if (_paused)
                {
                    Bass.BASS_ChannelPause(channel);
                }
                else
                {
                    Bass.BASS_ChannelPlay(channel, false);
                }
            }
        }

        public TimeSpan Position
        {
            get
            {
                if (channel == 0) return TimeSpan.Zero;
                // length in bytes
                long len = Bass.BASS_ChannelGetPosition(channel, BASSMode.BASS_POS_BYTES);
                if (len <= 0)
                {
                    return TimeSpan.Zero;
                }
                // the time length
                int seconds = (int)Bass.BASS_ChannelBytes2Seconds(channel, len);
                return TimeSpan.FromSeconds(seconds);
            }
        }
    }
}
