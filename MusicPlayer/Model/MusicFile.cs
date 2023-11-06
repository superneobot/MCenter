using System;
using MusicPlayer.ViewModel;

namespace MusicPlayer.Model
{
    public class MusicFile : BaseViewModel
    {
        private string _title;
        private string _artist;
        private string _poster;
        private string _remoteAddress;
        private string _localAddress;
        private string _id;
        private string _duration;
        private bool _liked;
        private bool _downloaded;
        private SourceType _source;
        //
        /// <summary>
        /// Title of song
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        /// <summary>
        /// Artist of song
        /// </summary>
        public string Artist
        {
            get => _artist;
            set
            {
                _artist = value;
                OnPropertyChanged(nameof(Artist));
            }
        }
        /// <summary>
        /// Path to poster image
        /// </summary>
        public string Poster
        {
            get => _poster;
            set
            {
                _poster = value;
                OnPropertyChanged(nameof(Poster));
            }
        }
        /// <summary>
        /// Path to music file in network
        /// </summary>
        public string RemoteAddress
        {
            get => _remoteAddress;
            set
            {
                _remoteAddress = value;
                OnPropertyChanged(nameof(RemoteAddress));
            }
        }

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(_id));
            }
        }

        public string LocalAddress
        {
            get => _localAddress;
            set
            {
                _localAddress = value;
                OnPropertyChanged(nameof(LocalAddress));
            }
        }
        /// <summary>
        /// Music duration
        /// </summary>
        public string Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                OnPropertyChanged(nameof(Duration));
            }
        }
        /// <summary>
        /// Source type of music file
        /// </summary>
        public SourceType Source
        {
            get => _source;
            set
            {
                _source = value;
                OnPropertyChanged(nameof(Source));
            }
        }

        public bool Liked
        {
            get => _liked;
            set
            {
                _liked = value;
                OnPropertyChanged(nameof(Liked));
            }
        }

        public bool Downloaded
        {
            get => _downloaded;
            set
            {
                _downloaded = value;
                OnPropertyChanged(nameof(Downloaded));
            }
        }

        private string GetArtist(string path) {
            var file = TagLib.File.Create(path);
            var art = file.Tag.FirstPerformer;
            if (art == null)
                _artist = "mp3";// Type.ToString();
            else
                _artist = art.Replace("[mp3xa.cc]", "")
                    .Replace("&amp;", "&")
                    .Replace("[drivemusic.me]", "")
                    .Replace("[mp3xa.me]", "")
                    .Replace("[muzmo.ru]", "");
            return Artist;
        }

        private string GetTitle(string path) {
            var file = TagLib.File.Create(path);
            var title = file.Tag.Title;
            if (title == null)
                _title = System.IO.Path.GetFileNameWithoutExtension(path);
            else
                _title = file.Tag.Title.Replace("[muzmo.ru]", "");
            return Title;
        }

        private string GetAddress(string file) {
            var path = System.IO.Path.GetFullPath(file);
            _localAddress = path;
            return LocalAddress;
        }

        private string GetDuration(string path) {
            var duration = "00:00";
            if (path != null) {
                var file = TagLib.File.Create(path);
                duration = file.Properties.Duration.ToString(@"mm\:ss");
            }
            _duration = duration;
            return Duration;
        }
        private void GetPoster(string path) {
            _poster = null;
        }

        private bool _isPlay;

        public bool IsPlay
        {
            get => _isPlay;
            set
            {
                _isPlay = value;
                OnPropertyChanged(nameof(IsPlay));
            }
        }

        private bool _isPaused;

        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                _isPaused = value;
                OnPropertyChanged(nameof(IsPaused));
            }
        }

        private bool _local;

        public bool Local
        {
            get => _local;
            set
            {
                _local = value;
                OnPropertyChanged(nameof(Local));
            }
        }
        
        private Location _location;
        /// <summary>
        /// Location
        /// </summary>
        public Location Location {
            get {
                return _location;
            }
            set {
                _location = value;
                OnPropertyChanged(nameof(Location));
            }
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        public MusicFile(){ }
        
        /// <summary>
        /// Music File Constructor
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="artist">Artist</param>
        /// <param name="poster">Poster</param>
        /// <param name="remoteAddress">Path</param>
        /// <param name="duration">Duration</param>
        public MusicFile(string title, string artist, string poster, string remoteAddress, string id, string duration)
        {
            _title = title;
            _artist = artist;
            _poster = poster;
            _remoteAddress = remoteAddress;
            _id = id;
            _duration = duration;
            _source = SourceType.Music;
            Location = Location.Internet;
            Local = false;
        }

        public MusicFile(string file)
        {
            GetTitle(file);
            GetArtist(file);
            GetAddress(file);
            GetDuration(file);
            GetPoster(file);
            _source = SourceType.Music;
            _id = Guid.NewGuid().ToString();
            Location = Location.Local;
            Local = true;
            //RemoteAddress = LocalAddress;
        }
        
        public override bool Equals(object obj)
        {
            var item = obj as MusicFile;
            if (item == null) return false;
            if (item.Id == this.Id)
                return true;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}