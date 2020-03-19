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

        public override string Title => Current.Title;

        public NicoWorkspaceViewModel Current
        {
            get { return _Current; }
            set { SetProperty(ref _Current, value, true); }
        }
        private NicoWorkspaceViewModel _Current;

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
            }
        });
        private ICommand _OnClickMenu;

    }
}
