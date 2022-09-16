using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;
using System;
using UnityEngine.XR.ARSubsystems;

public class ImageTrackingObjectManager: MonoBehaviour
{
    [Header("Debug")]
    public TMP_Text _debugText;

    [Header("AR")]
    private ARTrackedImageManager _artrackedImageManager;

    private ImageObjectMapper _imageObjectMapper;

    private Dictionary<Guid, GameObject> _guidObjectDictionary;



    public struct SpawnedPrefab{
        public SpawnedPrefab(Guid guid, GameObject gameObject)
        {
            this.guid = guid;
            this.gameObject = gameObject;
        }
        public Guid guid;
        public GameObject gameObject; 
    }

    public List<SpawnedPrefab> _spawnedPrefabs;

    public UIManager UI;
    private ARTrackedImage MainMarker;
    private float LastTime = 0; /** Tracks the time since marker is lost, used to add a delay before showing the ui message*/
    private bool is_tracked = true;

    private void Awake()
    {
        _artrackedImageManager = GetComponent<ARTrackedImageManager>(); 
        _spawnedPrefabs = new List<SpawnedPrefab>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _imageObjectMapper = GetComponent<ImageObjectMapper>();
        _guidObjectDictionary = _imageObjectMapper._guidObjectDictionary;


        // Finds the UIcontroller script in current scene
        UI = GameObject.Find("Canvas").GetComponent<UIManager>();
        this.LastTime = Time.time;
    }

    /**
     * # Event function when image status changed.
     * `GameObject` will be instantiate according to `ImageObjectMapper.cs` when image detected
     * `GameObject` will update the position and rotation when image is in tracking state
     * `GameObject` will deactivate when image can't be tracked anymore
     */
    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        // new image detected
        foreach(var img in args.added)
        {
            GameObject prefab;
            if(!_guidObjectDictionary.TryGetValue(img.referenceImage.guid, out prefab))
            {
                Debug.LogError("Something went wrong with creating dictionary");
                Application.Quit(-1);
            }
            var spawnedPrefab = Instantiate(prefab, img.transform.position, img.transform.rotation);

           // _debugText.text
           // prefab.name + " detected!\n";
            SpawnedPrefab temp = new SpawnedPrefab(img.referenceImage.guid, spawnedPrefab);
            _spawnedPrefabs.Add(temp);
          //  _debugText.text += "Size of spawned objects: " + _spawnedPrefabs.Count + "\n";

            if (img.referenceImage.name == "InstructionMarker")
            {
                this.MainMarker = img;
            //    _debugText.text += "entered : name: " + img.referenceImage.name + "\n";

            }
        }

        // img get updated
        foreach(var img in args.updated)
        {
            GameObject updatedPrefab = _spawnedPrefabs.Find(x => x.guid == img.referenceImage.guid).gameObject;
            if (img.trackingState == TrackingState.Tracking)
            {
                updatedPrefab.SetActive(true);
                updatedPrefab.transform.SetPositionAndRotation(img.transform.position, img.transform.rotation);
                Debug.Log(updatedPrefab + " updated, Position: " + updatedPrefab.transform.position);
            }
            else
            {
                updatedPrefab.SetActive(false);
            }
        }

        // Found the main marker, or it has been less than timeout since we lost the main marker
        if (this.MainMarker != null && (this.MainMarker.trackingState == TrackingState.Tracking || Time.time - this.LastTime < 2))
        {
            if (this.MainMarker.trackingState == TrackingState.Tracking)
            {
                is_tracked = true;
            }
            StartCoroutine(UI.mainDetected());

        }// Lost the main marker
        else if (this.MainMarker.trackingState != TrackingState.Tracking && this.is_tracked == true)
        {

            this.LastTime = Time.time;
            is_tracked = false;
        }// Lost main marker for more than timeout, so signal the ui manager
        else
        {
            UI.mainDissapear();
        }

        // img get removed 
        foreach (var img in args.removed)
        {
            var deletedPrefabs = _spawnedPrefabs.Find(x => x.guid == img.referenceImage.guid);
            Destroy(deletedPrefabs.gameObject);
            _spawnedPrefabs.Remove(deletedPrefabs);
        }
        

    }
    private void OnEnable()
    {
        _artrackedImageManager.trackedImagesChanged += OnImageChanged; 
    }

    private void OnDisable()
    {
        _artrackedImageManager.trackedImagesChanged -= OnImageChanged; 
    }

    [ContextMenu("TEST")]
    public void mainDissapear()
    {
        StartCoroutine(UI.mainDetected());

    }
}
