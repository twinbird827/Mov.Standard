using Codeplex.Data;
using Mov.Standard.Models;
using My.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        public static DateTime ToRankingDatetime(XElement e, string name)
        {
            // 2018年02月27日 20：00：00
            var s = GetData(e, name);

            return DateTime.ParseExact(s,
                "yyyy年MM月dd日 HH：mm：ss",
                System.Globalization.DateTimeFormatInfo.InvariantInfo,
                System.Globalization.DateTimeStyles.None
            );

        }

        public static NicoVideoModel CreateVideoFromXml(XElement item, string view, string mylist, string comment)
        {
            try
            {
                // 明細部読み込み
                var descriptionString = item.Element("description").Value;
                //descriptionString = descriptionString.Replace("&nbsp;", "&#x20;");
                ////descriptionString = HttpUtility.HtmlDecode(descriptionString);
                //descriptionString = descriptionString.Replace("&", "&amp;");
                ////descriptionString = descriptionString.Replace("'", "&apos;");
                var desc = WebUtil.ToXml($"<root>{descriptionString}</root>");

                var video = new NicoVideoModel()
                {
                    VideoId = item.Element("link").Value.Split('/').Last(),
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
                ServiceFactory.MessageService.Exception(ex);
                throw;
            }
        }

        public static async Task<IEnumerable<VideoModel>> GetRanking(string genre, string tag, string term)
        {
            //var p = period.Value;
            //var g = genre.Value;
            //var t = "all";

            var url = $"http://www.nicovideo.jp/ranking/genre/{genre}?tag={tag}&term={term}&rss=2.0&lang=ja-jp";
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

        public static async Task<IEnumerable<VideoModel>> GetTemporary()
        {
            const string url = "http://www.nicovideo.jp/api/deflist/list";

            var json = await WebUtil.GetJsonAsync(url, true);

            var videos = new List<NicoVideoModel>();

            foreach (dynamic item in json["mylistitem"])
            {
                VideoModel video;

                if (item["item_data"]["deleted"] == "0")
                {
                    video = await VideoModel.CreateInstance(item["item_data"]["video_id"]);
                }
                else
                {
                    video = new VideoModel();
                    video.VideoUrl = item["item_data"]["video_id"];
                    video.Title = item["item_data"]["title"];
                    video.Description = item["description"];
                    //video.Tags = data["tags"];
                    //video.CategoryTag = data["categoryTags"];
                    video.ViewCounter = long.Parse(item["item_data"]["view_counter"]);
                    video.MylistCounter = long.Parse(item["item_data"]["mylist_counter"]);
                    video.CommentCounter = long.Parse(item["item_data"]["num_res"]);
                    video.StartTime = NicoUtil.FromUnixTime((long)item["item_data"]["first_retrieve"]);
                    //video.LastCommentTime = Converter.item(data["lastCommentTime"]);
                    video.LengthSeconds = long.Parse(item["item_data"]["length_seconds"]);
                    video.ThumbnailUrl = item["item_data"]["thumbnail_url"];
                    //video.LastResBody = item["item_data"]["last_res_body"];
                    //video.CommunityIcon = data["communityIcon"];
                }

                Videos.Add(video);
            }
        }
    }
}
