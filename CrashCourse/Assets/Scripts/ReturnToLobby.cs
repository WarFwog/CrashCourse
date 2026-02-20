using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToLobby : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public void ReturnLobby()
    {
        SceneManager.LoadScene("LobbyScreen");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
