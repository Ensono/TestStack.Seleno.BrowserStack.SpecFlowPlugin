using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Newtonsoft.Json.Linq;
using TestStack.Seleno.BrowserStack.Core.Extensions;

namespace TestStack.Seleno.BrowserStack.Core.Services.BrowserStack
{
    public interface IBrowserStackTunnel : IDisposable
    {
        void AddBinaryPath(string binaryAbsolute);
        void AddBinaryArguments(string binaryArguments);
        void FallbackPaths();
        void Run(string accessKey, string folder, string logFilePath, string processType);
        bool IsConnected { get; }
        void Kill();
    }

    public class BrowserStackTunnel : IBrowserStackTunnel
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
        internal IProcess Process { get; set; }
        internal string BinaryAbsolute = "";
        internal string BinaryArguments = "";
        internal LocalState LocalState = LocalState.Idle;
        internal string LogFilePath = "";


        public void Dispose()
        {
            if (Process != null)
                Kill();
        }

        public virtual void AddBinaryPath(string binaryAbsolute)
        {
            if (string.IsNullOrWhiteSpace(binaryAbsolute))
                binaryAbsolute = Path.Combine(BasePaths.CyclicElementAtOrDefault(++_basePathsIndex), BinaryName);
            BinaryAbsolute = binaryAbsolute;
        }

        public virtual void AddBinaryArguments(string binaryArguments)
        {
            BinaryArguments = string.IsNullOrWhiteSpace(binaryArguments) ? string.Empty : binaryArguments;
        }

        public virtual void FallbackPaths()
        {
            if (_basePathsIndex >= BasePaths.Length - 1)
                throw new Exception("No More Paths to try. Please specify a binary path in options.");
            _basePathsIndex++;
            BinaryAbsolute = Path.Combine(BasePaths[_basePathsIndex], BinaryName);
        }

        public virtual void Run(string accessKey, string folder, string logFilePath, string processType)
        {
            var arguments = "-d " + processType + " ";
            if ((folder != null) && (folder.Trim().Length != 0))
                arguments += "-f " + accessKey + " " + folder + " " + BinaryArguments;
            else
                arguments += accessKey + " " + BinaryArguments;

            if (BinaryFileDoesNotExists)
            {
                DownloadBinary();
            }

            Process?.Close();

            if (processType.ToLower().Contains("start") && !string.IsNullOrEmpty(logFilePath) && LogFileExists(logFilePath))
                Log(logFilePath);

            RunProcess(arguments, processType);
        }

        public virtual bool IsConnected
        {
            get { return LocalState == LocalState.Connected; }
        }

        public virtual void Kill()
        {
            if (Process != null)
            {
                Process.Close();
                Process.Kill();
                Process = null;
                LocalState = LocalState.Disconnected;
            }
        }

        internal virtual bool LogFileExists(string logFilePath)
        {
            return File.Exists(logFilePath);
        }

        internal virtual void Log(string logFilePath)
        {
            File.WriteAllText(logFilePath, string.Empty);
        }

        internal virtual bool BinaryFileDoesNotExists
        {
            get { return !File.Exists(BinaryAbsolute); }
        }

        internal virtual void RunProcess(string arguments, string processType)
        {
            Process = CreateProcess(new ProcessStartInfo
            {
                FileName = BinaryAbsolute,
                Arguments = arguments,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false
            });

            DataReceivedEventHandler o = (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(e.Data)) return;
                var binaryOutput = JObject.Parse(e.Data);
                if ((binaryOutput.GetValue("state") != null) &&
                    !binaryOutput.GetValue("state").ToString().ToLower().Equals("connected"))
                {
                    LocalState = LocalState.Error;
                    throw new Exception("Eror while executing BrowserStackLocal " + processType + " " + e.Data);
                }

                LocalState =
                    string.Equals(binaryOutput.GetValue("state")?.ToString(), "connected",
                        StringComparison.InvariantCultureIgnoreCase)
                        ? LocalState.Connected
                        : LocalState.Connecting;
            };

            Process.OutputDataReceived += o;
            Process.ErrorDataReceived += o;
            Process.Exited += (s, e) => Process = null;

            Process.Start();

            Process.BeginOutputReadLine();
            Process.BeginErrorReadLine();

            LocalState = LocalState.Connecting;
            Process.CurrentAppDomainProcessExited +=  (s, e) => Kill();

            Process.WaitForExit();
        }

        internal virtual IProcess CreateProcess(ProcessStartInfo processStartInfo)
        {
            return new ProcessWrapper(new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            });
        }

        internal virtual void DownloadBinary()
        {
            var binaryDirectory = Path.Combine(BinaryAbsolute, "..");

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

      
    }
}