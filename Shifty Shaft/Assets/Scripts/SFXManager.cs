using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    public static SFXManager instance { get; private set; }

    [SerializeField] private AudioClip[] sfxClips;

    private Dictionary <string, AudioClip> sfxDict = new ();
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        for (int i = 0; i < sfxClips.Length; i++)
            sfxDict.Add(sfxClips[i].name, sfxClips[i]);
    }

    public void PlaySFX(string sfxName, float volume = 1f) => audioSource.PlayOneShot(sfxDict[sfxName], volume);
}
