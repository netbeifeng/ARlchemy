using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * This class handles behaviour when user presses the duplicate button
 * or presses the delete button
 */
public class DuplicateController : MonoBehaviour
{
    private ImageTrackingObjectManager _imageTrackingObjectManager;
    private ImageObjectMapper _imageObjectMapper;
    private MergeController _mergeController;
    private UIController _uiController;

    [SerializeField] private GameObject emptyPrefab;

    private void Start()
    {
        _imageTrackingObjectManager = GetComponent<ImageTrackingObjectManager>();
        _mergeController = GetComponent<MergeController>();
        _imageObjectMapper = GetComponent<ImageObjectMapper>();
        _uiController = GetComponent<UIController>();
    }

    /**
     * Duplicate the marker that has gameobject on it to another marker with no gameobject on it.
     * if more gameobject in the field of view, shows error.
     */
    public void DuplicatePressed()
    {
        Debug.Log("Pressed Duplicate Button");
        var originalObjectList = _imageTrackingObjectManager._spawnedPrefabs
            .FindAll(x => x.gameObject.activeSelf && !x.gameObject.CompareTag("Empty"));
        var emptyObjectList = _imageTrackingObjectManager._spawnedPrefabs
            .FindAll(x => x.gameObject.activeSelf && x.gameObject.CompareTag("Empty"));

        if (originalObjectList.Count != 1 || emptyObjectList.Count == 0)
        {
            StartCoroutine(_uiController.showError());
        }
        else
        {
            var originalObject = originalObjectList[0].gameObject;
            var emptyObjectInstance = emptyObjectList[0];
            var clone = Instantiate(originalObject, emptyObjectInstance.gameObject.transform.position, emptyObjectInstance.gameObject.transform.rotation);

            clone.name = originalObject.name;

            _imageTrackingObjectManager._spawnedPrefabs.Remove(emptyObjectInstance);
            Destroy(emptyObjectInstance.gameObject);
            emptyObjectInstance.gameObject = clone;
            _imageTrackingObjectManager._spawnedPrefabs.Add(emptyObjectInstance);

            _imageObjectMapper._guidObjectDictionary[emptyObjectInstance.guid] = emptyObjectInstance.gameObject;
        }
    }

    /**
     * Delete the gameobject on the marker
     * if more gameobject in the field of view, shows error.
     */
    public void DeletePressed()
    {
        Debug.Log("Pressed Duplicate Button");
        var objectList = _imageTrackingObjectManager._spawnedPrefabs
            .FindAll(x => x.gameObject.activeSelf && !x.gameObject.CompareTag("Empty"));

        if (objectList.Count != 1)
        {
            StartCoroutine(_uiController.showError());
        }
        else
        {
            var originalObject = objectList[0];
            var empty = Instantiate(emptyPrefab, originalObject.gameObject.transform.position, originalObject.gameObject.transform.rotation);
            _imageTrackingObjectManager._spawnedPrefabs.Remove(originalObject);
            Destroy(originalObject.gameObject);

            originalObject.gameObject = empty;
            _imageTrackingObjectManager._spawnedPrefabs.Add(originalObject);

            _imageObjectMapper._guidObjectDictionary[originalObject.guid] = originalObject.gameObject;

        }
    }
}
