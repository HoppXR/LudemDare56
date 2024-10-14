using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthenticationManager : MonoBehaviour {

    public async void LoginAnonymously() {

        await Authentication.Login();
        await SceneManager.LoadSceneAsync("Lobby");

    }
}