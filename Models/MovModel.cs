using Mov.Standard.Core;
using My.Core;
using My.Wpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mov.Standard.Models
{
    public class MovModel
    {
        public static MovModel Instance { get; private set; } = new MovModel();

        public ComboboxItemModel[] Combos { get; private set; }

        public async Task LoadXmlAsync()
        {
            var combo = XDocument.Load(CoreUtil.RelativePathToAbsolutePath(MovConst.NicoComboPath)).Root;

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

            await Task.Delay(1);
        }
    }
}
