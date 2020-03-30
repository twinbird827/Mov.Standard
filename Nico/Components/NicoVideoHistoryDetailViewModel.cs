using Mov.Standard.Nico.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Nico.Components
{
    public class NicoVideoHistoryDetailViewModel : NicoVideoDetailViewModel
    {
        private NicoVideoHistoryDetailViewModel(NicoVideoModel source) : base(source)
        {

        }

        public static async Task<NicoVideoHistoryDetailViewModel> Create(string id, DateTime lastdate, int count)
        {
            var source = await NicoUtil.GetVideo(id);
            var vm = new NicoVideoHistoryDetailViewModel(source);

            vm.LastDate = lastdate;
            vm.Count = count;

            return vm;
        }

        public int Count
        {
            get => _Count;
            set => SetProperty(ref _Count, value);
        }
        private int _Count;

        public DateTime LastDate
        {
            get => _LastDate;
            set => SetProperty(ref _LastDate, value);
        }
        private DateTime _LastDate;

    }
}
