using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] GameObject interactPrompt;
    
    public event Action onInteract;

    bool canInteract = true;

    bool containsPlayer = false;

    private void OnEnable()
    {
        InteractableManager.onActiveInteractableChange += OnInteractableChange;
    }

    private void OnDisable()
    {
        InteractableManager.onActiveInteractableChange -= OnInteractableChange;
    }

    private void OnInteractableChange(Interactable newInteractable, Interactable oldInteractable)
    {
        /*if (newInteractable == null && containsPlayer && oldInteractable != this)
        {
            InteractableManager.instance.SetActiveInteractable(this, false);
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            containsPlayer = true;
            if (canInteract)
            {
                InteractableManager.instance.SetActiveInteractable(this, false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            containsPlayer = false;
            if (canInteract)
            {
                InteractableManager.instance.SetActiveInteractable(this, true);
            }
        }
    }

    public void OnInteract()
    {
        onInteract?.Invoke();
    }

    public void SetInteractPrompt(bool active)
    {
        interactPrompt.SetActive(active);
    }

    public void SetCanInteract(bool can)
    {
        canInteract = can;

        if (!canInteract)
        {
            InteractableManager.instance.SetActiveInteractable(this, true);
        } else if (containsPlayer)
        {
            InteractableManager.instance.SetActiveInteractable(this, false);
        }
    }
}
