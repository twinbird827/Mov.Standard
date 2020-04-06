using My.Core;
using My.Wpf.Core;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Nico.Models
{
    public class NicoTemporaryModel : BindableBase
    {
        public static NicoTemporaryModel Instance { get; private set; } = new NicoTemporaryModel();

        public SortedObservableCollection<NicoVideoModel, DateTime> Videos
        {
            get => _Videos;
            set => SetProperty(ref _Videos, value);
        }
        public SortedObservableCollection<NicoVideoModel, DateTime> _Videos;

        public int Count
        {
            get => _Count;
            set => SetProperty(ref _Count, value);
        }
        private int _Count;

        public async Task LoadAsync()
        {
            Videos = new SortedObservableCollection<NicoVideoModel, DateTime>(
                await NicoUtil.GetTemporary(),
                video => video.StartTime,
                true
            );
            Count = Videos.Count;
        }

        public bool IsTemporary(string url)
        {
            return Videos.Any(video => video.VideoId == NicoUtil.ToVideoId(url));
        }
    }
}
