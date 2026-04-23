using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Damdor.Injectio.Tests
{
    [TestFixture]
    public class DependencyResolverTests
    {
        // A simple mock container implementing your interface for testing purposes
        private class MockContainer : IDependencyContainer
        {
            private readonly Dictionary<Type, object> _dependencies = new();

            public void Register(Type type, object instance)
            {
                _dependencies[type] = instance;
            }

            public object Get(Type type)
            {
                return _dependencies.GetValueOrDefault(type);
            }
        }

        private class TestDependency { }
        private class AnotherTestDependency { }

        private class BaseTargetClass
        {
            [Inject] public TestDependency BaseField;
        }

        private class TargetClass : BaseTargetClass
        {
            [Inject] public TestDependency PublicField;
            [Inject] private TestDependency _privateField;
            public TestDependency UninjectedField;

            [Inject] public TestDependency PublicProperty { get; set; }
            public TestDependency UninjectedProperty { get; set; }

            public TestDependency MethodDependency { get; private set; }
            public AnotherTestDependency AnotherMethodDependency { get; private set; }

            public TestDependency GetPrivateField() => _privateField;

            [Inject]
            public void InjectMethod(TestDependency dependency, AnotherTestDependency anotherDependency)
            {
                MethodDependency = dependency;
                AnotherMethodDependency = anotherDependency;
            }

            public void UninjectedMethod(TestDependency dependency)
            {
                throw new InvalidOperationException("This method lacks the [Inject] attribute and should not be called.");
            }
        }

        private MockContainer container;
        private TestDependency testDependency;
        private AnotherTestDependency anotherTestDependency;

        [SetUp]
        public void SetUp()
        {
            container = new MockContainer();
            testDependency = new TestDependency();
            anotherTestDependency = new AnotherTestDependency();

            container.Register(typeof(TestDependency), testDependency);
            container.Register(typeof(AnotherTestDependency), anotherTestDependency);
        }

        private void Resolve(object target)
        {
            // NOTE: Adjust this line to match your actual DependencyResolver API signature.
            DependencyResolver.Resolve(container, target);
        }

        [Test]
        public void Inject_PopulatesPublicAndPrivateFields()
        {
            var target = new TargetClass();
            Resolve(target);

            Assert.AreSame(testDependency, target.PublicField, "Public field with [Inject] was not populated.");
            Assert.AreSame(testDependency, target.GetPrivateField(), "Private field with [Inject] was not populated.");
            Assert.IsNull(target.UninjectedField, "Field without [Inject] should remain null.");
        }

        [Test]
        public void Inject_PopulatesProperties()
        {
            var target = new TargetClass();
            Resolve(target);

            Assert.AreSame(testDependency, target.PublicProperty, "Property with [Inject] was not populated.");
            Assert.IsNull(target.UninjectedProperty, "Property without [Inject] should remain null.");
        }

        [Test]
        public void Inject_InvokesMethodsWithDependencies()
        {
            var target = new TargetClass();
            Resolve(target);

            Assert.AreSame(testDependency, target.MethodDependency, "Method parameter 1 was not injected.");
            Assert.AreSame(anotherTestDependency, target.AnotherMethodDependency, "Method parameter 2 was not injected.");
        }
        
        [Test]
        public void Inject_TraversesInheritanceHierarchy()
        {
            var target = new TargetClass();
            Resolve(target);

            Assert.AreSame(testDependency, target.BaseField, "Base class field with [Inject] was not populated.");
        }
    }
}