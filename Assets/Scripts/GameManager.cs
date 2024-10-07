using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {
    
    public static GameManager instance;
    
    [Header("Player")]
    [SerializeField] private PlayerController[] playerPrefabs;
    private int _index = 0;
    private int _playerCount;
    
    [Header("UI")]
    [SerializeField] private GameObject gameOverUI;
    //[SerializeField] private GameObject gameUIPrefab;
    
    public override void OnNetworkSpawn() {
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    private void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        EnableTimeServerRpc();
        
        gameOverUI.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong playerId) {
        var spawn = Instantiate(playerPrefabs[_index]);
        spawn.NetworkObject.SpawnWithOwnership(playerId);
        _index++;
        _playerCount++;
    }
    
    public override void OnDestroy() {
        base.OnDestroy();
        StopAllCoroutines();
        MatchmakingService.LeaveLobby();
        if(NetworkManager.Singleton != null ) NetworkManager.Singleton.Shutdown();
    }

    public void MainMenu()
    {
        SceneManager.LoadSceneAsync("Main Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayerDie()
    {
        _playerCount--;

        if (_playerCount <= 1)
        {
            GameOverForAllClientsServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void GameOverForAllClientsServerRpc()
    {
        GameOverForAllClientsClientRpc();
    }

    [ClientRpc]
    private void GameOverForAllClientsClientRpc()
    {
        GameOver();
    }
    
    private void GameOver()
    {
        gameOverUI.SetActive(true);
        
        Time.timeScale = 0;
    }

    [ServerRpc(RequireOwnership = false)]
    private void EnableTimeServerRpc()
    {
        Time.timeScale = 1;
    }
}