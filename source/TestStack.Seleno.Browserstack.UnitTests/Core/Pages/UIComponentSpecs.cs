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

        public class FancyComponent : DummyComponent { }

        public class ResponsiveFancyComponent : DummyComponent, IResponsiveComponent { }

        [Test]
        public void GetResponsiveComponentBasedOn_ShouldReturnOriginalComponentWhenInvokedWithTrue()
        {
            // Arrange
            var sut = Substitute.ForPartsOf<DummyComponent>();
            var componentInstance = new FancyComponent();

            sut.When(x => x.GetComponent<FancyComponent>()).DoNotCallBase();
            sut.GetComponent<FancyComponent>().Returns(componentInstance);

            // Act
            var result = sut.GetResponsiveComponent<DummyComponent, FancyComponent>(true);

            // Assert
            result.Should().BeSameAs(componentInstance);
        }

        [Test]
        public void GetResponsiveComponentBasedOn_ShouldReturnOriginalComponentWhenInvokedWithFalseButNoMatchingResponsiveComponentFound()
        {
            // Arrange
            var sut = Substitute.ForPartsOf<DummyComponent>();
            var expectedFancyComponentInstance = new ResponsiveFancyComponent();

            sut.When(x => x.GetComponent<ResponsiveFancyComponent>()).DoNotCallBase();
            sut.GetComponent<ResponsiveFancyComponent>().Returns(expectedFancyComponentInstance);

            // Act
            var result = sut.GetResponsiveComponent<UiComponent,ResponsiveFancyComponent>(false);

            // Assert
            result.Should().BeSameAs(expectedFancyComponentInstance);
        }

        [Test]
        public void GetResponsiveComponentBasedOn_ShouldReturnMatchingResponsiveComponentByResponsiveMarkerAndNamingConvention()
        {
            // Arrange
            var sut = Substitute.ForPartsOf<DummyComponent>();
            var responsiveComponent = new ResponsiveFancyComponent();

            sut.WhenForAnyArgs(x => x.GetComponent(null)).DoNotCallBase();
            sut.GetComponent(typeof(ResponsiveFancyComponent)).Returns(responsiveComponent);
            sut.When(x => x.GetComponent<FancyComponent>()).DoNotCallBase();
            sut.GetComponent<FancyComponent>().Returns(null as FancyComponent);
            
            // Act
            var result = sut.GetResponsiveComponent<DummyComponent, FancyComponent>(false);

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