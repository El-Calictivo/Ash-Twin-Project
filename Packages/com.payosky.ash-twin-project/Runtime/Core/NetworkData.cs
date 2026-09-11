using System;
using Unity.Collections;
using Unity.Netcode;

namespace AshTwinProject.Core
{
    /// <summary>
    /// Represents an easy-to-use wrapper to convert any object into a Networking friendly struct representation
    /// </summary>
    /// <remarks>
    ///     The struct implementations are designed to encapsulate and transmit data
    ///     across a network in multiplayer environments. Each implementation defines
    ///     the maximum size of the payload that may be transported
    /// </remarks>
    public interface INetworkData : INetworkSerializable { }

    public struct NetworkData32 : INetworkData, IEquatable<NetworkData32>
    {
        public bool HasData;
        public FixedString512Bytes DataType;
        public FixedString32Bytes DataPayload;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref HasData);
            serializer.SerializeValue(ref DataType);
            serializer.SerializeValue(ref DataPayload);
        }

        public bool Equals(NetworkData32 other)
        {
            return HasData == other.HasData
                   && DataType.Equals(other.DataType)
                   && DataPayload.Equals(other.DataPayload);
        }
    }

    public struct NetworkData64 : INetworkData, IEquatable<NetworkData64>
    {
        public bool HasData;
        public FixedString512Bytes DataType;
        public FixedString64Bytes DataPayload;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref HasData);
            serializer.SerializeValue(ref DataType);
            serializer.SerializeValue(ref DataPayload);
        }

        public bool Equals(NetworkData64 other)
        {
            return HasData == other.HasData
                   && DataType.Equals(other.DataType)
                   && DataPayload.Equals(other.DataPayload);
        }
    }

    public struct NetworkData128 : INetworkData, IEquatable<NetworkData128>
    {
        public bool HasData;
        public FixedString512Bytes DataType;
        public FixedString128Bytes DataPayload;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref HasData);
            serializer.SerializeValue(ref DataType);
            serializer.SerializeValue(ref DataPayload);
        }

        public bool Equals(NetworkData128 other)
        {
            return HasData == other.HasData
                   && DataType.Equals(other.DataType)
                   && DataPayload.Equals(other.DataPayload);
        }
    }

    public struct NetworkData512 : INetworkData, IEquatable<NetworkData512>
    {
        public bool HasData;
        public FixedString512Bytes DataType;
        public FixedString512Bytes DataPayload;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref HasData);
            serializer.SerializeValue(ref DataType);
            serializer.SerializeValue(ref DataPayload);
        }

        public bool Equals(NetworkData512 other)
        {
            return HasData == other.HasData
                   && DataType.Equals(other.DataType)
                   && DataPayload.Equals(other.DataPayload);
        }
    }

    public struct NetworkData4096 : INetworkData, IEquatable<NetworkData4096>
    {
        public bool HasData;
        public FixedString512Bytes DataType;
        public FixedString4096Bytes DataPayload;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref HasData);
            serializer.SerializeValue(ref DataType);
            serializer.SerializeValue(ref DataPayload);
        }

        public bool Equals(NetworkData4096 other)
        {
            return HasData == other.HasData
                   && DataType.Equals(other.DataType)
                   && DataPayload.Equals(other.DataPayload);
        }
    }
}