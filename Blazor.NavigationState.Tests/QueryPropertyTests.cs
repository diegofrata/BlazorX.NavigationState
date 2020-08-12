using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Blazor.NavigationState.Tests
{
    [TestFixture]
    public class QueryPropertyTests
    {
        NavigationState _state;
        TestNavigationManager _navigationManager;
        string _location;

        [SetUp]
        public void SetUp()
        {
            _location = "";
            _navigationManager = new TestNavigationManager("http://domain/", "http://domain/path?p1=10");
            _navigationManager.LocationChanged += (sender, args) => _location = args.Location;

            _state = new NavigationState(_navigationManager);
        }

        [Test]
        public void Should_return_default_value_if_query_parameter_is_not_set()
        {
            var sut = _state.QueryProperty("p0", 0);
            Assert.That(sut.Value, Is.EqualTo(0));
        }

        [Test]
        public void Should_return_value_if_parameter_is_set()
        {
            var sut = _state.QueryProperty("p1", 0);
            Assert.That(sut.Value, Is.EqualTo(10));
        }

        [Test]
        public void Setting_existing_property_should_cause_navigation()
        {
            var sut = _state.QueryProperty("p1", 0);
            sut.Value = 50;

            Assert.That(sut.Value, Is.EqualTo(50));
            Assert.That(_location, Is.EqualTo("http://domain/path?p1=50"));
        }

        [Test]
        public void Setting_new_property_should_cause_navigation()
        {
            var sut = _state.QueryProperty("p0", 0);
            sut.Value = 50;

            Assert.That(sut.Value, Is.EqualTo(50));
            Assert.That(_location, Is.EqualTo("http://domain/path?p1=10&p0=50"));
        }

        [Test]
        public void Values_should_be_streamed()
        {
            var list = new List<int>();
            var sut = _state.QueryProperty("p0", 0);
            sut.ValueStream.Subscribe(v => list.Add(v));

            sut.Value = 10;
            sut.Value = 20;
            sut.Value = 30;
            
            Assert.That(list, Is.EqualTo(new [] { 0, 10, 20, 30}).AsCollection);
        }
        
        [Test]
        public void Repeated_values_should_not_be_published()
        {
            var list = new List<int>();
            var sut = _state.QueryProperty("p0", 0);
            sut.ValueStream.Subscribe(v => list.Add(v));

            sut.Value = 10;
            sut.Value = 10;
            sut.Value = 30;
            
            Assert.That(list, Is.EqualTo(new [] { 0, 10, 30}).AsCollection);
        }
    }
}