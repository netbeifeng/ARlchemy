using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectController : MonoBehaviour
{
    [SerializeField] private List<AudioClip> sfx = new List<AudioClip>();

    private AudioSource audioSource;
    private void Start()
    {
        audioSource = this.GetComponentInChildren<AudioSource>();
    }

    /**
     * Play soundeffect
     * @param index Index of the sfx should played.
     * List of sfx could be modified in Unity Editor
     */
    public void PlayerSoundEffect(int index)
    {
        audioSource.clip = sfx[index];
        audioSource.Play();
    }
}
