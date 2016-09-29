
using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using TestStack.Seleno.BrowserStack.Core.Services.BrowserStack;

namespace TestStack.Seleno.Browserstack.UnitTests.Core.Services.BrowserStack
{
    [TestFixture]
    public class BrowserStackLocalServerSpecs
    {
        private IBrowserStackTunnel _tunnel;
        private BrowserStackLocalServer _sut;

        private string GenerateAccessKey()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }

        [SetUp]
        public void SetUp()
        {
            _tunnel = Substitute.For<IBrowserStackTunnel>();
            _tunnel.IsConnected.Returns(true);

            _sut = new BrowserStackLocalServer(_tunnel);

            Environment.SetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY", null);
        }

        [Test]
        public void IsRunning_ShouldReturnFalseWhenTunnelNotInitialised()
        {
            // Arrange
            var sut = new BrowserStackLocalServer(null);

            // Act && Assert
            sut.IsRunning.Should().BeFalse();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsRunning_ShouldBeBasedOnWhetherTunnelConnected(bool tunnelIsConnected)
        {
            // Arrange
            _tunnel.IsConnected.Returns(tunnelIsConnected);

            // Act && Assert
            _sut.IsRunning.Should().Be(tunnelIsConnected);
        }

        [Test]
        public void Start_ShouldThrowExceptionWhenNoAccessKeyIsSpecified()
        {
            // Arrange
            Action startingWithNoAccessKey = () => _sut.Start();

            // Act & Assert
            startingWithNoAccessKey
                .ShouldThrow<Exception>()
                .WithMessage("BROWSERSTACK_ACCESS_KEY cannot be empty. " +
                             "Specify one by adding key to options or adding to the environment variable BROWSERSTACK_ACCESS_KEY.");
        }

        [Test]
        public void Start_ShouldAddLogFileInArgumentStringWhenCustomLogPathSpecified()
        {
            // Arrange
            Environment.SetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY", GenerateAccessKey());
            var logFileOption = new KeyValuePair<string, string>("logfile", @"C:\local.log");
            // Act

            _sut.Start(logFileOption);

            // Assert
            _tunnel.Received(1).AddBinaryArguments("-logFile \"C:\\local.log\" ");
        }


        [Test]
        public void Start_ShouldAddCustomBinaryPathWhenSpecified()
        {
            // Arrange
            const string browserStackBinaryAbsolutePath = @"C:\browserstack.exe";
            var options = new[]
            {
                new KeyValuePair<string, string>("key", GenerateAccessKey()),
                new KeyValuePair<string, string>("binarypath", browserStackBinaryAbsolutePath),
            };

            // Act
            _sut.Start(options);

            // Assert
            _tunnel.Received(1).AddBinaryPath(browserStackBinaryAbsolutePath);
        }

      

        [Test]
        public void Start_ShouldStartRunningTunnelWithAccessKeyWithFolderAndCustomLogPath()
        {
            // Arrange
            var accessKey = GenerateAccessKey();
            const string logFile = @"C:\local.log";
            const string testingFolder = "/my/awesome/folder";
            var options = new[]
            {
                new KeyValuePair<string, string>("key", accessKey),
                new KeyValuePair<string, string>("f", testingFolder),
                new KeyValuePair<string, string>("forcelocal", "true"),
                new KeyValuePair<string, string>("logfile", logFile)
            };
            _tunnel.IsConnected.Returns(false);
            
            // Act
            _sut.Start(options);

            // Assert
            _tunnel.Received(1).Run(accessKey, testingFolder, logFile, "start");
        }

        [Test]
        public void Start_ShouldCallFallBackPathsUntilServerIsUpAndRunning_WhenStartRunningTunnelThrewAnException()
        {
            // Arrange
            var accessKey = GenerateAccessKey();
            const string logFile = @"C:\local.log";
            const string testingFolder = "/my/awesome/folder";
            var threeAttempts = 0;
            const int fallbackTwiceMax = 2;
            var options = new[]
            {
                new KeyValuePair<string, string>("key", accessKey),
                new KeyValuePair<string, string>("f", testingFolder),
                new KeyValuePair<string, string>("forcelocal", "true"),
                new KeyValuePair<string, string>("logfile", logFile)
            };
            _tunnel.IsConnected.Returns(false);
            _tunnel.When(t => t.Run(accessKey, testingFolder, logFile, "start"))
                .Do(c => ThrowExceptionFirst(ref threeAttempts, fallbackTwiceMax));

            // Act
            _sut.Start(options);

            // Assert
            _tunnel.Received(threeAttempts).Run(accessKey, testingFolder, logFile, "start");
            _tunnel.Received(fallbackTwiceMax).FallbackPaths();
        }


        [Test]
        public void Start_ShouldPropagateExceptionThrownByFallBackPaths_WhenCouldNotStartTunnel()
        {
            // Arrange
            var accessKey = GenerateAccessKey();
            const string logFile = @"C:\local.log";
            const string testingFolder = "/my/awesome/folder";
            var fallBackException = new Exception("Something bad happened preventing tunnel from starting");
            var options = new[]
            {
                new KeyValuePair<string, string>("key", accessKey),
                new KeyValuePair<string, string>("f", testingFolder),
                new KeyValuePair<string, string>("forcelocal", "true"),
                new KeyValuePair<string, string>("logfile", logFile)
            };
            _tunnel.IsConnected.Returns(false);
            _tunnel.When(t => t.Run(accessKey, testingFolder, logFile, "start")).Throw<Exception>();
            _tunnel.When(t => t.FallbackPaths()).Throw(fallBackException);

            // Act
            Action notAbleToStartTunnel = () => _sut.Start(options);

            // Assert
            notAbleToStartTunnel.ShouldThrow<Exception>().Which.Should().BeSameAs(fallBackException);
        }

        [Test]
        public void Stop_ShouldHaltTunnelAndKillRelatedProcesses()
        {
            // Arrange
            var accessKey = GenerateAccessKey();
            const string logFile = @"C:\local.log";
            const string testingFolder = "/my/awesome/folder";

            var options = new[]
            {
                new KeyValuePair<string, string>("key", accessKey),
                new KeyValuePair<string, string>("f", testingFolder),
                new KeyValuePair<string, string>("forcelocal", "true"),
                new KeyValuePair<string, string>("logfile", logFile)
            };
            _sut.Start(options);

            // Act
            _sut.Stop();

            // Assert
            _tunnel.Received(1).Run(accessKey, testingFolder, logFile, "stop");
            _tunnel.Received(1).Kill();
        }



        private void ThrowExceptionFirst(ref int tunnelStartAttempts, int maxNumberOfAttempts)
        {
            if (tunnelStartAttempts++ < maxNumberOfAttempts) throw new Exception();
        }
    }
}