using My.Wpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Mov.Standard.Core.Nico
{
    public class NicoVideoModel : BindableBase
    {
        public string VideoId
        {
            get => _VideoId;
            set => SetProperty(ref _VideoId, value);
        }
        private string _VideoId;

        public string Title
        {
            get => _Title;
            set => SetProperty(ref _Title, HttpUtility.HtmlDecode(value));
        }
        private string _Title;

        public string Description
        {
            get => _Description;
            set => SetProperty(ref _Description, HttpUtility.HtmlDecode(value));
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

        public string ThumbnailUrl
        {
            get => _ThumbnailUrl;
            set => SetProperty(ref _ThumbnailUrl, value);
        }
        private string _ThumbnailUrl;

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

    }
}
