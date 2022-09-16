using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;


/**
 * This file handles the merge behaviour when 2 `GameObjects` get close
 * to the main marker.
 * Then these 2 `GameObjects` will merged through `CombinationManager.cs`
 */
public class MergeController: MonoBehaviour
{
    [SerializeField] private float _minDistance;

    private ImageObjectMapper _imageObjectMapper;
    
    private ImageTrackingObjectManager _imageTrackingObjectManager;

    private CombinationManager _combinationManager;

    private SoundEffectController _soundEffectController;

    private UIController _uIController;

    private List<ImageTrackingObjectManager.SpawnedPrefab> _spawnedPrefabs;

    private Guid _instructionGuid;

    private  UIManager UI; /** Needs UI to notify when the InstructionMarker is lost */

    [Header("Attributes")]
    public int level;

    private bool merging;
    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    private void Start()
    {
        this.UI = GameObject.Find("Canvas").GetComponent<UIManager>();

        _imageObjectMapper = GetComponent<ImageObjectMapper>();
        _imageTrackingObjectManager = GetComponent<ImageTrackingObjectManager>();
        _combinationManager = GetComponent<CombinationManager>();
        _uIController = GetComponent<UIController>();
        _soundEffectController = GetComponent<SoundEffectController>();

        _spawnedPrefabs = _imageTrackingObjectManager._spawnedPrefabs;
        _instructionGuid = _imageObjectMapper._imageLibrary[_imageObjectMapper._instructionIndex].guid;
    }
    private void Update()
    {
        string error_message = "";
        // Find whether instruction spawned
        GameObject instruction = _spawnedPrefabs.Find(x => x.guid == _instructionGuid).gameObject;
        if (instruction != null)
        {
            GameObject emptyPrefab = _spawnedPrefabs.Find(x => x.gameObject.CompareTag("Empty") && x.gameObject.activeSelf).gameObject;
            List<GameObject> prefabList = new List<GameObject>();
            foreach (var spawnedPrefab in _spawnedPrefabs)
            {
                if (spawnedPrefab.gameObject.activeSelf)
                {
                    if (spawnedPrefab.gameObject != instruction && spawnedPrefab.gameObject != null
                        && !spawnedPrefab.gameObject.CompareTag("Empty") && !spawnedPrefab.gameObject.tag.Contains("Level"))
                    {
                        if (CheckDistance(instruction.transform, spawnedPrefab.gameObject.transform))
                        {
                            prefabList.Add(spawnedPrefab.gameObject);
                        }
                    }
                }
            }
            if (prefabList.Count == 2)
            {
                if (emptyPrefab == null)
                {
                    error_message = "Can't find an empty marker. Please replace one of the markers with an empty one to continue";
                }
                else
                {
                    if (_combinationManager.checkCompatibility(prefabList[0].name.Replace("(Clone)", ""), prefabList[1].name.Replace("(Clone)", ""), level))
                        if (merging == false)
                            Merge(prefabList[0], prefabList[1], emptyPrefab);
                        else
                        {
                            StartCoroutine(_uIController.showError());
                        }
                }
            }
        }
        // Call the UI, if there has been no error, error_message is empty. Otherwise the UI will show the message
        UI.showError(error_message);
    }

    private bool CheckDistance(Transform instruction, Transform prefab)
    {
        float dist = Vector3.Distance(instruction.position, prefab.position);
        return dist < _minDistance;
    }

    /**
     * Merge `GameObject` g1 and g2 to the `emptyPrefabs` position.
     * @param g1 GameObject 1
     * @param g2 GameObject 2
     * @param emptyPrefab Positio the merged object should be generated
    */
    public void Merge(GameObject g1, GameObject g2, GameObject emptyPrefab)
    {
        merging = true;
        var gName = _combinationManager.getProductElementOfCombination(g1.name.Replace("(Clone)", ""), g2.name.Replace("(Clone)", ""), level).name;
        var g = Resources.Load(gName) as GameObject;

        _imageTrackingObjectManager.enabled = false;        
        var transformG1 = g1.transform;
        var transformG2 = g2.transform;
        Vector3 middlePoint = (g1.transform.position + g2.transform.position) / 2;

        g = Instantiate(g, middlePoint, emptyPrefab.transform.rotation);
        var scaleG = g.transform.localScale;
        g.transform.localScale = Vector3.zero;

        // Update list and dictionary
        var structEmpty = _spawnedPrefabs.Find(x => x.gameObject == emptyPrefab);
        _spawnedPrefabs.Remove(structEmpty);
        _spawnedPrefabs.Add(new ImageTrackingObjectManager.SpawnedPrefab(structEmpty.guid, g));
        _imageObjectMapper._guidObjectDictionary[structEmpty.guid] = g;

        // Animation
        var sequence = DOTween.Sequence();
        sequence.Insert(0, g1.transform.DOMove(middlePoint, 2));
        sequence.Insert(0, g1.transform.DOScale(Vector3.zero, 2));
        sequence.Insert(0, g2.transform.DOMove(middlePoint, 2));
        sequence.Insert(0, g2.transform.DOScale(Vector3.zero, 2).OnComplete(() =>
        {
            _soundEffectController.PlayerSoundEffect(0);
        }));
        sequence.Insert(2, g.transform.DOScale(scaleG, 1));
        sequence.Insert(2, g.transform.DOMove(emptyPrefab.transform.position, 1));
        sequence.Insert(2, g1.transform.DOMove(transformG1.position, 1));
        sequence.Insert(2, g2.transform.DOMove(transformG2.position, 1));
        sequence.Insert(2, g1.transform.DOScale(transformG1.localScale, 1));
        sequence.Insert(2, g2.transform.DOScale(transformG2.localScale, 1));
        sequence.OnComplete(() =>
        {
            Destroy(emptyPrefab);
			_imageTrackingObjectManager.enabled = true;
            if (_combinationManager.getLevelGoalsElement(level).name == g.name.Replace("(Clone)", ""))
            {
               SceneManager.LoadScene("EndScene");
            }
            merging = false;
        });
    }


}
