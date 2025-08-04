using UnityEngine;
using Ink;
using Ink.Runtime;
using TMPro;
using System;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public static event Action<string, bool> onStoryContinued;
    public static event Action onStoryEnd;
    public static event Action onDialogueSkipped;

    [SerializeField] GameObject choicesUI;
    [SerializeField] TMP_Text choiceOneText;
    [SerializeField] TMP_Text choiceTwoText;

    public Story currentStory;

    bool dialoguePlaying = false;
    bool waitingForChoice = false;

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

    private void OnEnable()
    {
        InputManager.onChoice += OnChoice;
        InputManager.onSkip += OnSkipDialogue;
    }

    public void EnterDialogue(Story story, string startingPath)
    {
        currentStory = story;

        currentStory.ChoosePathString(startingPath);

        if (currentStory.canContinue) 
        {
            dialoguePlaying = true;
            InputManager.instance.SwitchActionMap(InputManager.ActionMap.Dialogue);

            ContinueStory();
        } else
        {
            dialoguePlaying = false;
        }
    }

    private void ExitDialogue()
    {
        onStoryEnd?.Invoke();

        InputManager.instance.SwitchActionMap(InputManager.ActionMap.Player);
        currentStory = null;
        dialoguePlaying = false;
    }

    private void ContinueStory()
    {
        string text = currentStory.Continue();

        onStoryContinued?.Invoke(text, waitingForChoice);
    }

    public void OnChoice(int choice)
    {
        if (waitingForChoice)
        {
            if (currentStory.currentChoices.Count > 0)
            {
                currentStory.ChooseChoiceIndex(choice);
            }
            choicesUI.SetActive(false);

            ContinueStory();
        }
    }

    private void OnSkipDialogue()
    {
        if (!waitingForChoice && currentStory != null)
        {
            onDialogueSkipped?.Invoke();
        }
    }

    public void TryContinueDialogue()
    {
        if (!dialoguePlaying) return;

        if (!currentStory.canContinue)
        {
            List<Choice> currentChoices = currentStory.currentChoices;

            waitingForChoice = currentChoices.Count > 0;

            if (waitingForChoice)
            {
                choiceOneText.text = currentChoices[0].text;
                choiceTwoText.text = currentChoices[1].text;

                choicesUI.SetActive(true);

                waitingForChoice = true;
            } else
            {
                ExitDialogue();
            }
            
            return;
        }

        waitingForChoice = false;

        ContinueStory();

        
    }
}
