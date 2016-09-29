using System;
using System.Diagnostics;

namespace TestStack.Seleno.BrowserStack.Core.Services.BrowserStack
{
    public interface IProcess
    {
        void Close();

        void Kill();

        event EventHandler Exited;

        event DataReceivedEventHandler OutputDataReceived;

        event DataReceivedEventHandler ErrorDataReceived;

        event EventHandler CurrentAppDomainProcessExited;

        void Start();

        void BeginOutputReadLine();

        void BeginErrorReadLine();

        void WaitForExit();
    }

    public class ProcessWrapper : IProcess
    {
        private readonly Process _process;

        public ProcessWrapper(Process process)
        {
            _process = process;
        }

        public void Close()
        {
            _process.Close();
        }

        public void Kill()
        {
            _process.Kill();
        }

        public event EventHandler Exited
        {
            add { _process.Exited += value; }
            remove { _process.Exited -= value; }
        }

        public event DataReceivedEventHandler OutputDataReceived
        {
            add { _process.OutputDataReceived += value; }
            remove { _process.OutputDataReceived -= value; }
        }

        public event DataReceivedEventHandler ErrorDataReceived
        {
            add { _process.ErrorDataReceived += value; }
            remove { _process.ErrorDataReceived -= value; }
        }

        public event EventHandler CurrentAppDomainProcessExited
        {
            add { AppDomain.CurrentDomain.ProcessExit += value; }
            remove { AppDomain.CurrentDomain.ProcessExit -= value; }
        }

        public bool EnableRaisingEvents
        {
            get { return _process.EnableRaisingEvents; }
            set { _process.EnableRaisingEvents = value; }
        }

        public void Start()
        {
            _process.Start();
        }

        public void BeginOutputReadLine()
        {
            _process.BeginOutputReadLine();
        }

        public void BeginErrorReadLine()
        {
            _process.BeginErrorReadLine();
        }

        public void WaitForExit()
        {
            _process.WaitForExit();
        }

    }
}