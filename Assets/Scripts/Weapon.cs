using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int damageValue;
    public PlayerController owner;
    public bool ifDestroy;
    public float destroyTime;
    public GameObject effectGo;

    private void Start()
    {
        if (destroyTime > 0)
        {
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController.CanExecute() && owner.currentState == State.Assassin)
            {
                int random = Random.Range(1, 5);
                owner.Execute(random);
                playerController.BeExectued(random, owner.transform);
            }
            else
            {
                playerController.TakeDamage(damageValue, other.ClosestPoint(transform.position));
            }
            if (ifDestroy)
            {
                if(effectGo)
                {
                    Instantiate(effectGo, other.ClosestPoint(transform.position), Quaternion.identity);
                }
                Destroy(this.gameObject);
            }
        }
    }
}
