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

    //°µÓ°Ä§·¨Çò
    public GameObject shadowProjectileGo;
    public Transform leftHandTrans;
    public GameObject handBall;
    //°µÓ°Õ¶»÷
    public GameObject slashEffectGo;
    public Transform rightHandTrans;
    //°µÓ°³å»÷
    public GameObject cleaveEffectGo;

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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            animator.CrossFade("Skill1", 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            animator.CrossFade("Skill2", 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            animator.CrossFade("Skill3", 0.1f);
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
            animator.SetBool("Move", true);
            animator.SetFloat("InputH", 0);
            //animator.SetFloat("MoveState", Mathf.Abs(inputV));
            animator.SetFloat("InputV", inputV);
        }
        else
        {
            if(inputH != 0)
            {
                rigid.MovePosition(transform.position + transform.right * Time.deltaTime * moveSpeed * inputH);
                animator.SetBool("Move", true);
                //animator.SetFloat("MoveState", Mathf.Abs(inputH));
                animator.SetFloat("InputH", inputH);
            }
            else
            {
                animator.SetBool("Move", false);
                animator.SetFloat("InputH", 0);
                animator.SetFloat("InputV", 0);
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

    //public void TestAnimationEvent(AnimationEvent animationEvent)
    //{
    //    Debug.Log("Int£º" + animationEvent.intParameter);
    //    Debug.Log("Float£º" + animationEvent.floatParameter);
    //    Debug.Log("String£º" + animationEvent.stringParameter);
    //    Debug.Log("Object£º" + animationEvent.objectReferenceParameter);
    //    Debug.Log("FunctionName£º" + animationEvent.functionName);
    //}

    #region °µÓ°Ä§·¨Çò
    private void CreateShadowProjectile()
    {
        Instantiate(shadowProjectileGo, leftHandTrans.position, transform.rotation);
    }

    private void ShowBall()
    {
        handBall.SetActive(true);
    }

    private void HideBall()
    {
        handBall.SetActive(false);
    }

    #endregion

    #region °µÓ°Õ¶
    private void PlaySlashParticals()
    {
        Instantiate(slashEffectGo, transform.position + new Vector3(transform.forward.x, transform.forward.y + 1.3f, transform.forward.z * 1.3f), transform.rotation);
    }

    #endregion

    #region °µÓ°³å»÷

    private void PlayCleaveParticals()
    {
        Instantiate(cleaveEffectGo, transform.position + transform.forward, transform.rotation);
    }

    #endregion
}
