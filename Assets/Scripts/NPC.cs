using System;
using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public TextAsset dialogue;

    public GameObject dialogueDisplay;
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] TMP_Text nameText;

    [SerializeField] Slider progressBar;
    [SerializeField] GameObject editingDoneIcon;

    [SerializeField] Transform hairHolder;

    public enum NPCStatus
    {
        Idle,
        Waiting,
        Editing,
        Reviewing,
        Talking
    }

    public string name;
    public GameObject hair;
    public Material hairColor;

    public NPCStatus status = NPCStatus.Idle;
    private NPCStatus lastStatus = NPCStatus.Idle;

    public Interactable interactable;

    bool skipDialogue;

    Doc currentDoc = null;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
    }

    public void OnEnable()
    {
        interactable.onInteract += OnInteract;
    }

    public void OnDisable()
    {
        interactable.onInteract -= OnInteract;
    }

    public void SetUp(string newName, GameObject newHair, Material color)
    {
        name = newName;
        nameText.text = name;

        hairColor = color;

        hair = Instantiate(newHair, hairHolder);
        hair.GetComponent<Renderer>().material = color;
    }

    public void OnInteract()
    {
        if (status == NPCStatus.Talking) return;

        interactable.SetCanInteract(false);

        if (status == NPCStatus.Waiting || status == NPCStatus.Reviewing)
        {
            var docs = DocsManager.instance.GetRelevantDocs(this, status == NPCStatus.Reviewing);
            Debug.Log(docs[0]);
            if (docs.Count > 0)
            {
                currentDoc = docs[0];
            }
        }

        var story = new Story(dialogue.text);

        if (currentDoc != null)
        {
            if (status == NPCStatus.Reviewing)
            {
                if (!currentDoc.loop[this])
                {
                    DocsManager.instance.ReviewDocs(this);
                } else
                {
                    SetStatus(NPCStatus.Idle);
                }
            } else if (status == NPCStatus.Editing)
            {
                editingDoneIcon.SetActive(false);
                DocsManager.instance.GetNextEditor(currentDoc);
                story.variablesState["keepInLoop"] = DocsManager.instance.AddToLoop(this, currentDoc);
            }

            story.variablesState["nextPerson"] = currentDoc.nextEditor.name;
            story.variablesState["needToReview"] = currentDoc.NeedsReview();
        }

        DialogueManager.onStoryContinued += OnDialogueContinue;
        DialogueManager.onStoryEnd += OnDialogueEnd;
        DialogueManager.onDialogueSkipped += OnDialogueSkip;

        DialogueManager.instance.EnterDialogue(story, status.ToString());

        SetStatus(NPCStatus.Talking);
    }

    public void SetStatus(NPCStatus newStatus)
    {
        lastStatus = status;
        status = newStatus;
    }

    private void OnDialogueSkip()
    {
        skipDialogue = true;
    }

    public async void OnDialogueContinue(string text, bool isChoice)
    {
        dialogueDisplay.SetActive(!isChoice);

        if (!isChoice)
        {
            dialogueText.text = "";

            skipDialogue = false;

            foreach (char c in text)
            {
                if (skipDialogue)
                {
                    dialogueText.text = text;

                    break;
                }

                await Awaitable.WaitForSecondsAsync(1f / NPCManager.instance.talkSpeed);
                dialogueText.text = dialogueText.text + c;
            }

            await Awaitable.WaitForSecondsAsync(skipDialogue ? 0.5f : NPCManager.instance.readTime);
            skipDialogue = false;

            DialogueManager.instance.TryContinueDialogue();
        }
    }

    public void OnDialogueEnd()
    {
        DialogueManager.onStoryContinued -= OnDialogueContinue;
        DialogueManager.onStoryEnd -= OnDialogueEnd;
        DialogueManager.onDialogueSkipped -= OnDialogueSkip;

        dialogueDisplay.SetActive(false);
        interactable.SetCanInteract(true);

        switch (lastStatus)
        {
            case NPCStatus.Idle:
                SetStatus(NPCStatus.Idle);
                break;
            case NPCStatus.Waiting:
                if (DocsManager.instance.TryEditDoc(this, currentDoc))
                {
                    interactable.SetCanInteract(false);
                    SetStatus(NPCStatus.Editing);
                    EditingAsync();
                } else
                {
                    interactable.SetCanInteract(true);
                    SetStatus(NPCStatus.Waiting);
                }

                break;
            case NPCStatus.Editing:
                if (currentDoc != null)
                {
                    DocsManager.instance.FinishEdit(currentDoc);

                    SetStatus(NPCStatus.Idle);

                    currentDoc = null;
                }

                break;
            case NPCStatus.Reviewing:


                SetStatus(NPCStatus.Idle);

                break;
            case NPCStatus.Talking:
                break;
        }
    }

    private async void EditingAsync()
    {
        float step = NPCManager.instance.editingTime / 100f;

        progressBar.gameObject.SetActive(true);

        progressBar.value = 0;

        for (float i = 0; i < NPCManager.instance.editingTime; i+= step)
        {
            await Awaitable.WaitForSecondsAsync(step);

            progressBar.value = i / NPCManager.instance.editingTime;
        }

        progressBar.gameObject.SetActive(false);
        editingDoneIcon.gameObject.SetActive(true);

        interactable.SetCanInteract(true);
    }
}
