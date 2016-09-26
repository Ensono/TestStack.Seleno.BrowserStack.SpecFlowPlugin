using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace TestStack.Seleno.BrowserStack.Core.Services.BrowserStack
{
    public class BrowserStackLocalServer : IBrowserStackLocalServer
    {
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
        protected readonly BrowserStackTunnel Tunnel= new BrowserStackTunnel();

        public bool IsRunning
        {
            get { return Tunnel != null && Tunnel.IsConnected; }
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

        public void Start(params KeyValuePair<string, string>[] options)
        {
            foreach (var pair in options)
            {
                var key = pair.Key;
                var value = pair.Value;
                AddArgs(key, value);
            }

            if ((_accessKey == null) || (_accessKey.Trim().Length == 0))
            {
                _accessKey = Environment.GetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY");
                if ((_accessKey == null) || (_accessKey.Trim().Length == 0))
                    throw new Exception("BROWSERSTACK_ACCESS_KEY cannot be empty. " +
                                        "Specify one by adding key to options or adding to the environment variable BROWSERSTACK_ACCESS_KEY.");
                Regex.Replace(_accessKey, @"\s+", "");
            }

            if (!string.IsNullOrWhiteSpace(_customLogPath))
            {
                _argumentString += "-logFile \"" + _customLogPath + "\" ";
            }

            Tunnel.AddBinaryPath(_customBinaryPath);
            Tunnel.AddBinaryArguments(_argumentString);
            while (true)
            {
                var except = false;
                try
                {
                    Tunnel.Run(_accessKey, _folder, _customLogPath, "start");
                }
                catch (Exception)
                {
                    except = true;
                }
                if (except)
                    Tunnel.FallbackPaths();
                else
                    break;
            }
        }

        public void Stop()
        {
            Tunnel.Run(_accessKey, _folder, _customLogPath, "stop");
            Tunnel.Kill();
        }
    }
}