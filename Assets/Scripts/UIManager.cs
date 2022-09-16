using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/**
 * Manages the UI shown in the InstructionMarker or on the screen when InstructionsMarker is missing
 */
public class UIManager : MonoBehaviour
{
    public float Timeout = 3f; /**  Timeout delay for  transitioning from showing the goal in text to showing it as an object*/
    private bool IsTracked = false;
    private bool CallOnce = false;
    private bool IsMainLevel = false;
    
    private string GoalName;
    private string ErrorMessage = "";

    private TMP_Text Message; /** UI message to show when not tracking main marker*/
    private GameObject TextPanel; /** Replaces main marker to show text message*/

    private ImageObjectMapper _imageObjectMapper;
    private List<ImageTrackingObjectManager.SpawnedPrefab> _spawnedPrefabs;
    private MergeController MergeController;


    /**
     * Gets the reference to the Message text UI
     */
    void Awake()
    {
        Message = this.transform.Find("Message").gameObject.GetComponentInChildren<TMP_Text>();
        // Prevent screen from sleeping
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    /**
     * Gets references needed for the members of this class
     */
    private void Start()
    {
        this.MergeController = GameObject.Find("AR Session Origin").GetComponent<MergeController>();
        IsMainLevel = (MergeController == null);

        this.TextPanel = GameObject.Find("TextCanvas");
        this.TextPanel.SetActive(false);


        _imageObjectMapper = GameObject.Find("AR Session Origin").GetComponent<ImageObjectMapper>();
        _spawnedPrefabs = GameObject.Find("AR Session Origin").GetComponent<ImageTrackingObjectManager>()._spawnedPrefabs;

        // In a playable level also show the goal
        if (!IsMainLevel)
        {
            this.GoalName = GameObject.Find("AR Session Origin").GetComponent<CombinationManager>().getLevelGoalsElement(MergeController.level).name;
            this.TextPanel.GetComponentInChildren<TextMeshProUGUI>().SetText("Goal of this Level: " + this.GoalName);
        }

    }

    /**
     * Shows/hides the message for InstructionMarker missing
     */
    void Update()
    {
        Message.enabled = !IsTracked;
    }

    /**
     * Called when the InstructionMarker is detected
     * Hides the "InstructionMarker is missing" message and shows the appropriate AR Image
     */
    public IEnumerator mainDetected()
    {

        if (!this.CallOnce) // Only when level starts and marker is detected for the first time
        {
            this.CallOnce = true;
            // Small delay and after that show the ARImage
            // Delay is so that the player can see the main goal in text
            if (!this.IsMainLevel)
            {
                yield return new WaitForSeconds(this.Timeout);
                this.showGoal();
            }
            else
            {
                this.showTextpanel();
            }
            var temp = this.TextPanel.GetComponent<TextUpdate>();
            if (temp != null)// Main level, show instructions before playing
            {
                StartCoroutine(temp.showMessages(this.getTextPanel()));
            }
        }

        this.IsTracked = true;
        // After being called the first time, change the text
        this.Message.text = "Main marker lost! Please focus the camera on the main marker to continue!";
    }
    /**
   * Lost the InstructionMarker
   */
    public void mainDissapear()
    {
        this.IsTracked = false;
    }
    /**
    * Gets the text panel object
   */
    private GameObject getTextPanel()
    {
        return _spawnedPrefabs.Find(x => x.gameObject.tag == "Removable").gameObject;
    }
    /**
   * Instruction marker is replaced by the textpanel object
   */
    private void showTextpanel()
    {
        // Find and remove previous object mapped to the instructionmarker
        var previous = _spawnedPrefabs.Find(x => x.gameObject.tag == "Removable");
        _spawnedPrefabs.Remove(previous);

        // If there is an error message, show it
        if (this.ErrorMessage != "")
        {
            this.TextPanel.GetComponentInChildren<TextMeshProUGUI>().SetText(this.ErrorMessage);
        }

        // Add the textpanel to the mapper
        _spawnedPrefabs.Add(new ImageTrackingObjectManager.SpawnedPrefab(previous.guid, Instantiate(this.TextPanel)));
        _imageObjectMapper._guidObjectDictionary[previous.guid] = _spawnedPrefabs.Find(x => x.gameObject.tag == "Removable").gameObject;

        //Destroy object we removed
        Destroy(previous.gameObject);
    }
    /**
    * Instruction marker is replaced by the endgoal model object
    */
    private void showGoal()
    {
        // Find and remove previous object mapped to the instructionmarker
        var previous = _spawnedPrefabs.Find(x => x.gameObject.tag == "Removable");
        _spawnedPrefabs.Remove(previous);
        // Instantiate the goal object as the child of an empty game object so that we can freely modify the tag and transform 
        var temp = Resources.Load(this.GoalName) as GameObject;
        var container = new GameObject("GoalContainer");
        container.tag = "Removable";

        if (this.MergeController.level == 0)
        {
            var Goal = Instantiate(temp, new Vector3(0f,0f,0f), Quaternion.Euler(270, 90, 0));
            Goal.transform.localScale = new Vector3(0.0015f, 0.0015f, 0.001f);
            Goal.transform.parent = container.transform;
        } else
        {
            var Goal = Instantiate(temp);
            Goal.transform.parent = container.transform;
        }

        // Add the goal image to the mapper
        _spawnedPrefabs.Add(new ImageTrackingObjectManager.SpawnedPrefab(previous.guid, container));
        _imageObjectMapper._guidObjectDictionary[previous.guid] = container;

        //Destroy object we removed

        Destroy(previous.gameObject);
    }

    /**
    * Show an error in the textpanel
    * @param error_message string The error message, should be the empty string if no error has occurred
    */
    public void showError(string error_message)
    {

        // Should show goal
        if (error_message == "")
        {
            if (this.ErrorMessage == "") { }// Already showing goal, nothing to do
            else // Transition from textpanel into goal
            {
                this.showGoal();
            }
        }
        // Should show textpanel
        else
        {

            if (this.ErrorMessage != "")
            {// Already shoing textpanel
                // Check if the errormessage has changed and update it
                if (this.ErrorMessage != error_message)
                {
                    this.getTextPanel().GetComponentInChildren<TextMeshProUGUI>().SetText(error_message);
                }
            }

            else // Transition from goal into textpanel
            {
                this.ErrorMessage = error_message;
                this.showTextpanel();
            }
        }

        // Update errormessage with the new one we received
        this.ErrorMessage = error_message;
    }
}