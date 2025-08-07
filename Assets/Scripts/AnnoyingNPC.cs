using UnityEngine;
using UnityEngine.AI;
using Ink.Runtime;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Threading;

public class AnnoyingNPC : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    [SerializeField] GameObject alertIcon;
    [SerializeField] GameObject dialogueDisplay;
    [SerializeField] TMP_Text dialogueText;

    [SerializeField] LayerMask obstructionMask;

    [SerializeField] float maxViewDistance = 10f;
    [SerializeField] float fov = 90f;

    [SerializeField] float walkSpeed = 5f;
    public float chaseSpeed = 8f;

    [SerializeField] float rotSpeed = 15f;

    NavMeshAgent agent;
    Animator animator;

    [SerializeField] Transform idleSpot;
    [SerializeField] Transform[] waypoints;

    [SerializeField] string[] wayPointDialogues;

    private enum State
    {
        Idle,
        Search,
        Chase,
        None
    }

    [SerializeField] float cooldownTime = 5f;
    [SerializeField] float searchTime = 10f;
    [SerializeField] float timeTillAlert = 1f;
    [SerializeField] float waitTime = 3f;

    float timer = 0;
    float alertTimer = -100000;
    float waitTimer = -100000;

    private State state;
    private State lastState = State.None;

    int waypointIndex = 0;

    Quaternion targetRot = Quaternion.identity;

    [SerializeField] TextAsset dialogue;
    [SerializeField] List<string> knotNames;

    private CancellationTokenSource sayCancellationTokenSource;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        animator = GetComponentInChildren<Animator>();

        state = State.Idle;
    }

    private void Update()
    {
        if (!GameManager.instance.gameActive) return;

        var nextState = state;

        switch (state)
        {
            case State.Idle:
                agent.speed = walkSpeed;

                if (StateChanged())
                {
                    Say("I'm taking a break.", state, false);

                    timer = cooldownTime;
                    agent.stoppingDistance = 0;
                    agent.SetDestination(idleSpot.position);

                } else if (!IsMoving())
                {
                    if (timer <= 0)
                    {
                        Say("Alright, back to it.", state, true);

                        nextState = State.Search;
                    } else
                    {
                        timer -= Time.deltaTime;

                        Vector3 dir = -idleSpot.forward;
                        targetRot = Quaternion.LookRotation(dir);
                    }
                } 

                
                break;
            case State.Search:
                agent.speed = walkSpeed;

                if (StateChanged())
                {
                    timer = searchTime;

                    if (lastState == State.Chase)
                    {
                        Say("It's cool, I'll talk to you later", state, false);
                    }
                } 
                else if (timer <= 0 && !SeePlayer())
                {
                    nextState = State.Idle;
                }
                else
                {
                    timer -= Time.deltaTime;
                }

                if (SeePlayer())
                {
                    agent.isStopped = true;

                    if (alertTimer == -100000)
                    {
                        alertTimer = timeTillAlert;
                        alertIcon.SetActive(true);
                    } else if (alertTimer <= 0)
                    {
                        agent.isStopped = false;
                        nextState = State.Chase;
                        alertTimer = -100000;
                        waitTimer = -100000;
                    }

                    alertIcon.GetComponentInChildren<Slider>().value = 1 - alertTimer / timeTillAlert;

                    Vector3 dir = (transform.position - playerController.transform.position).normalized;
                    targetRot = Quaternion.LookRotation(dir);

                    alertTimer -= Time.deltaTime;
                } 
                else 
                {
                    alertIcon.SetActive(false);

                    alertTimer = -100000;

                    if (!IsMoving())
                    {
                        if (waitTimer == -100000)
                        {
                            waitTimer = waitTime;

                            if (lastState == State.Search)
                            {
                                Say(wayPointDialogues[GetCurrentWaypoint()], state, false);
                            }
                        }
                        else if (waitTimer <= 0)
                        {
                            waitTimer = -100000;

                            agent.isStopped = false;
                            agent.stoppingDistance = 0;
                            agent.SetDestination(waypoints[waypointIndex].position);

                            waypointIndex = waypointIndex + 1 >= waypoints.Length ? 0 : waypointIndex + 1;
                        } else
                        {
                            waitTimer -= Time.deltaTime;

                            Vector3 dir = -waypoints[GetCurrentWaypoint()].forward;
                            targetRot = Quaternion.LookRotation(dir);
                        }
                    } 
                }

                break;
            case State.Chase:
                agent.speed = chaseSpeed;

                if (StateChanged())
                {
                    agent.stoppingDistance = 5;
                    agent.SetDestination(playerController.transform.position);

                    Say("Hey, what's up?", state, false);
                } 
                else if (SeePlayer())
                {
                    agent.stoppingDistance = 5;
                    agent.SetDestination(playerController.transform.position);

                    if (!IsMoving() && !DialogueManager.instance.dialoguePlaying && InputManager.instance.activeActionMap != InputManager.ActionMap.UI)
                    {
                        EnterDialogue();
                    }
                } 
                else if (!IsMoving())
                {
                    nextState = State.Search;
                    alertIcon.SetActive(false);
                }

                break;
        }

        if (IsMoving())
        {
            if (agent.desiredVelocity.magnitude > 0) {
                targetRot = Quaternion.LookRotation(-agent.desiredVelocity.normalized);
            }

            animator.SetInteger("State", 1);
        } else
        {
            animator.SetInteger("State", 0);
        }


        RotateToTarget(targetRot);

        lastState = state;
        state = nextState;
    }

    private async void Say(string text, State currentState, bool overrideState)
    {
        if (sayCancellationTokenSource != null)
        {
            sayCancellationTokenSource.Cancel();
            sayCancellationTokenSource.Dispose();
        }
        sayCancellationTokenSource = new CancellationTokenSource();
        var token = sayCancellationTokenSource.Token;


        dialogueDisplay.SetActive(true);

        await DisplayDialogue(text, currentState, overrideState, token);

        if (!token.IsCancellationRequested)
        {
            dialogueDisplay.SetActive(false);
        }
    }

    private async Awaitable DisplayDialogue(string text, State currentState, bool overrideState, CancellationToken token)
    {
        dialogueText.text = "";

        var newText = "";

        foreach (char c in text)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            if (state == currentState || overrideState)
            {
                await Awaitable.WaitForSecondsAsync(1f / NPCManager.instance.talkSpeed);
                newText = newText + c;
                dialogueText.text = newText;
            }
            else
            {
                Debug.Log(text);
                Debug.Log(state + " / " + currentState);
            }
        }

        await Awaitable.WaitForSecondsAsync(NPCManager.instance.readTime);
    }

    private void EnterDialogue()
    {
        DialogueManager.onStoryContinued += OnDialogueContinue;
        DialogueManager.onStoryEnd += OnDialogueEnd;

        alertIcon.SetActive(false);

        var story = new Story(dialogue.text);

        string knot = knotNames[Random.Range(0, knotNames.Count)];
        knotNames.Remove(knot);

        if (knot == "Gossip")
        {
            var coworkers = new List<NPC>(NPCManager.instance.coworkers);

            for (int i = 0; i < 4; i++)
            {
                var coworker = coworkers[Random.Range(0, coworkers.Count)];

                story.variablesState["name" + i] = coworker.name;

                coworkers.Remove(coworker);
            }
        }

        DialogueManager.instance.EnterDialogue(story, knot);
    }

    private async void OnDialogueContinue(string text, bool isChoice)
    {
        dialogueDisplay.SetActive(!isChoice);

        if (!isChoice)
        {
            if (sayCancellationTokenSource != null)
            {
                sayCancellationTokenSource.Cancel();
                sayCancellationTokenSource.Dispose();
            }
            sayCancellationTokenSource = new CancellationTokenSource();

            dialogueDisplay.SetActive(true);
            await DisplayDialogue(text, state, false, sayCancellationTokenSource.Token);

            DialogueManager.instance.TryContinueDialogue();
        }
    }

    private void OnDialogueEnd()
    {
        DialogueManager.onStoryContinued -= OnDialogueContinue;
        DialogueManager.onStoryEnd -= OnDialogueEnd;

        GameManager.instance.AddMistake();

        dialogueDisplay.SetActive(false);

        state = State.Idle;
    }


    private bool IsMoving()
    {
        return !(agent.remainingDistance <= agent.stoppingDistance) && !agent.isStopped;
    }

    private bool StateChanged()
    {
        return state != lastState;
    }

    private bool SeePlayer()
    {
        var dist = (transform.position - playerController.transform.position).magnitude;
        var direction = -(transform.position - playerController.transform.position).normalized;

        if (dist <= maxViewDistance && !Physics.Raycast(transform.position, direction, dist, obstructionMask))
        {
            var angle = Vector3.Angle(-transform.forward, direction);

            if (angle <= fov)
            {
                return true;
            }
        }

        return false;
    }

    private void RotateToTarget(Quaternion targetRot)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotSpeed);
    }

    private int GetCurrentWaypoint()
    {
        return waypointIndex == 0 ? waypoints.Length - 1 : waypointIndex - 1;
    }
}
