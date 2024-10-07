using Unity.Netcode;
using UnityEngine;

public class DupeNetworkCheck : MonoBehaviour
{
    private void Awake()
    {
        if (FindObjectsOfType<NetworkManager>().Length > 1)
        {
            Destroy(gameObject);
        }
    }
}
