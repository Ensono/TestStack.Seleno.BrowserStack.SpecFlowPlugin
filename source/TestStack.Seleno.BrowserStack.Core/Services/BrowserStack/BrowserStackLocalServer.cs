using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace TestStack.Seleno.BrowserStack.Core.Services.BrowserStack
{
    public class BrowserStackLocalServer : IBrowserStackLocalServer
    {
        private readonly IBrowserStackTunnel _tunnel;
        private static readonly KeyValuePair<string, string> EmptyStringPair = new KeyValuePair<string, string>();

        private static readonly List<KeyValuePair<string, string>> ValueCommands = new List
            <KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("localIdentifier", "-localIdentifier"),
                new KeyValuePair<string, string>("hosts", ""),
                new KeyValuePair<string, string>("proxyHost", "-proxyHost"),
                new KeyValuePair<string, string>("proxyPort", "-proxyPort"),
                new KeyValuePair<string, string>("proxyUser", "-proxyUser"),
                new KeyValuePair<string, string>("proxyPass", "-proxyPass")
            };

        private static readonly List<KeyValuePair<string, string>> BooleanCommands = new List
            <KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("v", "-vvv"),
                new KeyValuePair<string, string>("force", "-force"),
                new KeyValuePair<string, string>("forcelocal", "-forcelocal"),
                new KeyValuePair<string, string>("forceproxy", "-forceproxy"),
                new KeyValuePair<string, string>("onlyAutomate", "-onlyAutomate")
            };

        private string _folder = "";
        private string _accessKey = "";
        private string _argumentString = "";
        private string _customBinaryPath = "";
        private string _customLogPath = "";

        public BrowserStackLocalServer() : this(new BrowserStackTunnel()) { }

        public BrowserStackLocalServer(IBrowserStackTunnel tunnel)
        {
            _tunnel = tunnel;
        }

        public bool IsRunning
        {
            get { return _tunnel != null && _tunnel.IsConnected; }
        }


        public void Start(params KeyValuePair<string, string>[] options)
        {
            foreach (var pair in options)
            {
                var key = pair.Key;
                var value = pair.Value;
                AddArgs(key, value);
            }

            if (string.IsNullOrWhiteSpace(_accessKey))
            {
                _accessKey = Environment.GetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY");
                if (string.IsNullOrWhiteSpace(_accessKey))
                    throw new Exception("BROWSERSTACK_ACCESS_KEY cannot be empty. " +
                                        "Specify one by adding key to options or adding to the environment variable BROWSERSTACK_ACCESS_KEY.");
                Regex.Replace(_accessKey, @"\s+", "");
            }

            if (!string.IsNullOrWhiteSpace(_customLogPath))
            {
                _argumentString += "-logFile \"" + _customLogPath + "\" ";
            }

            _tunnel.AddBinaryPath(_customBinaryPath);
            _tunnel.AddBinaryArguments(_argumentString);
            var hasStarted = _tunnel.IsConnected;
            while (!hasStarted)
            {
                try
                {
                    _tunnel.Run(_accessKey, _folder, _customLogPath, "start");
                    hasStarted = true;
                }
                catch (Exception)
                {
                    _tunnel.FallbackPaths();
                }
            }
        }

        public void Stop()
        {
            _tunnel.Run(_accessKey, _folder, _customLogPath, "stop");
            _tunnel.Kill();
        }

        private void AddArgs(string key, string value)
        {
            key = key.Trim();

            if (key.Equals("key"))
            {
                _accessKey = value;
            }
            else if (key.Equals("f"))
            {
                _folder = value;
            }
            else if (key.Equals("binarypath"))
            {
                _customBinaryPath = value;
            }
            else if (key.Equals("logfile"))
            {
                _customLogPath = value;
            }
            else if (key.Equals("verbose"))
            {
            }
            else
            {
                var result = ValueCommands.Find(pair => pair.Key == key);
                if (!result.Equals(EmptyStringPair))
                {
                    _argumentString += result.Value + " " + value + " ";
                    return;
                }

                result = BooleanCommands.Find(pair => pair.Key == key);
                if (!result.Equals(EmptyStringPair))
                    if (value.Trim().ToLower() == "true")
                    {
                        _argumentString += result.Value + " ";
                        return;
                    }

                if (value.Trim().ToLower() == "true")
                    _argumentString += "-" + key + " ";
                else
                    _argumentString += "-" + key + " " + value + " ";
            }
        }

    }
}