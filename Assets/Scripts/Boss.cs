using UnityEngine;
using Ink.Runtime;

public class Boss : NPC
{
    [SerializeField] TextAsset bossDialogue;

    bool firstTalk = true;

    [SerializeField] string[] yellDialogues;

    private new void OnEnable()
    {
        interactable.onInteract += OnInteract;
    }

    private new void OnDisable()
    {
        interactable.onInteract -= OnInteract;
    }

    public new void OnInteract()
    {
        if (status == NPCStatus.Talking) return;

        interactable.SetCanInteract(false);

        if (firstTalk)
        {
            GameManager.instance.HideInstruction();

            DialogueManager.onStoryContinued += OnDialogueContinue;
            DialogueManager.onStoryEnd += OnDialogueEnd;

            if (!GameManager.instance.tutorialActive)
            {
                DialogueManager.onDialogueSkipped += OnDialogueSkip;
            }

            DialogueManager.instance.EnterDialogue(new Story(bossDialogue.text), GameManager.instance.tutorialActive ? "Tutorial" : "Normal");

            status = NPCStatus.Talking;

            //OnDialogueEnd();
        } else
        {
            base.OnInteract();
        }
    }

    public new void OnDialogueEnd()
    {
        DialogueManager.onStoryEnd -= OnDialogueEnd;                // has to unsubscribe from the new one
        
        base.OnDialogueEnd();

        if (firstTalk)
        {
            firstTalk = false;
            GameManager.instance.NextInstruction(1);
            SetStatus(NPCStatus.Reviewing);
        }
    }

    public async void YellAtPlayer(int level)
    {
        dialogueDisplay.SetActive(true);

        dialogueText.text = "";

        string text = yellDialogues[level];

        foreach (char c in text)
        {
            await Awaitable.WaitForSecondsAsync(1f / NPCManager.instance.talkSpeed);
            dialogueText.text = dialogueText.text + c;
        }

        await Awaitable.WaitForSecondsAsync(NPCManager.instance.readTime);

        dialogueDisplay.SetActive(false);
    }
}
