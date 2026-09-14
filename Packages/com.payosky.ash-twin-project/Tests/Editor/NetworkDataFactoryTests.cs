using AshTwinProject.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace AshTwinProject.Editor.Tests
{
    public class NetworkDataFactoryTests
    {
        private const uint k_TestStateTypeId = 1001;
        private const uint k_OtherTestStateTypeId = 1002;
        private const uint k_OtherNetworkableTypeId = 1003;

        [SetUp]
        public void SetUp()
        {
            NetworkTypeRegistry.ResetForTests();
            NetworkTypeRegistry.Register<TestState>(k_TestStateTypeId);
            NetworkTypeRegistry.Register<OtherTestState>(k_OtherTestStateTypeId);
            NetworkTypeRegistry.Register<OtherNetworkable>(k_OtherNetworkableTypeId);
        }

        [TearDown]
        public void TearDown()
        {
            NetworkTypeRegistry.ResetForTests();
        }

        [Test]
        public void To32_WithNullObject_ReturnsDefault()
        {
            INetworkable value = null;

            NetworkData32 result = value.To32();

            Assert.IsFalse(result.HasData);
            Assert.AreEqual(0, result.TypeID);
            Assert.IsTrue(result.DataPayload.IsEmpty);
        }

        [Test]
        public void To32_SetsHasData()
        {
            TestState state = new()
            {
                Value = 10
            };

            NetworkData32 result = state.To32();

            Assert.IsTrue(result.HasData);
        }

        [Test]
        public void To32_SetsCorrectTypeId()
        {
            TestState state = new()
            {
                Value = 10
            };

            NetworkData32 result = state.To32();

            Assert.AreEqual(k_TestStateTypeId, result.TypeID);
        }

        [Test]
        public void To32_SerializesPayload()
        {
            TestState state = new()
            {
                Value = 10
            };

            NetworkData32 result = state.To32();

            Assert.AreEqual("{\"Value\":10}", result.DataPayload.Value);
        }

        [Test]
        public void ToObject_ReconstructsConcreteType()
        {
            TestState original = new()
            {
                Value = 10
            };

            NetworkData32 networkData = original.To32();

            TestState reconstructed = networkData.ToObject<TestState>();

            Assert.IsNotNull(reconstructed);
            Assert.AreEqual(original.Value, reconstructed.Value);
        }

        [Test]
        public void ToObject_WithInterface_ReconstructsConcreteImplementation()
        {
            TestState original = new()
            {
                Value = 42
            };

            NetworkData32 networkData = original.To32();

            ITestState reconstructed = networkData.ToObject<ITestState>();

            Assert.IsNotNull(reconstructed);
            Assert.IsInstanceOf<TestState>(reconstructed);

            TestState state = (TestState)reconstructed;

            Assert.AreEqual(42, state.Value);
        }

        [Test]
        public void ToObject_UsesTypeIdToDetermineConcreteImplementation()
        {
            OtherTestState original = new()
            {
                Name = "Test"
            };

            NetworkData64 networkData = original.To64();

            ITestState reconstructed = networkData.ToObject<ITestState>();

            Assert.IsInstanceOf<OtherTestState>(reconstructed);

            OtherTestState state = (OtherTestState)reconstructed;

            Assert.AreEqual("Test", state.Name);
        }

        [Test]
        public void ToObject_ThroughINetworkData_ReconstructsObject()
        {
            TestState original = new()
            {
                Value = 42
            };

            INetworkData networkData = original.To32();

            ITestState reconstructed = networkData.ToObject<ITestState>();

            Assert.IsInstanceOf<TestState>(reconstructed);
        }

        [Test]
        public void ToObject_WithIncompatibleRequestedType_ReturnsNull()
        {
            TestState original = new()
            {
                Value = 10
            };

            NetworkData32 networkData = original.To32();

            LogAssert.Expect(
                LogType.Error, $"Failed to deserialize object of type {typeof(IOtherNetworkable)} from {typeof(TestState)}"
            );

            IOtherNetworkable result = networkData.ToObject<IOtherNetworkable>();

            Assert.IsNull(result);
        }

        [Test]
        public void ToObject_WithNoData_ReturnsNull()
        {
            NetworkData32 networkData = default;

            ITestState result = networkData.ToObject<ITestState>();

            Assert.IsNull(result);
        }

        [Test]
        public void ToObject_WithUnknownTypeId_ReturnsNull()
        {
            NetworkData32 networkData = new()
            {
                HasData = true,
                TypeID = 999999,
                DataPayload = new Unity.Collections.FixedString32Bytes(
                    "{\"Value\":10}"
                )
            };

            LogAssert.Expect(
                LogType.Error, $"Failed to deserialize object of type {typeof(ITestState)} from "
            );

            ITestState result = networkData.ToObject<ITestState>();

            Assert.IsNull(result);
        }
    }
}