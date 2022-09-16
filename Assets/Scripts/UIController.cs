using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/**
 * Controls the 2D UI in the scene
 */
public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text _debugText;
    [SerializeField] private Image _error;
    [SerializeField] private float _errorSeconds;
    [SerializeField] private GameObject _hierarchy;
    [SerializeField] private GameObject _inspector;
    private bool _enabled = false;

    private void Start()
    {
        _debugText.enabled = false;
    }
    
    /**
     * Toggle debug on and off.
     * With debug on you will see the objects instantiated and a run-time hierarchy + inspector.
     */
    public void ToggleDebugOn()
    {
        _enabled = !_enabled;
        _debugText.enabled = _enabled;
        _hierarchy.SetActive(_enabled);
        _inspector.SetActive(_enabled);
    }

    /**
     * Shows a red x icon.
     */
    public IEnumerator showError()
    {
        _error.enabled = true;
        yield return new WaitForSeconds(_errorSeconds);
        _error.enabled = false;
    }
}
