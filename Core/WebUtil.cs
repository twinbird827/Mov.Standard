using Codeplex.Data;
using My.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mov.Standard.Core
{
    public static class WebUtil
    {
        /// <summary>
        /// URLの内容を取得します。
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns></returns>
        public static async Task<string> GetStringAsync(string url, CookieContainer cookie = null, bool retry = true)
        {
            using (var handler = new HttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                if (cookie != null)
                {
                    handler.CookieContainer = cookie;
                }

                try
                {
                    var txt = await client.GetStringAsync(url);

                    return txt;
                }
                catch (HttpRequestException ex)
                {
                    if (retry)
                    {
                        await Task.Delay(1000);

                        return await GetStringAsync(url, cookie, false);
                    }
                    else
                    {
                        ServiceFactory.MessageService.Exception(ex);
                        throw;
                    }
                }

                //txt = txt.Replace("&copy;", "");
                //txt = txt.Replace("&nbsp;", " ");
                //txt = txt.Replace("&#x20;", " ");
                //txt = txt.Replace("&", "&amp;");

            }
        }

        /// <summary>
        /// URLの内容をJson形式で取得します。
        /// </summary>
        /// <param name="url">URL</param>
        public static async Task<dynamic> GetJsonAsync(string url, CookieContainer cookie = null)
        {
            return DynamicJson.Parse(await GetStringAsync(url, cookie));
        }

        /// <summary>
        /// URLの内容をXml形式で取得します。
        /// </summary>
        /// <param name="url">URL</param>
        public static async Task<XElement> GetXmlAsync(string url, CookieContainer cookie = null)
        {
            return ToXml(await GetStringAsync(url, cookie));
        }

        /// <summary>
        /// URLのchannelﾀｸﾞの内容をXml形式で取得します。
        /// </summary>
        /// <param name="url">URL</param>
        public static async Task<XElement> GetXmlChannelAsync(string url, CookieContainer cookie = null)
        {
            var xml = await GetXmlAsync(url, cookie);
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
