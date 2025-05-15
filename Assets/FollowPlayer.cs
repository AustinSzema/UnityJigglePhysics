using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform targetLookat;
    
    // Update is called once per frame
    void Update()
    {
        transform.position = targetLookat.position;
    }
}
