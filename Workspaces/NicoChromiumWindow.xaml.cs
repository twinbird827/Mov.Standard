using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mov.Standard.Workspaces
{
    /// <summary>
    /// NicoChromiumWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class NicoChromiumWindow : UserControl
    {
        #region DependencyProperty

        private static DependencyProperty Register<T>(string name, T defaultValue, PropertyChangedCallback callback, FrameworkPropertyMetadataOptions fpmo = FrameworkPropertyMetadataOptions.None)
        {
            if (callback != null)
            {
                return DependencyProperty.Register(
                    name,
                    typeof(T),
                    typeof(NicoChromiumWindow),
                    new FrameworkPropertyMetadata(defaultValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, callback)
                );
            }
            else
            {
                return DependencyProperty.Register(
                    name,
                    typeof(T),
                    typeof(NicoChromiumWindow),
                    new FrameworkPropertyMetadata(defaultValue, fpmo)
                );
            }
        }

        public static readonly DependencyProperty CookieManagerProperty =
            Register(nameof(CookieManager), default(ICookieManager), null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault);

        /// <summary>
        /// 表示文字
        /// </summary>
        public ICookieManager CookieManager
        {
            get { return (ICookieManager)GetValue(CookieManagerProperty); }
            set { SetValue(CookieManagerProperty, value); }
        }

        #endregion

        public NicoChromiumWindow()
        {
            InitializeComponent();

            CookieManager = WebBrowser.GetCookieManager();
        }
    }
}
