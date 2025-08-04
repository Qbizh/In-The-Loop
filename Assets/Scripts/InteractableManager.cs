using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    public static InteractableManager instance;

    public static event Action<Interactable, Interactable> onActiveInteractableChange;

    [SerializeField] Interactable activeInteractable;

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
        InputManager.onInteract += OnInteract;
    }

    private void OnDisable()
    {
        InputManager.onInteract -= OnInteract;
    }

    public void SetActiveInteractable(Interactable interactable, bool remove)
    {
        if (activeInteractable == interactable) 
        {
            if (remove)
            {
                activeInteractable.SetInteractPrompt(false);
                onActiveInteractableChange?.Invoke(interactable, null);
                activeInteractable = null;
                
            }
        } else
        {
            if (!remove)
            {
                activeInteractable?.SetInteractPrompt(false);

                onActiveInteractableChange?.Invoke(activeInteractable, interactable);
                activeInteractable = interactable;
                

                activeInteractable.SetInteractPrompt(true);
            }
        }
    }

    private void OnInteract()
    {
        activeInteractable?.OnInteract();
    }
}
