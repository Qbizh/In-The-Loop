using UnityEngine;

public class GameDataHolder : MonoBehaviour
{
    public static GameDataHolder instance;

    public GameData data = new GameData();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }

        DontDestroyOnLoad(gameObject);


        SetData(PlayerPrefs.GetFloat("MusicVolume", data.musicVolume), 
            PlayerPrefs.GetFloat("SFXVolume", data.sfxVolume), 
            PlayerPrefs.GetInt("Difficulty", data.difficulty), 
            false);
    }

    private void OnDisable()
    {
        SaveData();
    }

    private void SaveData()
    {
        PlayerPrefs.SetFloat("MusicVolume", data.musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", data.sfxVolume);
        PlayerPrefs.SetInt("Difficulty", data.difficulty);

        PlayerPrefs.Save();
    }

    public void SetData(float music, float sfx, int diff, bool tutorial)
    {
        data.musicVolume = music;
        data.sfxVolume = sfx;
        data.difficulty = diff;
        data.tutorialActive = tutorial;

        SaveData();
    }
}
