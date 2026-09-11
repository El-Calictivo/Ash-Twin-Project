using System;
using Newtonsoft.Json;
using Unity.Collections;
using UnityEngine;

namespace AshTwinProject.Core
{
    /// <summary>
    ///     Provides extension methods for transforming objects to <see cref="INetworkable"/> from <see cref="INetworkData"/> and vice versa.
    /// </summary>
    public static class NetworkDataFactory
    {
        public static NetworkData32 To32(this INetworkable objectToTransform)
        {
            if (objectToTransform == null) {
                return default;
            }

            (string objectType, string objectPayload) = SerializeObject(objectToTransform);

            return new NetworkData32
            {
                HasData = true,
                DataType = new FixedString512Bytes(objectType),
                DataPayload = new FixedString32Bytes(objectPayload)
            };
        }

        public static NetworkData64 To64(this INetworkable objectToTransform)
        {
            if (objectToTransform == null) {
                return default;
            }

            (string objectType, string objectPayload) = SerializeObject(objectToTransform);

            return new NetworkData64
            {
                HasData = true,
                DataType = new FixedString512Bytes(objectType),
                DataPayload = new FixedString64Bytes(objectPayload)
            };
        }

        public static NetworkData128 To128(this INetworkable objectToTransform)
        {
            if (objectToTransform == null) {
                return default;
            }

            (string objectType, string objectPayload) = SerializeObject(objectToTransform);

            return new NetworkData128
            {
                HasData = true,
                DataType = new FixedString512Bytes(objectType),
                DataPayload = new FixedString128Bytes(objectPayload)
            };
        }

        public static NetworkData512 To512(this INetworkable objectToTransform)
        {
            if (objectToTransform == null) {
                return default;
            }

            (string objectType, string objectPayload) = SerializeObject(objectToTransform);

            return new NetworkData512
            {
                HasData = true,
                DataType = new FixedString512Bytes(objectType),
                DataPayload = new FixedString512Bytes(objectPayload)
            };
        }

        public static NetworkData4096 To4096(this INetworkable objectToTransform)
        {
            if (objectToTransform == null) {
                return default;
            }

            (string objectType, string objectPayload) = SerializeObject(objectToTransform);

            return new NetworkData4096
            {
                HasData = true,
                DataType = new FixedString512Bytes(objectType),
                DataPayload = new FixedString4096Bytes(objectPayload)
            };
        }

        private static (string, string ) SerializeObject(INetworkable objectToSerialize)
        {
            string objectType = objectToSerialize.GetType().AssemblyQualifiedName;
            string objectPayload = JsonConvert.SerializeObject(objectToSerialize, Formatting.None);
            return (objectType, objectPayload);
        }

        public static T ToObject<T>(this INetworkData networkData) where T : class, INetworkable
        {
            return networkData switch
            {
                NetworkData32 networkData32 => networkData32.ToObject<T>(),
                NetworkData64 networkData64 => networkData64.ToObject<T>(),
                NetworkData128 networkData128 => networkData128.ToObject<T>(),
                NetworkData512 networkData512 => networkData512.ToObject<T>(),
                NetworkData4096 networkData4096 => networkData4096.ToObject<T>(),
                _ => null
            };
        }

        public static T ToObject<T>(this NetworkData32 networkData) where T : class, INetworkable
        {
            Type objectType = Type.GetType(networkData.DataType.Value);

            if (objectType == null || !typeof(T).IsAssignableFrom(objectType)) {
#if UNITY_ENABLE_CHECKS
                Debug.LogError($"Failed to deserialize object of type {typeof(T)} from {networkData.DataType.Value}");
#endif
                return null;
            }

            T reconstructedObject = JsonConvert.DeserializeObject(networkData.DataPayload.Value, objectType) as T;
            return reconstructedObject;
        }

        public static T ToObject<T>(this NetworkData64 networkData) where T : class, INetworkable
        {
            Type objectType = Type.GetType(networkData.DataType.Value);

            if (objectType == null || !typeof(T).IsAssignableFrom(objectType)) {
#if UNITY_ENABLE_CHECKS
                Debug.LogError($"Failed to deserialize object of type {typeof(T)} from {networkData.DataType.Value}");
#endif
                return null;
            }

            T reconstructedObject = JsonConvert.DeserializeObject(networkData.DataPayload.Value, objectType) as T;
            return reconstructedObject;
        }

        public static T ToObject<T>(this NetworkData128 networkData) where T : class, INetworkable
        {
            Type objectType = Type.GetType(networkData.DataType.Value);

            if (objectType == null || !typeof(T).IsAssignableFrom(objectType)) {
#if UNITY_ENABLE_CHECKS
                Debug.LogError($"Failed to deserialize object of type {typeof(T)} from {networkData.DataType.Value}");
#endif
                return null;
            }

            T reconstructedObject = JsonConvert.DeserializeObject(networkData.DataPayload.Value, objectType) as T;
            return reconstructedObject;
        }

        public static T ToObject<T>(this NetworkData512 networkData) where T : class, INetworkable
        {
            Type objectType = Type.GetType(networkData.DataType.Value);

            if (objectType == null || !typeof(T).IsAssignableFrom(objectType)) {
#if UNITY_ENABLE_CHECKS
                Debug.LogError($"Failed to deserialize object of type {typeof(T)} from {networkData.DataType.Value}");
#endif
                return null;
            }

            T reconstructedObject = JsonConvert.DeserializeObject(networkData.DataPayload.Value, objectType) as T;
            return reconstructedObject;
        }

        public static T ToObject<T>(this NetworkData4096 networkData) where T : class, INetworkable
        {
            Type objectType = Type.GetType(networkData.DataType.Value);

            if (objectType == null || !typeof(T).IsAssignableFrom(objectType)) {
#if UNITY_ENABLE_CHECKS
                Debug.LogError($"Failed to deserialize object of type {typeof(T)} from {networkData.DataType.Value}");
#endif
                return null;
            }

            T reconstructedObject = JsonConvert.DeserializeObject(networkData.DataPayload.Value, objectType) as T;
            return reconstructedObject;
        }
    }
}