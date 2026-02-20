using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Play()
    {
        SceneManager.LoadScene("LevelScreen");
    }

    public void Return()
    {
        SceneManager.LoadScene("StartScreen");
    }
}
