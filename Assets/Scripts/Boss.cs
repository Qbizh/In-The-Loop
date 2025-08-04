using UnityEngine;
using Ink.Runtime;

public class Boss : NPC
{
    [SerializeField] TextAsset bossDialogue;

    bool firstTalk = true;

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

            /*DialogueManager.onStoryContinued += OnDialogueContinue;
            DialogueManager.onStoryEnd += OnDialogueEnd;

            DialogueManager.instance.EnterDialogue(new Story(bossDialogue.text), "Main");*/

            status = NPCStatus.Talking;

            OnDialogueEnd();
        } else
        {
            base.OnInteract();
        }
    }

    public new void OnDialogueEnd()
    {
        base.OnDialogueEnd();

        if (firstTalk)
        {
            firstTalk = false;
            DocsManager.instance.GenerateDoc();
            GameManager.instance.NextInstruction();
            SetStatus(NPCStatus.Reviewing);
        }
    }
}
