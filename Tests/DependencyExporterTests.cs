using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Damdor.Injectio.Tests
{
    [TestFixture]
    public class DependencyExporterTests
    {
        // A simple mock register implementing IDependencyRegister for testing purposes
        private class MockRegister : IDependencyRegister
        {
            private readonly Dictionary<Type, object> _dependencies = new();

            public void Register<T>(T obj)
            {
                _dependencies[typeof(T)] = obj;
            }

            public void Register(Type type, object obj)
            {
                _dependencies[type] = obj;
            }

            public void Remove(Type type)
            {
                _dependencies.Remove(type);
            }

            public void Remove<T>()
            {
                _dependencies.Remove(typeof(T));
            }

            public void Clear()
            {
                _dependencies.Clear();
            }

            public object Get(Type type)
            {
                return _dependencies.GetValueOrDefault(type);
            }
        }

        private interface ITestInterface { }

        private class BaseTestDependency { }
        private class FieldTestDependency { }
        private class PrivateFieldTestDependency { }
        private class PropertyTestDependency { }
        private class PrivatePropertyTestDependency { }
        private class InterfaceTestDependency : ITestInterface { }
        private class UnexportedDependency { }

        private class BaseSourceClass
        {
            [Export] public BaseTestDependency BaseField = new();
        }

        private class SourceClass : BaseSourceClass
        {
            [Export] public FieldTestDependency PublicField = new();
            [Export] private PrivateFieldTestDependency _privateField = new();
            public UnexportedDependency UnexportedField = new();

            [Export] public PropertyTestDependency PublicProperty { get; set; } = new();
            [Export] private PrivatePropertyTestDependency PrivateProperty { get; set; } = new();
            public UnexportedDependency UnexportedProperty { get; set; } = new();

            [Export(typeof(ITestInterface))] public InterfaceTestDependency InterfaceField = new();

            public PrivateFieldTestDependency GetPrivateField() => _privateField;
            public PrivatePropertyTestDependency GetPrivateProperty() => PrivateProperty;
        }

        private MockRegister register;

        [SetUp]
        public void SetUp()
        {
            register = new MockRegister();
        }

        private void Export(object source)
        {
            DependencyExporter.Export(register, source);
        }

        [Test]
        public void Export_RegistersPublicAndPrivateFields()
        {
            var source = new SourceClass();
            Export(source);

            Assert.AreSame(source.PublicField, register.Get(typeof(FieldTestDependency)), "Public field with [Export] was not registered.");
            Assert.AreSame(source.GetPrivateField(), register.Get(typeof(PrivateFieldTestDependency)), "Private field with [Export] was not registered.");
            Assert.IsNull(register.Get(typeof(UnexportedDependency)), "Field without [Export] should not be registered.");
        }

        [Test]
        public void Export_RegistersProperties()
        {
            var source = new SourceClass();
            Export(source);

            Assert.AreSame(source.PublicProperty, register.Get(typeof(PropertyTestDependency)), "Public property with [Export] was not registered.");
            Assert.AreSame(source.GetPrivateProperty(), register.Get(typeof(PrivatePropertyTestDependency)), "Private property with [Export] was not registered.");
            Assert.IsNull(register.Get(typeof(UnexportedDependency)), "Property without [Export] should not be registered.");
        }

        [Test]
        public void Export_RegistersWithExplicitType()
        {
            var source = new SourceClass();
            Export(source);

            Assert.AreSame(source.InterfaceField, register.Get(typeof(ITestInterface)), "Field with explicit [Export(Type)] was not registered under the interface type.");
        }

        [Test]
        public void Export_TraversesInheritanceHierarchy()
        {
            var source = new SourceClass();
            Export(source);

            Assert.AreSame(source.BaseField, register.Get(typeof(BaseTestDependency)), "Base class field with [Export] was not registered.");
        }

        [Test]
        public void Export_IgnoresNullValues()
        {
            var source = new SourceClass();
            source.PublicField = null;
            source.PublicProperty = null;

            Assert.DoesNotThrow(() => Export(source));
            Assert.IsNull(register.Get(typeof(FieldTestDependency)), "Null field should not register a value.");
            Assert.IsNull(register.Get(typeof(PropertyTestDependency)), "Null property should not register a value.");
        }

        [Test]
        public void Export_HandlesNullRegisterOrSource()
        {
            var source = new SourceClass();

            Assert.DoesNotThrow(() => DependencyExporter.Export(null, source));
            Assert.DoesNotThrow(() => DependencyExporter.Export(register, null));
        }
    }
}
