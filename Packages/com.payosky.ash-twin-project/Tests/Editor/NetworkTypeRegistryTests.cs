using System;
using AshTwinProject.Core;
using NUnit.Framework;

namespace AshTwinProject.Editor.Tests
{
    public class NetworkTypeRegistryTests
    {
        [SetUp]
        public void SetUp()
        {
            NetworkTypeRegistry.ResetForTests();
        }

        [TearDown]
        public void TearDown()
        {
            NetworkTypeRegistry.ResetForTests();
        }

        [Test]
        public void Register_AddsTypeToRegistry()
        {
            const uint typeId = 1234;

            NetworkTypeRegistry.Register<TestState>(typeId);

            Type registeredType = NetworkTypeRegistry.GetType(typeId);
            uint registeredId = NetworkTypeRegistry.GetTypeId(typeof(TestState));

            Assert.AreEqual(typeof(TestState), registeredType);
            Assert.AreEqual(typeId, registeredId);
        }

        [Test]
        public void Register_SameTypeAndIdTwice_DoesNotThrow()
        {
            const uint typeId = 1234;

            NetworkTypeRegistry.Register<TestState>(typeId);

            Assert.DoesNotThrow(() => { NetworkTypeRegistry.Register<TestState>(typeId); });
        }

        [Test]
        public void Register_ZeroId_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => { NetworkTypeRegistry.Register<TestState>(0); });
        }

        [Test]
        public void Register_SameIdForDifferentTypes_ThrowsInvalidOperationException()
        {
            const uint typeId = 1234;

            NetworkTypeRegistry.Register<TestState>(typeId);

            Assert.Throws<InvalidOperationException>(() => { NetworkTypeRegistry.Register<OtherTestState>(typeId); });
        }

        [Test]
        public void Register_SameTypeWithDifferentIds_ThrowsInvalidOperationException()
        {
            NetworkTypeRegistry.Register<TestState>(1234);

            Assert.Throws<InvalidOperationException>(() => { NetworkTypeRegistry.Register<TestState>(5678); });
        }

        [Test]
        public void GetTypeId_WithRegisteredObject_ReturnsId()
        {
            const uint typeId = 1234;

            NetworkTypeRegistry.Register<TestState>(typeId);

            TestState state = new();

            uint result = NetworkTypeRegistry.GetTypeId(state);

            Assert.AreEqual(typeId, result);
        }

        [Test]
        public void GetTypeId_WithNullObject_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => { NetworkTypeRegistry.GetTypeId((INetworkable)null); });
        }

        [Test]
        public void GetTypeId_WithUnregisteredType_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => { NetworkTypeRegistry.GetTypeId(typeof(UnregisteredState)); });
        }

        [Test]
        public void GetType_WithUnregisteredId_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => { NetworkTypeRegistry.GetType(999999); });
        }

        [Test]
        public void TryGetType_WithRegisteredId_ReturnsTrue()
        {
            const uint typeId = 1234;

            NetworkTypeRegistry.Register<TestState>(typeId);

            bool result = NetworkTypeRegistry.TryGetType(typeId, out Type type);

            Assert.IsTrue(result);
            Assert.AreEqual(typeof(TestState), type);
        }

        [Test]
        public void TryGetType_WithUnregisteredId_ReturnsFalse()
        {
            bool result = NetworkTypeRegistry.TryGetType(1234, out Type type);

            Assert.IsFalse(result);
            Assert.IsNull(type);
        }
    }
}