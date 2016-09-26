using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using TestStack.Seleno.BrowserStack.Core.Configuration;

namespace TestStack.Seleno.Browserstack.UnitTests.Core.Configuration
{
    [TestFixture]
    public class PrivateLocalServerSpecs
    {
        private PrivateLocalServer _sut;
        private IConfigurationProvider _configuration;

        [SetUp]
        public void SetUp()
        {
            _configuration = Substitute.For<IConfigurationProvider>();
            _sut = Substitute.ForPartsOf<PrivateLocalServer>(_configuration);    
        }

        [Test]
        public void Constructor_ShouldNotStartLocalServer()
        {
            // Assert
            _sut.DidNotReceiveWithAnyArgs().StartServerWithOptions();
            _sut.IsRunning.Should().BeFalse();
        }

        [Test]
        public void Start_ShouldStartServerWithAccessKeyAndForceLocale()
        {
            // Arrange
            var accessKey = Guid.NewGuid().ToString().Replace("-", string.Empty);

            _configuration.AccessKey.Returns(accessKey);
            _sut.WhenForAnyArgs(x => x.StartServerWithOptions()).DoNotCallBase();

            // Act
            _sut.Start();

            // Assert
            _sut.Received(1)
                .StartServerWithOptions(new KeyValuePair<string, string>("key", _configuration.AccessKey),
                                        new KeyValuePair<string, string>("forcelocal", "true"));
        }


        [Test]
        public void Start_ShouldNotStartServerAgainWhenAlreadyRunning()
        {
            // Arrange
            var accessKey = Guid.NewGuid().ToString().Replace("-", string.Empty);

            _configuration.AccessKey.Returns(accessKey);
            _sut.WhenForAnyArgs(x => x.StartServerWithOptions()).DoNotCallBase();
            _sut.IsRunning.Returns(true);

            // Act
            _sut.Start();

            // Assert
            _sut.DidNotReceiveWithAnyArgs().StartServerWithOptions();
        }

        [Test]
        public void Stop_ShouldStopLocalServerOnlyWhenRunning()
        {
            // Arrange
            _sut.IsRunning.Returns(true);
            _sut.When(x => x.StopServer()).DoNotCallBase();

            // Act
            _sut.Stop();

            // Assert
            _sut.Received(1).StopServer();
        }

        [Test]
        public void Stop_ShouldNotStopLocalServerWhenWasNotStarted()
        {
            // Arrange
            var accessKey = Guid.NewGuid().ToString().Replace("-", string.Empty);
            _configuration.AccessKey.Returns(accessKey);
            _sut.IsRunning.Returns(false);

            // Act
            _sut.Stop();

            // Assert
            _sut.DidNotReceive().StopServer();
            _sut.IsRunning.Should().BeFalse();
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