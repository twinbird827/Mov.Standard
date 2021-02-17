using MahApps.Metro.IconPacks;
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

namespace Mov.Standard.Windows
{
    /// <summary>
    /// IconPacksButton.xaml の相互作用ロジック
    /// </summary>
    public partial class IconPacksButton : UserControl
    {
        #region DependencyProperty

        private static DependencyProperty Register<T>(string name, T defaultValue, PropertyChangedCallback callback)
        {
            return DependencyProperty.Register(
                name,
                typeof(T),
                typeof(IconPacksButton),
                new FrameworkPropertyMetadata(defaultValue, FrameworkPropertyMetadataOptions.None, callback)
            );
        }

        public static readonly DependencyProperty TextProperty =
            Register(nameof(Text), default(object), null);

        public static readonly DependencyProperty KindProperty =
            Register(nameof(Kind), default(PackIconMaterialKind), null);

        public static readonly DependencyProperty CommandProperty =
            Register(nameof(Command), default(ICommand), null);

        public static readonly DependencyProperty CommandParameterProperty =
            Register(nameof(CommandParameter), default(object), null);

        /// <summary>
        /// 表示文字
        /// </summary>
        public object Text
        {
            get { return GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// ｱｲｺﾝの種類
        /// </summary>
        public PackIconMaterialKind Kind
        {
            get { return (PackIconMaterialKind)GetValue(KindProperty); }
            set { SetValue(KindProperty, value); }
        }

        /// <summary>
        /// 実行する処理
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// 実行する処理のﾊﾟﾗﾒｰﾀ
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        #endregion

        public IconPacksButton()
        {
            InitializeComponent();

            baseContainer.DataContext = this;
        }
    }
}
