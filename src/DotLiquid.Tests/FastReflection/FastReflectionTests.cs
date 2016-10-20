using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.FastReflection;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotLiquid.Tests.FastReflection
{
    [TestFixture]
    public class FastReflectionTests
    {
        public class TestClass
        {
#pragma warning disable 169
            private int privateIntField;
            private string privateStringField;
#pragma warning restore 169
            public int publicIntField;
            public string publicStringField;

            private int PrivateIntProperty { get; set; }
            private string PrivateStringProperty { get; set; }
            public int PublicIntProperty { get; set; }
            public string PublicStringProperty { get; set; }

            public static int StaticIntProperty { get; set; }
            public static string StaticStringProperty { get; set; }

            public static string PublicStaticMethod(int x)
            {
                return x.ToString();
            }

            private void PrivateVoidMethod(List<int> value)
            {
                value.Add(123);
            }

            public int PublicIntMethod(int a, int b)
            {
                return a + b;
            }
        }

        [Test]
        public void TestFastInvoke()
        {
            var type = typeof(TestClass);
            var staticMethod = type.FastGetMethod("PublicStaticMethod", BindingFlags.Static | BindingFlags.Public);
            var voidMethod = type.FastGetMethod("PrivateVoidMethod", BindingFlags.Instance | BindingFlags.NonPublic);
            var intMethod = type.FastGetMethod("PublicIntMethod", BindingFlags.Instance | BindingFlags.Public);
            var lst = new List<int>();
            var instance = new TestClass();
            Assert.AreEqual(staticMethod.FastInvoke(null, 123), "123");
            Assert.AreEqual(voidMethod.FastInvoke(instance, lst), null);
            Assert.AreEqual(intMethod.FastInvoke(instance, 1, 2), 3);
        }

        [Test]
        public void TestFastSetValue()
        {
            var type = typeof(TestClass);
            var privateIntField = type.FastGetField(
                "privateIntField", BindingFlags.Instance | BindingFlags.NonPublic);
            var publicIntField = type.FastGetField("publicIntField");
            var privateIntProperty = type.FastGetProperty(
                "PrivateIntProperty", BindingFlags.Instance | BindingFlags.NonPublic);
            var publicIntProperty = type.FastGetProperty("PublicIntProperty");
            var staticIntProperty = type.FastGetProperty("StaticIntProperty");

            var a = new TestClass();
            var b = new TestClass();

            privateIntField.FastSetValue(a, 123);
            Assert.AreEqual(privateIntField.FastGetValue(a), 123);
            Assert.AreEqual(privateIntField.FastGetValue(b), 0);

            publicIntField.FastSetValue(a, 456);
            Assert.AreEqual(a.publicIntField, 456);
            Assert.AreEqual(b.publicIntField, 0);

            privateIntProperty.FastSetValue(b, 321);
            Assert.AreEqual(privateIntProperty.FastGetValue(a), 0);
            Assert.AreEqual(privateIntProperty.FastGetValue(b), 321);

            publicIntProperty.FastSetValue(b, 654);
            Assert.AreEqual(a.PublicIntProperty, 0);
            Assert.AreEqual(b.PublicIntProperty, 654);

            staticIntProperty.FastSetValue(null, 789);
            Assert.AreEqual(TestClass.StaticIntProperty, 789);
        }

        [Test]
        public void TestFastGetValue()
        {
            var type = typeof(TestClass);
            var privateStringField = type.FastGetField(
                "privateStringField", BindingFlags.Instance | BindingFlags.NonPublic);
            var publicStringField = type.FastGetField("publicStringField");
            var privateStringProperty = type.FastGetProperty(
                "PrivateStringProperty", BindingFlags.Instance | BindingFlags.NonPublic);
            var publicStringProperty = type.FastGetProperty("PublicStringProperty");
            var staticStringProperty = type.FastGetProperty("StaticStringProperty");

            var a = new TestClass();
            var b = new TestClass();

            privateStringField.FastSetValue(a, "123");
            Assert.AreEqual(privateStringField.FastGetValue(a), "123");
            Assert.AreEqual(privateStringField.FastGetValue(b), null);

            a.publicStringField = "456";
            Assert.AreEqual(publicStringField.FastGetValue(a), "456");
            Assert.AreEqual(publicStringField.FastGetValue(b), null);

            privateStringProperty.FastSetValue(b, "321");
            Assert.AreEqual(privateStringProperty.FastGetValue(a), null);
            Assert.AreEqual(privateStringProperty.FastGetValue(b), "321");

            b.PublicStringProperty = "654";
            Assert.AreEqual(publicStringProperty.FastGetValue(a), null);
            Assert.AreEqual(publicStringProperty.FastGetValue(b), "654");

            TestClass.StaticStringProperty = "789";
            Assert.AreEqual(staticStringProperty.FastGetValue(null), "789");
        }

        [Test]
        public void TestFastGetProperties()
        {
            CollectionAssert.AreEquivalent(
                typeof(TestClass).GetTypeInfo().GetProperties(),
                typeof(TestClass).FastGetProperties());
            var bindingFlags = (BindingFlags.Static |
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            CollectionAssert.AreEquivalent(
                typeof(TestClass).GetTypeInfo().GetProperties(bindingFlags),
                typeof(TestClass).FastGetProperties(bindingFlags));
        }

        [Test]
        public void TestFastGetFields()
        {
            CollectionAssert.AreEquivalent(
                typeof(TestClass).GetTypeInfo().GetFields(),
                typeof(TestClass).FastGetFields());
            var bindingFlags = (BindingFlags.Static |
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            CollectionAssert.AreEquivalent(
                typeof(TestClass).GetTypeInfo().GetFields(bindingFlags),
                typeof(TestClass).FastGetFields(bindingFlags));
        }

        [Test]
        public void TestFastGetMethods()
        {
            CollectionAssert.AreEquivalent(
                typeof(TestClass).GetTypeInfo().GetMethods(),
                typeof(TestClass).FastGetMethods());
            var bindingFlags = (BindingFlags.Static |
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            CollectionAssert.AreEquivalent(
                typeof(TestClass).GetTypeInfo().GetMethods(bindingFlags),
                typeof(TestClass).FastGetMethods(bindingFlags));
        }

        [Test]
        public void TestFastGetProperty()
        {
            var type = typeof(TestClass);
            Assert.AreEqual(
                type.GetTypeInfo().GetProperty("PublicIntProperty"),
                type.FastGetProperty("PublicIntProperty"));
            var bindingFlags = (BindingFlags.Static |
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.AreEqual(
                type.GetTypeInfo().GetProperty("PrivateIntProperty", bindingFlags),
                type.FastGetProperty("PrivateIntProperty", bindingFlags));
            Assert.AreEqual(null, type.FastGetProperty("PrivateIntProperty"));
        }

        [Test]
        public void TestFastGetField()
        {
            var type = typeof(TestClass);
            Assert.AreEqual(
                type.GetTypeInfo().GetField("publicIntField"),
                type.FastGetField("publicIntField"));
            var bindingFlags = (BindingFlags.Static |
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.AreEqual(
                type.GetTypeInfo().GetField("privateIntField", bindingFlags),
                type.FastGetField("privateIntField", bindingFlags));
            Assert.AreEqual(null, type.FastGetField("privateIntField"));
        }

        [Test]
        public void TestFastGetMethod()
        {
            var type = typeof(TestClass);
            Assert.AreEqual(
                type.GetTypeInfo().GetMethod("PublicIntMethod"),
                type.FastGetMethod("PublicIntMethod"));
            var bindingFlags = (BindingFlags.Static |
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.AreEqual(
                type.GetTypeInfo().GetMethod("PrivateVoidMethod", bindingFlags),
                type.FastGetMethod("PrivateVoidMethod", bindingFlags));
            Assert.AreEqual(null, type.FastGetMethod("PrivateVoidMethod"));
        }
    }
}
