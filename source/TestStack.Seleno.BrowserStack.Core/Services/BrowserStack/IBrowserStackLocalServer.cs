using System.Collections.Generic;

namespace TestStack.Seleno.BrowserStack.Core.Services.BrowserStack
{
    public interface IBrowserStackLocalServer
    {
        bool IsRunning { get; }
        void Start(params KeyValuePair<string, string>[] options);
        void Stop();
    }
}