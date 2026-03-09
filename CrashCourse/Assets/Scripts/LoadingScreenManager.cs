using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;
    public GameObject m_LoadingScreenObject;
    public Slider ProgressBar;

    void Start()
    {
        m_LoadingScreenObject.SetActive(false);
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance  = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void SwitchToScene(int id)
    {
        m_LoadingScreenObject.SetActive(true);
        ProgressBar.value = 0;
        StartCoroutine(SwitchToSceneAsyc(id));
    }

    IEnumerator SwitchToSceneAsyc(int id)
    {
        m_LoadingScreenObject.SetActive(true);
        ProgressBar.value = 0;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(id);
        asyncLoad.allowSceneActivation = false;

        float fakeProgress = 0f;
        float speed = 0.5f;

        while (!asyncLoad.isDone)
        {
            float targetProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            fakeProgress = Mathf.MoveTowards(fakeProgress, targetProgress, speed * Time.deltaTime);
            ProgressBar.value = fakeProgress;

            if (fakeProgress >= 1f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.3f);
        m_LoadingScreenObject.SetActive(false);
    }

}
