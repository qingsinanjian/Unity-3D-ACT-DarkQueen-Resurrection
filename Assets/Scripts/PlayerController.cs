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

    //��Ӱħ����
    public GameObject shadowProjectileGo;
    public Transform leftHandTrans;
    public GameObject leftHandBall;
    public GameObject rightHandBall;
    //��Ӱն��
    public GameObject slashEffectGo;
    public Transform rightHandTrans;
    //��Ӱ���
    public GameObject cleaveEffectGo;
    //��Ӱ����
    public GameObject shadowShieldGo;
    //��Ӱ���
    public GameObject bigShadowProjectileGo;
    //������Ч
    public ParticleSystem transfigurationEffect;
    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        transfigurationEffect.Stop();
        transfigurationEffect.gameObject.SetActive(false);
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

        if (Input.GetKeyDown(KeyCode.C))
        {
            animator.CrossFade("Transfiguration", 0.1f);
        }

        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    animator.CrossFade("Skill1", 0.1f);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    animator.CrossFade("Skill2", 0.1f);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    animator.CrossFade("Skill3", 0.1f);
        //}
        GetPlayerSkillInput();
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
    //    Debug.Log("Int��" + animationEvent.intParameter);
    //    Debug.Log("Float��" + animationEvent.floatParameter);
    //    Debug.Log("String��" + animationEvent.stringParameter);
    //    Debug.Log("Object��" + animationEvent.objectReferenceParameter);
    //    Debug.Log("FunctionName��" + animationEvent.functionName);
    //}

    #region ��Ӱħ����
    private void CreateShadowProjectile(int isLeft)
    {
        if(isLeft == 1)
        {
            Instantiate(shadowProjectileGo, leftHandTrans.position, transform.rotation);
        }
        else
        {
            Instantiate(shadowProjectileGo, rightHandTrans.position, transform.rotation);
        }
    }

    private void ShowBall(int isLeft)
    {
        if (isLeft == 1)
        {
            leftHandBall.SetActive(true);

        }
        else
        {
            rightHandBall.SetActive(true);
        }
    }

    private void HideBall(int isLeft)
    {
        if (isLeft == 1)
        {
            leftHandBall.SetActive(false);

        }
        else
        {
            rightHandBall.SetActive(false);
        }
    }

    #endregion

    #region ��Ӱն
    private void PlaySlashParticals()
    {
        Instantiate(slashEffectGo, transform.position + new Vector3(transform.forward.x, transform.forward.y + 1.3f, transform.forward.z * 1.3f), transform.rotation);
    }

    #endregion

    #region ��Ӱ���

    private void PlayCleaveParticals()
    {
        Instantiate(cleaveEffectGo, transform.position + transform.forward, transform.rotation);
    }

    #endregion

    #region ��Ӱ����
    private void CreateShadowShield()
    {
        GameObject itemGo = Instantiate(shadowShieldGo, transform.position, transform.rotation);
        itemGo.transform.SetParent(transform);
    }
    #endregion

    private void GetPlayerSkillInput()
    {
        for (int i = 0; i < 10; i++)
        {
            if(Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                animator.CrossFade("Skill" + i, 0.1f);
            }
        }
    }

    #region ��Ӱ���
    private void CreateBigShadowProjectile()
    {
        Instantiate(bigShadowProjectileGo, leftHandTrans.position, transform.rotation);
    }
    #endregion

    #region ����
    private void PlayTransfigurationEffect(int show)
    {
        bool state = System.Convert.ToBoolean(show);
        if (state)
        {
            transfigurationEffect.gameObject.SetActive(true);
            transfigurationEffect.Play();
        }
        else
        {
            transfigurationEffect.Stop();
        }
    }
    #endregion
}
