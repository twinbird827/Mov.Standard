using Mov.Standard.Models;
using Mov.Standard.Nico.Components;
using Mov.Standard.Nico.Models;
using My.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Nico.Workspaces
{
    public class NicoVideoHistoryViewModel : NicoWorkspaceViewModel
    {
        public NicoVideoHistoryViewModel()
        {
            Loaded += async (sender, e) =>
            {
                var histories = await NicoVideoHistoryModel.Instance.Histories
                    .GroupBy(history => history.VideoId)
                    .Select(group => new
                    {
                        VideoId = group.Key,
                        Date = group.Max(history => history.Date),
                        Count = group.Count(),
                        Status = VideoStatus.See
                    })
                    .Select(group => NicoVideoHistoryDetailViewModel.Create(group.VideoId, group.Date, group.Count))
                    .WhenAll();

                Views = new ObservableCollection<NicoVideoHistoryDetailViewModel>(histories);
            };
        }

        public override string Title => "VideoHistory";

        public ObservableCollection<NicoVideoHistoryDetailViewModel> Views
        {
            get { return _Views; }
            set { SetProperty(ref _Views, value); }
        }
        private ObservableCollection<NicoVideoHistoryDetailViewModel> _Views;

    }
}
