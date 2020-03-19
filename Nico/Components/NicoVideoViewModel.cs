using Mov.Standard.Models;
using Mov.Standard.Nico.Models;
using My.Core;
using My.Wpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Media.Imaging;

namespace Mov.Standard.Nico.Components
{
    public class NicoVideoViewModel : BindableBase
    {
        public NicoVideoViewModel(NicoVideoModel source)
        {
            Source = source;

            VideoId = Source.VideoId;
            Title = Source.Title;
            Description = Source.Description;
            Tags = Source.Tags;
            ViewCounter = Source.ViewCounter;
            MylistCounter = Source.MylistCounter;
            CommentCounter = Source.CommentCounter;
            StartTime = Source.StartTime;
            LengthSeconds = Source.LengthSeconds;
            ThumbnailUrl = Source.ThumbnailUrl;
            Username = Source.Username;
            Status = Source.Status;

            NicoUtil.SetThumbnail(this);

            Source.AddOnPropertyChanged(this, (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(Title):
                        Title = Source.Title;
                        break;
                    case nameof(Description):
                        Description = Source.Description;
                        break;
                    case nameof(Tags):
                        Tags = Source.Tags;
                        break;
                    case nameof(ViewCounter):
                        ViewCounter = Source.ViewCounter;
                        break;
                    case nameof(MylistCounter):
                        MylistCounter = Source.MylistCounter;
                        break;
                    case nameof(CommentCounter):
                        CommentCounter = Source.CommentCounter;
                        break;
                    case nameof(StartTime):
                        StartTime = Source.StartTime;
                        break;
                    case nameof(LengthSeconds):
                        LengthSeconds = Source.LengthSeconds;
                        break;
                    case nameof(ThumbnailUrl):
                        ThumbnailUrl = Source.ThumbnailUrl;
                        NicoUtil.SetThumbnail(this);
                        break;
                    case nameof(Username):
                        Username = Source.Username;
                        break;
                    case nameof(Status):
                        Status = Source.Status;
                        break;
                }
            });
        }

        public NicoVideoModel Source { get; private set; }

        public string VideoUrl => $"http://nico.ms/{VideoId}";

        public string VideoId
        {
            get { return _VideoId; }
            set { SetProperty(ref _VideoId, value); OnPropertyChanged(nameof(VideoUrl)); }
        }
        private string _VideoId = null;

        public string Title
        {
            get { return _Title; }
            set { SetProperty(ref _Title, value); }
        }
        private string _Title = null;

        public string Description
        {
            get { return _Description; }
            set { SetProperty(ref _Description, value); }
        }
        private string _Description = null;

        public string Tags
        {
            get { return _Tags; }
            set { SetProperty(ref _Tags, value); }
        }
        private string _Tags = null;

        public double ViewCounter
        {
            get { return _ViewCounter; }
            set { SetProperty(ref _ViewCounter, value); }
        }
        private double _ViewCounter = default(int);

        public double MylistCounter
        {
            get { return _MylistCounter; }
            set { SetProperty(ref _MylistCounter, value); }
        }
        private double _MylistCounter = default(int);

        public double CommentCounter
        {
            get { return _CommentCounter; }
            set { SetProperty(ref _CommentCounter, value); }
        }
        private double _CommentCounter = default(int);

        public DateTime StartTime
        {
            get { return _StartTime; }
            set { SetProperty(ref _StartTime, value); }
        }
        private DateTime _StartTime = default(DateTime);

        public long LengthSeconds
        {
            get { return _LengthSeconds; }
            set { SetProperty(ref _LengthSeconds, value); }
        }
        private long _LengthSeconds = default(long);

        public string ThumbnailUrl
        {
            get { return _ThumbnailUrl; }
            set { SetProperty(ref _ThumbnailUrl, value); }
        }
        private string _ThumbnailUrl = null;

        public BitmapImage Thumbnail
        {
            get => _Thumbnail;
            set => SetProperty(ref _Thumbnail, value);
        }
        private BitmapImage _Thumbnail;

        public string Username
        {
            get { return _Username; }
            set { SetProperty(ref _Username, value); }
        }
        private string _Username = null;

        public VideoStatus Status
        {
            get { return _Status; }
            set { if (SetProperty(ref _Status, value)) OnPropertyChanged(nameof(StatusString)); }
        }
        private VideoStatus _Status = VideoStatus.None;

        /// <summary>
        /// ｽﾃｰﾀｽ文字
        /// </summary>
        public string StatusString => Status.GetLabel();

    }
}
