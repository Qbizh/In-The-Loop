using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager : MonoBehaviour, PlayerInput.IPlayerActions, PlayerInput.IDialogueActions, PlayerInput.IUIActions
{
    public static InputManager instance { get; private set; }
    PlayerInput playerInput;

    public ActionMap activeActionMap = ActionMap.Player;

    public enum ActionMap
    {
        Player,
        Dialogue,
        UI
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        playerInput = new PlayerInput();

        playerInput.Player.AddCallbacks(this);
        playerInput.Dialogue.AddCallbacks(this);
        playerInput.UI.AddCallbacks(this);

        playerInput.Player.Enable();
    }

    public void SwitchActionMap(ActionMap mapType)
    {
        InputActionMap map = playerInput.Player;
        activeActionMap = mapType;

        switch (mapType)
        {
            case ActionMap.Player:
                map = playerInput.Player;
                break;
            case ActionMap.Dialogue:
                map = playerInput.Dialogue;
                break;
            case ActionMap.UI:
                map = playerInput.UI;
                break;
        }

        foreach (var actionMap in playerInput.asset.actionMaps)
        {
            if (actionMap != map)
            {
                actionMap.Disable();
            }
            else if (actionMap == map)
            {
                actionMap.Enable();
            }
        }
    }

    public static event Action<Vector2> onMove;
    public static event Action onInteract;

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed || ctx.phase == InputActionPhase.Canceled)
        {
            onMove?.Invoke(ctx.ReadValue<Vector2>());
        }
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            onInteract?.Invoke();
        }
    }

    public static event Action<int> onChoice;
    public static event Action onSkip;

    public void OnChooseOne(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            onChoice?.Invoke(0);
        }
    }

    public void OnChooseTwo(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            onChoice?.Invoke(1);
        }
    }

    public void OnSkip(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            onSkip?.Invoke();
        }
    }

    public static event Action onEscape;

    public void OnEscape(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            onEscape?.Invoke();
        }
    }
}
