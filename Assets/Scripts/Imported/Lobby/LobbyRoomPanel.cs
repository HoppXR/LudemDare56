using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyRoomPanel : MonoBehaviour {

    [SerializeField] private TMP_Text _nameText, _playerCountText;

    public Lobby Lobby { get; private set; }

    public static event Action<Lobby> LobbySelected;

    public void Init(Lobby lobby) {
        UpdateDetails(lobby);
    }

    public void UpdateDetails(Lobby lobby) {
        Lobby = lobby;
        _nameText.text = lobby.Name;

        _playerCountText.text = $"{lobby.Players.Count} - {lobby.MaxPlayers}";
    }

    public void Clicked() {
        LobbySelected?.Invoke(Lobby);
    }
}