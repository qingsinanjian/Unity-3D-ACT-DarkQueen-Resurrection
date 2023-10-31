using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeItem : MonoBehaviour
{
    public float rotateSpeed = 90;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * rotateSpeed, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Biomech")
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if(pc.currentState == State.Blademan)
            {
                pc.HasNewBlade();
                Destroy(this.gameObject);
            }
        }
    }
}
