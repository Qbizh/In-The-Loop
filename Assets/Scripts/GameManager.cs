using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] TMP_Text timerDisplay;

    [SerializeField] float gameTime = 120f;

    [SerializeField] GameObject instructionsPanel;

    [SerializeField] string[] instructions;
    int instructionsIndex;

    float timer = 0;

    bool gameActive = false;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        NextInstruction();
    }

    private void Update()
    {
        if (gameActive) 
        {
            timer -= Time.deltaTime;

            TimeSpan time = TimeSpan.FromSeconds(timer);
            string display = time.ToString("m\\:ss");

            timerDisplay.text = display;
        }
    }

    private void StartGame()
    {
        timer = gameTime;
        timerDisplay.gameObject.SetActive(true);
        gameActive = true;
    }

    public void NextInstruction()
    {
        if (instructionsIndex >= instructions.Length)
        {
            instructionsPanel.SetActive(false);
            StartGame();
        } else
        {
            instructionsPanel.SetActive(true);
            instructionsPanel.GetComponentInChildren<TMP_Text>().text = instructions[instructionsIndex];
            instructionsIndex++;
        }
    }

    public void HideInstruction()
    {
        instructionsPanel.SetActive(false);
    }
}
