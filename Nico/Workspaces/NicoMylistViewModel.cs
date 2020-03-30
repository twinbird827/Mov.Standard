using Mov.Standard.Models;
using Mov.Standard.Nico.Components;
using Mov.Standard.Nico.Models;
using Mov.Standard.Windows;
using My.Wpf.Core;
using Notifications.Wpf;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Mov.Standard.Nico.Workspaces
{
    public class NicoMylistViewModel : NicoWorkspaceViewModel
    {
        public NicoMylistViewModel()
        {
            Loaded += (sender, e) =>
            {
                Orders = new ObservableCollection<ComboboxItemModel>(
                    MovModel.Instance.Combos
                        .Where(combo => combo.Group == "mylist_order")
                );
                SelectedOrder = Orders.First();
            };
        }

        public override string Title => "Mylist";

        public NicoMylistModel Source
        {
            get => _Source;
            set => SetProperty(ref _Source, value, true);
        }
        private NicoMylistModel _Source;

        public ObservableCollection<ComboboxItemModel> Orders
        {
            get => _Orders;
            set => SetProperty(ref _Orders, value);
        }
        public ObservableCollection<ComboboxItemModel> _Orders;

        public ComboboxItemModel SelectedOrder
        {
            get => _SelectedOrder;
            set => SetProperty(ref _SelectedOrder, value);
        }
        public ComboboxItemModel _SelectedOrder;

        public string Text
        {
            get => _Text;
            set => SetProperty(ref _Text, value);
        }
        public string _Text;

        public string MylistId
        {
            get { return _MylistId; }
            set { SetProperty(ref _MylistId, value); }
        }
        private string _MylistId = null;

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

        public SynchronizationContextCollection<NicoVideoDetailViewModel> Views
        {
            get { return _Views; }
            set { SetProperty(ref _Views, value); }
        }
        private SynchronizationContextCollection<NicoVideoDetailViewModel> _Views;

        public bool IsCreatorVisible => Views != null ? Views.Any() : false;

        public ICommand OnClickSearch
        {
            get
            {
                return _OnClickSearch = _OnClickSearch ?? new RelayCommand(async _ =>
                {
                    // 入力値をﾓﾃﾞﾙにｾｯﾄ
                    Source = await NicoMylistModel.CreateAsync(Text, SelectedOrder.Value);

                    MylistId = Source.MylistId;
                    MylistTitle = Source.MylistTitle;
                    MylistCreator = Source.MylistCreator;
                    MylistDescription = Source.MylistDescription;
                    UserId = Source.UserId;
                    Thumbnail = await NicoUtil.GetThumbnailAsync(Source.ThumbnailUrl);
                    MylistDate = Source.MylistDate;

                    Views?.Clear();
                    Views?.Dispose();
                    Views = Source.Videos.ToSyncedSynchronizationContextCollection(
                        video => new NicoVideoDetailViewModel(video),
                        System.Threading.SynchronizationContext.Current
                    );

                    // ｵｰﾅｰ情報を表示するかどうか
                    OnPropertyChanged(nameof(IsCreatorVisible));
                },
                _ => {
                    return !string.IsNullOrWhiteSpace(Text);
                });
            }
        }
        public ICommand _OnClickSearch;

        public ICommand OnClickAdd
        {
            get
            {
                return _OnClickAdd = _OnClickAdd ?? new RelayCommand(async _ =>
                {
                    await NicoFavoriteModel.Instance.AddFavorite(Text);

                    MainViewModel.Instance.ShowToast("追加しました。", NotificationType.Information);
                },
                _ => {
                    return IsCreatorVisible && !NicoFavoriteModel.Instance.Exists(Text);
                });
            }
        }
        public ICommand _OnClickAdd;

        public ICommand OnClickDelete
        {
            get
            {
                return _OnClickDelete = _OnClickDelete ?? new RelayCommand(async _ =>
                {
                    await NicoFavoriteModel.Instance.DeleteFavorite(Text);

                    MainViewModel.Instance.ShowToast("削除しました。", NotificationType.Information);
                },
                _ => {
                    return NicoFavoriteModel.Instance.Exists(Text);
                });
            }
        }
        public ICommand _OnClickDelete;

    }
}
