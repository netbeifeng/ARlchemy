using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

/**
 * When an Input is detected then we get the Touch Object and Cast a ray into the screen where the finger hit the screen.
 * We detect what we hit with this ray and if it was our Level Loader Object load the scene.
 */
public class LevelLoader : MonoBehaviour
{
    public string levelName;
    // Update is called once per frame    

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //Select stage    
                if (hit.transform.tag == levelName)
                {
                    LoaderUtility.Deinitialize();
                    SceneManager.LoadScene(levelName, LoadSceneMode.Single);
                    LoaderUtility.Initialize();
                }
            }
        }
    }
}
