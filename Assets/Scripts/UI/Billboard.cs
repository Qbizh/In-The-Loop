using UnityEngine;

public class Billboard : MonoBehaviour
{
    private void Update()
    {
        var cam = Camera.main;

        Vector3 dir = (transform.position - cam.transform.position).normalized;
        transform.forward = dir;

        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 45f, transform.rotation.eulerAngles.z));
    }
}
