using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Biomech_Common
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
    public bool canGetPlayerInputValue = true;
    public bool canMove = true;
    public bool canJump = true;
    public bool canAttack = true;
    public bool canEquip = true;
    public bool canUseSkill = true;

    #region 事件函数
    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        transfigurationEffect.Stop();
        transfigurationEffect.gameObject.SetActive(false);
    }

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        if (!canGetPlayerInputValue) return;
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
                jumpCount = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            Debug.Log("受到2点伤害");
            TakeDamage(2, other.ClosestPoint(transform.position));
        }
    }
    #endregion

    /// <summary>
    /// 刀客状态下捡起刀
    /// </summary>
    public void HasNewBlade()
    {
        equipBladeGo.SetActive(true);
        hasBlade = true;
        animator.CrossFade("EquipState", 0.1f);
        animator.SetBool("Equip", true);
        ifEquip = true;
    }

    #region 移动与跳跃
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
            //animator.SetBool("Move", true);
            animator.SetFloat("InputH", 0);
            //animator.SetFloat("MoveState", Mathf.Abs(inputV));
            animator.SetFloat("InputV", inputV);
        }
        else
        {
            if (inputH != 0)
            {
                rigid.MovePosition(transform.position + transform.right * Time.deltaTime * moveSpeed * inputH);
                //animator.SetBool("Move", true);
                //animator.SetFloat("MoveState", Mathf.Abs(inputH));
                animator.SetFloat("InputH", inputH);
            }
            else
            {
                //animator.SetBool("Move", false);
                animator.SetFloat("InputH", 0);
                animator.SetFloat("InputV", 0);
            }
        }
    }

    /// <summary>
    /// 玩家移动输入
    /// </summary>
    private void PlayerMoveInput()
    {
        if (!canMove)
        {
            inputH = inputV = 0;
            return;
        }
        if (isRunning)
        {
            moveScale = 3;
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveScale = 1;
            }
            else
            {
                moveScale = 2;
            }
        }

        if (Input.GetButtonDown("Vertical"))
        {
            if (Time.time - timeLost < 0.5f && DoubleForward())
            {
                isRunning = true;
            }
            timeLost = Time.time;
        }

        if (Input.GetButtonUp("Vertical"))
        {
            isRunning = false;
        }

        inputH = Input.GetAxis("Horizontal") * moveScale;
        inputV = Input.GetAxis("Vertical") * moveScale;
        if (inputH != 0 && inputV != 0)
        {
            float targetRotation = rotateSpeed * inputH;
            transform.eulerAngles = Vector3.up * Mathf.Lerp(transform.eulerAngles.y, transform.eulerAngles.y + targetRotation, Time.deltaTime);
        }
        if (inputV != 0 || (inputV == 0 && inputH != 0))
        {
            animator.SetFloat("MoveState", 1);
        }
        else
        {
            animator.SetFloat("MoveState", 0);
        }
    }

    /// <summary>
    /// 跳跃
    /// </summary>
    private void PlayerJumpInput()
    {
        if (!canJump)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount <= jumpNum)
        {
            if (isGround)
            {
                isGround = false;
                animator.SetBool("IsGround", isGround);
            }
            rigid.AddForce(Vector3.up * jumpForce * (jumpCount * 0.3f + 1));
            if (jumpCount == 0)
            {
                animator.CrossFade("Jump", 0.1f);
            }
            else
            {
                animator.CrossFade("DoubleJump", 0.1f);
            }
            jumpCount++;
            //if (ifEquip)
            //{
            //    animator.CrossFade("JumpB", 0.1f);
            //}
            //else
            //{
            //    animator.CrossFade("JumpA", 0.1f);
            //}
        }
    }

    /// <summary>
    /// 判断玩家是否在规定时间内按下两次向前按键
    /// </summary>
    /// <returns></returns>
    private bool DoubleForward()
    {
        return inputV > 0 && Input.GetAxis("Vertical") > 0 && currentState != State.Master;
    }
    #endregion

    #region 玩家输入
    /// <summary>
    /// 玩家输入
    /// </summary>
    private void PlayerInput()
    {
        if (!canGetPlayerInputValue) return;
        PlayerMoveInput();
        PlayerEquipInput();
        PlayerAttckInput();
        PlayerJumpInput();
        PlayerSkillInput();
    }

    /// <summary>
    /// 玩家攻击输入
    /// </summary>
    private void PlayerAttckInput()
    {
        if (!canAttack) return;
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
    /// 检测玩家的技能输入
    /// </summary>
    private void PlayerSkillInput()
    {
        if (!canUseSkill) return;
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
        if (currentState == State.Blademan || currentState == State.Swordman)
        {
            if (!ifEquip)
            {
                return;
            }
        }

        for (int i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                animator.CrossFade("Skill" + i, 0.1f);
            }
        }
    }

    /// <summary>
    /// 封锁玩家所有的输入内容
    /// </summary>
    public void UnLockAll()
    {
        canGetPlayerInputValue = true;
        canMove = true;
        canJump = true;
        canAttack = true;
        canEquip = true;
        canUseSkill = true;
    }
    #endregion

    //public void TestAnimationEvent(AnimationEvent animationEvent)
    //{
    //    Debug.Log("Int：" + animationEvent.intParameter);
    //    Debug.Log("Float：" + animationEvent.floatParameter);
    //    Debug.Log("String：" + animationEvent.stringParameter);
    //    Debug.Log("Object：" + animationEvent.objectReferenceParameter);
    //    Debug.Log("FunctionName：" + animationEvent.functionName);
    //}

    #region 变身
    /// <summary>
    /// 玩家切换装备输入及切换职业
    /// </summary>
    private void PlayerEquipInput()
    {
        if (!canEquip) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentState == State.Blademan)
            {
                if (hasBlade)
                {
                    ifEquip = !ifEquip;
                    animator.SetBool("Equip", ifEquip);
                }
            }
            else if (currentState == State.Swordman)
            {
                ifEquip = !ifEquip;
                animator.SetBool("Equip", ifEquip);
                if (ifEquip)//A转B
                {
                    animator.CrossFade("EquipSword", 0.1f);
                }
                //SetAnimationPlaySpeed(-1);
            }
            else if (currentState == State.Assassin)
            {
                ifEquip = !ifEquip;
                animator.SetBool("Equip", ifEquip);
            }
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            animator.CrossFade("Transfiguration", 0.1f);
        }
    }
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
        if (System.Convert.ToInt32(currentState) > 3)
        {
            currentState = State.Master;
        }
        ResetState();
        switch (currentState)
        {
            case State.Master:
                ChangeMaterials(masterMaterials);
                animator.runtimeAnimatorController = masterRA;
                ShowBall(1);
                jumpNum = 0;
                break;
            case State.Blademan:
                ChangeMaterials(blademanMaterials);
                animator.runtimeAnimatorController = blademanRA;
                if (hasBlade)
                {
                    ShowOrHideUnEquipBladeGo(1);
                }
                break;
            case State.Swordman:
                ChangeMaterials(swordmanMaterials);
                animator.runtimeAnimatorController = swordmanRA;
                ShowOrHideSword(true);
                break;
            case State.Assassin:
                ChangeMaterials(assassinMaterials);
                animator.runtimeAnimatorController = assassinRA;
                ShowOrHideUnEquipDaggers_N(0, true);
                ShowOrHideUnEquipDaggers_N(1, true);
                break;
            default:
                break;
        }
        canGetPlayerInputValue = true;
        ifEquip = false;
        animator.SetBool("Equip", false);
    }

    /// <summary>
    /// 重置所有状态下的武器
    /// </summary>
    private void ResetState()
    {
        ifEquip = false;
        HideBall(0);
        HideBall(1);
        ShowOrHideEquipBladeGo(0);
        ShowOrHideUnEquipBladeGo(0);
        ShowOrHideSword(false);
        jumpNum = 1;
        ShowOrHideUnEquipDaggers_N(0, false);
        ShowOrHideUnEquipDaggers_N(1, false);
        ShowOrHideEquipDaggers_N(0, false);
        ShowOrHideEquipDaggers_N(1, false);
    }

    private void ChangeMaterials(Material[] materials)
    {
        for (int i = 0; i < materials.Length; i++)
        {
            sr.materials[i].CopyPropertiesFromMaterial(materials[i]);
        }
    }
    #endregion

    #region 受到伤害
    private void TakeDamage(float damageValue, Vector3 hitPos)
    {
        float x = Vector3.Dot(transform.right, hitPos);
        float y = Vector3.Dot(transform.forward, hitPos - transform.position);
        animator.SetTrigger("Hit");
        if (ForwardBehindOrLeftRight(hitPos))
        {
            if(y > 0)
            {
                Debug.Log("在前方");
                animator.SetFloat("HitY", 1);
            }
            else
            {
                Debug.Log("在后方");
                animator.SetFloat("HitY", -1);
            }
        }
        else
        {
            if (x > 0)
            {
                Debug.Log("在右方");
                animator.SetFloat("HitX", 1);
            }
            else
            {
                Debug.Log("在左方");
                animator.SetFloat("HitX", -1);
            }
        }
    }

    /// <summary>
    /// 判断前后左右影响度（true前后影响更大,false左右影响更大）
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    private bool ForwardBehindOrLeftRight(Vector3 targetPos)
    {
        float ZDistance = Mathf.Abs(transform.position.z - targetPos.z);
        float XDistance = Mathf.Abs(transform.position.x - targetPos.x);
        if(ZDistance >= XDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #endregion

    #region Biomech_Master
    [Header("*************Biomech_Master*************")]
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

    public int moveScale = 1;
    private float timeLost;
    private bool isRunning;
    private int jumpCount;
    public int jumpNum;

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
    private void PlaySlashParticals(float angle)
    {
        Instantiate(slashEffectGo, transform.position + new Vector3(transform.forward.x, transform.forward.y + 1.3f, transform.forward.z * 1.3f), Quaternion.Euler(transform.rotation.eulerAngles - new Vector3(0, 0, angle)));
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
    [Header("*************Biomech_Blademan*************")]
    public Material[] blademanMaterials;
    public RuntimeAnimatorController blademanRA;
    public bool ifEquip;
    public GameObject equipBladeGo;
    public GameObject unEquipBladeGo;
    public bool startCombo = false;
    public ParticleSystem leftHitBallPS;
    public ParticleSystem rightHitBallPS;
    public GameObject bladeGo;
    public GameObject darkBladeGo;
    public ParticleSystem changeBladeEffect;
    public bool hasBlade;

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
        animator.ResetTrigger("AttackCombo");
    }

    private void PlayHitBallEffect(int isLeft)
    {
        if (isLeft == 1)
        {
            leftHitBallPS.Play();

        }
        else
        {
            rightHitBallPS.Play(true);
        }
    }

    private void StopHitBallPS(int isLeft)
    {
        if (isLeft == 1)
        {
            leftHitBallPS.Stop();

        }
        else
        {
            rightHitBallPS.Stop();
        }
    }

    private void ShowOrHideBlade(AnimationEvent animationEvent)
    {
        if(animationEvent.intParameter == 0)
        {
            bladeGo.SetActive(System.Convert.ToBoolean(animationEvent.stringParameter));
        }
        else
        {
            darkBladeGo.SetActive(System.Convert.ToBoolean(animationEvent.stringParameter));
        }
    }

    private void PlayChangeBladeEffect()
    {
        changeBladeEffect.Play();
    }

    #endregion

    #region Biomech_Swordman
    [Header("*************Biomech_Swordman*************")]
    public Material[] swordmanMaterials;
    public RuntimeAnimatorController swordmanRA;
    public ParticleSystem swordEffect;
    public GameObject swordGo;

    private void ShowOrHideSword(bool show)
    {
        swordGo.SetActive(show);
    }

    private void PlaySwordEffect()
    {
        swordEffect.Play();
    }
    #endregion

    #region Biomech_Assassin
    [Header("*************Biomech_Assassin*************")]
    public Material[] assassinMaterials;
    public RuntimeAnimatorController assassinRA;
    public GameObject ueLeftDagger;
    public GameObject ueRightDagger;
    public GameObject leftDagger;
    public GameObject rightDagger;

    /// <summary>
    /// 显示与隐藏腰间的匕首
    /// </summary>
    /// <param name="handID">0为右手，1为左手</param>
    /// <param name="show"></param>
    private void ShowOrHideUnEquipDaggers_N(int handID, bool show)
    {
        if (handID == 0)
        {
            //右手
            ueRightDagger.SetActive(show);
        }
        else
        {
            //左手
            ueLeftDagger.SetActive(show);
        }
    }

    private void ShowOrHideUnEquipDaggers(AnimationEvent animationEvent)
    {
        if(animationEvent.intParameter == 0)
        {
            //右手
            ueRightDagger.SetActive(System.Convert.ToBoolean(animationEvent.stringParameter));
        }
        else
        {
            //左手
            ueLeftDagger.SetActive(System.Convert.ToBoolean(animationEvent.stringParameter));
        }
    }

    /// <summary>
    /// 显示与隐藏手上的匕首
    /// </summary>
    /// <param name="handID">0为右手，1为左手</param>
    /// <param name="show"></param>
    private void ShowOrHideEquipDaggers_N(int handID, bool show)
    {
        if (handID == 0)
        {
            //右手
            rightDagger.SetActive(show);
        }
        else
        {
            //左手
            leftDagger.SetActive(show);
        }
    }

    private void ShowOrHideEquipDaggers(AnimationEvent animationEvent)
    {
        if (animationEvent.intParameter == 0)
        {
            //右手
            rightDagger.SetActive(System.Convert.ToBoolean(animationEvent.stringParameter));
        }
        else
        {
            //左手
            leftDagger.SetActive(System.Convert.ToBoolean(animationEvent.stringParameter));
        }
    }
    #endregion

    /// <summary>
    /// 设置某个动画状态的播放速度(此状态已经设置为受动画参数影响的状态)
    /// </summary>
    /// <param name="speed"></param>
    private void SetAnimationPlaySpeed(float speed)
    {
        animator.SetFloat("AnimationPlaySpeed", speed);
    }
}

public enum State
{
    Master,
    Blademan,
    Swordman,
    Assassin
}
