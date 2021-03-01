using Mov.Standard.Windows;
using My.Wpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mov.Standard.Workspaces
{
    public abstract class WorkspaceViewModel : BindableBase
    {
        public WorkspaceViewModel()
        {
            Loaded += (sender, e) =>
            {
                if (_initialize)
                {
                    _initialize = false;
                    return;
                }

                MainViewModel.Instance.ShowProgressRing(async () =>
                {
                    await ExecuteLoadTasks();
                    //MainViewModel.Instance.IsShowDialog = false;
                });
            };
        }

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
        public ICommand OnLoaded => _OnLoaded = _OnLoaded ?? new RelayCommand(_ =>
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
            return $"{Properties.Resources.APP_Title} [{Title}] Ver: {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }


        private List<object> LoadTasks { get; set; } = new List<object>();

        protected void AddLoadTask(Func<Task> task)
        {
            LoadTasks.Add(task);
        }

        protected void AddLoadTask(Action task)
        {
            LoadTasks.Add(task);
        }

        public bool IsLoaded
        {
            get => _IsLoaded;
            set => SetProperty(ref _IsLoaded, value);
        }
        private bool _IsLoaded = false;

        private static bool _initialize = true;

        public async Task ExecuteLoadTasks()
        {
            foreach (var task in LoadTasks)
            {
                if (task is Func<Task> func)
                {
                    await func();
                }
                else if (task is Action action)
                {
                    action();
                }
            }
            IsLoaded = true;
            MainViewModel.Instance.IsShowDialog = false;
        }
    }
}
