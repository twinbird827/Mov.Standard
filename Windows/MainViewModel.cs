using Mov.Standard.Core.Database;
using Mov.Standard.Core.Nico;
using Mov.Standard.Workspaces;
using My.Core;
using My.Wpf.Core;
using My.Wpf.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Mov.Standard.Windows
{
    public class MainViewModel : MainViewModelBase
    {
        public MainViewModel()
        {
            if (!WpfUtil.IsDesignMode() && Instance != null)
            {
                throw new InvalidOperationException("This ViewModel cannot create multiple instances.");
            }
            Instance = this;
        }

        #region ﾌﾟﾛﾊﾟﾃｨ

        /// <summary>
        /// 本ｲﾝｽﾀﾝｽ(ｼﾝｸﾞﾙﾄﾝ)
        /// </summary>
        public static MainViewModel Instance { get; private set; }

        /// <summary>
        /// ｶﾚﾝﾄﾜｰｸｽﾍﾟｰｽ
        /// </summary>
        public WorkspaceViewModel Current
        {
            get { return _Current; }
            set { SetProperty(ref _Current, value, true); }
        }
        private WorkspaceViewModel _Current;

        public MovieType MovieType
        {
            get { return _MovieType; }
            set { SetProperty(ref _MovieType, value); }
        }
        private MovieType _MovieType = MovieType.Niconico;

        public string TemporaryString => $"Temp ({NicoUtil.Temporaries?.Count ?? 0})";

        /// <summary>
        /// ﾛｸﾞｵﾌﾀｲﾏｰ
        /// </summary>
        public DispatcherTimer CheckTimer { get; private set; }

        #endregion

        #region 起動・終了処理

        protected override async Task DoLoading(ProgressViewModel vm)
        {
            // TODO 設定ﾌｧｲﾙの初期化

            using (var command = await DbUtil.GetControl())
            {
                // ﾃﾞｰﾀﾍﾞｰｽの初期化
                await DbUtil.Initialize(command);

                // Nico状態の初期化
                await NicoUtil.Initialize(command);
            }

            // 自動ﾀｲﾏｰ起動
            CheckTimer = new DispatcherTimer();
            CheckTimer.Interval = TimeSpan.FromMinutes(5);
            CheckTimer.Tick += async (sender, e) =>
            {
                foreach (var x in NicoUtil.Favorites)
                {
                    // 全件取得
                    var videos = await NicoUtil.GetMylistVideos(x.MylistId, "0");
                    // 対象絞り込み
                    var targets = videos
                        .Where(video => x.Date < video.StartTime)
                        .ToArray();

                    if (targets.Any())
                    {
                        await targets
                            .Select(async video => await NicoUtil.AddTemporary(video.VideoId))
                            .WhenAll();

                        await NicoUtil.AddFavorite(x.MylistId, targets.Max(target => target.StartTime));
                    }
                }
            };
            CheckTimer.Start();

            // TemporaryStringの更新ｲﾍﾞﾝﾄ関連付け
            NicoUtil.Temporaries.CollectionChanged += (sender, e) =>
            {
                OnPropertyChanged(nameof(TemporaryString));
            };
            OnPropertyChanged(nameof(TemporaryString));

            ChangeCurrent(MenuType.NicoSearch);

            await Current.ExecuteLoadTasks();
        }

        protected override bool DoClosing()
        {
            // TODO 設定ﾌｧｲﾙを閉じる
            CheckTimer.Stop();

            return base.DoClosing();
        }

        #endregion

        public ICommand OnClickNiconico => _OnClickNiconico = _OnClickNiconico ?? new RelayCommand(_ =>
        {
            MovieType = MovieType.Youtube;
        });
        private ICommand _OnClickNiconico;

        public ICommand OnClickYoutube => _OnClickYoutube = _OnClickYoutube ?? new RelayCommand(_ =>
        {
            MovieType = MovieType.Niconico;
        });
        private ICommand _OnClickYoutube;

        public ICommand OnClickMenu => _OnClickMenu = _OnClickMenu ?? new RelayCommand<MenuType>(
            ChangeCurrent
        );
        private ICommand _OnClickMenu;

        public void ChangeCurrent(MenuType menu)
        {
            var newtype = _menu[menu];
            if (Current != null && newtype == Current.GetType())
            {
                return;
            }
            else
            {
                Current = Activator.CreateInstance(newtype) as WorkspaceViewModel;
            }
        }

        private Dictionary<MenuType, Type> _menu = new Dictionary<MenuType, Type>()
        {
            [MenuType.NicoSearch] = typeof(NicoSearchViewModel),
            [MenuType.NicoTemporary] = typeof(NicoTemporaryViewModel),
            [MenuType.NicoFavorite] = typeof(NicoFavoriteViewModel),
            [MenuType.NicoFavoriteDetail] = typeof(NicoFavoriteDetailViewModel),
            [MenuType.NicoHistory] = typeof(NicoHistoryViewModel),
        };

    }
}
