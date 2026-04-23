using System.Linq;
using NUnit.Framework;

namespace Damdor.Injectio.Tests
{
    [TestFixture]
    public class DependencyResolverTypeDataTests
    {
        private class BaseTestClass
        {
            [Inject]
            public object BaseValidField;
        }

        private class TestClass : BaseTestClass
        {
            // Fields
            [Inject] public object ValidField;
            [Inject] private string ValidPrivateField;
            [Inject] public int InvalidValueTypeField;
            public object FieldWithoutAttribute;

            // Properties
            [Inject] public object ValidProperty { get; set; }
            [Inject] private string ValidPrivateProperty { get; set; }
            [Inject] public object PropertyWithoutSetter { get; }
            [Inject] public int InvalidValueTypeProperty { get; set; }
            public object PropertyWithoutAttribute { get; set; }

            // Methods
            [Inject]
            public void ValidMethod(object param1, string param2) { }

            [Inject]
            private void ValidPrivateMethod(object param1) { }

            [Inject]
            public void InvalidMethodWithValueType(object param1, int param2) { }

            public void MethodWithoutAttribute(object param1) { }
        }

        [Test]
        public void Constructor_ExtractsValidFields()
        {
            var typeData = new DependencyResolverTypeData(typeof(TestClass));

            Assert.AreEqual(2, typeData.Fields.Count, "Should extract exactly 2 valid fields.");
            Assert.IsTrue(typeData.Fields.Any(f => f.Field.Name == nameof(TestClass.ValidField)));
            Assert.IsTrue(typeData.Fields.Any(f => f.Field.Name == "ValidPrivateField"));
        }

        [Test]
        public void Constructor_IgnoresFieldsWithValueTypes()
        {
            var typeData = new DependencyResolverTypeData(typeof(TestClass));
            Assert.IsFalse(typeData.Fields.Any(f => f.Field.Name == nameof(TestClass.InvalidValueTypeField)));
        }

        [Test]
        public void Constructor_ExtractsValidProperties()
        {
            var typeData = new DependencyResolverTypeData(typeof(TestClass));

            Assert.AreEqual(2, typeData.Properties.Count, "Should extract exactly 2 valid properties.");
            Assert.IsTrue(typeData.Properties.Any(p => p.Property.Name == nameof(TestClass.ValidProperty)));
            Assert.IsTrue(typeData.Properties.Any(p => p.Property.Name == "ValidPrivateProperty"));
        }

        [Test]
        public void Constructor_IgnoresPropertiesWithoutSetterOrWithValueTypes()
        {
            var typeData = new DependencyResolverTypeData(typeof(TestClass));
            
            Assert.IsFalse(typeData.Properties.Any(p => p.Property.Name == nameof(TestClass.PropertyWithoutSetter)));
            Assert.IsFalse(typeData.Properties.Any(p => p.Property.Name == nameof(TestClass.InvalidValueTypeProperty)));
        }

        [Test]
        public void Constructor_ExtractsValidMethods()
        {
            var typeData = new DependencyResolverTypeData(typeof(TestClass));

            Assert.AreEqual(2, typeData.Methods.Count, "Should extract exactly 2 valid methods.");
            Assert.IsTrue(typeData.Methods.Any(m => m.Method.Name == nameof(TestClass.ValidMethod)));
            Assert.IsTrue(typeData.Methods.Any(m => m.Method.Name == "ValidPrivateMethod"));

            var validMethod = typeData.Methods.First(m => m.Method.Name == nameof(TestClass.ValidMethod));
            Assert.AreEqual(2, validMethod.Parameters.Length);
            Assert.AreEqual(typeof(object), validMethod.Parameters[0]);
            Assert.AreEqual(typeof(string), validMethod.Parameters[1]);
        }

        [Test]
        public void Constructor_IgnoresMethodsWithValueTypeParameters()
        {
            var typeData = new DependencyResolverTypeData(typeof(TestClass));

            Assert.IsFalse(typeData.Methods.Any(m => m.Method.Name == nameof(TestClass.InvalidMethodWithValueType)));
        }

        [Test]
        public void Constructor_IgnoresInheritedMembers()
        {
            // BindingFlags.DeclaredOnly is used, so base class members should not be included
            var typeData = new DependencyResolverTypeData(typeof(TestClass));

            Assert.IsFalse(typeData.Fields.Any(f => f.Field.Name == nameof(BaseTestClass.BaseValidField)));
        }
    }
}