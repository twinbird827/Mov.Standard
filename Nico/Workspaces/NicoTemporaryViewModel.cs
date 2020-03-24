using Mov.Standard.Nico.Components;
using Mov.Standard.Nico.Models;
using My.Core;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Nico.Workspaces
{
    public class NicoTemporaryViewModel : NicoWorkspaceViewModel
    {
        public NicoTemporaryViewModel()
        {
            Views = NicoTemporaryModel.Instance.Videos.ToSyncedSynchronizationContextCollection(
                video => new NicoVideoViewModel(video),
                System.Threading.SynchronizationContext.Current
            );
        }

        public override string Title => "Temporary";

        public SynchronizationContextCollection<NicoVideoViewModel> Views
        {
            get { return _Views; }
            set { SetProperty(ref _Views, value); }
        }
        private SynchronizationContextCollection<NicoVideoViewModel> _Views;

    }
}
