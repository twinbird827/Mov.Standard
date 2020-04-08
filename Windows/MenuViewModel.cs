using My.Wpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mov.Standard.Windows
{
    public abstract class MenuViewModel : BindableBase
    {
        /// <summary>
        /// 画面表示時の処理
        /// </summary>
        public ICommand OnLoaded => _OnLoaded = _OnLoaded ?? new RelayCommand(_ =>
        {
            Loaded?.Invoke(this, EventArgs.Empty);
        });
        private ICommand _OnLoaded;

        /// <summary>
        /// 画面表示時に発生するｲﾍﾞﾝﾄ
        /// </summary>
        public event EventHandler Loaded;
    }
}
