using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void Playgame()
    {
        SceneManager.LoadScene("LobbyScreen");
    }

   public void Quitgame()
    {
        Application.Quit();
    }
}
