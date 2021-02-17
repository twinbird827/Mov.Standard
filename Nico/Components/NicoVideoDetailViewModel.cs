﻿using Mov.Standard.Models;
using Mov.Standard.Nico.Models;
using My.Core;
using My.Wpf.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Mov.Standard.Nico.Components
{
    public class NicoVideoDetailViewModel : BindableBase
    {
        public NicoVideoDetailViewModel(NicoVideoModel source)
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
            LengthSeconds = TimeSpan.FromSeconds(Source.LengthSeconds);
            ThumbnailUrl = Source.ThumbnailUrl;
            Username = Source.Username;
            Status = Source.Status == VideoStatus.Delete
                ? VideoStatus.Delete
                : NicoVideoHistoryModel.Instance.IsSee(VideoId)
                ? VideoStatus.See
                : NicoVideoHistoryModel.Instance.IsNew(VideoId)
                ? VideoStatus.New
                : NicoTemporaryModel.Instance.IsTemporary(VideoId)
                ? VideoStatus.Favorite
                : VideoStatus.None;

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
                        LengthSeconds = TimeSpan.FromSeconds(Source.LengthSeconds);
                        break;
                    case nameof(ThumbnailUrl):
                        ThumbnailUrl = Source.ThumbnailUrl;
                        break;
                    case nameof(Username):
                        Username = Source.Username;
                        break;
                    case nameof(Status):
                        Status = Source.Status;
                        break;
                }
            });

            Loaded += async (sender, e) =>
            {
                await NicoUtil.ReloadVideoAsync(Source);

                Thumbnail = await NicoUtil.GetThumbnailAsync(this);
            };
        }

        public NicoVideoModel Source { get; private set; }

        public string VideoUrl => $"http://nico.ms/{VideoId}";

        public string VideoId
        {
            get { return _VideoId; }
            set { SetProperty(ref _VideoId, NicoUtil.ToVideoId(value)); OnPropertyChanged(nameof(VideoUrl)); }
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

        public TimeSpan LengthSeconds
        {
            get { return _LengthSeconds; }
            set { SetProperty(ref _LengthSeconds, value); }
        }
        private TimeSpan _LengthSeconds = TimeSpan.Zero;

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

        public ICommand OnLoaded => _OnLoaded = _OnLoaded ?? new RelayCommand(_ =>
        {
            Loaded?.Invoke(this, EventArgs.Empty);
        });
        private ICommand _OnLoaded;

        public event EventHandler Loaded;

        public ICommand OnDoubleClick => _OnDoubleClick = _OnDoubleClick ?? new RelayCommand(async _ =>
        {
            Process.Start(@"C:\Program Files\Google\Chrome\Application\chrome.exe", VideoUrl);

            Source.Status = VideoStatus.See;

            // 視聴ﾘｽﾄに追加
            await NicoVideoHistoryModel.Instance.AddVideoHistory(Source.VideoId, VideoStatus.See);

            // 詳細にあるidをtemporaryに追加
            foreach (var id in Regex.Matches(Description, @"(?<id>sm[\d]+)").OfType<Match>()
                    .Select(m => m.Groups["id"].Value)
                    .Where(tmp => !NicoVideoHistoryModel.Instance.Exists(tmp))
                )
            {
                await NicoUtil.AddVideo(id);
            }
        });
        private ICommand _OnDoubleClick;

        public ICommand OnKeyDown => _OnKeyDown = _OnKeyDown ?? new RelayCommand<KeyEventArgs>(e =>
        {
            if (e.Key == Key.Enter)
            {
                OnDoubleClick.Execute(null);
            }
        });
        private ICommand _OnKeyDown;

        public ICommand OnDownload => _OnDownload = _OnDownload ?? new RelayCommand(_ =>
        {
            NicoUtil.Download(Source);
        });
        private ICommand _OnDownload;

        public ICommand OnAdd => _OnAdd = _OnAdd ?? new RelayCommand(async _ =>
        {
            if (!NicoTemporaryModel.Instance.Videos.Any(video => video.VideoId == Source.VideoId))
            {
                await NicoUtil.AddVideo(Source);
            }
        });
        private ICommand _OnAdd;

        public ICommand OnDelete => _OnDelete = _OnDelete ?? new RelayCommand(async _ =>
        {
            if (NicoTemporaryModel.Instance.Videos.Any(video => video.VideoId == Source.VideoId))
            {
                await NicoUtil.DeleteVideo(Source);
            }
        });
        private ICommand _OnDelete;

    }
}
