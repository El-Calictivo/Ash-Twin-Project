using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshTwinProject.Core
{
    /// <summary>
    /// The registry maps Types to a numeric ID that is used for serialization and deserialization of polymorphic networkd data 
    /// </summary>
    public static class NetworkTypeRegistry
    {
        private static readonly Dictionary<uint, Type> IdToType = new();
        private static readonly Dictionary<Type, uint> TypeToId = new();

        public static void Register<T>(uint id) where T : INetworkable
        {
            Register(id, typeof(T));
        }

        public static void Register(uint id, Type type)
        {
            if (id == 0) {
                throw new ArgumentException(
                    "Network type ID 0 is reserved."
                );
            }

            if (IdToType.TryGetValue(id, out Type registeredType)) {
                if (registeredType == type) {
                    return;
                }

                throw new InvalidOperationException(
                    $"Network type ID {id} is already registered to {registeredType.FullName}."
                );
            }

            if (TypeToId.TryGetValue(type, out uint registeredId)) {
                if (registeredId == id) {
                    return;
                }

                throw new InvalidOperationException(
                    $"Network type {type.FullName} is already registered with ID {registeredId}."
                );
            }

            IdToType.Add(id, type);
            TypeToId.Add(type, id);
        }

        public static uint GetTypeId(INetworkable value)
        {
            return value == null ? throw new ArgumentNullException(nameof(value)) : GetTypeId(value.GetType());
        }

        public static uint GetTypeId(Type type)
        {
            return !TypeToId.TryGetValue(type, out uint id) ? throw new InvalidOperationException($"Network type {type.FullName} is not registered.") : id;
        }

        public static Type GetType(uint id)
        {
            return !IdToType.TryGetValue(id, out Type type) ? throw new InvalidOperationException($"Network type ID {id} is not registered.") : type;
        }

        public static bool TryGetType(uint id, out Type type)
        {
            return IdToType.TryGetValue(id, out type);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            IdToType.Clear();
            TypeToId.Clear();
        }

#if UNITY_INCLUDE_TESTS
        public static void ResetForTests()
        {
            IdToType.Clear();
            TypeToId.Clear();
        }
#endif
    }
}