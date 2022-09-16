using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

/**
* # This script realize that each element from the list has the image name from the ImageLibraryReference
*/
#if UNITY_EDITOR
[CustomEditor(typeof(ImageObjectMapper))]
[CanEditMultipleObjects]
public class ImageObjectMapperEditor: Editor
{
    SerializedProperty _prefabList;
    SerializedProperty _imageLibrary;
    SerializedProperty _instructionIndex;
    
    private ImageObjectMapper _imageObjectMapper;
    private void OnEnable()
    {
        _prefabList = serializedObject.FindProperty("_prefabList");
        _imageLibrary = serializedObject.FindProperty("_imageLibrary");
        _instructionIndex = serializedObject.FindProperty("_instructionIndex");
    }
     public void ShowArrayProperty(UnityEditor.SerializedProperty list)
     {
        serializedObject.Update();
        _imageObjectMapper = target as ImageObjectMapper;
        int number = _imageObjectMapper._imageLibrary.count;
        _prefabList.arraySize = number;
         EditorGUI.indentLevel += 1;
         for (int i = 0; i < list.arraySize; i++)
         {
             EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new UnityEngine.GUIContent(_imageObjectMapper._imageLibrary[i].name));
         }
         EditorGUI.indentLevel -= 1;
     }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(_imageLibrary);
        ShowArrayProperty(_prefabList);
        EditorGUILayout.PropertyField(_instructionIndex);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif


/**
 * Add image guid and the gameobject assigned to a dictionary
 */
public class ImageObjectMapper : MonoBehaviour
{
    [Tooltip("Reference Image Library")] 
    public XRReferenceImageLibrary _imageLibrary;

    public List<GameObject> _prefabList;

    public Dictionary<Guid, GameObject> _guidObjectDictionary = new Dictionary<Guid, GameObject>();

    public int _instructionIndex;

    private void Awake()
    {
        if (_imageLibrary.count != _prefabList.Count)
        {
            Debug.LogError("Number of Image does not match with the prefab number of image");
            Application.Quit(-1);
        }
        else
        {
            for(int i = 0; i < _imageLibrary.count; ++i)
            {
                _guidObjectDictionary.Add(_imageLibrary[i].guid, _prefabList[i]);
            }
        }
    }
}
