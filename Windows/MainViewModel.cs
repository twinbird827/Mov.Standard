using Mov.Standard.Core.Databases;
using Mov.Standard.Models;
using Mov.Standard.Nico.Models;
using Mov.Standard.Nico.Workspaces;
using My.Wpf;
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
                case MenuType.NicoNico:
                    Current = new NicoMainViewModel();
                    break;
            }
        });
        private ICommand _OnClickMenu;

        protected override bool DoClosing()
        {
            Current?.Dispose();

            return false;
        }

        protected override async Task DoLoading(ProgressViewModel vm)
        {
            await vm.SetMessageAsync("設定ﾌｧｲﾙ読み込み中");

            // Xmlﾌｧｲﾙ読み込み
            await MovModel.Instance.LoadXmlAsync();

            await vm.SetMessageAsync("ｱｶｳﾝﾄ情報読み込み中");

            using (var command = DbUtil.GetControl())
            {
                var settings = await command.SelectTSetting();
                var mail = settings.FirstOrDefault(s => s.Group == "setting" && s.Key == "mail")?.Value;
                var pass = settings.FirstOrDefault(s => s.Group == "setting" && s.Key == "password")?.Value;
                var change = false;
                while (true)
                {
                    if (await Session.Instance.TryLoginAsync(mail, pass) != null)
                    {
                        break;
                    }

                    var nicovm = new NicoAccountViewModel(mail, pass);
                    var nicowindow = new NicoAccountWindow(nicovm);

                    nicowindow.ShowModalWindow();

                    mail = nicovm.Mail;
                    pass = nicovm.Password;
                    change = true;
                }

                if (change)
                {
                    await command.BeginTransaction();
                    await command.InsertTSetting("setting", "mail", mail);
                    await command.InsertTSetting("setting", "password", pass);
                    await command.Commit();
                }
            }

            // 初回読取
            await NicoTemporaryModel.Instance.LoadAsync();

            // 画面遷移
            OnClickMenu.Execute(MenuType.NicoNico);
        }
    }
}
