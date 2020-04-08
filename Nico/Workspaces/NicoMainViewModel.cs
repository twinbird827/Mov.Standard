using Mov.Standard.Nico.Models;
using Mov.Standard.Windows;
using My.Wpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mov.Standard.Nico.Workspaces
{
    public class NicoMainViewModel : MenuViewModel
    {
        public NicoMainViewModel()
        {
            MainViewModel.Instance.Current = new NicoRankingViewModel();

            NicoTemporaryModel.Instance.AddOnPropertyChanged(this, (sender, e) =>
            {
                if (e.PropertyName == nameof(NicoTemporaryModel.Count))
                {
                    TempCount = NicoTemporaryModel.Instance.Count;
                }
            });
            TempCount = NicoTemporaryModel.Instance.Count;

        }

        public int TempCount
        {
            get { return _TempCount; }
            set { SetProperty(ref _TempCount, value, true); }
        }
        private int _TempCount;

        /// <summary>
        /// ﾒﾆｭｰ処理
        /// </summary>
        public ICommand OnClickMenu =>
            _OnClickMenu = _OnClickMenu ?? new RelayCommand(
        type =>
        {
            switch (type)
            {
                case NicoMenuType.Settings:
                    break;
                case NicoMenuType.Ranking:
                    MainViewModel.Instance.Current = new NicoRankingViewModel();
                    break;
                case NicoMenuType.Temporary:
                    MainViewModel.Instance.Current = new NicoTemporaryViewModel();
                    break;
                case NicoMenuType.Mylist:
                    MainViewModel.Instance.Current = new NicoMylistViewModel();
                    break;
                case NicoMenuType.Favorite:
                    MainViewModel.Instance.Current = new NicoFavoriteViewModel();
                    break;
                case NicoMenuType.VideoHistory:
                    MainViewModel.Instance.Current = new NicoVideoHistoryViewModel();
                    break;
            }
        });
        private ICommand _OnClickMenu;
    }
}
