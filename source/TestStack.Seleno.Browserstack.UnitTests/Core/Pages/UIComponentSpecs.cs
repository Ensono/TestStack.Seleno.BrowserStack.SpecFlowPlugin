using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using TestStack.Seleno.BrowserStack.Core.Pages;

namespace TestStack.Seleno.Browserstack.UnitTests.Core.Pages
{
    [TestFixture]
    public class UiComponentSpecs
    {
        public class DummyComponent : UiComponent { }

        public class FancyComponent : UiComponent { }

        public class AnotherFancyComponent : UiComponent, IResponsiveComponent { }

        [Test]
        public void GetResponsiveComponentBasedOn_ShouldReturnOriginalComponentWhenInvokedWithTrue()
        {
            // Arrange
            var sut = Substitute.ForPartsOf<DummyComponent>();
            var anotherDummyComponentInstance = new DummyComponent();

            sut.When(x => x.GetComponent<DummyComponent>()).DoNotCallBase();
            sut.GetComponent<DummyComponent>().Returns(anotherDummyComponentInstance);

            // Act
            var result = sut.GetResponsiveComponentBasedOn<DummyComponent>(true);

            // Assert
            result.Should().BeSameAs(anotherDummyComponentInstance);
        }

        [Test]
        public void GetResponsiveComponentBasedOn_ShouldReturnOriginalComponentWhenInvokedWithFalseButNoMatchingResponsiveComponentFound()
        {
            // Arrange
            var sut = Substitute.ForPartsOf<DummyComponent>();
            var anotherDummyComponentInstance = new DummyComponent();

            sut.When(x => x.GetComponent<DummyComponent>()).DoNotCallBase();
            sut.GetComponent<DummyComponent>().Returns(anotherDummyComponentInstance);

            // Act
            var result = sut.GetResponsiveComponentBasedOn<DummyComponent>(false);

            // Assert
            result.Should().BeSameAs(anotherDummyComponentInstance);
        }

        [Test]
        public void GetResponsiveComponentBasedOn_ShouldReturnMatchingResponsiveComponentByResponsiveMarkerAndNameConvention()
        {
            // Arrange
            var sut = Substitute.ForPartsOf<DummyComponent>();
            var responsiveComponent = new AnotherFancyComponent();

            sut.WhenForAnyArgs(x => x.GetComponent(null)).DoNotCallBase();
            sut.GetComponent(typeof(AnotherFancyComponent)).Returns(responsiveComponent);
            sut.When(x => x.GetComponent<FancyComponent>()).DoNotCallBase();
            sut.GetComponent<FancyComponent>().Returns(null as FancyComponent);

            // Act
            var result = sut.GetResponsiveComponentBasedOn<FancyComponent>(false);

            // Assert
            result.Should().BeSameAs(responsiveComponent);
        }

        [Test]
        public void GetComponent_ShouldCallGenericGetComponentForSpecifiedType()
        {
            // Arrange
            var sut = Substitute.ForPartsOf<DummyComponent>();
            var componentInstance = new FancyComponent();

            sut.When(x => x.GetComponent<FancyComponent>()).DoNotCallBase();
            sut.GetComponent<FancyComponent>().Returns(componentInstance);

            // Act
            var result = sut.GetComponent(typeof(FancyComponent));

            // Assert
            result.Should().BeSameAs(componentInstance);
        }
    }
}