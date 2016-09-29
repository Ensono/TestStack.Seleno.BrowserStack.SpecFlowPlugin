using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Helpers;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using TestStack.Seleno.BrowserStack.Core.Services.BrowserStack;
using TestStack.Seleno.Browserstack.UnitTests.Assertions;

namespace TestStack.Seleno.Browserstack.UnitTests.Core.Services.BrowserStack
{
    [TestFixture]
    public class BrowserStackTunnelSpecs
    {
        private BrowserStackTunnel _sut;

        public string[] BinaryAbsolutePaths
        {
            get
            {
                return new[]
                {
                    Path.Combine(Path.Combine(Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%"), ".browserstack"), "BrowserStackLocal.exe"),
                    Path.Combine(Directory.GetCurrentDirectory(), "BrowserStackLocal.exe"),
                    Path.Combine(Path.GetTempPath(), "BrowserStackLocal.exe"),
                };
            }
        }

        [SetUp]
        public void SetUp()
        {
            _sut = Substitute.ForPartsOf<BrowserStackTunnel>();

            _sut.When(x => x.DownloadBinary()).DoNotCallBase();
        }

        [Test]
        public void Dispose_ShouldKillProcessWhenInstantiated()
        {
            // Arrange
            _sut.Process = Substitute.For<IProcess>();
            _sut.When(x => x.Kill()).DoNotCallBase();

            // Act
            _sut.Dispose();

            // Assert
            _sut.Received(1).Kill();
        }

        [Test]
        public void AddBinaryPath_ShouldSetBinaryAbsolute()
        {
            // Arrange
            const string binaryAbsolute = "/somwhere/on/local/drive";

            // Act
            _sut.AddBinaryPath(binaryAbsolute);

            // Assert
            _sut.BinaryAbsolute.Should().Be(binaryAbsolute);
        }

        [Test]
        public void AddBinaryPath_ShouldSetDefaultBinaryAbsolute()
        {
            // Arrange
            var binaryPaths = new[] { null, string.Empty, " ", "  "};

            for (var i = 0; i < binaryPaths.Length; i++)
            {
                // Act
                _sut.AddBinaryPath(binaryPaths[i]);

                // Assert
                _sut.BinaryAbsolute.Should().Be(BinaryAbsolutePaths[i % 3]);
            }
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void AddBinaryArguments_ShouldSetBinaryArguments(string arguments)
        {
            // Act
            _sut.AddBinaryArguments(arguments);

            // Assert
            _sut.BinaryArguments.Should().BeEmpty();
        }

        [Test]
        public void FallbackPaths_ShouldThrowExceptionsWhenNumberOfRetriesExceedAvailableBasePaths()
        {
            // Arrange
            for (var i = 0; i < BrowserStackTunnel.BasePaths.Length; i++) _sut.FallbackPaths();

            // Act
            Action exaustedNumberOfRetries = () => _sut.FallbackPaths();

            // Assert
            exaustedNumberOfRetries
                .ShouldThrow<Exception>()
                .WithMessage("No More Paths to try. Please specify a binary path in options.");
        }

        [TestCase("987987", "/folder", "/BrowserStackLocal.exe", "-d start -f 987987 /folder /BrowserStackLocal.exe")]
        [TestCase("145645", null, "/BrowserStackLocal.exe", "-d start 145645 /BrowserStackLocal.exe")]
        [TestCase("456789", null, "", "-d start 456789 ")]
        public void Run_ShouldStartProcessWithAccessKeyFolderAndBinaryPath_WhenSpecifiedWithProcessTypeStart(
            string accessKey, string testingFolder, string binaryPath, string arguments)
        {
            // Arrange
            _sut.BinaryArguments = binaryPath;
            _sut.WhenForAnyArgs(x => x.RunProcess(null, null)).DoNotCallBase();

            // Act
            _sut.Run(accessKey, testingFolder, null, "start");

            // Assert
            _sut.Received().RunProcess(arguments, "start");
        }

        [TestCase("start")]
        [TestCase("stop")]
        public void Run_ShouldDownloadBinary_WhenItDoesNotExistLocally(string processType)
        {
            // Arrange
            const string testingFolder = "/my/awesome/folder";
            const string logFile = @"C:\local.log";
            var accessKey = Guid.NewGuid().ToString().Replace("-", string.Empty);
            _sut.BinaryFileDoesNotExists.Returns(true);
            _sut.WhenForAnyArgs(x => x.RunProcess(null, null)).DoNotCallBase();

            // Act
            _sut.Run(accessKey, testingFolder, logFile, processType);

            // Assert
            _sut.Received(1).DownloadBinary();
        }

        [TestCase("start")]
        [TestCase("stop")]
        public void Run_ShouldNotDownloadBinary_WhenItDoesExistLocally(string processType)
        {
            // Arrange
            const string testingFolder = "/my/awesome/folder";
            const string logFile = @"C:\local.log";
            var accessKey = Guid.NewGuid().ToString().Replace("-", string.Empty);
            _sut.BinaryFileDoesNotExists.Returns(false);
            _sut.WhenForAnyArgs(x => x.RunProcess(null, null)).DoNotCallBase();

            // Act
            _sut.Run(accessKey, testingFolder, logFile, processType);

            // Assert
            _sut.DidNotReceive().DownloadBinary();
        }

        [TestCase("start")]
        [TestCase("stop")]
        public void Run_ShouldCloseExistingProcess(string processType)
        {
            // Arrange
            const string testingFolder = "/my/awesome/folder";
            const string logFile = @"C:\local.log";
            var accessKey = Guid.NewGuid().ToString().Replace("-", string.Empty);
            _sut.BinaryFileDoesNotExists.Returns(false);
            _sut.Process = Substitute.For<IProcess>();
            _sut.WhenForAnyArgs(x => x.RunProcess(null, null)).DoNotCallBase();

            // Act
            _sut.Run(accessKey, testingFolder, logFile, processType);

            // Assert
            _sut.Process.Received(1).Close();
        }

        [Test]
        public void Run_ShouldLogWhenStartingProcess()
        {
            // Arrange
            const string testingFolder = "/my/awesome/folder";
            const string logFile = @"C:\local.log";
            var accessKey = Guid.NewGuid().ToString().Replace("-", string.Empty);
            _sut.BinaryFileDoesNotExists.Returns(false);
            _sut.LogFileExists(logFile).Returns(true);
            _sut.When(x => x.Log(logFile)).DoNotCallBase();
            _sut.WhenForAnyArgs(x => x.RunProcess(null, null)).DoNotCallBase();

            // Act
            _sut.Run(accessKey, testingFolder, logFile, "start");

            // Assert
            _sut.Received(1).Log(logFile);
        }

        [Test]
        public void IsConnected_ShouldReturnTrueWhenLocalStateIsConnected()
        {
            // Arrange
            _sut.LocalState = LocalState.Connected;

            // Act
            _sut.IsConnected.Should().BeTrue();
        }

        [TestCase(LocalState.Connecting)]
        [TestCase(LocalState.Disconnected)]
        [TestCase(LocalState.Error)]
        [TestCase(LocalState.Idle)]
        public void IsConnected_ShouldReturnFalseWhenLocalStateIsNotConnected(LocalState localState)
        {
            // Arrange
            _sut.LocalState = localState;

            // Act
            _sut.IsConnected.Should().BeFalse();
        }

        [Test]
        public void Kill_ShouldCloseAndKillProcessAndSetLocaleStateToDisconnected()
        {
            // Arrange
            var someProcess = Substitute.For<IProcess>();
            _sut.Process = someProcess;
            
            // Act
            _sut.Kill();

            // Assert
            someProcess.Received(1).Close();
            someProcess.Received(1).Kill();
            _sut.Process.Should().BeNull();
            _sut.LocalState.Should().Be(LocalState.Disconnected);
        }

        [Test]
        public void RunProcess_ShouldStartNewProcessAndChangeLocalStateToConnecting()
        {
            // Arrange
            const string arguments = "-d start -f 987987 /folder /BrowserStackLocal.exe";
            const string processType = "start";
            var newProcess = Substitute.For<IProcess>();
            var binaryAbsolutePath = BinaryAbsolutePaths[0];
            var processStartInfo = default(ProcessStartInfo);

            _sut.CreateProcess(Arg.Any<ProcessStartInfo>()).Returns(newProcess).AndDoes(c => processStartInfo = (ProcessStartInfo)c[0]);
            _sut.BinaryAbsolute = binaryAbsolutePath;

            // Act
            _sut.RunProcess(arguments, processType);

            // Assert
            processStartInfo.Should().MatchWith(binaryAbsolutePath, arguments);
            Received.InOrder(() =>
                    {
                        newProcess.Start();
                        newProcess.BeginOutputReadLine();
                        newProcess.BeginErrorReadLine();
                        newProcess.WaitForExit();
                    });
        }

        [Test]
        public void ErrorDataReceivedEvent_ShouldThrowAnExceptionWhenRaisedWithErrorDataReceived()
        {
            // Arrange
            const string arguments = "-d start -f 987987 /folder /BrowserStackLocal.exe";
            const string processType = "start";
            bool wasCalled = false;

            var newProcess = Substitute.For<IProcess>();
            var eventArgs = DataReceivedEventArgs(new { state = "error" });


            _sut.CreateProcess(Arg.Any<ProcessStartInfo>()).Returns(newProcess);
            _sut.RunProcess(arguments, processType);
            

            // Act
            Action errorConnecting = () => newProcess.ErrorDataReceived += Raise.Event<DataReceivedEventHandler>(new object(),eventArgs);
            

           // Assert
            errorConnecting
                .ShouldThrow<Exception>()
                .WithMessage($"Eror while executing BrowserStackLocal {processType} {eventArgs.Data}");


        }

        [Test]
        public void OutputDataReceivedEvent_ShouldUpdateLocalStateToConnectedWhenStateFromDataReceivedEventArgsIsConnected()
        {
            // Arrange
            const string arguments = "-d start -f 987987 /folder /BrowserStackLocal.exe";
            const string processType = "start";

            var newProcess = Substitute.For<IProcess>();
            var eventArgs = DataReceivedEventArgs(new { state = "connected" });


            _sut.CreateProcess(Arg.Any<ProcessStartInfo>()).Returns(newProcess);
            _sut.RunProcess(arguments, processType);


            // Act
            newProcess.OutputDataReceived += Raise.Event<DataReceivedEventHandler>(new object(), eventArgs);


            // Assert
            _sut.LocalState.Should().Be(LocalState.Connected);
        }

        [Test]
        public void ProcessExitedEvent_ShouldSetProcessToNull()
        {
            // Arrange
            const string arguments = "-d start -f 987987 /folder /BrowserStackLocal.exe";
            const string processType = "start";

            var newProcess = Substitute.For<IProcess>();


            _sut.CreateProcess(Arg.Any<ProcessStartInfo>()).Returns(newProcess);
            _sut.RunProcess(arguments, processType);


            // Act
            newProcess.Exited += Raise.EventWith(new EventArgs());
            
            // Assert
            _sut.Received(1).Process = null;
        }


        [Test]
        public void AppDomainProcessExitedEvent_ShouldKillProcess()
        {
            // Arrange
            const string arguments = "-d start -f 987987 /folder /BrowserStackLocal.exe";
            const string processType = "start";

            var newProcess = Substitute.For<IProcess>();

            _sut.CreateProcess(Arg.Any<ProcessStartInfo>()).Returns(newProcess);
            _sut.RunProcess(arguments, processType);
            _sut.When(x => x.Kill()).DoNotCallBase();

            // Act
            newProcess.CurrentAppDomainProcessExited += Raise.EventWith(new object(),new EventArgs());

            // Assert
            _sut.Received(1).Kill();
        }

        private DataReceivedEventArgs DataReceivedEventArgs(object data)
        {
            string jsonData = null;
            if (data != null) jsonData = JsonConvert.SerializeObject(data);;

            return 
                    (DataReceivedEventArgs)
                    typeof(DataReceivedEventArgs).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0].Invoke(new object[] { jsonData });
        }


    }
}