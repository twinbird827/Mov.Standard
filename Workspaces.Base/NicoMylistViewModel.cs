using Mov.Standard.Core;
using Mov.Standard.Core.Nico;
using My.Wpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Mov.Standard.Workspaces.Base
{
    public class NicoMylistViewModel : BindableBase
    {
        public NicoMylistViewModel(string mylistid, string orderby)
        {
            Initialize(mylistid, orderby);
        }

        public NicoMylistViewModel(string mylistid) : this(mylistid, "0")
        {

        }

        private async void Initialize(string mylistid, string orderby)
        {
            MylistId = mylistid;
            OrderBy = "0";

            var xml = await WebUtil.GetXmlChannelAsync(MylistUrl);

            // ﾏｲﾘｽﾄ情報を本ｲﾝｽﾀﾝｽのﾌﾟﾛﾊﾟﾃｨに転記
            MylistTitle = xml.Element("title").Value;
            MylistCreator = xml.Element(XName.Get("creator", "http://purl.org/dc/elements/1.1/")).Value;
            MylistDate = DateTime.Parse(xml.Element("lastBuildDate").Value);
            MylistDescription = xml.Element("description").Value;

            const string thumbnailurl = "https://secure-dcdn.cdn.nimg.jp/nicoaccount/usericon/";
            var mylistpage = await WebUtil.GetStringAsync($"http://www.nicovideo.jp/mylist/{MylistId}");

            // ｻﾑﾈｲﾙURLを取得
            var iframestr = thumbnailurl + Regex.Match(mylistpage, thumbnailurl + "(?<url>[^\"]+)").Groups["url"].Value;

            // ﾕｰｻﾞIDを取得
            var userid = Regex.Match(iframestr, @"\/(?<user_id>[\d]+)\.").Groups["user_id"].Value;
            UserId = userid;

            // ｻﾑﾈｲﾙ取得
            await NicoUtil
                .GetThumbnailAsync(iframestr)
                .ContinueWith(x => Thumbnail = x.Result);

        }

        #region ﾌﾟﾛﾊﾟﾃｨ

        public string MylistUrl => $"http://www.nicovideo.jp/mylist/{MylistId}?rss=2.0&numbers=1&sort={OrderBy}";

        public string MylistId
        {
            get { return _MylistId; }
            set { SetProperty(ref _MylistId, value); OnPropertyChanged(nameof(MylistUrl)); }
        }
        private string _MylistId = null;

        public string OrderBy
        {
            get { return _OrderBy; }
            set { SetProperty(ref _OrderBy, value); OnPropertyChanged(nameof(MylistUrl)); }
        }
        private string _OrderBy = null;

        public string MylistTitle
        {
            get { return _MylistTitle; }
            set { SetProperty(ref _MylistTitle, value); }
        }
        private string _MylistTitle = null;

        public string MylistCreator
        {
            get { return _MylistCreator; }
            set { SetProperty(ref _MylistCreator, value); }
        }
        private string _MylistCreator = null;

        public string MylistDescription
        {
            get { return _MylistDescription; }
            set { SetProperty(ref _MylistDescription, value); }
        }
        private string _MylistDescription = null;

        public string UserId
        {
            get { return _UserId; }
            set { SetProperty(ref _UserId, value); }
        }
        private string _UserId = null;

        public BitmapImage Thumbnail
        {
            get { return _Thumbnail; }
            set { SetProperty(ref _Thumbnail, value); }
        }
        private BitmapImage _Thumbnail;

        public DateTime MylistDate
        {
            get { return _MylistDate; }
            set { SetProperty(ref _MylistDate, value); }
        }
        private DateTime _MylistDate = default(DateTime);

        #endregion

        #region ｲﾍﾞﾝﾄ

        public ICommand OnDoubleClick => _OnDoubleClick = _OnDoubleClick ?? new RelayCommand(_ =>
        {

        });
        private ICommand _OnDoubleClick;

        public ICommand OnKeyDown => _OnKeyDown = _OnKeyDown ?? new RelayCommand<KeyEventArgs>(e =>
        {
            if (e.Key == Key.Enter)
            {
                OnDoubleClick.Execute(null);
            }
            else if (e.Key == Key.Delete)
            {
                OnFavoriteDel.Execute(null);
            }
        });
        private ICommand _OnKeyDown;

        public ICommand OnFavoriteDel => _OnFavoriteDel = _OnFavoriteDel ?? new RelayCommand(async _ =>
        {
            await NicoUtil.DeleteFavorite(MylistId);
        });
        private ICommand _OnFavoriteDel;

        #endregion

    }
}
