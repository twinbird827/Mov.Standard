using My.Wpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Windows
{
    public class NicoAccountViewModel : ModalWindowViewModel
    {
        public NicoAccountViewModel(string mail, string password)
        {
            Mail = mail;
            Password = password;
        }

        public string Mail
        {
            get { return _Mail; }
            set { SetProperty(ref _Mail, value, true); }
        }
        private string _Mail;

        public string Password
        {
            get { return _Password; }
            set { SetProperty(ref _Password, value, true); }
        }
        private string _Password;

        protected override bool CanClickOK
        {
            get => !string.IsNullOrEmpty(Mail) && !string.IsNullOrEmpty(Password);
            set => base.CanClickOK = value;
        }
    }
}
