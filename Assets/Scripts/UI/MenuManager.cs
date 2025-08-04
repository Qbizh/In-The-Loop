using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayButtonPressed()
    {
        SceneManager.LoadScene("Game");
        SceneManager.UnloadSceneAsync("Menu");
    }
}
