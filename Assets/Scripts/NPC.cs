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
    public TMP_Text dialogueText;
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

    Quaternion originalRot;
    Quaternion targetRot;

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
        originalRot = transform.rotation;
        targetRot = originalRot;
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

            if (docs.Count > 0)
            {
                currentDoc = docs[0];
            } else
            {
                currentDoc = null;
            }
        }

        var story = new Story(dialogue.text);

        if (currentDoc != null)
        {
            if (status == NPCStatus.Reviewing)
            {
                DocsManager.instance.ReviewDoc(this, currentDoc);
            } else if (status == NPCStatus.Editing)
            {
                editingDoneIcon.SetActive(false);

                bool lastEditor = currentDoc.editHistory.Count >= DocsManager.instance.editorsNecessary;

                DocsManager.instance.GetNextEditor(currentDoc);

                if (currentDoc.nextEditor == null)
                {
                    currentDoc.completed = true;
                    lastEditor = true;
                }

                story.variablesState["lastEditor"] = lastEditor;

                story.variablesState["keepInLoop"] = DocsManager.instance.AddToLoop(this, currentDoc) && !lastEditor;
            }

            story.variablesState["nextPerson"] = currentDoc.nextEditor.name;
            story.variablesState["needToReview"] = currentDoc.NeedsReview();
        } else if (status == NPCStatus.Reviewing)
        {
            SetStatus(NPCStatus.Idle);
        }

        DialogueManager.onStoryContinued += OnDialogueContinue;
        DialogueManager.onStoryEnd += OnDialogueEnd;
        DialogueManager.onDialogueSkipped += OnDialogueSkip;

        DialogueManager.instance.EnterDialogue(story, status.ToString());

        SetStatus(NPCStatus.Talking);

        var player = GameObject.FindGameObjectWithTag("Player").transform;

        Vector3 dir = -(transform.position - player.position).normalized;
        targetRot = Quaternion.LookRotation(dir, transform.up);
    }

    public void SetStatus(NPCStatus newStatus)
    {
        lastStatus = status;
        status = newStatus;
    }

    public void OnDialogueSkip()
    {
        skipDialogue = true;
    }

    public async void OnDialogueContinue(string text, bool isChoice)
    {
        dialogueDisplay.SetActive(!isChoice);

        if (!isChoice)
        {
            if (!String.IsNullOrWhiteSpace(text))
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
            }

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

        targetRot = originalRot;

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

                    GameManager.instance.NextInstruction(4);
                }

                break;
            case NPCStatus.Reviewing:
                var docs = DocsManager.instance.GetRelevantDocs(this, true);

                if (docs.Count == 0)
                {
                    SetStatus(NPCStatus.Idle);
                } else
                {
                    SetStatus(NPCStatus.Reviewing);
                }          

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

    private void Update()
    {
        if (Mathf.Abs(transform.rotation.eulerAngles.y - targetRot.eulerAngles.y) >= 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 7);
        } else
        {
            transform.rotation = targetRot;
        }
    }
}
