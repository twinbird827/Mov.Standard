using CefSharp;
using CefSharp.Wpf;
using Mov.Standard.Core.Nico;
using Mov.Standard.Windows;
using My.Core;
using My.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Workspaces
{
    public class NicoChromiumViewModel : WorkspaceViewModel
    {
        public override string Title => MenuType.NicoChromium.GetLabel();

        public NicoChromiumViewModel(string address)
        {
            Address = address;
        }

        public string Address
        {
            get => _Address;
            set => SetProperty(ref _Address, value);
        }
        private string _Address;

        public ICookieManager CookieManager
        {
            get => _CookieManager;
            set
            {
                if (SetProperty(ref _CookieManager, value) && value != null)
                {
                    SetCookies();
                }
            }
        }
        private ICookieManager _CookieManager;

        private void SetCookies()
        {
            ServiceFactory.MessageService.Debug("set cookie!!");
            //var cc = await NicoUtil.TryLoginAsync();
            //var url = "test";

            //foreach (var cookie in cc.GetCookies(new Uri(url)))
            //{
            //    cookie
            //}
            //await CookieManager.()
        }
    }
}
