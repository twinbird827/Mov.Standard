using Mov.Standard.Core.Database;
using Mov.Standard.Windows;
using My.Core;
using My.Core.Databases.SQLite;
using My.Core.Services;
using My.Wpf.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Mov.Standard.Core.Nico
{
    public static class NicoUtil
    {
        public static StatefulModel.SortedObservableCollection<TNicoTemporary, DateTime> Temporaries { get; private set; }

        public static StatefulModel.SortedObservableCollection<VNicoHistory, DateTime> Histories { get; private set; }

        public static StatefulModel.SortedObservableCollection<TNicoFavorite, DateTime> Favorites { get; private set; }

        public static ComboboxItemModel[] Combos { get; private set; }

        private const string NicoComboPath = @"lib\nico-combo-setting.xml";

        public static async Task Initialize(SQLiteControl command)
        {
            Temporaries = new StatefulModel.SortedObservableCollection<TNicoTemporary, DateTime>(
                await command.SelectNicoTemporary(), x => x.Date, true
            );

            Histories = new StatefulModel.SortedObservableCollection<VNicoHistory, DateTime>(
                await command.SelectNicoHistory(), x => x.Date, true
            );

            Favorites = new StatefulModel.SortedObservableCollection<TNicoFavorite, DateTime>(
                await command.SelectNicoFavorite(), x => x.Date, true
            );

            var combo = XDocument.Load(CoreUtil.RelativePathToAbsolutePath(NicoComboPath)).Root;

            Combos = combo.Descendants("combo")
                .SelectMany(xml =>
                {
                    return xml.Descendants("item")
                        .Select(tag => new ComboboxItemModel(
                            (string)xml.Attribute("group"),
                            (string)tag.Attribute("value"),
                            (string)tag.Attribute("display")
                    ));
                })
                .ToArray();
        }

        private static long ToLong(string value)
        {
            return long.Parse(value.Replace(",", ""));
        }

        private static string GetData(XElement e, string name)
        {
            return (string)e
                .Descendants("strong")
                .Where(x => (string)x.Attribute("class") == name)
                .FirstOrDefault();
        }

        private static long ToCounter(XElement e, string name)
        {
            var s = GetData(e, name);

            if (string.IsNullOrEmpty(s))
            {
                return 0;
            }
            else
            {
                return ToLong(s);
            }
        }

        private static long ToLengthSeconds(XElement xml)
        {
            var lengthSecondsStr = (string)xml
                    .Descendants("strong")
                    .Where(x => (string)x.Attribute("class") == "nico-info-length")
                    .First();
            return ToLengthSeconds(lengthSecondsStr);
        }

        private static long ToLengthSeconds(string lengthSecondsStr)
        {
            var lengthSecondsIndex = 0;
            var lengthSeconds = lengthSecondsStr
                    .Split(':')
                    .Select(s => long.Parse(s))
                    .Reverse()
                    .Select(l => l * (long)Math.Pow(60, lengthSecondsIndex++))
                    .Sum();
            return lengthSeconds;
        }

        private static DateTime FromUnixTime(long time)
        {
            DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            return UnixEpoch.AddSeconds(time).ToLocalTime();
        }

        private static DateTime ToRankingDatetime(XElement e, string name)
        {
            // 2018年02月27日 20：00：00
            var s = GetData(e, name);

            return DateTime.ParseExact(s,
                "yyyy年MM月dd日 HH：mm：ss",
                System.Globalization.DateTimeFormatInfo.InvariantInfo,
                System.Globalization.DateTimeStyles.None
            );

        }

        public static string ToVideoId(string url)
        {
            return CoreUtil.Nvl(url).Split('/').Last().Split('?').First();
        }

        public static string ToNicolistId(string url)
        {
            return CoreUtil.Nvl(url).Split('/').Last().Split('?').First();
        }

        public static NicoVideoModel CreateVideoFromXml(XElement item, string view, string mylist, string comment)
        {
            string tmp = string.Empty;
            try
            {
                // 明細部読み込み
                var descriptionString = item.Element("description").Value;
                descriptionString = descriptionString.Replace("&nbsp;", "&#x20;");
                //descriptionString = HttpUtility.HtmlDecode(descriptionString);
                descriptionString = descriptionString.Replace("&", "&amp;");
                descriptionString = descriptionString.Replace("'", "&apos;");
                tmp = descriptionString;
                var desc = WebUtil.ToXml($"<root>{descriptionString}</root>");

                var video = new NicoVideoModel()
                {
                    VideoId = ToVideoId(item.Element("link").Value),
                    Title = item.Element("title").Value,
                    ViewCounter = ToCounter(desc, view),
                    MylistCounter = ToCounter(desc, mylist),
                    CommentCounter = ToCounter(desc, comment),
                    StartTime = ToRankingDatetime(desc, "nico-info-date"),
                    ThumbnailUrl = (string)desc.Descendants("img").First().Attribute("src"),
                    LengthSeconds = NicoUtil.ToLengthSeconds(desc),
                    Description = (string)desc.Descendants("p").FirstOrDefault(x => (string)x.Attribute("class") == "nico-description")
                };

                /*
                <item>
                  <title>第1位：【女子2人】初めてパンの気持ちを理解する実況【I am Bread】</title>
                  <link>http://www.nicovideo.jp/watch/sm34525974</link>
                  <guid isPermaLink="false">tag:nicovideo.jp,2019-01-25:/watch/sm34525974</guid>
                  <pubDate>Thu, 31 Jan 2019 07:06:01 +0900</pubDate>
                  <description><![CDATA[
                                      <p class="nico-thumbnail"><img alt="【女子2人】初めてパンの気持ちを理解する実況【I am Bread】" src="http://tn.smilevideo.jp/smile?i=34525974.59215" width="94" height="70" border="0"/></p>
                                                <p class="nico-description">全パンの想いを背負いし者----------------------関西弁女子実況グループ『サイコロジカルサーカス』が、第13回実況者杯に参加します!フリー部門実況動画の部、謎部門にエントリー！再生数・コメント・マイリス数で順位が決まります！！応援よろよろ！( ˘ω˘ )ニコニ広告が可能です！よければ広告で宣伝もよろしくお願いします(強欲の壺)テーマは【初】この動画はフリー部門実況動画の部の動画です。 今回は映画風の始まりにしてみました！楽しんで見てもらえるとうれしい！(*^^*)今回のプログラム⇒mylist/63232841大会詳細⇒sm33431601パンフレット⇒mylist/55555016舞台袖⇒co3253598プレイメンバー…ノイジーワールド(紫)・馬面なおと(桃) Twitter…https://twitter.com/rojikaru2525</p>
                                                <p class="nico-info"><small><strong class="nico-info-number">20,404</strong>pts.｜<strong class="nico-info-length">20:04</strong>｜<strong class="nico-info-date">2019年01月25日 18：11：01</strong> 投稿<br/><strong>合計</strong>  再生：<strong class="nico-info-total-view">729</strong>  コメント：<strong class="nico-info-total-res">59</strong>  マイリスト：<strong class="nico-info-total-mylist">9</strong><br/><strong>毎時</strong>  再生：<strong class="nico-info-hourly-view">4</strong>  コメント：<strong class="nico-info-hourly-res">0</strong>  マイリスト：<strong class="nico-info-hourly-mylist">0</strong><br/></small></p>
                                  ]]></description>
                </item>
                */
                return video;
            }
            catch (Exception ex)
            {
                ServiceFactory.MessageService.Debug(tmp);
                ServiceFactory.MessageService.Exception(ex);
                throw;
            }
        }

        public static async Task<NicoVideoModel> GetVideo(string videoid)
        {
            var video = new NicoVideoModel();

            try
            {
                var txt = await WebUtil.GetStringAsync($"http://ext.nicovideo.jp/api/getthumbinfo/{videoid}", null, false);
                var xml = WebUtil.ToXml(txt);

                if (xml == null || (string)xml.Attribute("status") == "fail")
                {
                    video.VideoId = videoid;
                    video.Status = VideoStatus.Delete;
                    return video;
                }
                xml = xml.Descendants("thumb").First();
                video.VideoId = ToVideoId((string)xml.Element("watch_url"));
                video.Title = (string)xml.Element("title");
                video.Description = (string)xml.Element("description");
                video.ThumbnailUrl = (string)xml.Element("thumbnail_url");
                video.ViewCounter = (long)xml.Element("view_counter");
                video.CommentCounter = (long)xml.Element("comment_num");
                video.MylistCounter = (long)xml.Element("mylist_counter");
                video.StartTime = DateTime.Parse((string)xml.Element("first_retrieve"));
                video.LengthSeconds = NicoUtil.ToLengthSeconds((string)xml.Element("length"));
                video.Tags = xml.Descendants("tags").First().Descendants("tag").Select(tag => (string)tag).GetString(" ");
                video.Username = (string)xml.Element("user_nickname");
            }
            catch
            {
                video.VideoId = videoid;
                video.Status = VideoStatus.Delete;
            }

            return video;
        }

        public static async Task AddTemporary(string videoid)
        {
            if (!Temporaries.Any(x => x.VideoId == videoid))
            {
                Temporaries.Add(new TNicoTemporary(videoid, DateTime.Now.Ticks));

                using (var control = await DbUtil.GetControl())
                {
                    await control.BeginTransaction();
                    await control.MergeNicoTemporary(videoid, DateTime.Now);
                    await control.Commit();
                }
            }
        }

        public static async Task DeleteTemporary(string videoid)
        {
            if (Temporaries.Any(x => x.VideoId == videoid))
            {
                Temporaries.Remove(Temporaries.First(x => x.VideoId == videoid));

                using (var control = await DbUtil.GetControl())
                {
                    await control.BeginTransaction();
                    await control.DeleteNicoTemporary(videoid);
                    await control.Commit();
                }

                await AddHistory(videoid);
            }
        }

        public static async Task AddHistory(string videoid)
        {
            using (var control = await DbUtil.GetControl())
            {
                await control.BeginTransaction();
                await control.MergeNicoHistory(videoid);
                await control.Commit();
            }

            var history = Histories.FirstOrDefault(x => x.VideoId == videoid);
            if (history != null)
            {
                history.Date = DateTime.Now;
                history.Count += 1;
            }
            else
            {
                Histories.Add(new VNicoHistory(videoid, DateTime.Now.Ticks, 1));
            }
        }

        public static async Task DeleteHistory(string videoid)
        {
            if (Histories.Any(x => x.VideoId == videoid))
            {
                Histories.Remove(Histories.First(x => x.VideoId == videoid));

                using (var control = await DbUtil.GetControl())
                {
                    await control.BeginTransaction();
                    await control.DeleteNicoHistory(videoid);
                    await control.Commit();
                }
            }
        }

        public static async Task AddFavorite(string mylistid)
        {
            if (!Favorites.Any(x => x.MylistId == mylistid))
            {
                Favorites.Add(new TNicoFavorite(mylistid, DateTime.Now.Ticks));

                using (var control = await DbUtil.GetControl())
                {
                    await control.BeginTransaction();
                    await control.MergeNicoFavorite(mylistid, DateTime.Now);
                    await control.Commit();
                }
            }
        }

        public static async Task AddFavorite(string mylistid, DateTime date)
        {
            var tmp = Favorites.FirstOrDefault(x => x.MylistId == mylistid);
            if (tmp != null)
            {
                tmp.Date = date;

                using (var control = await DbUtil.GetControl())
                {
                    await control.BeginTransaction();
                    await control.MergeNicoFavorite(mylistid, date);
                    await control.Commit();
                }
            }
        }

        public static async Task DeleteFavorite(string mylistid)
        {
            if (Favorites.Any(x => x.MylistId == mylistid))
            {
                Favorites.Remove(Favorites.First(x => x.MylistId == mylistid));

                using (var control = await DbUtil.GetControl())
                {
                    await control.BeginTransaction();
                    await control.DeleteNicoFavorite(mylistid);
                    await control.Commit();
                }
            }
        }

        public static async Task<BitmapImage> GetThumbnailAsync(string url)
        {
            try
            {
                byte[] bytes = default(byte[]);

                using (var client = new HttpClient())
                {
                    bytes = await client.GetByteArrayAsync(url).ConfigureAwait(false);
                }

                using (WrappingStream stream = new WrappingStream(new MemoryStream(bytes)))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.DecodePixelWidth = 160 + 48 * 0;
                    bitmap.DecodePixelHeight = 120 + 36 * 0;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    if (bitmap.CanFreeze)
                    {
                        bitmap.Freeze();
                    }
                    return bitmap;
                }
            }
            catch (Exception ex)
            {
                ServiceFactory.MessageService.Debug(url);
                ServiceFactory.MessageService.Exception(ex);
                return null;
            }
        }

        public static async Task<BitmapImage> GetThumbnailAsync(IEnumerable<string> urls)
        {
            foreach (var url in urls)
            {
                var thumnail = await GetThumbnailAsync(url).ConfigureAwait(false);
                if (thumnail != null)
                {
                    return thumnail;
                }
            }
            return null;
        }

        //public static async Task<BitmapImage> GetThumbnailAsync(NicoVideoDetailViewModel vm)
        //{
        //    var url = vm.ThumbnailUrl.Replace(".M", "").Replace(".L", "");
        //    var urls = new string[]
        //    {
        //        $"{url}.L",
        //        $"{url}.M",
        //        $"{url}",
        //    };

        //    return await GetThumbnailAsync(urls);
        //}

        public static async Task<IEnumerable<NicoVideoModel>> GetRanking(string genre, string tag, string term)
        {
            //var p = period.Value;
            //var g = genre.Value;
            //var t = "all";

            //var url = $"http://www.nicovideo.jp/ranking/genre/{genre}?tag={tag}&term={term}&rss=2.0&lang=ja-jp";
            var url = $"https://www.nicovideo.jp/ranking/genre/{genre}?video_ranking_menu?tag={tag}&term={term}&rss=2.0&lang=ja-jp";
            var xml = await WebUtil.GetXmlChannelAsync(url);
            var videos = xml.Descendants("item")
                .Select(item => CreateVideoFromXml(item,
                    "nico-info-total-view",
                    "nico-info-total-mylist",
                    "nico-info-total-res")
                );

            // TODO 取得できていない項目の取得
            // TODO ｻﾑﾈｲﾙの取得

            return videos;
        }

        public static async Task<IEnumerable<NicoVideoModel>> GetMylistVideos(string mylistid, string orderby)
        {
            var MylistUrl = $"http://www.nicovideo.jp/mylist/{mylistid}?rss=2.0&numbers=1&sort={orderby}";
            var xml = await WebUtil.GetXmlChannelAsync(MylistUrl);

            // Video更新
            return xml.Descendants("item")
                .Select(item => NicoUtil.CreateVideoFromXml(item,
                    "nico-numbers-view",
                    "nico-numbers-mylist",
                    "nico-numbers-res"
                ));
        }


        //public static async Task<IEnumerable<NicoVideoModel>> GetTemporary()
        //{
        //    using (var control = await DbUtil.GetControl())
        //    {
        //        var videos = new List<NicoVideoModel>();
        //        var temporaries = await control.SelectTTemporaryAsync();

        //        foreach (var temp in temporaries)
        //        {
        //            var video = await GetVideo(temp.VideoId);

        //            video.StartTime = temp.Date;
        //            videos.Add(video);
        //        }

        //        return videos;
        //    }
        //    //const string url = "http://www.nicovideo.jp/api/deflist/list";

        //    //var json = await WebUtil.GetJsonAsync(url, true);

        //    //var videos = new List<NicoVideoModel>();

        //    //foreach (dynamic item in json["mylistitem"])
        //    //{
        //    //    var video = new NicoVideoModel();
        //    //    video.VideoId = item["item_data"]["video_id"];
        //    //    video.Title = item["item_data"]["title"];
        //    //    video.Description = item["description"];
        //    //    //video.Tags = data["tags"];
        //    //    //video.CategoryTag = data["categoryTags"];
        //    //    video.ViewCounter = long.Parse(item["item_data"]["view_counter"]);
        //    //    video.MylistCounter = long.Parse(item["item_data"]["mylist_counter"]);
        //    //    video.CommentCounter = long.Parse(item["item_data"]["num_res"]);
        //    //    video.StartTime = FromUnixTime((long)item["update_time"]);
        //    //    //video.LastCommentTime = Converter.item(data["lastCommentTime"]);
        //    //    video.LengthSeconds = long.Parse(item["item_data"]["length_seconds"]);
        //    //    video.ThumbnailUrl = item["item_data"]["thumbnail_url"];
        //    //    //video.LastResBody = item["item_data"]["last_res_body"];
        //    //    //video.CommunityIcon = data["communityIcon"];
        //    //    video.Status = item["item_data"]["deleted"] != "0"
        //    //        ? VideoStatus.Delete
        //    //        : VideoStatus.None;

        //    //    videos.Add(video);
        //    //}

        //    //return videos;
        //}

        public static void Download(string videoid, string title)
        {
            string path = ServiceFactory.MessageService.SelectedSaveFile(title, Properties.Resources.S_Filter_Common_MP4);

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            MainViewModel.Instance.ShowProgress(async (progress) =>
            {
                using (var handler = new HttpClientHandler())
                using (var client = new HttpClient(handler))
                {
                    client.Timeout = new TimeSpan(1, 0, 0);

                    //// ﾛｸﾞｲﾝｸｯｷｰ設定
                    handler.CookieContainer = await TryLoginAsync();

                    // 対象動画にｼﾞｬﾝﾌﾟ
                    var watchurl = $"http://www.nicovideo.jp/watch/{videoid}";
                    await client.PostAsync(watchurl, null);

                    // 動画URL全文を取得
                    var flapiurl = $"http://flapi.nicovideo.jp/api/getflv/{videoid}";
                    var flapibody = await client.GetStringAsync(flapiurl);
                    var movieurl = Regex.Match(Uri.UnescapeDataString(flapibody), @"&url=.*").Value.Replace("&url=", "");
                    var res = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, flapiurl));

                    File.WriteAllBytes(path, await client.GetByteArrayAsync(movieurl));
                }
            });
        }

        private static async Task<CookieContainer> TryLoginAsync()
        {
            using (var command = await DbUtil.GetControl())
            {
                var mail = AppSetting.Instance.NicoUserId;
                var pass = AppSetting.Instance.NicoPassword;
                return await LoginAsync(mail, pass);
            }
        }

        private static async Task<CookieContainer> LoginAsync(string mail, string password)
        {
            const string loginUrl = "https://secure.nicovideo.jp/secure/login?site=niconico";

            if (string.IsNullOrWhiteSpace(mail) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            try
            {
                using (var handler = new HttpClientHandler())
                using (var client = new HttpClient(handler))
                {
                    var content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "next_url", string.Empty },
                        { "mail", mail },
                        { "password", password }
                    });

                    await client.PostAsync(loginUrl, content).ConfigureAwait(false);

                    return handler.CookieContainer;
                }
            }
            catch (Exception ex)
            {
                ServiceFactory.MessageService.Exception(ex);
                return null;
            }
        }

    }
}
