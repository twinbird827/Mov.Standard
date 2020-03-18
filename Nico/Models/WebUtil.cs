using Codeplex.Data;
using Mov.Standard.Nico.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mov.Standard.Nico.Models
{
    public static class WebUtil
    {
        /// <summary>
        /// URLの内容を取得します。
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns></returns>
        public static async Task<string> GetStringAsync(string url, bool login)
        {
            using (var handler = new HttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                if (login)
                {
                    handler.CookieContainer = await Session.Instance.TryLoginAsync();
                }

                var txt = await client.GetStringAsync(url);

                //txt = txt.Replace("&copy;", "");
                //txt = txt.Replace("&nbsp;", " ");
                //txt = txt.Replace("&#x20;", " ");
                //txt = txt.Replace("&", "&amp;");

                return txt;
            }
        }

        /// <summary>
        /// URLの内容をJson形式で取得します。
        /// </summary>
        /// <param name="url">URL</param>
        public static async Task<dynamic> GetJsonAsync(string url, bool login)
        {
            return DynamicJson.Parse(await GetStringAsync(url, login));
        }

        /// <summary>
        /// URLの内容をXml形式で取得します。
        /// </summary>
        /// <param name="url">URL</param>
        public static async Task<XElement> GetXmlAsync(string url, bool login)
        {
            return ToXml(await GetStringAsync(url, login));
        }

        /// <summary>
        /// URLのchannelﾀｸﾞの内容をXml形式で取得します。
        /// </summary>
        /// <param name="url">URL</param>
        public static async Task<XElement> GetXmlChannelAsync(string url, bool login)
        {
            var xml = await GetXmlAsync(url, login);
            var tmp = xml.Descendants("channel");
            return tmp.First();
        }

        /// <summary>
        /// 文字列をXml形式に変換します。
        /// </summary>
        /// <param name="url">URL</param>
        public static XElement ToXml(string value)
        {
            return XDocument.Load(new StringReader(value)).Root;
        }
    }
}
