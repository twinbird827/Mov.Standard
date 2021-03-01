using My.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Core
{
    public class AppSetting : JsonBase<AppSetting>
    {
        private const string _path = @"lib\app-setting.json";

        public static AppSetting Instance
        {
            get => _Instance = _Instance ?? new AppSetting();
        }
        private static AppSetting _Instance;

        public AppSetting() : base(_path)
        {
            if (!Load())
            {
                NicoUserId = "twinbird827@yahoo.co.jp";
                NicoPassword = "zaq12wsx";
            }
        }

        public string NicoRankingGenre
        {
            get => GetProperty(_NicoRankingGenre);
            set => SetProperty(ref _NicoRankingGenre, value);
        }
        private string _NicoRankingGenre;

        public string NicoRankingPeriod
        {
            get => GetProperty(_NicoRankingPeriod);
            set => SetProperty(ref _NicoRankingPeriod, value);
        }
        private string _NicoRankingPeriod;

        public string NicoMylistOrder
        {
            get => GetProperty(_NicoMylistOrder);
            set => SetProperty(ref _NicoMylistOrder, value);
        }
        private string _NicoMylistOrder;

        public string NicoUserId
        {
            get => GetProperty(_NicoUserId);
            set => SetProperty(ref _NicoUserId, value);
        }
        private string _NicoUserId;

        public string NicoPassword
        {
            get => GetProperty(_NicoPassword);
            set => SetProperty(ref _NicoPassword, value);
        }
        private string _NicoPassword;

    }
}
