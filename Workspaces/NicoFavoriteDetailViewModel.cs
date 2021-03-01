using Mov.Standard.Core;
using Mov.Standard.Core.Nico;
using Mov.Standard.Windows;
using Mov.Standard.Workspaces.Base;
using My.Core;
using My.Wpf.Core;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mov.Standard.Workspaces
{
    public class NicoFavoriteDetailViewModel : WorkspaceViewModel
    {
        public override string Title => MenuType.NicoFavoriteDetail.GetLabel();

        public NicoFavoriteDetailViewModel(string mylistid) : this()
        {
            AddLoadTask(() =>
            {
                Text = mylistid;

            });

            Disposed += (sender, e) =>
            {
                Videos.ForEach(x => x.Dispose());
                Videos.ClearOnUI();
            };
        }

        public NicoFavoriteDetailViewModel()
        {
            Videos = new ObservableCollection<NicoVideoViewModel>();

            Orders = new ObservableCollection<ComboboxItemModel>(
                NicoUtil.Combos
                    .Where(combo => combo.Group == "mylist_order")
            );
            SelectedOrder = Orders.FirstOrDefault(genre => genre.Value == AppSetting.Instance.NicoMylistOrder) ?? Orders.First();

            Disposed += (sender, e) =>
            {
                AppSetting.Instance.NicoMylistOrder = SelectedOrder.Value;
                AppSetting.Instance.Save();
            };
        }

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

        public NicoMylistViewModel Source
        {
            get => _Source;
            set => SetProperty(ref _Source, value);
        }
        public NicoMylistViewModel _Source;

        public ObservableCollection<NicoVideoViewModel> Videos
        {
            get => _Videos;
            set => SetProperty(ref _Videos, value);
        }
        public ObservableCollection<NicoVideoViewModel> _Videos;

        public bool IsCreatorVisible => Videos != null ? Videos.Any() : false;

        #region ｲﾍﾞﾝﾄ

        public ICommand OnClickSearch => _OnClickSearch = _OnClickSearch ?? new RelayCommand(async _ =>
        {
            // 入力値をﾓﾃﾞﾙにｾｯﾄ
            Source = new NicoMylistViewModel(NicoUtil.ToNicolistId(Text), SelectedOrder.Value);

            Videos.ClearOnUI();

            await NicoUtil.GetMylistVideos(NicoUtil.ToNicolistId(Text), SelectedOrder.Value)
                .ContinueWith(x => Videos.AddRangeOnUI(x.Result.Select(video => new NicoVideoViewModel(video))));

            // ｵｰﾅｰ情報を表示するかどうか
            OnPropertyChanged(nameof(IsCreatorVisible));
        });
        private ICommand _OnClickSearch;

        public ICommand OnClickAdd => _OnClickAdd = _OnClickAdd ?? new RelayCommand(async _ =>
        {
            await NicoUtil.AddFavorite(NicoUtil.ToNicolistId(Text));
        });
        private ICommand _OnClickAdd;

        public ICommand OnClickDelete => _OnClickDelete = _OnClickDelete ?? new RelayCommand(async _ =>
        {
            await NicoUtil.DeleteFavorite(NicoUtil.ToNicolistId(Text));
        });
        private ICommand _OnClickDelete;

        #endregion

    }
}
