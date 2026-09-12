using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AshTwinProject.Core;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Compilation;
using UnityEngine;

namespace AshTwinProject.Editor
{
    internal static class NetworkTypeRegistryGenerator
    {
        private const string k_GeneratedDirectory = "Assets/Generated/AshTwinProject.Networking";
        private const string k_DatabasePath = k_GeneratedDirectory + "/NetworkTypeIds.json";
        private const string k_GeneratedCodePath = k_GeneratedDirectory + "/GeneratedNetworkTypeRegistry.g.cs";

        [Serializable]
        private class RegistryDatabase
        {
            public List<RegistryEntry> Entries = new();
        }

        [Serializable]
        private class RegistryEntry
        {
            public string AssemblyName;
            public string TypeName;
            public long Id;
        }

        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            EditorApplication.delayCall += Generate;
        }

        [MenuItem("Tools/AshTwinProject/Regenerate Network Type Registry")]
        public static void Generate()
        {
            HashSet<string> playerAssemblies = CompilationPipeline
                .GetAssemblies(AssembliesType.Player)
                .Select(assembly => assembly.name)
                .ToHashSet();

            Type[] networkTypes = TypeCache
                .GetTypesDerivedFrom<INetworkable>()
                .Where(IsValidNetworkType)
                .Where(type =>
                    playerAssemblies.Contains(
                        type.Assembly.GetName().Name
                    )
                )
                .OrderBy(
                    type => type.FullName,
                    StringComparer.Ordinal
                )
                .ToArray();

            RegistryDatabase database = LoadDatabase();

            HashSet<uint> usedIds = database.Entries
                .Select(entry => (uint)entry.Id)
                .ToHashSet();

            bool databaseChanged = false;

            foreach (Type type in networkTypes) {
                if (FindEntry(database, type) != null) {
                    continue;
                }

                uint id = GenerateUniqueId(usedIds);

                database.Entries.Add(
                    new RegistryEntry
                    {
                        AssemblyName = type.Assembly.GetName().Name,
                        TypeName = type.FullName,
                        Id = id
                    }
                );

                usedIds.Add(id);

                databaseChanged = true;

                Debug.Log($"Registered network type " + $"{type.FullName} with ID {id}.");
            }

            database.Entries = database.Entries.OrderBy(entry => entry.Id).ToList();

            bool codeChanged = GenerateCode(database, networkTypes);

            if (databaseChanged) {
                SaveDatabase(database);
            }

            if (databaseChanged || codeChanged) {
                AssetDatabase.Refresh();
            }
        }

        private static bool IsValidNetworkType(Type type)
        {
            if (type == null) {
                return false;
            }

            bool isPublic =
                type.IsPublic
                || type.IsNestedPublic;

            return isPublic
                   && !type.IsAbstract
                   && !type.IsInterface
                   && !type.ContainsGenericParameters;
        }

        private static RegistryDatabase LoadDatabase()
        {
            if (!File.Exists(k_DatabasePath)) {
                return new RegistryDatabase();
            }

            string json = File.ReadAllText(k_DatabasePath);

            RegistryDatabase database = JsonUtility.FromJson<RegistryDatabase>(json);

            return database ?? new RegistryDatabase();
        }

        private static void SaveDatabase(RegistryDatabase database)
        {
            EnsureDirectory();

            string json =
                JsonUtility.ToJson(database, true);

            File.WriteAllText(k_DatabasePath, json, new UTF8Encoding(false));
        }

        private static RegistryEntry FindEntry(RegistryDatabase database, Type type)
        {
            string assemblyName = type.Assembly.GetName().Name;

            return database.Entries.FirstOrDefault(entry => entry.AssemblyName == assemblyName && entry.TypeName == type.FullName);
        }

        private static uint GenerateUniqueId(HashSet<uint> usedIds)
        {
            using RandomNumberGenerator generator = RandomNumberGenerator.Create();

            byte[] bytes = new byte[sizeof(uint)];

            while (true) {
                generator.GetBytes(bytes);

                uint id = BitConverter.ToUInt32(bytes, 0);

                if (id == 0) {
                    continue;
                }

                if (usedIds.Contains(id)) {
                    continue;
                }

                return id;
            }
        }

        private static bool GenerateCode(RegistryDatabase database, Type[] networkTypes)
        {
            StringBuilder builder = new();

            builder.AppendLine(
                "// <auto-generated />"
            );

            builder.AppendLine(
                "using UnityEngine;"
            );

            builder.AppendLine();

            builder.AppendLine(
                "namespace AshTwinProject.Core.Generated"
            );

            builder.AppendLine("{");

            builder.AppendLine(
                "    internal static class GeneratedNetworkTypeRegistry"
            );

            builder.AppendLine("    {");

            builder.AppendLine(
                "        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]"
            );

            builder.AppendLine(
                "        private static void Register()"
            );

            builder.AppendLine("        {");

            foreach (Type type in networkTypes) {
                RegistryEntry entry = FindEntry(database, type);

                if (entry == null) {
                    continue;
                }

                string typeName = $"global::{type.FullName.Replace('+', '.')}";

                builder.AppendLine($"            global::AshTwinProject.Core.NetworkTypeRegistry.Register<{typeName}>({entry.Id});");
            }

            builder.AppendLine("        }");
            builder.AppendLine("    }");
            builder.AppendLine("}");

            string generatedCode = builder.ToString();

            if (File.Exists(k_GeneratedCodePath) && File.ReadAllText(k_GeneratedCodePath) == generatedCode) {
                return false;
            }

            EnsureDirectory();

            File.WriteAllText(k_GeneratedCodePath, generatedCode, new UTF8Encoding(false));

            return true;
        }

        private static void EnsureDirectory()
        {
            if (!Directory.Exists(k_GeneratedDirectory)) {
                Directory.CreateDirectory(k_GeneratedDirectory);
            }
        }
    }
}