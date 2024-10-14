namespace LudemDare.Editor
{
    using Unity.Netcode;
    using UnityEditor;
    using UnityEngine;
    using Util;

#if UNITY_EDITOR
    [CustomEditor(typeof(NetworkPrefabsList))]
    public sealed class NetworkPrefabsListEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            NetworkPrefabsList list = target as NetworkPrefabsList;

            if (GUILayout.Button("Update All Network Prefabs"))
            {
                NetworkBuildPostProcessor.UpdateNetworkPrefabsList(list);
            }
        }
    }
#endif
}