namespace Util
{
    using System.Collections.Generic;
    using System.Linq;
    using Unity.Netcode;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;
#endif
    using UnityEngine;

    public sealed class NetworkBuildPostProcessor
#if UNITY_EDITOR
        : IPreprocessBuildWithReport
#endif
    {
        public int callbackOrder => 0;

#if UNITY_EDITOR

        public void OnPreprocessBuild(BuildReport report)
        {
            foreach (string assetGuid in AssetDatabase.FindAssets("t:NetworkPrefabsList"))
            {
                string path = AssetDatabase.GUIDToAssetPath(assetGuid);
                NetworkPrefabsList list = AssetDatabase.LoadAssetAtPath<NetworkPrefabsList>(path);
                if (list != null)
                {
                    UpdateNetworkPrefabsList(list);
                }
            }
        }

        public static void UpdateNetworkPrefabsList(NetworkPrefabsList list)
        {
            HashSet<GameObject> initialPrefabs = list.PrefabList.Select(prefab => prefab.Prefab).ToHashSet();
            foreach (NetworkPrefab networkPrefab in list.PrefabList.ToArray())
            {
                list.Remove(networkPrefab);
            }

            List<NetworkPrefab> newPrefabs = new();
            foreach (GameObject prefab in Helpers.EnumeratePrefabs())
            {
                newPrefabs.AddRange(
                    prefab.GetComponentsInChildren<NetworkObject>(true).Select(
                        networkObject => new NetworkPrefab
                        {
                            Prefab = networkObject.gameObject,
                        }));
            }

            newPrefabs.Sort(
                (lhs, rhs) =>
                {
                    int comparison = string.CompareOrdinal(lhs.Prefab.name, rhs.Prefab.name);
                    if (comparison != 0)
                    {
                        return comparison;
                    }

                    string lhsAssetPath = AssetDatabase.GetAssetPath(lhs.Prefab);
                    string rhsAssetPath = AssetDatabase.GetAssetPath(rhs.Prefab);
                    return string.CompareOrdinal(lhsAssetPath, rhsAssetPath);
                });

            if (!initialPrefabs.SetEquals(newPrefabs.Select(prefab => prefab.Prefab)))
            {
                IEnumerable<string> oldPrefabNames = initialPrefabs.Select(prefab => prefab.name)
                    .Except(newPrefabs.Select(prefab => prefab.Prefab.name));
                IEnumerable<string> newPrefabNames = newPrefabs.Select(prefab => prefab.Prefab.name)
                    .Except(initialPrefabs.Select(prefab => prefab.name));
                Debug.Log(
                    $"Updated Network Prefabs from size {initialPrefabs.Count} to {newPrefabs.Count}. " +
                    $"Old removed: [{string.Join(",", oldPrefabNames)}], " +
                    $"New added: [{string.Join(",", newPrefabNames)}].");
            }

            foreach (NetworkPrefab prefab in newPrefabs)
            {
                list.Add(prefab);
            }

            EditorUtility.SetDirty(list);
            AssetDatabase.SaveAssetIfDirty(list);
        }

#endif
    }
}