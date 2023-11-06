using System;
using System.Threading.Tasks;
using System.Windows;
using NAudio.Wave;

namespace MusicPlayer.Model;

public class Player
{
    public double VolumeLevel;
    public bool IsPlaying { get; set; }
    private IWavePlayer _waveout { get; set; }
    private AudioFileReader _reader { get; set; }
    public NAudio.Wave.PlaybackState State { get; set; }
    
    public Player(long volume) {
        _waveout = new WaveOut();
        _waveout.Volume = volume / 100;
    }
    public Task Play(string file)
    {
        try {
            if (_waveout == null | _reader == null) {
                _waveout = new WaveOut();

                _waveout.PlaybackStopped += (s, a) => {
                    {
                        _waveout.Dispose();
                        _reader.Dispose();
                        _waveout = null;
                        _reader = null;
                        State = PlaybackState.Stopped;
                    }
                };
            }
            if (_reader == null | _reader != null) {
                    
                _reader = new AudioFileReader(file);
                _waveout.Init(_reader);
                   

            }
            _waveout.Play();
            State = PlaybackState.Playing;
        } catch { }
        return Task.CompletedTask;
    }

    public Task Pause()
    {
        _waveout?.Pause();
        State = PlaybackState.Paused;
        IsPlaying = false;
        return Task.CompletedTask;
    }

    public Task Resume()
    {
        _waveout.Play();
        State = PlaybackState.Playing;
        IsPlaying = true;
        return Task.CompletedTask;
    }

    public Task Stop()
    {
        _waveout?.Stop();
        _waveout?.Dispose();
        _reader?.Dispose();
        _waveout = null;
        _reader = null;
        State = PlaybackState.Stopped;
        IsPlaying = false;
        return Task.CompletedTask;
    }
    
    public string getPositionString() {
        var time = "00:00";
        if (_reader != null)
            time = _reader.CurrentTime.ToString(@"mm\:ss");
        return time;
    }
    public string getTotalTimeString() {
        var time = "00:00";
        if (_reader != null)
            time = _reader?.TotalTime.ToString(@"mm\:ss");
        return time;
    }
    public float getVolume() {
        return _waveout.Volume;
    }
    public void setVolume(float volume) {
        if (_waveout != null)
            _waveout.Volume = volume;
    }
    public long getTotalTime() {
        long time = 10;
        if (_reader != null)
            time = _reader.Length;
        return time;
    }
    public long getPosition() {
        long pos = 0;
        if (_reader != null)
            pos = _reader.Position;
        return pos;
    }
    public Task setPosition(long position) {
        if (_reader != null)
            _reader.Position = position;
        return Task.CompletedTask;
    }
}

public enum PLayerState
{
    Pause,
    Play,
    Stop
}