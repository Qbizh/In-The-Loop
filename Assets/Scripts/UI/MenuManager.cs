using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider difficultySlider;

    [SerializeField] GameObject tutorialCheckmark;

    [SerializeField] TMP_Text difficultyText;

    bool tutorialActive = false;

    private void Start()
    {
        if (PlayerPrefs.GetInt("FirstTime") == 0)
        {
            tutorialActive = true;
            tutorialCheckmark.SetActive(true);

            difficultySlider.value = 1;
            difficultySlider.interactable = false;

            PlayerPrefs.SetInt("FirstTime", 1);
            PlayerPrefs.Save();
        } else
        {
            tutorialActive = false;
            tutorialCheckmark.SetActive(false);
        }

        var data = GameDataHolder.instance.data;
        musicSlider.value = data.musicVolume;
        sfxSlider.value = data.sfxVolume;
        difficultySlider.value = data.difficulty;
    }

    public void PlayButtonPressed()
    {
        GameDataHolder.instance.SetData(musicSlider.value, sfxSlider.value, (int)difficultySlider.value, tutorialActive);

        SceneManager.LoadScene("Game");
    }

    public void TutorialButtonPressed()
    {
        tutorialActive = !tutorialActive;
        tutorialCheckmark.SetActive(tutorialActive);
        if (tutorialActive)
        {
            difficultySlider.value = 1;
            difficultySlider.interactable = false;
        } else
        {
            difficultySlider.interactable = true;
        }
    }

    public void OnDifficultyChanged()
    {
        difficultyText.text = difficultySlider.value.ToString();
    }
}
