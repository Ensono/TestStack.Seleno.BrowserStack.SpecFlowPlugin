using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TestStack.Seleno.BrowserStack.Core.Services.BrowserStack
{
    public class BrowserStackTunnel : IDisposable
    {
        private static readonly string BinaryName = "BrowserStackLocal.exe";

        private static readonly string DownloadUrl =
            "https://s3.amazonaws.com/browserStack/browserstack-local/BrowserStackLocal.exe";

        public static readonly string[] BasePaths =
        {
            Path.Combine(Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%"), ".browserstack"),
            Directory.GetCurrentDirectory(),
            Path.GetTempPath()
        };

        private int _basePathsIndex = -1;

        private Process _process;
        protected string BinaryAbsolute = "";
        protected string BinaryArguments = "";
        public LocalState LocalState;
        protected string LogFilePath = "";
        protected FileSystemWatcher LogfileWatcher;

        protected StringBuilder Output;

        public BrowserStackTunnel()
        {
            LocalState = LocalState.Idle;
            Output = new StringBuilder();
        }

        public void Dispose()
        {
            if (_process != null)
                Kill();
        }

        public virtual void AddBinaryPath(string binaryAbsolute)
        {
            if ((binaryAbsolute == null) || (binaryAbsolute.Trim().Length == 0))
                binaryAbsolute = Path.Combine(BasePaths[++_basePathsIndex], BinaryName);
            BinaryAbsolute = binaryAbsolute;
        }

        public virtual void AddBinaryArguments(string binaryArguments)
        {
            if (binaryArguments == null)
                binaryArguments = "";
            BinaryArguments = binaryArguments;
        }

        public virtual void FallbackPaths()
        {
            if (_basePathsIndex >= BasePaths.Length - 1)
                throw new Exception("No More Paths to try. Please specify a binary path in options.");
            _basePathsIndex++;
            BinaryAbsolute = Path.Combine(BasePaths[_basePathsIndex], BinaryName);
        }

        public void DownloadBinary()
        {
            var binaryDirectory = Path.Combine(BinaryAbsolute, "..");
            //string binaryAbsolute = Path.Combine(binaryDirectory, binaryName);

            Directory.CreateDirectory(binaryDirectory);

            using (var client = new WebClient())
            {
                client.DownloadFile(DownloadUrl, BinaryAbsolute);
            }

            if (!File.Exists(BinaryAbsolute))
                throw new Exception("Error accessing file " + BinaryAbsolute);

            var dInfo = new DirectoryInfo(BinaryAbsolute);
            var dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
        }

        public virtual void Run(string accessKey, string folder, string logFilePath, string processType)
        {
            var arguments = "-d " + processType + " ";
            if ((folder != null) && (folder.Trim().Length != 0))
                arguments += "-f " + accessKey + " " + folder + " " + BinaryArguments;
            else
                arguments += accessKey + " " + BinaryArguments;
            if (!File.Exists(BinaryAbsolute))
                DownloadBinary();

            _process?.Close();

            if (processType.ToLower().Contains("start") && !string.IsNullOrEmpty(logFilePath) && File.Exists(logFilePath))
                File.WriteAllText(logFilePath, string.Empty);

            RunProcess(arguments, processType);
        }

        private void RunProcess(string arguments, string processType)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = BinaryAbsolute,
                Arguments = arguments,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false
            };

            _process = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };
            DataReceivedEventHandler o = (s, e) =>
            {
                if (e.Data != null)
                {
                    JObject binaryOutput = JObject.Parse(e.Data);
                    if ((binaryOutput.GetValue("state") != null) &&
                        !binaryOutput.GetValue("state").ToString().ToLower().Equals("connected"))
                        throw new Exception("Eror while executing BrowserStackLocal " + processType + " " + e.Data);
                }
            };

            _process.OutputDataReceived += o;
            _process.ErrorDataReceived += o;
            _process.Exited += (s, e) => { _process = null; };

            _process.Start();

            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            TunnelStateChanged(LocalState.Idle, LocalState.Connecting);
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Kill();

            _process.WaitForExit();
        }

        private void TunnelStateChanged(LocalState prevState, LocalState state)
        {
        }

        public bool IsConnected
        {
            get { return LocalState == LocalState.Connected; }
        }

        public void Kill()
        {
            if (_process != null)
            {
                _process.Close();
                _process.Kill();
                _process = null;
                LocalState = LocalState.Disconnected;
            }
        }
    }
}