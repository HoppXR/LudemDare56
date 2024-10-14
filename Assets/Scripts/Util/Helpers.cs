namespace Util
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public static class Helpers
    {
#if UNITY_EDITOR
        public static IEnumerable<GameObject> EnumeratePrefabs(IEnumerable<string> assetPaths = null)
        {
            assetPaths ??= new[] { "Assets/Prefabs", "Assets/Resources" };

            foreach (string assetGuid in AssetDatabase.FindAssets("t:prefab", assetPaths.ToArray()))
            {
                string path = AssetDatabase.GUIDToAssetPath(assetGuid);
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (go != null)
                {
                    yield return go;
                }
            }
        }
#endif
    }
}