using Mov.Standard.Nico.Models;
using Mov.Standard.Nico.Workspaces;
using Mov.Standard.Windows;
using My.Wpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Mov.Standard.Nico.Components
{
    public class NicoMylistDetailViewModel : BindableBase
    {
        public NicoMylistDetailViewModel(NicoMylistModel source)
        {
            Source = source;
            // 初期値設定
            MylistTitle = Source.MylistTitle;
            MylistCreator = Source.MylistCreator;
            MylistDescription = Source.MylistDescription;
            UserId = Source.UserId;
            MylistDate = Source.MylistDate;

            // ﾓﾃﾞﾙ側で変更があったら通知する
            Source.AddOnPropertyChanged(this, (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(MylistTitle):
                        MylistTitle = Source.MylistTitle;
                        break;
                    case nameof(MylistCreator):
                        MylistCreator = Source.MylistCreator;
                        break;
                    case nameof(MylistDescription):
                        MylistDescription = Source.MylistDescription;
                        break;
                    case nameof(UserId):
                        UserId = Source.UserId;
                        break;
                    case nameof(MylistDate):
                        MylistDate = Source.MylistDate;
                        break;
                }
            });

            Loaded += async (sender, e) =>
            {
                try
                {
                    Thumbnail = await NicoUtil.GetThumbnailAsync(Source.ThumbnailUrl);
                }
                catch
                {
                    Thumbnail = null;
                }
            };
        }

        public NicoMylistModel Source { get; set; }

        public string MylistUrl => $"http://www.nicovideo.jp/mylist/{MylistId}?rss=2.0&numbers=1&sort={OrderBy}";

        public string MylistId
        {
            get { return _MylistId; }
            set { SetProperty(ref _MylistId, NicoUtil.ToNicolistId(value)); OnPropertyChanged(nameof(MylistUrl)); }
        }
        private string _MylistId = null;

        public string OrderBy
        {
            get { return _OrderBy; }
            set { SetProperty(ref _OrderBy, value); OnPropertyChanged(nameof(MylistUrl)); }
        }
        private string _OrderBy = null;

        public string MylistTitle
        {
            get { return _MylistTitle; }
            set { SetProperty(ref _MylistTitle, value); }
        }
        private string _MylistTitle = null;

        public string MylistCreator
        {
            get { return _MylistCreator; }
            set { SetProperty(ref _MylistCreator, value); }
        }
        private string _MylistCreator = null;

        public string MylistDescription
        {
            get { return _MylistDescription; }
            set { SetProperty(ref _MylistDescription, value); }
        }
        private string _MylistDescription = null;

        public string UserId
        {
            get { return _UserId; }
            set { SetProperty(ref _UserId, value); }
        }
        private string _UserId = null;

        public BitmapImage Thumbnail
        {
            get { return _Thumbnail; }
            set { SetProperty(ref _Thumbnail, value); }
        }
        private BitmapImage _Thumbnail;

        public DateTime MylistDate
        {
            get { return _MylistDate; }
            set { SetProperty(ref _MylistDate, value); }
        }
        private DateTime _MylistDate = default(DateTime);

        public ICommand OnLoaded => _OnLoaded = _OnLoaded ?? new RelayCommand(_ =>
        {
            Loaded?.Invoke(this, EventArgs.Empty);
        });
        private ICommand _OnLoaded;

        public event EventHandler Loaded;

        /// <summary>
        /// 項目ﾀﾞﾌﾞﾙｸﾘｯｸ時ｲﾍﾞﾝﾄ
        /// </summary>
        public ICommand OnDoubleClick => _OnDoubleClick = _OnDoubleClick ?? new RelayCommand(_ =>
        {
            // 検索画面を出す
            var vm = new NicoMylistViewModel();

            vm.Text = Source.MylistId;
            vm.OnClickSearch.Execute(null);

            MainViewModel.Instance.Current = vm;
        });
        public ICommand _OnDoubleClick;

        /// <summary>
        /// ﾏｲﾘｽﾄ削除ｲﾍﾞﾝﾄ
        /// </summary>
        public ICommand OnFavoriteDel
        {
            get
            {
                return _OnFavoriteDel = _OnFavoriteDel ?? new RelayCommand(
                async _ =>
                {
                    // ﾏｲﾘｽﾄ削除
                    await NicoFavoriteModel.Instance.DeleteFavorite(Source.MylistId);
                });
            }
        }
        public ICommand _OnFavoriteDel;

        /// <summary>
        /// 項目ｷｰ入力時ｲﾍﾞﾝﾄ
        /// </summary>
        public ICommand OnKeyDown
        {
            get
            {
                return _OnKeyDown = _OnKeyDown ?? new RelayCommand<KeyEventArgs>(
                e =>
                {
                    switch (e.Key)
                    {
                        case Key.Enter:
                            // ﾀﾞﾌﾞﾙｸﾘｯｸと同じ処理
                            OnDoubleClick.Execute(null);
                            break;
                        case Key.Delete:
                            // ﾏｲﾘｽﾄ削除
                            OnFavoriteDel.Execute(null);
                            break;
                    }
                });
            }
        }
        public ICommand _OnKeyDown;

    }
}
