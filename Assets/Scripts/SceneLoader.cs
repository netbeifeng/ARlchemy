using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Image _loadImage;
    [SerializeField] private Button _quitButton;
    
    /**
     * Load the scene asyncronized
     * This methode will Deinitialize and Initialize the AR Subsystem
     * @param sceneName Name of the scene
     */
    public void LoadScene(string sceneName)
    {
        LoaderUtility.Deinitialize();
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        LoaderUtility.Initialize();
    }

    public void QuitApplication()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        if (_quitButton != null)
            _quitButton.enabled = false;
        if (_loadImage != null)
            _loadImage.enabled = true;
        while (!asyncLoad.isDone)
        {
            yield return null; 
        }
        if (asyncLoad.isDone)
        {
            if (_loadImage != null)
                _loadImage.enabled = false;
        }
    }
}