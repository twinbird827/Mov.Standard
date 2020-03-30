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
    public class NicoMainViewModel : WorkspaceViewModel
    {
        public NicoMainViewModel()
        {
            Current = new NicoRankingViewModel();

            NicoTemporaryModel.Instance.AddOnPropertyChanged(this, (sender, e) =>
            {
                if (e.PropertyName == nameof(NicoTemporaryModel.Count))
                {
                    TempCount = NicoTemporaryModel.Instance.Count;
                }
            });
            TempCount = NicoTemporaryModel.Instance.Count;

        }

        public override string Title => Current.Title;

        public NicoWorkspaceViewModel Current
        {
            get { return _Current; }
            set { SetProperty(ref _Current, value, true); }
        }
        private NicoWorkspaceViewModel _Current;

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
                    Current = new NicoRankingViewModel();
                    break;
                case NicoMenuType.Temporary:
                    Current = new NicoTemporaryViewModel();
                    break;
                case NicoMenuType.Mylist:
                    Current = new NicoMylistViewModel();
                    break;
                case NicoMenuType.Favorite:
                    Current = new NicoFavoriteViewModel();
                    break;
                case NicoMenuType.VideoHistory:
                    Current = new NicoVideoHistoryViewModel();
                    break;
            }
        });
        private ICommand _OnClickMenu;

        public static void SetCurrent(NicoWorkspaceViewModel vm)
        {
            ((NicoMainViewModel)MainViewModel.Instance.Current).Current = vm;
        }
    }
}
