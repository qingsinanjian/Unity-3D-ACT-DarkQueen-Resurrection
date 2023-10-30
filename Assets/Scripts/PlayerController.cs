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

    public State currentState;
    public SkinnedMeshRenderer sr;
    public GameObject transEffect;

    #region Biomech_Master
    //暗影魔法球
    public GameObject shadowProjectileGo;
    public Transform leftHandTrans;
    public GameObject leftHandBall;
    public GameObject rightHandBall;
    //暗影斩击
    public GameObject slashEffectGo;
    public Transform rightHandTrans;
    //暗影冲击
    public GameObject cleaveEffectGo;
    //暗影护盾
    public GameObject shadowShieldGo;
    //暗影轰击
    public GameObject bigShadowProjectileGo;
    //变身特效
    public ParticleSystem transfigurationEffect;
    public Material[] masterMaterials;
    public RuntimeAnimatorController masterRA;

    #region 暗影魔法球
    private void CreateShadowProjectile(int isLeft)
    {
        if (isLeft == 1)
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

    #region 暗影斩
    private void PlaySlashParticals()
    {
        Instantiate(slashEffectGo, transform.position + new Vector3(transform.forward.x, transform.forward.y + 1.3f, transform.forward.z * 1.3f), transform.rotation);
    }

    #endregion

    #region 暗影冲击

    private void PlayCleaveParticals()
    {
        Instantiate(cleaveEffectGo, transform.position + transform.forward, transform.rotation);
    }

    #endregion

    #region 暗影护盾
    private void CreateShadowShield()
    {
        GameObject itemGo = Instantiate(shadowShieldGo, transform.position, transform.rotation);
        itemGo.transform.SetParent(transform);
    }
    #endregion

    #region 暗影轰击
    private void CreateBigShadowProjectile()
    {
        Instantiate(bigShadowProjectileGo, leftHandTrans.position, transform.rotation);
    }
    #endregion

    #endregion

    #region Biomech_Blademan
    public Material[] blademanMaterials;
    public RuntimeAnimatorController blademanRA;
    public bool ifEquip;
    public GameObject equipBladeGo;
    public GameObject unEquipBladeGo;
    private bool startCombo;

    private void ShowOrHideEquipBladeGo(int show)
    {
        bool showState = System.Convert.ToBoolean(show);
        equipBladeGo.SetActive(showState);
    }

    private void ShowOrHideUnEquipBladeGo(int show)
    {
        bool showState = System.Convert.ToBoolean(show);
        unEquipBladeGo.SetActive(showState);
    }

    /// <summary>
    /// 关闭进入攻击一的入口
    /// </summary>
    private void EndAttackState()
    {
        animator.SetBool("Attack", false);
        startCombo = true;
    }

    /// <summary>
    /// 关闭连击
    /// </summary>
    private void EndComboState()
    {
        startCombo = false;
    }

    #endregion

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        transfigurationEffect.Stop();
        transfigurationEffect.gameObject.SetActive(false);
    }

    /// <summary>
    /// 玩家输入
    /// </summary>
    private void PlayerInput()
    {
        inputH = Input.GetAxis("Horizontal");
        inputV = Input.GetAxis("Vertical");
        if (inputH != 0 && inputV != 0)
        {
            float targetRotation = rotateSpeed * inputH;
            transform.eulerAngles = Vector3.up * Mathf.Lerp(transform.eulerAngles.y, transform.eulerAngles.y + targetRotation, Time.deltaTime);
        }
        //inputDir = new Vector3(inputH, 0, inputV);

        if (Input.GetKeyDown(KeyCode.C))
        {
            animator.CrossFade("Transfiguration", 0.1f);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ifEquip = !ifEquip;
            animator.SetBool("Equip", ifEquip);
        }

        if (ifEquip)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!startCombo)
                {
                    animator.SetBool("Attack", true);
                }
                else
                {
                    animator.SetTrigger("AttackCombo");
                }
            }
        }
    }

    /// <summary>
    /// 跳跃
    /// </summary>
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            rigid.AddForce(Vector3.up * jumpForce);
            isGround = false;
            animator.SetBool("IsGround", isGround);
            if (ifEquip)
            {
                animator.CrossFade("JumpB", 0.1f);
            }
            else
            {
                animator.CrossFade("JumpA", 0.1f);
            }
        }

    }

    private void Update()
    {
        PlayerInput();
        Jump();
        GetPlayerSkillInput();
    }

    /// <summary>
    /// 移动
    /// </summary>
    private void Move()
    {
        //if (inputH != 0 || inputV != 0)
        //{
        //    rigid.MovePosition(transform.position + transform.TransformDirection(inputDir) * Time.deltaTime * moveSpeed);
        //}

        if (inputV != 0)
        {
            rigid.MovePosition(transform.position + transform.forward * Time.deltaTime * moveSpeed * inputV);
            animator.SetBool("Move", true);
            animator.SetFloat("InputH", 0);
            //animator.SetFloat("MoveState", Mathf.Abs(inputV));
            animator.SetFloat("InputV", inputV);
        }
        else
        {
            if (inputH != 0)
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

    private void FixedUpdate()
    {
        Move();
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
    //    Debug.Log("Int：" + animationEvent.intParameter);
    //    Debug.Log("Float：" + animationEvent.floatParameter);
    //    Debug.Log("String：" + animationEvent.stringParameter);
    //    Debug.Log("Object：" + animationEvent.objectReferenceParameter);
    //    Debug.Log("FunctionName：" + animationEvent.functionName);
    //}

    private void GetPlayerSkillInput()
    {
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

        if(currentState == State.Blademan)
        {
            if (!ifEquip)
            {
                return;
            }
        }

        for (int i = 0; i < 10; i++)
        {
            if(Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                animator.CrossFade("Skill" + i, 0.1f);
            }
        }
    }

    #region 变身
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
            Instantiate(transEffect, transform.position, transEffect.transform.rotation);
        }
    }

    private void ChangeStateProperties()
    {
        currentState++;
        if(System.Convert.ToInt32(currentState) > 1)
        {
            currentState = State.Master;
        }
        switch (currentState)
        {
            case State.Master:
                ChangeMaterials(masterMaterials);
                animator.runtimeAnimatorController = masterRA;
                break;
            case State.Blademan:
                ChangeMaterials(blademanMaterials);
                animator.runtimeAnimatorController = blademanRA;
                HideBall(0);
                HideBall(1);
                break;
            case State.Swordman:
                break;
            case State.Assassin:
                break;
            default:
                break;
        }
    }

    private void ChangeMaterials(Material[] materials)
    {
        for (int i = 0; i < materials.Length; i++)
        {
            sr.materials[i].CopyPropertiesFromMaterial(materials[i]);
        }
    }
    #endregion
}

public enum State
{
    Master,
    Blademan,
    Swordman,
    Assassin
}
