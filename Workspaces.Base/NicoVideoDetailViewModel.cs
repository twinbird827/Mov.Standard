using Mov.Standard.Core;
using Mov.Standard.Core.Nico;
using My.Wpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Mov.Standard.Workspaces.Base
{
    public class NicoVideoDetailViewModel : BindableBase
    {
        public NicoVideoDetailViewModel()
        {

        }

        private async void Intialize(NicoVideoModel x)
        {
            SetModel(x);

            var urls = new string[]
            {
                    $"{x.ThumbnailUrl}.L",
                    $"{x.ThumbnailUrl}.M",
                    $"{x.ThumbnailUrl}",
            };

            await NicoUtil
                .GetThumbnailAsync(urls)
                .ContinueWith(image => Thumbnail = image.Result);
        }

        private void SetModel(NicoVideoModel x)
        {
            VideoId = x.VideoId;
            Title = x.Title;
            Description = x.Description;
            ViewCounter = x.ViewCounter;
            MylistCounter = x.MylistCounter;
            CommentCounter = x.CommentCounter;
            StartTime = x.StartTime;
            TempTime = x.TempTime;
            LengthSeconds = x.LengthSeconds;
            Username = x.Username;
            Status = x.Status;
            Tags = x.Tags;
        }

        #region ﾌﾟﾛﾊﾟﾃｨ

        public string VideoUrl => $"http://nico.ms/{VideoId}";

        public string VideoId
        {
            get => _VideoId;
            set { if (SetProperty(ref _VideoId, value)) OnPropertyChanged(nameof(VideoUrl)); }
        }
        private string _VideoId;

        public string Title
        {
            get => _Title;
            set => SetProperty(ref _Title, value);
        }
        private string _Title;

        public string Description
        {
            get => _Description;
            set => SetProperty(ref _Description, value);
        }
        private string _Description;

        public long ViewCounter
        {
            get => _ViewCounter;
            set => SetProperty(ref _ViewCounter, value);
        }
        private long _ViewCounter;

        public long MylistCounter
        {
            get => _MylistCounter;
            set => SetProperty(ref _MylistCounter, value);
        }
        private long _MylistCounter;

        public long CommentCounter
        {
            get => _CommentCounter;
            set => SetProperty(ref _CommentCounter, value);
        }
        private long _CommentCounter;

        public DateTime StartTime
        {
            get => _StartTime;
            set => SetProperty(ref _StartTime, value);
        }
        private DateTime _StartTime;

        public DateTime TempTime
        {
            get => _TempTime;
            set => SetProperty(ref _TempTime, value);
        }
        private DateTime _TempTime;

        public BitmapImage Thumbnail
        {
            get => _Thumbnail;
            set => SetProperty(ref _Thumbnail, value);
        }
        private BitmapImage _Thumbnail;

        public long LengthSeconds
        {
            get => _LengthSeconds;
            set => SetProperty(ref _LengthSeconds, value);
        }
        private long _LengthSeconds;

        public string Username
        {
            get { return _Username; }
            set { SetProperty(ref _Username, value); }
        }
        private string _Username = null;

        public VideoStatus Status
        {
            get => _Status;
            set => SetProperty(ref _Status, value);
        }
        private VideoStatus _Status = VideoStatus.None;

        public string Tags
        {
            get { return _Tags; }
            set { SetProperty(ref _Tags, value); }
        }
        private string _Tags = null;

        #endregion


    }
}
