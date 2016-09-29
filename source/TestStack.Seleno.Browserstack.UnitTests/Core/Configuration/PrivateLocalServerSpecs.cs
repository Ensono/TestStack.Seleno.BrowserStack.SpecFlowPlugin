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
        private IDateTimeProvider _dateTimeProvider;

        [SetUp]
        public void SetUp()
        {
            _configuration = Substitute.For<IConfigurationProvider>();
            _localServer = Substitute.For<IBrowserStackLocalServer>();
            _dateTimeProvider = Substitute.For<IDateTimeProvider>();
            _sut = Substitute.ForPartsOf<PrivateLocalServer>(_localServer, _configuration, _dateTimeProvider);
        }

        [Test]
        public void Start_ShouldStartServerWithAccessKeyAndForceLocale()
        {
            // Arrange
            var accessKey = Guid.NewGuid().ToString().Replace("-", string.Empty);

            _configuration.AccessKey.Returns(accessKey);
            _localServer.IsRunning.Returns(false);
            _sut.When(x => x.WaitUntilServerHasStarted()).DoNotCallBase();
            
            // Act
            _sut.Start();

            // Assert
            _localServer.Received(1).Start(new KeyValuePair<string, string>("key", _configuration.AccessKey),
                                           new KeyValuePair<string, string>("forcelocal", "true"),
                                           new KeyValuePair<string, string>("force", "true"));
            _sut.Received(1).WaitUntilServerHasStarted();
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
            _sut.DidNotReceive().WaitUntilServerHasStarted();
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
        public void WaitUntilServerHasStarted_ShouldTimeOutAfter30SecondsWhenServerDidNotStart()
        {
            // Arrange
            _localServer.IsRunning.Returns(false);
            var now = new DateTime(2016,11,1, 9,0,0);
            var numberOfIteration = 0;

            _dateTimeProvider
                .Now
                .Returns(now, now.AddSeconds(10), now.AddSeconds(20))
                .AndDoes(c => ++numberOfIteration);
            
            // Act
            _sut.WaitUntilServerHasStarted();

            // Assert
            numberOfIteration.Should().Be(3);
        }

        [Test]
        public void WaitUntilServerHasStarted_ShouldNotWait_WhenServerHasStarted()
        {
            // Arrange
            _localServer.IsRunning.Returns(true);
            var now = new DateTime(2016, 11, 1, 9, 0, 0);
            var numberOfIteration = 0;

            _dateTimeProvider
                .Now
                .Returns(now, now.AddSeconds(10), now.AddSeconds(20)).AndDoes(c => ++numberOfIteration);

            // Act
            _sut.WaitUntilServerHasStarted();

            // Assert
            numberOfIteration.Should().Be(1);
        }
    }
}