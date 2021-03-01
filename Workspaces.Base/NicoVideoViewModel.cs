using Mov.Standard.Core;
using Mov.Standard.Core.Nico;
using Mov.Standard.Windows;
using My.Core;
using My.Wpf.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Mov.Standard.Workspaces.Base
{
    public class NicoVideoViewModel : BindableBase
    {
        private NicoVideoViewModel()
        {
            NicoUtil.Temporaries.CollectionChanged += Temporaries_Or_Histories_CollectionChanged;
            NicoUtil.Histories.CollectionChanged += Temporaries_Or_Histories_CollectionChanged;

            Disposed += (sender, e) =>
            {
                NicoUtil.Temporaries.CollectionChanged -= Temporaries_Or_Histories_CollectionChanged;
                NicoUtil.Histories.CollectionChanged -= Temporaries_Or_Histories_CollectionChanged;
            };
        }

        public NicoVideoViewModel(string videoid) : this()
        {
            Initialize(videoid);

        }

        public NicoVideoViewModel(NicoVideoModel x, bool reload = false) : this()
        {
            Initialize(x, reload);
        }

        private void Temporaries_Or_Histories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshVideoStatus(Status);
        }

        private void RefreshVideoStatus(VideoStatus _default)
        {
            // Temporaryの有無でﾌﾟﾛﾊﾟﾃｨを変更
            var tmp = NicoUtil.Temporaries.FirstOrDefault(x => x.VideoId == VideoId);
            if (tmp != null)
            {
                IsTemporary = true;
                TempTime = tmp.Date;
            }
            else
            {
                IsTemporary = false;
            }

            Status = NicoUtil.Histories.Any(x => x.VideoId == VideoId)
                ? VideoStatus.See
                : NicoUtil.Temporaries.Any(x => x.VideoId == VideoId && MainViewModel.Instance.LoadingTime < x.Date)
                ? VideoStatus.New
                : NicoUtil.Temporaries.Any(x => x.VideoId == VideoId)
                ? VideoStatus.Temporary
                : _default;

            StatusString = Status.GetLabel();
        }

        private async void Initialize(string videoid)
        {
            Initialize(await NicoUtil.GetVideo(videoid), false);
        }

        private async void Initialize(NicoVideoModel video, bool reload)
        {
            SetModel(video, false);

            if (reload)
            {
                await NicoUtil
                    .GetVideo(VideoId)
                    .ContinueWith(x => SetModel(x.Result, true));
            }

            var urls = new string[]
            {
                    $"{video.ThumbnailUrl}.L",
                    $"{video.ThumbnailUrl}.M",
                    $"{video.ThumbnailUrl}",
            };

            await NicoUtil
                .GetThumbnailAsync(urls)
                .ContinueWith(x => Thumbnail = x.Result);
        }

        private void SetModel(NicoVideoModel video, bool reload)
        {
            VideoId = video.VideoId;

            if (video.Status == VideoStatus.Delete) return;

            if (!reload) Title = video.Title;

            Description = video.Description;
            ViewCounter = video.ViewCounter;
            MylistCounter = video.MylistCounter;
            CommentCounter = video.CommentCounter;
            StartTime = video.StartTime;
            TempTime = video.TempTime;
            LengthSeconds = TimeSpan.FromSeconds(video.LengthSeconds);
            Username = video.Username;
            Tags = video.Tags;

            RefreshVideoStatus(video.Status);
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

        public TimeSpan LengthSeconds
        {
            get => _LengthSeconds;
            set => SetProperty(ref _LengthSeconds, value);
        }
        private TimeSpan _LengthSeconds;

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

        public string StatusString
        {
            get { return _StatusString; }
            set { SetProperty(ref _StatusString, value); }
        }
        private string _StatusString = null;

        public bool IsTemporary
        {
            get { return _IsTemporary; }
            set { SetProperty(ref _IsTemporary, value); }
        }
        private bool _IsTemporary = false;

        #endregion

        #region ｲﾍﾞﾝﾄ

        public ICommand OnDoubleClick => _OnDoubleClick = _OnDoubleClick ?? new RelayCommand(async _ =>
        {
            Process.Start(@"C:\Program Files\Google\Chrome\Application\chrome.exe", VideoUrl);

            // 詳細にあるidをtemporaryに追加
            foreach (var videoid in Regex.Matches(Description, @"(?<id>sm[\d]+)").OfType<Match>()
                    .Select(m => m.Groups["id"].Value)
                    .Where(tmp => !NicoUtil.Histories.Any(x => x.VideoId == tmp))
                )
            {
                await NicoUtil.AddTemporary(videoid);
            }

            // 視聴ﾘｽﾄに追加
            await NicoUtil.AddHistory(VideoId);

            // ｽﾃｰﾀｽをﾘﾌﾚｯｼｭ
            RefreshVideoStatus(Status);
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
            NicoUtil.Download(VideoId, Title);
        });
        private ICommand _OnDownload;

        public ICommand OnAdd => _OnAdd = _OnAdd ?? new RelayCommand(async _ =>
        {
            // Temporaryに追加
            await NicoUtil.AddTemporary(VideoId);

            // ｽﾃｰﾀｽをﾘﾌﾚｯｼｭ
            RefreshVideoStatus(Status);

        });
        private ICommand _OnAdd;

        public ICommand OnTemporaryDelete => _OnTemporaryDelete = _OnTemporaryDelete ?? new RelayCommand(async _ =>
        {
            // Temporaryから削除
            await NicoUtil.DeleteTemporary(VideoId);

            // ｽﾃｰﾀｽをﾘﾌﾚｯｼｭ
            RefreshVideoStatus(Status);
        });
        private ICommand _OnTemporaryDelete;

        public ICommand OnHistoryDelete => _OnHistoryDelete = _OnHistoryDelete ?? new RelayCommand(async _ =>
        {
            // Historyから削除
            await NicoUtil.DeleteHistory(VideoId);

            // ｽﾃｰﾀｽをﾘﾌﾚｯｼｭ
            RefreshVideoStatus(Status);
        });
        private ICommand _OnHistoryDelete;

        #endregion
    }
}
