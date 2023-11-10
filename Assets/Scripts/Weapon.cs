using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int damageValue;
    public PlayerController owner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController.CanExecute())
            {
                int random = Random.Range(1, 4);
                owner.Execute(random);
                playerController.BeExectued(random, owner.transform);
            }
            else
            {
                playerController.TakeDamage(damageValue, other.ClosestPoint(transform.position));
            }
        }
    }
}
