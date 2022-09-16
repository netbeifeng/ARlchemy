using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    public float Timeout = 8f;
    private TextMeshProUGUI  text;
    private ImageTrackingObjectManager _imageTrackingObjectManager;

    public string[] Messages = {
        "Welcome to AR Alchemy",
        "The goal of the game is to unlock new elements by combining existing ones",
        "To create a new element, place two elements and an empty marker next to the main marker",
        "Combine until you unlock the EndGoal for each level!",
        "To start the game, focus the camera on the Level Marker and click on it."
    };
    private void Awake()
    {
        _imageTrackingObjectManager = GameObject.Find("AR Session Origin").GetComponent<ImageTrackingObjectManager>();

    }
    public IEnumerator showMessages(GameObject text)
    {
        foreach (string message in Messages)
        {
            text.GetComponentInChildren<TextMeshProUGUI>().SetText(message);
            yield return new WaitForSeconds(this.Timeout);
        }
        
    }

}
