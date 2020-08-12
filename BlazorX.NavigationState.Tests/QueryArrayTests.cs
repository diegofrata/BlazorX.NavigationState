using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace BlazorX.NavigationState.Tests
{
    [TestFixture]
    public class QueryArrayTests
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
            var sut = _state.QueryArray("p0", Array.Empty<int>());
            Assert.That(sut.Value, Is.EqualTo(Array.Empty<int>()).AsCollection);
        }

        [Test]
        public void Should_return_value_if_parameter_is_set()
        {
            var sut = _state.QueryArray("p1", Array.Empty<int>());
            Assert.That(sut.Value, Is.EqualTo(new [] { 10 }).AsCollection);
        }

        [Test]
        public void Setting_existing_query_parameter_should_cause_navigation()
        {
            var sut = _state.QueryArray("p1", Array.Empty<int>());
            sut.Value = new [] { 50, 25 };

            Assert.That(sut.Value, Is.EqualTo(new [] { 50, 25 }).AsCollection);
            Assert.That(_location, Is.EqualTo("http://domain/path?p1=50&p1=25"));
        }

        [Test]
        public void Setting_new_property_should_cause_navigation()
        {
            var sut = _state.QueryArray("p0", Array.Empty<int>());
            sut.Value = new [] { 50 };

            Assert.That(sut.Value, Is.EqualTo(new [] { 50 }).AsCollection);
            Assert.That(_location, Is.EqualTo("http://domain/path?p1=10&p0=50"));
        }

        [Test]
        public void Explicitly_emptied_parameter_should_not_return_default_value()
        {
            var sut = _state.QueryArray("p1", new [] { 100 });
            sut.Value = Array.Empty<int>();

            Assert.That(sut.Value, Is.EqualTo(Array.Empty<int>()).AsCollection);
            Assert.That(_location, Is.EqualTo("http://domain/path?p1:empty="));
        }

        [Test]
        public void Values_should_be_streamed()
        {
            var list = new List<int[]>();
            var sut = _state.QueryArray("p0", Array.Empty<int>());
            sut.ValueStream.Subscribe(v => list.Add(v));

            sut.Value = new [] { 10 };
            sut.Value = new [] { 10, 20 };
            sut.Value = new [] { 10, 20 };
            sut.Value = new [] { 20, 30 };

            Assert.That(list.Count, Is.EqualTo(4));
            Assert.That(list[0], Is.EqualTo(Array.Empty<int>()).AsCollection);
            Assert.That(list[1], Is.EqualTo(new [] { 10 }).AsCollection);
            Assert.That(list[2], Is.EqualTo(new [] { 10, 20 }).AsCollection);
            Assert.That(list[3], Is.EqualTo(new [] { 20, 30 }).AsCollection);
        }
    }
}