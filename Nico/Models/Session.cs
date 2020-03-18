using Mov.Standard.Core.Databases;
using My.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Nico.Models
{
    public class Session
    {
        public static Session Instance => new Session();

        private CookieContainer Cookies { get; set; }

        public async Task<CookieContainer> TryLoginAsync()
        {
            using (var command = DbUtil.GetControl())
            {
                var settings = await command.SelectTSetting();
                var mail = settings.FirstOrDefault(s => s.Group == "setting" && s.Key == "mail")?.Value;
                var pass = settings.FirstOrDefault(s => s.Group == "setting" && s.Key == "password")?.Value;
                return await TryLoginAsync(mail, pass);
            }
        }

        public async Task<CookieContainer> TryLoginAsync(string mail, string password)
        {
            var cookies = await LoginAsync(mail, password);

            if (cookies != null)
            {
                return Cookies = cookies;
            }
            else
            {
                return null;
            }
        }

        private async Task<CookieContainer> LoginAsync(string mail, string password)
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
