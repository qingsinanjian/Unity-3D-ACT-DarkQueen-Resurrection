using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// °µÓ°Ä§·¨Çò
/// </summary>
public class ShadowProjectilMega : MonoBehaviour
{
    public float moveSpeed;

    private void Start()
    {
        Destroy(gameObject, 10);
    }

    private void Update()
    {
        transform.Translate(transform.forward * moveSpeed * Time.deltaTime);
    }
}
