using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] Boss boss;

    [SerializeField] TMP_Text timerDisplay;
    [SerializeField] List<Image> mistakeMarks;
    [SerializeField] Color markColor;

    [SerializeField] GameObject firedNotice;
    [SerializeField] TMP_Text firedReasonText;

    [SerializeField] GameObject winScreen;
    [SerializeField] TMP_Text difficultyText;
    [SerializeField] TMP_Text spareTimeText;
    [SerializeField] TMP_Text mistakesText;

    [SerializeField] GameObject instructionsPanel;

    [SerializeField] string[] instructions;

    [SerializeField] AnnoyingNPC annoyingCoworker;

    [Header("Difficulty Settings")]

    [SerializeField] int[] numberOfDocs;
    [SerializeField] float[] gameTime;
    [SerializeField] float[] enemyChaseSpeed;
    [SerializeField] int[] numberOfMistakes;

    int instructionsIndex;

    float timer = 0;

    public bool gameActive = false;

    public bool tutorialActive = true;

    int mistakes = 1;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
        
        tutorialActive = GameDataHolder.instance.data.tutorialActive;
    }

    private void Start()
    {
        NextInstruction(0);
    }

    private void Update()
    {
        if (gameActive) 
        {
            if (timer <= 0)
            {
                EndGame(false);
            } else
            {
                timer -= Time.deltaTime;

                TimeSpan time = TimeSpan.FromSeconds(timer);
                string display = time.ToString("m\\:ss");

                timerDisplay.text = display;
            }
        }
    }

    private void StartGame()
    {
        SetUpDifficulty();

        timerDisplay.gameObject.SetActive(true);
        gameActive = true;
    }

    private void SetUpDifficulty()
    {
        int difficulty = GameDataHolder.instance.data.difficulty - 1;

        timer = gameTime[difficulty];

        for (int i = 0; i < numberOfMistakes[difficulty]; i++)
        {
            mistakeMarks[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < numberOfDocs[difficulty]; i++)
        {
            DocsManager.instance.GenerateDoc();
        }

        annoyingCoworker.chaseSpeed = enemyChaseSpeed[difficulty];
    }

    public void EndGame(bool won)
    {
        gameActive = false;

        InputManager.instance.SwitchActionMap(InputManager.ActionMap.UI);

        if (won)
        {
            difficultyText.text = "Difficulty " + GameDataHolder.instance.data.difficulty;
            spareTimeText.text = timerDisplay.text + " to spare";
            mistakesText.text = mistakes - 1 + " mistakes";

            winScreen.SetActive(true);
            
        } else
        {
            if (timer > 0)
            {
                firedReasonText.text = "Made too many mistakes";
            } else
            {
                firedReasonText.text = "Ran out of time";
            }

            firedNotice.SetActive(true);
        }
    }

    public void OnEndAnimationOver()
    {
        SceneManager.LoadScene("Menu");
    }

    public void NextInstruction(int index)
    {
        if (index == instructionsIndex && tutorialActive)
        {
            if (instructionsIndex >= instructions.Length)
            {
                instructionsPanel.SetActive(false);
                tutorialActive = false;
            }
            else
            {
                instructionsPanel.SetActive(true);
                instructionsPanel.GetComponentInChildren<TMP_Text>().text = instructions[instructionsIndex];
                instructionsIndex++;
            }
        }

        if (index == 1)
        {
            StartGame();
        }
    }

    public void HideInstruction()
    {
        instructionsPanel.SetActive(false);
    }

    public void ShowInstrunction()
    {
        if (tutorialActive)
        {
            instructionsPanel.SetActive(true);
        }
    }

    public void AddMistake()
    {
        mistakeMarks[mistakes - 1].color = markColor;

        if (mistakes > numberOfMistakes[GameDataHolder.instance.data.difficulty - 1])
        {
            boss.YellAtPlayer(0);       // fired

            EndGame(false);
            return;
        }
        else if (mistakes == numberOfMistakes[GameDataHolder.instance.data.difficulty - 1])
        {
            boss.YellAtPlayer(1);       // last straw
        } else
        {
            boss.YellAtPlayer(mistakes + 1);
        }

        mistakes++;
    }
}
