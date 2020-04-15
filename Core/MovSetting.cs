using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Core
{
    public static class MovSetting
    {
        static MovSetting()
        {
            NicoMylistOrder = Properties.Settings.Default.NicoMylistOrder;
            NicoRankingGenre = Properties.Settings.Default.NicoRankingGenre;
            NicoRankingPeriod = Properties.Settings.Default.NicoRankingPeriod;
        }

        public static string NicoMylistOrder
        {
            get => _NicoMylistOrder;
            set => SetProperty(ref _NicoMylistOrder, value);
        }
        private static string _NicoMylistOrder;

        public static string NicoRankingGenre
        {
            get => _NicoRankingGenre;
            set => SetProperty(ref _NicoRankingGenre, value);
        }
        private static string _NicoRankingGenre;

        public static string NicoRankingPeriod
        {
            get => _NicoRankingPeriod;
            set => SetProperty(ref _NicoRankingPeriod, value);
        }
        private static string _NicoRankingPeriod;


        private static void SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            storage = value;
            Properties.Settings.Default[propertyName] = value;
            Properties.Settings.Default.Save();
        }
    }
}
