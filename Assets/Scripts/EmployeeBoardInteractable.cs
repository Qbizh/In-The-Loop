using UnityEngine;

public class EmployeeBoardInteractable : MonoBehaviour
{
    [SerializeField] GameObject employeeBoard;

    private void OnEnable()
    {
        GetComponent<Interactable>().onInteract += OnInteract;
    }

    private void OnInteract()
    {
        if (!employeeBoard.activeInHierarchy) 
        {
            employeeBoard.SetActive(true);
        }
    }
}
