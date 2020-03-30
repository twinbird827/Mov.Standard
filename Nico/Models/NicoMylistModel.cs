using My.Core;
using My.Wpf.Core;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Mov.Standard.Nico.Models
{
    public class NicoMylistModel : BindableBase
    {
        private NicoMylistModel()
        {
            Videos = new ObservableSynchronizedCollection<NicoVideoModel>();
        }

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

        public string ThumbnailUrl
        {
            get { return _ThumbnailUrl; }
            set { SetProperty(ref _ThumbnailUrl, value); }
        }
        private string _ThumbnailUrl;

        public DateTime MylistDate
        {
            get { return _MylistDate; }
            set { SetProperty(ref _MylistDate, value); }
        }
        private DateTime _MylistDate = default(DateTime);

        public DateTime ConfirmDate
        {
            get { return _ConfirmDate; }
            set { SetProperty(ref _ConfirmDate, value); }
        }
        private DateTime _ConfirmDate = default(DateTime);

        public ObservableSynchronizedCollection<NicoVideoModel> Videos
        {
            get { return _Videos; }
            set { SetProperty(ref _Videos, value); }
        }
        private ObservableSynchronizedCollection<NicoVideoModel> _Videos;

        public static async Task<NicoMylistModel> CreateAsync(string value, string orderby)
        {
            var m = new NicoMylistModel();
            return await m.GetInstanceAsync(value, orderby);
        }

        private async Task<NicoMylistModel> GetInstanceAsync(string value, string orderby)
        {
            var listid = NicoUtil.ToNicolistId(value);

            MylistId = listid;
            OrderBy = orderby;

            var xml = await WebUtil.GetXmlChannelAsync(MylistUrl, false);

            // ﾏｲﾘｽﾄ情報を本ｲﾝｽﾀﾝｽのﾌﾟﾛﾊﾟﾃｨに転記
            MylistTitle = xml.Element("title").Value;
            MylistCreator = xml.Element(XName.Get("creator", "http://purl.org/dc/elements/1.1/")).Value;
            MylistDate = DateTime.Parse(xml.Element("lastBuildDate").Value);
            MylistDescription = xml.Element("description").Value;

            // ﾕｰｻﾞIDを取得
            var userid = await WebUtil.GetStringAsync($"http://www.nicovideo.jp/mylist/{MylistId}", false);
            var useridstr = Regex.Match(userid, "user_id: (?<user_id>[\\d]+)").Groups["user_id"].Value;
            UserId = useridstr;

            // ｻﾑﾈｲﾙURLを取得
            const string thumbnailurl = "https://secure-dcdn.cdn.nimg.jp/nicoaccount/usericon/";
            var iframe = await WebUtil.GetStringAsync($"http://ext.nicovideo.jp/thumb_user/{UserId}", false);
            var iframestr = Regex.Match(iframe, thumbnailurl + "(?<url>[^\"]+)").Groups["url"].Value;
            ThumbnailUrl = thumbnailurl + iframestr;

            // Video更新
            ReloadVideos(xml);

            return this;
        }

        public async Task ReloadVideos()
        {
            ReloadVideos(await WebUtil.GetXmlChannelAsync(MylistUrl, false));
        }

        private void ReloadVideos(XElement xml)
        {
            Videos.Clear();

            var videos = xml.Descendants("item")
                .Select(item => NicoUtil.CreateVideoFromXml(item,
                    "nico-numbers-view",
                    "nico-numbers-mylist",
                    "nico-numbers-res"
                ));

            Videos.AddRange(videos);
        }

    }
}
