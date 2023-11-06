using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;
using MusicPlayer.Model;
using NAudio.Wave;
using TagLib.Mpeg;

namespace MusicPlayer.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        #region Player

        private DispatcherTimer _timer;

        public DispatcherTimer Timer
        {
            get
            {
                if (_timer == null)
                {
                    _timer = new();
                    _timer.Interval = new TimeSpan(0, 0, 0, 1);
                    _timer.Tick += async delegate(object sender, EventArgs args)
                    {
                        try
                        {
                            if (MPlayer.State == NAudio.Wave.PlaybackState.Playing)
                            {
                                Time = MPlayer.getPositionString();
                                Check(CurrentCollection, PlayingMusic);
                            }
                            if (MPlayer.State == PlaybackState.Stopped & IsPlay)
                                if (CurrentCollection.Count > 0) {
                                    if (HistoryIndex == CurrentCollection.Count - 1)
                                        HistoryIndex = 0;
                                    else
                                        HistoryIndex++;
                                    var file = CurrentCollection[HistoryIndex];
                                    await Play(file, CurrentCollection);
                                }
                            if (HistoryIndex != 0 & CurrentCollection.Count != 0) {
                                var prev = HistoryIndex - 1;
                                PrevMusic = CurrentCollection[prev];
                            } else {
                                PrevMusic = null;
                            }
                            if (HistoryIndex != CurrentCollection.Count - 1) {
                                var next = HistoryIndex + 1;
                                NextMusic = CurrentCollection[next];
                            } else {
                                NextMusic = null;
                            }  
                        }
                        catch
                        {
                            Timer.Stop();
                        }
                    };
                }

                return _timer;
            }
            set
            {
                _timer = value;
                OnPropertyChanged(nameof(_timer));
            }
        }

        private Player _mPlayer;

        public Player MPlayer
        {
            get { return _mPlayer ??= new Player(100); }
            set
            {
                _mPlayer = value;
                OnPropertyChanged(nameof(MPlayer));
            }
        }

        private PlaybackState _playbackState;

        public PlaybackState PlaybackState
        {
            get
            {
                _playbackState = MPlayer.State;
                return _playbackState;
            }
            set
            {
                _playbackState = value;
                OnPropertyChanged(nameof(PlaybackState));
            }
        }

        private string _programTitle;

        public string ProgramTitle
        {
            get => _programTitle;
            set
            {
                _programTitle = value;
                OnPropertyChanged(nameof(_programTitle));
            }
        }

        #endregion

        #region Conditions

        private bool _menuState;

        public bool MenuState
        {
            get => _menuState;
            set
            {
                _menuState = value;
                OnPropertyChanged(nameof(_menuState));
            }
        }

        private MusicCenterState _centerState;

        public MusicCenterState CenterState
        {
            get => _centerState;
            set
            {
                _centerState = value;
                OnPropertyChanged(nameof(CenterState));
            }
        }

        private ItemsControl _content;

        public ItemsControl Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        private bool _isPlay;

        public bool IsPlay
        {
            get => _isPlay;
            set
            {
                _isPlay = value;
                OnPropertyChanged(nameof(_isPlay));
            }
        }

        #endregion

        #region Collections

        private ObservableCollection<MusicFile> _currentCollection;
        private ObservableCollection<MusicFile> _myCollection;
        private ObservableCollection<MusicFile> _favoriteCollection;
        private ObservableCollection<MusicFile> _newMusicCollection;
        private ObservableCollection<MusicFile> _popularCollection;
        private ObservableCollection<MusicFile> _searchCollection;
        private ObservableCollection<MusicFile> _tempSearchCollection;

        public ObservableCollection<MusicFile> CurrentCollection
        {
            get { return _currentCollection ??= new(); }
            set
            {
                _currentCollection = value;
                OnPropertyChanged(nameof(CurrentCollection));
            }
        }

        public ObservableCollection<MusicFile> MyCollection
        {
            get { return _myCollection ??= new(); }
            set
            {
                _myCollection = value;
                OnPropertyChanged(nameof(MyCollection));
            }
        }

        public ObservableCollection<MusicFile> FavoriteCollection
        {
            get { return _favoriteCollection ??= new(); }
            set
            {
                _favoriteCollection = value;
                OnPropertyChanged(nameof(FavoriteCollection));
            }
        }

        public ObservableCollection<MusicFile> NewMusicCollection
        {
            get { return _newMusicCollection ??= new(); }
            set
            {
                _newMusicCollection = value;
                OnPropertyChanged(nameof(NewMusicCollection));
            }
        }

        public ObservableCollection<MusicFile> PopularCollection
        {
            get { return _popularCollection ??= new(); }
            set
            {
                _popularCollection = value;
                OnPropertyChanged(nameof(NewMusicCollection));
            }
        }

        public ObservableCollection<MusicFile> SearchCollection
        {
            get { return _searchCollection ??= new(); }
            set
            {
                _searchCollection = value;
                OnPropertyChanged(nameof(SearchCollection));
            }
        }

        public ObservableCollection<MusicFile> TempSearchCollection
        {
            get { return _tempSearchCollection ??= new(); }
            set
            {
                _tempSearchCollection = value;
                OnPropertyChanged(nameof(TempSearchCollection));
            }
        }

        #endregion

        #region MusicFile

        private MusicFile _selectedMusic;

        public MusicFile SelectedMusic
        {
            get => _selectedMusic;
            set
            {
                _selectedMusic = value;
                OnPropertyChanged(nameof(SelectedMusic));
            }
        }

        private MusicFile _playingMusic;

        public MusicFile PlayingMusic
        {
            get => _playingMusic;
            set
            {
                _playingMusic = value;
                OnPropertyChanged(nameof(PlayingMusic));
            }
        }

        private MusicFile _nextMusic;

        public MusicFile NextMusic
        {
            get => _nextMusic;
            set
            {
                _nextMusic = value;
                OnPropertyChanged(nameof(NextMusic));
            }
        }

        private MusicFile _prevMusic;

        public MusicFile PrevMusic
        {
            get => _prevMusic;
            set
            {
                _prevMusic = value;
                OnPropertyChanged(nameof(PrevMusic));
            }
        }

        private int _historyIndex;

        public int HistoryIndex
        {
            get => _historyIndex;
            set
            {
                _historyIndex = value;
                OnPropertyChanged(nameof(HistoryIndex));
            }
        }

        private int _selectedMusicIndex;

        public int SelectedMusicIndex
        {
            get { return _selectedMusicIndex; }
            set
            {
                _selectedMusicIndex = value;
                OnPropertyChanged(nameof(SelectedMusicIndex));
            }
        }

        private string _searchText;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }

        public string Title => PlayingMusic.Title;
        public string Artist => PlayingMusic.Artist;

        private string _time;

        public string Time
        {
            get
            {
                _time = MPlayer.getPositionString();
                return _time;
            }
            set
            {
                _time = value;
                OnPropertyChanged(nameof(Time));
            }
        }

        private string _duration;

        public string Duration
        {
            get
            {
                _duration = MPlayer.getTotalTimeString();
                return _duration;
            }
            set
            {
                _duration = value;
                OnPropertyChanged(nameof(Duration));
            }
        }

        private long _maximum;

        public long Maximum
        {
            get
            {
                if (_maximum == 0)
                {
                    _maximum = 1;
                }

                return _maximum;
            }
            set
            {
                _maximum = value;
                OnPropertyChanged(nameof(Maximum));
            }
        }

        private long _value;

        public long Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        private string _timeTip;

        public string TimeTip
        {
            get { return _timeTip; }
            set
            {
                _timeTip = value;
                OnPropertyChanged(nameof(TimeTip));
            }
        }

        #endregion

        #region Commands

        public ICommand OpenPlayList
        {
            get
            {
                return new Command((e) =>
                {
                    Content = new ItemsControl();
                    Content.ItemsSource = MyCollection;
                    CenterState = MusicCenterState.МояМузыка;
                    //CurrentCollection = MyCollection;
                    Check(CurrentCollection, PlayingMusic);
                });
            }
        }

        public ICommand OpenFavorite
        {
            get
            {
                return new Command(async (e) =>
                {
                    if(System.IO.File.Exists(System.IO.Path.Combine(AppContext.BaseDirectory, "favorite.json")))
                        await LoadFavorites();
                    Content = new ItemsControl();
                    Content.ItemsSource = FavoriteCollection;
                    CenterState = MusicCenterState.Нравится;
                    Check(FavoriteCollection, PlayingMusic);
                });
            }
        }

        public ICommand OpenSearch
        {
            get
            {
                return new Command((e) =>
                {
                    Content = new ItemsControl();
                    Content.ItemsSource = SearchCollection;
                    CenterState = MusicCenterState.Поиск;
                    //CurrentCollection = SearchCollection;
                    Check(SearchCollection, PlayingMusic);
                });
            }
        }

        public ICommand OpenNew
        {
            get
            {
                return new Command(async (e) =>
                {
                    if (System.IO.File.Exists(System.IO.Path.Combine(AppContext.BaseDirectory, "newMusic.json")))
                    {
                        NewMusicCollection =
                            Serializer.Load(System.IO.Path.Combine(AppContext.BaseDirectory, "newMusic.json"));
                    }
                    else
                    {
                        await MusicFinder.Find(NewMusicCollection, MusicCenterState.Новое);
                        Serializer.Save(NewMusicCollection,
                            System.IO.Path.Combine(AppContext.BaseDirectory, "newMusic.json"));
                    }
                    Content = new ItemsControl();
                    Content.ItemsSource = NewMusicCollection;
                    CenterState = MusicCenterState.Новое;
                    //CurrentCollection = NewMusicCollection;
                    Check(NewMusicCollection, PlayingMusic);
                });
            }
        }

        public ICommand OpenPop
        {
            get
            {
                return new Command(async (e) =>
                {
                    if (System.IO.File.Exists(System.IO.Path.Combine(AppContext.BaseDirectory, "popular.json")))
                    {
                        PopularCollection =
                            Serializer.Load(System.IO.Path.Combine(AppContext.BaseDirectory, "popular.json"));
                    }
                    else
                    {
                        await MusicFinder.Find(PopularCollection, MusicCenterState.Новое);
                        Serializer.Save(PopularCollection,
                            System.IO.Path.Combine(AppContext.BaseDirectory, "popular.json"));
                    }
                    Content = new ItemsControl();
                    Content.ItemsSource = PopularCollection;
                    CenterState = MusicCenterState.Популярное;
                    //CurrentCollection = PopularCollection;
                    Check(PopularCollection, PlayingMusic);
                });
            }
        }

        //

        public ICommand PlayFileFromList
        {
            get
            {
                return new Command(async item =>
                {
                    //await MPlayer.Stop();
                    IsPlay = false;
                    if (CenterState == MusicCenterState.МояМузыка)
                    {
                        CurrentCollection = MyCollection;
                        await Play(SelectedMusic, CurrentCollection);
                        HistoryIndex = SelectedMusicIndex;
                        Check(CurrentCollection, PlayingMusic);
                        IsPlay = true;
                    }
                    else if (CenterState == MusicCenterState.Нравится)
                    {
                        CurrentCollection = FavoriteCollection;
                        await Play(SelectedMusic, CurrentCollection);
                        HistoryIndex = SelectedMusicIndex;
                        Check(CurrentCollection, PlayingMusic);
                        IsPlay = true;
                    }    
                    else if (CenterState == MusicCenterState.Поиск)
                    {
                        CurrentCollection = SearchCollection;
                        HistoryIndex = SelectedMusicIndex;
                        await Play(SelectedMusic, CurrentCollection);
                        Check(CurrentCollection, PlayingMusic);
                        IsPlay = true;
                    }
                    else if (CenterState == MusicCenterState.Популярное)
                    {
                        CurrentCollection = PopularCollection;
                        HistoryIndex = SelectedMusicIndex;
                        await Play(SelectedMusic, CurrentCollection);
                        Check(CurrentCollection, PlayingMusic);
                        IsPlay = true;
                    }
                    else if (CenterState == MusicCenterState.Новое)
                    {
                        CurrentCollection = NewMusicCollection;
                        HistoryIndex = SelectedMusicIndex;
                        await Play(SelectedMusic, CurrentCollection);
                        Check(CurrentCollection, PlayingMusic);
                        IsPlay = true;
                    }
                });
            }
        }

        public ICommand PlayFile
        {
            get
            {
                return new Command(async item =>
                {
                    IsPlay = false;
                    if (CenterState == MusicCenterState.МояМузыка)
                    {
                        CurrentCollection = MyCollection;
                        HistoryIndex = SelectedMusicIndex;
                        await Play(SelectedMusic, CurrentCollection);
                        Check(CurrentCollection, PlayingMusic);
                        IsPlay = true;
                    }
                    else if (CenterState == MusicCenterState.Нравится)
                    {
                        CurrentCollection = FavoriteCollection;
                        await Play(SelectedMusic, CurrentCollection);
                        HistoryIndex = SelectedMusicIndex;
                        Check(CurrentCollection, PlayingMusic);
                        IsPlay = true;
                    }    
                    else if (CenterState == MusicCenterState.Поиск)
                    {
                        CurrentCollection = SearchCollection;
                        HistoryIndex = SelectedMusicIndex;
                        await Play(SelectedMusic, CurrentCollection);
                        Check(CurrentCollection, PlayingMusic);
                        IsPlay = true;
                    }
                    else if (CenterState == MusicCenterState.Популярное)
                    {
                        CurrentCollection = PopularCollection;
                        HistoryIndex = SelectedMusicIndex;
                        await Play(SelectedMusic, CurrentCollection);
                        Check(CurrentCollection, PlayingMusic);
                        IsPlay = true;
                    }
                    else if (CenterState == MusicCenterState.Новое)
                    {
                        CurrentCollection = NewMusicCollection;
                        HistoryIndex = SelectedMusicIndex;
                        await Play(SelectedMusic, CurrentCollection);
                        Check(CurrentCollection, PlayingMusic);
                        IsPlay = true;
                    }
                });
            }
        }

        public ICommand PlayPause
        {
            get
            {
                return new Command(async e =>
                {
                    if (CenterState == MusicCenterState.МояМузыка)
                    {
                        CurrentCollection = MyCollection;
                        HistoryIndex = SelectedMusicIndex;
                        await Play(SelectedMusic, CurrentCollection);
                    }
                    else if (CenterState == MusicCenterState.Нравится)
                    {
                        CurrentCollection = FavoriteCollection;
                        HistoryIndex = SelectedMusicIndex;
                        await Play(SelectedMusic, CurrentCollection);
                    }
                    else if (CenterState == MusicCenterState.Популярное)
                    {
                        CurrentCollection = PopularCollection;
                        HistoryIndex = SelectedMusicIndex;
                        await Play(SelectedMusic, CurrentCollection);
                    }
                    else if (CenterState == MusicCenterState.Новое)
                    {
                        CurrentCollection = NewMusicCollection;
                        HistoryIndex = SelectedMusicIndex;
                        await Play(SelectedMusic, CurrentCollection);
                    }
                });
            }
        }

        public ICommand PlayNext
        {
            get
            {
                return new Command(async e =>
                {
                    if (CurrentCollection.Count == 0) return;
                    if (CurrentCollection.Count > 0)
                    {
                        if (HistoryIndex == CurrentCollection.Count - 1)
                            HistoryIndex = 0;
                        else
                            HistoryIndex++;
                        var file = CurrentCollection[HistoryIndex];
                        await Play(file, CurrentCollection);
                    }
                });
            }
        }

        public ICommand PlayPrev
        {
            get { return new Command(e => { }); }
        }

        public ICommand AddFiles
        {
            get { return new Command(e => { AddMusic(); }); }
        }

        public ICommand Search
        {
            get
            {
                return new Command(text =>
                {
                    SearchText = text.ToString();
                    MusicFinder.Find(SearchText, SearchCollection, MusicCenterState.Поиск);
                    CenterState = MusicCenterState.Поиск;
                    Content = new ItemsControl();
                    Content.ItemsSource = SearchCollection;
                });
            }
        }

        public ICommand ReloadNewMusic
        {
            get { return new Command(async e =>
            {
                await MusicFinder.Find(NewMusicCollection, MusicCenterState.Новое);
                Serializer.Save(NewMusicCollection,
                    System.IO.Path.Combine(AppContext.BaseDirectory, "newMusic.json"));
                CurrentCollection = NewMusicCollection;
                Check(NewMusicCollection, PlayingMusic);
            }); }
        }

        public ICommand ReloadPopular
        {
            get { return new Command(async e =>
            {
                await MusicFinder.Find(PopularCollection, MusicCenterState.Популярное);
                Serializer.Save(PopularCollection,
                    System.IO.Path.Combine(AppContext.BaseDirectory, "popular.json"));
                CurrentCollection = PopularCollection;
                Check(PopularCollection, PlayingMusic);
            }); }
        }
        
        public ICommand AddToFavoriteList {
            get {
                return new Command((e) => {
                    TrackIsFavorite(SelectedMusic);
                    SaveFavoriteList();
                });
            }
        }

        #endregion

        #region Voids
        
        private void TrackIsFavorite(MusicFile file) {
            if (file.Liked == false) {
                file.Liked = true;
                FavoriteCollection.Add(file);
            } else {
                file.Liked = false;
                FavoriteCollection.Remove(file);
                if (file.Downloaded == true) {
                    if(System.IO.File.Exists(file.LocalAddress))
                        System.IO.File.Delete(file.LocalAddress);
                    file.Downloaded = false;
                }
            }
        }
        public void SaveFavoriteList() {
            foreach (var item in FavoriteCollection)
            {
                item.IsPlay = false;
                item.IsPaused = false;
            }
            Serializer.Save(FavoriteCollection, System.IO.Path.Combine(AppContext.BaseDirectory, "favorite.json"));
        }
        
        private Task LoadFavorites()
        {
            FavoriteCollection = Serializer.Load(System.IO.Path.Combine(AppContext.BaseDirectory, "favorite.json"));
            return Task.CompletedTask;
        }

        private async Task Play(MusicFile file, ObservableCollection<MusicFile> list)
        {
            if (file == null) return;
            string path = "";
            if (file.Location == Location.Internet)
            {
                path = file.RemoteAddress;
            }
            else if (file.Location == Location.Local)
            {
                path = file.LocalAddress;
            }

            //MessageBox.Show(path);

            if (MPlayer.State == PlaybackState.Playing)
            {
                if (Equals(file, PlayingMusic))
                {
                    await MPlayer.Pause();
                    PlayingMusic.IsPlay = false;
                    PlayingMusic.IsPaused = true;
                }
                else if (!Equals(file, PlayingMusic))
                {
                    await MPlayer.Stop();
                    await MPlayer.Play(path);
                    PlayingMusic = file;
                    PlayingMusic.IsPlay = true;
                    Timer.Start();
                    Check(CurrentCollection, PlayingMusic);
                }
            }
            else if (MPlayer.State == PlaybackState.Paused)
            {
                if (Equals(file, PlayingMusic))
                {
                    await MPlayer.Resume();
                    PlayingMusic.IsPaused = false;
                    PlayingMusic.IsPlay = true;
                }
                else
                {
                    PlayingMusic.IsPaused = false;
                    PlayingMusic.IsPlay = false;
                    await MPlayer.Stop();
                    await MPlayer.Play(path);
                    PlayingMusic = file;
                    PlayingMusic.IsPlay = true;
                    Timer.Start();
                    Check(CurrentCollection, PlayingMusic);
                }
            }
            else if (MPlayer.State == PlaybackState.Stopped)
            {
                await MPlayer.Play(path);
                Duration = MPlayer.getTotalTimeString();
                Time = MPlayer.getPositionString();
                Maximum = MPlayer.getTotalTime();
                PlayingMusic = file;
                PlayingMusic.IsPlay = true;
                Timer.Start();
                Check(CurrentCollection, PlayingMusic);
            }
        }

        private void AddMusic()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter =
                "Все музыкальные форматы (*.mp3, *.flac)|*.mp3;*.flac|MP3 музыка (*.mp3)|*.mp3|FLAC музыка (*.flac)|*.flac";
            ofd.Multiselect = true;
            ofd.RestoreDirectory = true;
            var result = ofd.ShowDialog();
            if (result == true)
            {
                var files = ofd.FileNames;
                foreach (var item in files)
                    MyCollection.Add(new MusicFile(item));
            }
        }

        private void Check(ObservableCollection<MusicFile> source, MusicFile target)
        {
            foreach (var file in source)
            {
                if (MPlayer.State == NAudio.Wave.PlaybackState.Playing)
                {
                    if (Equals(file, target))
                    {
                        file.IsPlay = true;
                        file.IsPaused = false;
                    }
                    else
                    {
                        file.IsPlay = false;
                        file.IsPaused = false;
                    }
                }

                if (MPlayer.State == NAudio.Wave.PlaybackState.Paused)
                {
                    if (Equals(file, target))
                    {
                        file.IsPaused = true;
                        file.IsPlay = false;
                    }
                }

                if (MPlayer.State == NAudio.Wave.PlaybackState.Stopped)
                {
                    file.IsPlay = false;
                    file.IsPaused = false;
                }
            }
        }

        #endregion

        public MainViewModel()
        {
            ProgramTitle = $"Music Player [{CenterState}]";
            MyCollection = new();
            FavoriteCollection = new();
            SearchCollection = new();
            //
            Content = new ItemsControl();
            Content.ItemsSource = MyCollection;
            MenuState = false;
            CenterState = MusicCenterState.МояМузыка;
        }
    }
}