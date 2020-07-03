using Codeplex.Data;
using Mov.Standard.Core.Databases;
using Mov.Standard.Models;
using Mov.Standard.Nico.Components;
using Mov.Standard.Windows;
using My.Core;
using My.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Mov.Standard.Nico.Models
{
    public static class NicoUtil
    {
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
            return CoreUtil.Nvl(url).Split('/').Last();
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

        public static async Task<IEnumerable<NicoVideoModel>> GetRanking(string genre, string tag, string term)
        {
            //var p = period.Value;
            //var g = genre.Value;
            //var t = "all";

            //var url = $"http://www.nicovideo.jp/ranking/genre/{genre}?tag={tag}&term={term}&rss=2.0&lang=ja-jp";
            var url = $"https://www.nicovideo.jp/ranking/genre/{genre}?video_ranking_menu?tag={tag}&term={term}&rss=2.0&lang=ja-jp";
            var xml = await WebUtil.GetXmlChannelAsync(url, false);
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

        public static async Task<IEnumerable<NicoVideoModel>> GetTemporary()
        {
            using (var control = await DbUtil.GetControl())
            {
                var videos = new List<NicoVideoModel>();
                var temporaries = await control.SelectTTemporaryAsync();

                foreach (var temp in temporaries)
                {
                    var video = await GetVideo(temp.VideoId);

                    video.StartTime = temp.Date;
                    videos.Add(video);
                }

                return videos;
            }
            //const string url = "http://www.nicovideo.jp/api/deflist/list";

            //var json = await WebUtil.GetJsonAsync(url, true);

            //var videos = new List<NicoVideoModel>();

            //foreach (dynamic item in json["mylistitem"])
            //{
            //    var video = new NicoVideoModel();
            //    video.VideoId = item["item_data"]["video_id"];
            //    video.Title = item["item_data"]["title"];
            //    video.Description = item["description"];
            //    //video.Tags = data["tags"];
            //    //video.CategoryTag = data["categoryTags"];
            //    video.ViewCounter = long.Parse(item["item_data"]["view_counter"]);
            //    video.MylistCounter = long.Parse(item["item_data"]["mylist_counter"]);
            //    video.CommentCounter = long.Parse(item["item_data"]["num_res"]);
            //    video.StartTime = FromUnixTime((long)item["update_time"]);
            //    //video.LastCommentTime = Converter.item(data["lastCommentTime"]);
            //    video.LengthSeconds = long.Parse(item["item_data"]["length_seconds"]);
            //    video.ThumbnailUrl = item["item_data"]["thumbnail_url"];
            //    //video.LastResBody = item["item_data"]["last_res_body"];
            //    //video.CommunityIcon = data["communityIcon"];
            //    video.Status = item["item_data"]["deleted"] != "0"
            //        ? VideoStatus.Delete
            //        : VideoStatus.None;

            //    videos.Add(video);
            //}

            //return videos;
        }

        public static async Task<NicoVideoModel> GetVideo(string url)
        {
            var video = new NicoVideoModel();
            var videoid = ToVideoId(url);
            try
            {
                var txt = await WebUtil.GetStringAsync($"http://ext.nicovideo.jp/api/getthumbinfo/{videoid}", false);
                var xml = WebUtil.ToXml(txt);

                if (xml == null || (string)xml.Attribute("status") == "fail")
                {
                    video.VideoId = videoid;
                    video.Status = VideoStatus.Delete;
                    return video;
                }
                xml = xml.Descendants("thumb").First();
                video.VideoId = (string)xml.Element("watch_url");
                video.Title = (string)xml.Element("title");
                video.Description = (string)xml.Element("description");
                video.ThumbnailUrl = (string)xml.Element("thumbnail_url");
                video.ViewCounter = (double)xml.Element("view_counter");
                video.CommentCounter = (double)xml.Element("comment_num");
                video.MylistCounter = (double)xml.Element("mylist_counter");
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

        public static async Task<NicoVideoModel> ReloadVideoAsync(NicoVideoModel src)
        {
            var dst = await GetVideo(src.VideoUrl);

            src.Status = dst.Status;

            if (dst.Status != VideoStatus.Delete)
            {
                src.Title = CoreUtil.Nvl(src.Title, dst.Title);
                src.Description = CoreUtil.Nvl(src.Description, dst.Description);
                src.ThumbnailUrl = CoreUtil.Nvl(src.ThumbnailUrl, dst.ThumbnailUrl);

                src.ViewCounter = dst.ViewCounter;
                src.CommentCounter = dst.CommentCounter;
                src.MylistCounter = dst.MylistCounter;
                src.StartTime = dst.StartTime;
                src.LengthSeconds = dst.LengthSeconds;

                src.Tags = dst.Tags;
                src.Username = dst.Username;
            }

            return src;
        }

        public static async Task AddVideo(string id)
        {
            if (!NicoTemporaryModel.Instance.Videos.Any(video => video.VideoId == id))
            {
                await AddVideo(await GetVideo(id));
            }
        }

        public static async Task AddVideo(NicoVideoModel vm)
        {
            if (NicoTemporaryModel.Instance.Videos.Any(video => video.VideoId == vm.VideoId))
            {
                return;
            }

            using (var control = await DbUtil.GetControl())
            {
                await control.BeginTransaction();
                await control.InsertTTemporaryAsync(vm.VideoId);
                await control.Commit();
            }

            // ﾃﾝﾎﾟﾗﾘに追加
            NicoTemporaryModel.Instance.Videos.Add(vm);

            // ｽﾃｰﾀｽ変更
            vm.Status = VideoStatus.New;

            // 履歴に追加
            await NicoVideoHistoryModel.Instance.AddVideoHistory(vm.VideoId, VideoStatus.New);
            NicoTemporaryModel.Instance.Count += 1;

            //if (!NicoTemporaryModel.Instance.Videos.Any(video => video.VideoId == vm.VideoId))
            //{
            //    var itemid = vm.VideoId;
            //    var description = "";
            //    var token = await GetToken();
            //    var url = $"http://www.nicovideo.jp/api/deflist/add?item_type=0&item_id={itemid}&description={description}&token={token}";

            //    // 追加用URLを実行
            //    var txt = await WebUtil.GetStringAsync(url, true);

            //    // ﾃﾝﾎﾟﾗﾘに追加
            //    NicoTemporaryModel.Instance.Videos.Add(vm);

            //    // ｽﾃｰﾀｽ変更
            //    vm.Status = VideoStatus.New;

            //    // 履歴に追加
            //    await NicoVideoHistoryModel.Instance.AddVideoHistory(vm.VideoId, VideoStatus.New);
            //    NicoTemporaryModel.Instance.Count += 1;
            //}
        }

        public static async Task DeleteVideo(string id)
        {
            if (NicoTemporaryModel.Instance.Videos.Any(video => video.VideoId == id))
            {
                await DeleteVideo(NicoTemporaryModel.Instance.Videos.First(video => video.VideoId == id));
            }
        }

        public static async Task DeleteVideo(NicoVideoModel vm)
        {
            if (!NicoTemporaryModel.Instance.Videos.Any(video => video.VideoId == vm.VideoId))
            {
                return;
            }

            using (var control = await DbUtil.GetControl())
            {
                await control.BeginTransaction();
                await control.DeleteTTemporaryAsync(vm.VideoId);
                await control.Commit();
            }

            // ｽﾃｰﾀｽ更新
            vm.Status = NicoVideoHistoryModel.Instance.IsSee(vm.VideoId)
                ? VideoStatus.See
                : VideoStatus.None;

            // ﾃﾝﾎﾟﾗﾘから削除
            NicoTemporaryModel.Instance.Videos.Remove(
                NicoTemporaryModel.Instance.Videos.First(video => video.VideoId == vm.VideoId)
            );
            NicoTemporaryModel.Instance.Count -= 1;

            //if (NicoTemporaryModel.Instance.Videos.Any(video => video.VideoId == vm.VideoId))
            //{
            //    var itemid = await GetItemid(vm.VideoId);
            //    var token = await GetToken();
            //    var url = $"http://www.nicovideo.jp/api/deflist/delete?id_list[0][]={itemid}&token={token}";

            //    // 削除用URLを実行
            //    var txt = await WebUtil.GetStringAsync(url, true);

            //    // ｽﾃｰﾀｽ更新
            //    vm.Status = NicoVideoHistoryModel.Instance.IsSee(vm.VideoId)
            //        ? VideoStatus.See
            //        : VideoStatus.None;

            //    // ﾃﾝﾎﾟﾗﾘから削除
            //    NicoTemporaryModel.Instance.Videos.Remove(
            //        NicoTemporaryModel.Instance.Videos.First(video => video.VideoId == vm.VideoId)
            //    );
            //    NicoTemporaryModel.Instance.Count -= 1;
            //}
        }

        //private static async Task<string> GetToken()
        //{
        //    var url = "http://www.nicovideo.jp/my/top";
        //    var txt = await WebUtil.GetStringAsync(url, true);
        //    return Regex.Match(txt, "data-csrf-token=\"(?<token>[^\"]+)\"").Groups["token"].Value;
        //}

        //private static async Task<string> GetItemid(string id)
        //{
        //    var url = "http://www.nicovideo.jp/api/deflist/list";
        //    var json = await WebUtil.GetJsonAsync(url, true);

        //    foreach (dynamic item in json["mylistitem"])
        //    {
        //        if (id == item["item_data"]["video_id"])
        //        {
        //            return item["item_id"];
        //        }
        //    }
        //    return null;
        //}

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

        public static async Task<BitmapImage> GetThumbnailAsync(NicoVideoDetailViewModel vm)
        {
            var url = vm.ThumbnailUrl.Replace(".M", "").Replace(".L", "");
            var urls = new string[]
            {
                $"{url}.L",
                $"{url}.M",
                $"{url}",
            };

            return await GetThumbnailAsync(urls);
        }

        public static void Download(NicoVideoModel vm)
        {
            string path = ServiceFactory.MessageService.SelectedSaveFile(vm.Title, Properties.Resources.S_Filter_Common_MP4);

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

                    // ﾛｸﾞｲﾝｸｯｷｰ設定
                    handler.CookieContainer = await Session.Instance.TryLoginAsync();

                    // 対象動画にｼﾞｬﾝﾌﾟ
                    var watchurl = $"http://www.nicovideo.jp/watch/{vm.VideoId}";
                    await client.PostAsync(watchurl, null);

                    // 動画URL全文を取得
                    var flapiurl = $"http://flapi.nicovideo.jp/api/getflv/{vm.VideoId}";
                    var flapibody = await client.GetStringAsync(flapiurl);
                    var movieurl = Regex.Match(Uri.UnescapeDataString(flapibody), @"&url=.*").Value.Replace("&url=", "");
                    var res = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, flapiurl));

                    File.WriteAllBytes(path, await client.GetByteArrayAsync(movieurl));
                }
            });
        }
    }
}
