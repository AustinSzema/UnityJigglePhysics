using System;
using UnityEngine;

public class Pizza : MonoBehaviour
{
    private float spinSpeed = 100f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, Time.deltaTime * spinSpeed, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }

    }
}
