using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float inputH;
    private float inputV;
    private Rigidbody rigid;

    public float moveSpeed;
    public float rotateSpeed;
    //private Vector3 inputDir;

    private bool isGround = true;
    public float jumpForce;

    private Animator animator;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        inputH = Input.GetAxis("Horizontal");
        inputV = Input.GetAxis("Vertical");
        if(inputH != 0 && inputV != 0)
        {
            float targetRotation = rotateSpeed * inputH;
            transform.eulerAngles = Vector3.up * Mathf.Lerp(transform.eulerAngles.y, transform.eulerAngles.y + targetRotation, Time.deltaTime);
        }
        //inputDir = new Vector3(inputH, 0, inputV);

        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            rigid.AddForce(Vector3.up * jumpForce);
            isGround = false;
            animator.SetBool("IsGround", isGround);
            animator.CrossFade("Jump", 0.1f);
        }
    }

    private void FixedUpdate()
    {
        //if (inputH != 0 || inputV != 0)
        //{
        //    rigid.MovePosition(transform.position + transform.TransformDirection(inputDir) * Time.deltaTime * moveSpeed);
        //}

        if(inputV != 0)
        {
            rigid.MovePosition(transform.position + transform.forward * Time.deltaTime * moveSpeed * inputV);
            //animator.SetBool("Move", true);
            animator.SetFloat("MoveState", Mathf.Abs(inputV));
        }
        else
        {
            if(inputH != 0)
            {
                rigid.MovePosition(transform.position + transform.right * Time.deltaTime * moveSpeed * inputH);
                //animator.SetBool("Move", true);
                animator.SetFloat("MoveState", Mathf.Abs(inputH));
            }
            else
            {
                //animator.SetBool("Move", false);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isGround)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGround = true;
                animator.SetBool("IsGround", isGround);
            }
        }
    }
}
