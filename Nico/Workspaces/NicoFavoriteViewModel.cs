using Mov.Standard.Nico.Components;
using Mov.Standard.Nico.Models;
using Mov.Standard.Windows;
using My.Wpf;
using My.Wpf.Core;
using My.Wpf.Services;
using Notifications.Wpf;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mov.Standard.Nico.Workspaces
{
    public class NicoFavoriteViewModel : WorkspaceViewModel
    {
        public NicoFavoriteViewModel()
        {
            Views = NicoFavoriteModel.Instance.Mylists.ToSyncedSynchronizationContextCollection(
                mylist => new Components.NicoMylistDetailViewModel(mylist),
                System.Threading.SynchronizationContext.Current
            );
        }

        public override string Title => "Favorite";

        public SynchronizationContextCollection<NicoMylistDetailViewModel> Views
        {
            get { return _Views; }
            set { SetProperty(ref _Views, value); }
        }
        private SynchronizationContextCollection<NicoMylistDetailViewModel> _Views;

        public ICommand OnClickAdd => _OnClickAdd = _OnClickAdd ?? new RelayCommand(async _ =>
        {
            using (var vm = new WpfMessageInputViewModel(
                    "",
                    "追加する Url を入力してください。",
                    "Url",
                    true
                ))
            {
                var dialog = new WpfMessageInputWindow(vm);

                if (!dialog.ShowModalWindow())
                {
                    return;
                }

                await NicoFavoriteModel.Instance.AddFavorite(vm.Input);
            }
        });
        private ICommand _OnClickAdd;

        public ICommand OnClickDelete => _OnClickDelete = _OnClickDelete ?? new RelayCommand(_ =>
        {
            MainViewModel.Instance.ShowToast("Test delete", NotificationType.Information);
        });
        private ICommand _OnClickDelete;

    }
}
