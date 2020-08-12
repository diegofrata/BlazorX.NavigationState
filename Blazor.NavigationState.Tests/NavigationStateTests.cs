using System.Linq;
using Microsoft.AspNetCore.Components;
using NUnit.Framework;

namespace Blazor.NavigationState.Tests
{
    class TestNavigationManager : NavigationManager
    {
        public TestNavigationManager(string baseUri, string uri)
        {
            Initialize(baseUri, uri);
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            Uri = uri;
            NotifyLocationChanged(false);
        }
    }
    
    [TestFixture]
    public class NavigationStateTests
    {
        NavigationState _sut;
        TestNavigationManager _navigationManager;

        [SetUp]
        public void SetUp()
        {
            _navigationManager = new TestNavigationManager("http://domain/", "http://domain/path?p1=10&p2=1&p2=3");
            _sut = new NavigationState(_navigationManager);
        }

        [TestFixture]
        public class  Dispose : NavigationStateTests
        {
            [Test]
            public void Should_cancel_subscription_when_disposed()
            {
                _sut.Dispose();
                _navigationManager.NavigateTo("http://domain/path?a=true");

                Assert.That(_sut.GetQueryParameters("a").Count, Is.EqualTo(0));
            }
        }

        [TestFixture]
        public class GetQueryParameters : NavigationStateTests
        {
            [Test]
            public void Should_return_empty_collection_when_parameter_does_not_exists()
            {
                var result = _sut.GetQueryParameters("p");
                Assert.That(result.Count, Is.EqualTo(0));
            }

            [Test]
            public void Should_return_a_parameter_if_it_exists()
            {
                var result = _sut.GetQueryParameters("p1").Single();
                Assert.That(result.Name, Is.EqualTo("p1"));
                Assert.That(result.Value, Is.EqualTo("10"));
            }

            [Test]
            public void Should_return_multiple_parameters_if_it_exists()
            {
                var result = _sut.GetQueryParameters("p2");
                Assert.That(result.Count, Is.EqualTo(2));

                Assert.That(result[0].Name, Is.EqualTo("p2"));
                Assert.That(result[0].Value, Is.EqualTo("1"));

                Assert.That(result[1].Name, Is.EqualTo("p2"));
                Assert.That(result[1].Value, Is.EqualTo("3"));
            }
        }
        
        [TestFixture]
        public class SetQueryParameters : NavigationStateTests
        {
            [Test]
            public void Setting_query_parameters_causes_navigation()
            {
                var location = "";
                _navigationManager.LocationChanged += (sender, args) => location = args.Location; 
                
                _sut.SetQueryParameters("p1", new[] { 25 });
                
                Assert.That(location, Is.EqualTo("http://domain/path?p1=25&p2=1&p2=3"));
            }            
        }

    }
}