using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Mov.Standard
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void SetCurrentCulture(string culname)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culname);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culname);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // 環境変数を使用
            string langstr = "ja-JP";
            SetCurrentCulture(langstr);
        }
    }
}
