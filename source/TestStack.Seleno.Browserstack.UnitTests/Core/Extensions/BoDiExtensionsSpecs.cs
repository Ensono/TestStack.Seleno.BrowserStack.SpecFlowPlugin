
using System;
using BoDi;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using TestStack.Seleno.BrowserStack.Core.Extensions;
using TestStack.Seleno.BrowserStack.Core.Pages;

namespace TestStack.Seleno.Browserstack.UnitTests.Core.Extensions
{
    [TestFixture]
    public class BoDiExtensionsSpecs
    {
        public class Animal { }
        public class Human : Animal { }
        public class DummyPage : Page { }

        [Test]
        public void DoesNotContains_ShouldReturnTrueWhenContainerIsNullForAnyType()
        {
            // Arrange
            IObjectContainer container = null;
            var anyType = GetType();

            // Act & Assert
            container.DoesNotContains(anyType).Should().BeTrue();
        }

        [Test]
        public void DoesNotContains_ShouldReturnTrueWhenContainerCannotResolveSpecifiedType()
        {
            // Arrange
            var container = Substitute.For<IObjectContainer>();
            var type = typeof (IServiceProvider);
            container.ResolveAll<IServiceProvider>().Returns(new IServiceProvider[0]);

            // Act & Assert
            container.DoesNotContains(type).Should().BeTrue();
        }

        [Test]
        public void DoesNotContains_ShouldReturnFalseWhenContainerCanResolveSpecifiedTypeAndName()
        {
            // Arrange
            var container = Substitute.For<IObjectContainer>();
            var type = typeof(IServiceProvider);
            container.ResolveAll<IServiceProvider>().Returns(new [] { Substitute.For<IServiceProvider>()});

            // Act & Assert
            container.DoesNotContains(type).Should().BeFalse();
        }


        [Test]
        public void DoesNotContains_ShouldReturnTrueWhenInstanceIsNull()
        {
            // Arrange
            var container = Substitute.For<IObjectContainer>();
            object instance = null;

            // Act & Assert
            container.DoesNotContains(instance).Should().BeTrue();
        }


        [Test]
        public void DoesNotContains_ShouldReturnTrueWhenContainerCannotResolveInstanceTypeAndName()
        {
            // Arrange
            var container = Substitute.For<IObjectContainer>();
            var instance = Substitute.For<IServiceProvider>();

            // Act & Assert
            container.DoesNotContains(instance).Should().BeTrue();
        }

        [Test]
        public void DoesNotContains_ShouldReturnFalseWhenContainerCanResolveInstanceTypeAndName()
        {
            // Arrange
            var container = Substitute.For<IObjectContainer>();
            var instance = this;
            container.ResolveAll<BoDiExtensionsSpecs>().Returns(new []{ this});

            // Act & Assert
            container.DoesNotContains(instance).Should().BeFalse();
        }

        [Test]
        public void RegisterInstance_ShouldRegisterInstanceRuntimeTypeAndItsBaseTypeWhenContainerDoesNoContainNeither()
        {
            // Arrange
            var container = Substitute.For<IObjectContainer>();
            Animal animal = new Human();

            // Act
            var result = container.RegisterInstance(animal);

            // Assert
            container.Received(1).RegisterInstanceAs(animal, typeof(Human));
            container.Received(1).RegisterInstanceAs(animal, typeof(Animal));
            result.Should().BeSameAs(animal);
        }

        [Test]
        public void RegisterInstance_ShouldNotRegisterInstanceRuntimeTypeWhenContainerContainsIt()
        {
            // Arrange
            var container = Substitute.For<IObjectContainer>();
            Animal animal = new Human();
            container.ResolveAll<Human>().Returns(new [] {new Human()});

            // Act
            var result = container.RegisterInstance(animal);

            // Assert
            container.DidNotReceive().RegisterInstanceAs(animal, typeof(Human));
            container.Received(1).RegisterInstanceAs(animal, typeof(Animal));
            result.Should().BeSameAs(animal);
        }

        [Test]
        public void RegisterInstance_ShouldNotRegisterInstanceRuntimeBaseTypeWhenContainerContainsIt()
        {
            // Arrange
            var container = Substitute.For<IObjectContainer>();
            Animal animal = new Human();
            container.ResolveAll<Animal>().Returns(new []{ new Animal() });

            // Act
            var result = container.RegisterInstance(animal);

            // Assert
            container.DidNotReceive().RegisterInstanceAs(animal, typeof(Animal));
            container.Received().RegisterInstanceAs(animal, typeof(Human));
            result.Should().BeSameAs(animal);
        }
    }
}