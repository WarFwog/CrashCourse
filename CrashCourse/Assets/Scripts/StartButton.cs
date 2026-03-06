using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{    public void OnClickGame()
    {
       LoadingScreenManager.Instance.SwitchToScene(1);
       // SceneManager.LoadScene("LobbyScreen");
        


    }

   public void Quitgame()
    {
        Application.Quit();
    }
}
