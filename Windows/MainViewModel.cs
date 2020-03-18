using My.Wpf.Core;
using My.Wpf.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mov.Standard.Windows
{
    public class MainViewModel : MainViewModelBase
    {
        public MainViewModel()
        {
            if (!WpfUtil.IsDesignMode() && Instance != null)
            {
                throw new InvalidOperationException("This ViewModel cannot create multiple instances.");
            }
            Instance = this;
        }

        /// <summary>
        /// 本ｲﾝｽﾀﾝｽ(ｼﾝｸﾞﾙﾄﾝ)
        /// </summary>
        public static MainViewModel Instance { get; private set; }

        public WorkspaceViewModel Current
        {
            get { return _Current; }
            set { SetProperty(ref _Current, value, true); }
        }
        private WorkspaceViewModel _Current;

        /// <summary>
        /// ﾒﾆｭｰ処理
        /// </summary>
        public ICommand OnClickMenu =>
            _OnClickMenu = _OnClickMenu ?? new RelayCommand(
        type =>
        {
            switch (type)
            {
                case MenuType.Settings:
                    break;
            }
        });
        private ICommand _OnClickMenu;

        protected override bool DoClosing()
        {
            return false;
        }

        protected override async Task DoLoading(ProgressViewModel vm)
        {
            await Task.Delay(1);
        }
    }
}
