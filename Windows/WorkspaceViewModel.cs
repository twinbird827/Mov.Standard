using My.Wpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mov.Standard.Windows
{
    public abstract class WorkspaceViewModel : BindableBase
    {
        /// <summary>
        /// ﾀｲﾄﾙ
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// ｱﾌﾟﾘｹｰｼｮﾝﾀｲﾄﾙ
        /// </summary>
        public string ApplicationTitle
        {
            get { return CreateTitle(); }
        }

        /// <summary>
        /// 画面表示時の処理
        /// </summary>
        public ICommand OnLoaded =>
            _OnLoaded = _OnLoaded ?? new RelayCommand(
        _ =>
        {
            Loaded?.Invoke(this, EventArgs.Empty);
        });
        private ICommand _OnLoaded;

        /// <summary>
        /// 画面表示時に発生するｲﾍﾞﾝﾄ
        /// </summary>
        public event EventHandler Loaded;

        /// <summary>
        /// 画面ﾀｲﾄﾙを作成する。
        /// </summary>
        /// <returns>画面ﾀｲﾄﾙ</returns>
        private string CreateTitle()
        {
            // ｱｾﾝﾌﾞﾘ
            var assembly = this.GetType().Assembly;

            // ｱｾﾝﾌﾞﾘ名のみを取得
            var assemblyName = System.IO.Path.GetFileNameWithoutExtension(assembly.Location);

            // ﾊﾞｰｼﾞｮﾝを取得
            System.Version version = assembly.GetName().Version;

            // ｱｾﾝﾌﾞﾘ名とﾊﾞｰｼﾞｮﾝを結合して画面ﾀｲﾄﾙを作成
            return $"{Properties.Resources.V_MainTitle} [{Title}] Ver: {version.Major}.{version.Minor}.{version.Revision}.{version.Build}";
        }
    }
}
