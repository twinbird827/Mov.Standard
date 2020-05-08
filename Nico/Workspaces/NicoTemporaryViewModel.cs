using Mov.Standard.Nico.Components;
using Mov.Standard.Nico.Models;
using Mov.Standard.Windows;
using My.Core;
using My.Wpf.Core;
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
    public class NicoTemporaryViewModel : WorkspaceViewModel
    {
        public NicoTemporaryViewModel()
        {
            Loaded += async (sender, e) =>
            {
                NicoTemporaryModel.Instance.Videos.Clear();
                NicoTemporaryModel.Instance.Videos.AddRange(await NicoUtil.GetTemporary());
                NicoTemporaryModel.Instance.Count = NicoTemporaryModel.Instance.Videos.Count;

                Views = NicoTemporaryModel.Instance.Videos.ToSyncedSynchronizationContextCollection(
                    video => new NicoVideoDetailViewModel(video),
                    System.Threading.SynchronizationContext.Current
                );
            };
        }

        public override string Title => "Temporary";

        public SynchronizationContextCollection<NicoVideoDetailViewModel> Views
        {
            get { return _Views; }
            set { SetProperty(ref _Views, value); }
        }
        private SynchronizationContextCollection<NicoVideoDetailViewModel> _Views;

        public ICommand OnClickAdd => _OnClickAdd = _OnClickAdd ?? new RelayCommand(_ =>
        {
            MainViewModel.Instance.ShowToast("Test add", NotificationType.Information);
        });
        private ICommand _OnClickAdd;

        public ICommand OnClickDelete => _OnClickDelete = _OnClickDelete ?? new RelayCommand(_ =>
        {
            MainViewModel.Instance.ShowToast("Test delete", NotificationType.Information);
        });
        private ICommand _OnClickDelete;

    }
}
