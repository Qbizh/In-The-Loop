using UnityEngine;

public class EmployeeBoardInteractable : MonoBehaviour
{
    [SerializeField] EmployeeBoard boardLogic;

    [SerializeField] GameObject docsButton;

    private void OnEnable()
    {
        GetComponent<Interactable>().onInteract += OnInteract;
    }

    private void OnInteract()
    {
        docsButton.SetActive(false);

        boardLogic.OnBoardOpen();
    }
}
