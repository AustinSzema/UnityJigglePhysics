using System;
using UnityEngine;

public class PlayerShadow : MonoBehaviour
{

    [SerializeField] private GameObject shadow;
    [SerializeField] private LayerMask groundLayer;

    private void Start()
    {
        shadow.transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, groundLayer)) { 
            Debug.DrawLine (transform.position, hit.point, Color.cyan);
            
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.gameObject)
            {
                shadow.transform.position = hit.collider.ClosestPoint(transform.position) - new Vector3(0f, 0f, 0.8f);
            }
        }
    }
}
