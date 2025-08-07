using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;

    private void Awake()
    {
        mixer.SetFloat("MusicVolume", VolToDecibls(GameDataHolder.instance.data.musicVolume));
        mixer.SetFloat("SFXVolume", VolToDecibls(GameDataHolder.instance.data.sfxVolume));
    }

    private float VolToDecibls(float vol)
    {
        return Mathf.Log10(vol) * 20;
    }
}
