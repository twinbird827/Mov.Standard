using Mov.Standard.Core.Databases;
using Mov.Standard.Models;
using Mov.Standard.Nico.Models;
using Mov.Standard.Nico.Workspaces;
using My.Wpf;
using My.Wpf.Core;
using My.Wpf.Windows;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

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

        public MenuViewModel Menu
        {
            get { return _Menu; }
            set { SetProperty(ref _Menu, value, true); }
        }
        private MenuViewModel _Menu;

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
                    Menu = new NicoMainViewModel();
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

            using (var command = await DbUtil.GetControl())
            {
                await vm.SetMessageAsync("ﾃﾞｰﾀﾍﾞｰｽﾌｧｲﾙ読み込み中");

                // Xmlﾌｧｲﾙ読み込み
                await command.InitializeDatabase();

                await NicoFavoriteModel.Instance.Initialize(command);

                await NicoVideoHistoryModel.Instance.Initialize(command);

                await vm.SetMessageAsync("ｱｶｳﾝﾄ情報読み込み中");

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

        /// <summary>
        /// ﾄｰｽﾄ通知を表示します。
        /// </summary>
        /// <param name="title">ﾀｲﾄﾙ</param>
        /// <param name="message">ﾒｯｾｰｼﾞ</param>
        /// <param name="type">通知の種類</param>
        /// <param name="timeout">通知を表示する時間</param>
        public void ShowToast(string title, string message, NotificationType type, TimeSpan timeout)
        {
            var manager = new NotificationManager(Dispatcher.CurrentDispatcher);
            var content = new NotificationContent
            {
                Title = title,
                Message = message,
                Type = type
            };
            manager.Show(content, "", timeout);
        }

        /// <summary>
        /// ﾄｰｽﾄ通知を表示します。
        /// </summary>
        /// <param name="message">ﾒｯｾｰｼﾞ</param>
        /// <param name="type">通知の種類</param>
        /// <param name="timeout">通知を表示する時間</param>
        public void ShowToast(string message, NotificationType type, TimeSpan timeout)
        {
            ShowToast(type.ToString(), message, type, timeout);
        }

        /// <summary>
        /// ﾄｰｽﾄ通知を表示します。
        /// </summary>
        /// <param name="message">ﾒｯｾｰｼﾞ</param>
        /// <param name="type">通知の種類</param>
        public void ShowToast(string message, NotificationType type)
        {
            ShowToast(message, type, TimeSpan.FromMilliseconds(2000));
        }

    }
}
