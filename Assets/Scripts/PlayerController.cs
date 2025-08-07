using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    CharacterController characterController;
    [SerializeField] GameObject dialogueDisplay;
    [SerializeField] TMP_Text dialogueText;

    [SerializeField] Animator animator;

    [SerializeField] float moveSpeed;
    [SerializeField] float rotSpeed;

    Vector3 moveInput;

    float initalY;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        initalY = transform.position.y;

        InputManager.onMove += OnMove;
        DialogueManager.onStoryContinued += ShowChoice;
        DialogueManager.onStoryEnd += OnDialogueEnd;
    }

    private void OnDisable()
    {
        InputManager.onMove -= OnMove;
    }

    private void OnMove(Vector2 input)
    {
        moveInput = input;
    }

    void Update()
    {
        Vector3 move = moveInput;

        if (move.magnitude > 0)
        {
            animator.SetInteger("State", 1);
            move.Normalize();

            float r = move.magnitude;

            float angle = Vector2.SignedAngle(new Vector2(-move.x, move.y), -Vector2.right) - 45;
            move = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * r, Mathf.Sin(angle * Mathf.Deg2Rad) * r);

            Vector3 moveVector = new Vector3(move.x, 0, move.y) * moveSpeed * Time.deltaTime;

            characterController.Move(moveVector);

            Quaternion targetRot = Quaternion.LookRotation(-moveVector.normalized);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotSpeed);
        } else
        {
            animator.SetInteger("State", 0);
        }
        
        transform.position = new Vector3(transform.position.x, initalY, transform.position.z);
    }

    private async void ShowChoice(string text, bool isChoice)
    {
        dialogueDisplay.SetActive(isChoice);

        if (isChoice)
        {
            dialogueText.text = "";


            foreach (char c in text)
            {
                await Awaitable.WaitForSecondsAsync(1f / NPCManager.instance.talkSpeed);
                dialogueText.text = dialogueText.text + c;
            }

            await Awaitable.WaitForSecondsAsync(0.5f);

            DialogueManager.instance.TryContinueDialogue();
        }
    }

    private void OnDialogueEnd()
    {
        dialogueDisplay.SetActive(false); 
    }
}
