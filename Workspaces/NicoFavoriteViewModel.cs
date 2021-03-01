using Mov.Standard.Core.Nico;
using Mov.Standard.Windows;
using Mov.Standard.Workspaces.Base;
using My.Core;
using My.Wpf;
using My.Wpf.Core;
using My.Wpf.Services;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mov.Standard.Workspaces
{
    public class NicoFavoriteViewModel : WorkspaceViewModel
    {
        public override string Title => MenuType.NicoFavorite.GetLabel();

        public NicoFavoriteViewModel()
        {
            AddLoadTask(() =>
            {
                Mylists = NicoUtil.Favorites.ToSyncedSynchronizationContextCollection(
                    x => new NicoMylistViewModel(x.MylistId),
                    System.Threading.SynchronizationContext.Current
                );
            });

            Disposed += (sender, e) =>
            {
                Mylists.ForEach(x => x.Dispose());
                Mylists.ClearOnUI();
            };
        }

        public SynchronizationContextCollection<NicoMylistViewModel> Mylists
        {
            get => _Mylists;
            set => SetProperty(ref _Mylists, value);
        }
        public SynchronizationContextCollection<NicoMylistViewModel> _Mylists;

        public NicoMylistViewModel SelectedMylist
        {
            get => _SelectedMylist;
            set => SetProperty(ref _SelectedMylist, value);
        }
        public NicoMylistViewModel _SelectedMylist;

        #region ｲﾍﾞﾝﾄ

        public ICommand OnClickAdd => _OnClickAdd = _OnClickAdd ?? new RelayCommand(async _ =>
        {
            using (var vm = new WpfMessageInputViewModel("Add Mylist", "Enter the Mylist you want to add.", "Mylist", true))
            {
                var dialog = new WpfMessageInputWindow(vm);

                if (!dialog.ShowModalWindow())
                {
                    return;
                }

                await NicoUtil.AddFavorite(vm.Input);
            }
        });
        private ICommand _OnClickAdd;

        public ICommand OnClickDelete => _OnClickDelete = _OnClickDelete ?? new RelayCommand(async _ =>
        {
            if (SelectedMylist == null) return;

            await NicoUtil.DeleteFavorite(SelectedMylist.MylistId);
        });
        private ICommand _OnClickDelete;

        #endregion

    }
}
