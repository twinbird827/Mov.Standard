using Mov.Standard.Core.Nico;
using Mov.Standard.Windows;
using Mov.Standard.Workspaces.Base;
using My.Core;
using My.Wpf.Core;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Workspaces
{
    public class NicoHistoryViewModel : WorkspaceViewModel
    {
        public override string Title => MenuType.NicoTemporary.GetLabel();

        public NicoHistoryViewModel()
        {
            AddLoadTask(() =>
            {
                Videos = NicoUtil.Histories.ToSyncedSynchronizationContextCollection(
                    x => new NicoVideoViewModel(x.VideoId),
                    System.Threading.SynchronizationContext.Current
                );
            });

            Disposed += (sender, e) =>
            {
                Videos.ForEach(x => x.Dispose());
                Videos.ClearOnUI();
            };
        }

        public SynchronizationContextCollection<NicoVideoViewModel> Videos
        {
            get => _Videos;
            set => SetProperty(ref _Videos, value);
        }
        public SynchronizationContextCollection<NicoVideoViewModel> _Videos;

    }
}
