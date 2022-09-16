using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controls background music of the whole game
 */
public class AudioController : MonoBehaviour
{
    private void Awake()
    {
        GameObject.DontDestroyOnLoad(this);
    }
}
