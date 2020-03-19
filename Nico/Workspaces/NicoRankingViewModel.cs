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
    public class NicoRankingViewModel : NicoWorkspaceViewModel
    {
        public NicoRankingViewModel()
        {
            Videos = new ObservableSynchronizedCollection<NicoVideoModel>();
            Views = Videos.ToSyncedSynchronizationContextCollection(
                video => new NicoVideoViewModel(video),
                System.Threading.SynchronizationContext.Current
            );

            Loaded += async (sender, e) =>
            {
                Videos.AddRange(await NicoUtil.GetRanking("hourly", "all", "all"));

                foreach (var video in Videos)
                {
                    await NicoUtil.ReloadVideoAsync(video);
                }
            };
        }

        public override string Title => "Ranking";

        public ObservableSynchronizedCollection<NicoVideoModel> Videos
        {
            get => _Videos;
            set => SetProperty(ref _Videos, value);
        }
        public ObservableSynchronizedCollection<NicoVideoModel> _Videos;

        public SynchronizationContextCollection<NicoVideoViewModel> Views
        {
            get { return _Views; }
            set { SetProperty(ref _Views, value); }
        }
        private SynchronizationContextCollection<NicoVideoViewModel> _Views;

    }
}
