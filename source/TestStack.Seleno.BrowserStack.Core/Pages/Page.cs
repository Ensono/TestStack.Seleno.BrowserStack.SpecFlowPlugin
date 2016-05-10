using System;
using TestStack.Seleno.BrowserStack.Core.Extensions;

namespace TestStack.Seleno.BrowserStack.Core.Pages
{
    public abstract class Page : UiComponent
    {
        public string Title
        {
            get
            {
                return Browser.TitleWithWait();
            }
        }

        public string Url
        {
            get { return Browser.Url; }
        }

    }
}