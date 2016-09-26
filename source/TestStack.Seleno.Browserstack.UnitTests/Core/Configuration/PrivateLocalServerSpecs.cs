using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using TestStack.Seleno.BrowserStack.Core.Configuration;
using TestStack.Seleno.BrowserStack.Core.Services.BrowserStack;

namespace TestStack.Seleno.Browserstack.UnitTests.Core.Configuration
{
    [TestFixture]
    public class PrivateLocalServerSpecs
    {
        private PrivateLocalServer _sut;
        private IConfigurationProvider _configuration;
        private IBrowserStackLocalServer _localServer;

        [SetUp]
        public void SetUp()
        {
            _configuration = Substitute.For<IConfigurationProvider>();
            _localServer = Substitute.For<IBrowserStackLocalServer>();
            _sut = new PrivateLocalServer(_localServer, _configuration);
        }

        [Test]
        public void Start_ShouldStartServerWithAccessKeyAndForceLocale()
        {
            // Arrange
            var accessKey = Guid.NewGuid().ToString().Replace("-", string.Empty);

            _configuration.AccessKey.Returns(accessKey);
            _localServer.IsRunning.Returns(false);

            // Act
            _sut.Start();

            // Assert
            _localServer.Received(1).Start(new KeyValuePair<string, string>("key", _configuration.AccessKey),
                                           new KeyValuePair<string, string>("forcelocal", "true"));
        }


        [Test]
        public void Start_ShouldNotStartServerAgainWhenAlreadyRunning()
        {
            // Arrange
            var accessKey = Guid.NewGuid().ToString().Replace("-", string.Empty);

            _configuration.AccessKey.Returns(accessKey);
            _localServer.IsRunning.Returns(true);

            // Act
            _sut.Start();

            // Assert
            _localServer.DidNotReceiveWithAnyArgs().Start();
        }

        [Test]
        public void Stop_ShouldStopLocalServerOnlyWhenRunning()
        {
            // Arrange
            _localServer.IsRunning.Returns(true);

            // Act
            _sut.Stop();

            // Assert
            _localServer.Received(1).Stop();
        }

        [Test]
        public void Stop_ShouldNotStopLocalServerWhenWasNotStarted()
        {
            // Arrange
            var accessKey = Guid.NewGuid().ToString().Replace("-", string.Empty);
            _configuration.AccessKey.Returns(accessKey);
            _localServer.IsRunning.Returns(false);

            // Act
            _sut.Stop();

            // Assert
            _localServer.DidNotReceive().Stop();
        }

        [Test]
        public void BaseUrl_ShouldBeSameAsConfigurationRemoteUrl()
        {
            // Arrange
            const string remoteUrl = "http:/localhost/some/where";
            _configuration.RemoteUrl.Returns(remoteUrl);
            
            // Act && Assert
            _sut.BaseUrl.Should().Be(remoteUrl);

        }
    }
}