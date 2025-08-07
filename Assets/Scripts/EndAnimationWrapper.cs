using UnityEngine;

public class EndAnimationWrapper : MonoBehaviour
{
    public void OnAnimationOver()
    {
        GameManager.instance.OnEndAnimationOver();
    }
}
