using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

/*
生命周期: awake -> enable -> start -> physicsUpdate -> update -> fixedUpdate -> diable -> destroy
使用 public 定义的变量的可以在 Unity 面板中进行赋值
*/
public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb; // 刚体
    [HideInInspector] public CapsuleCollider2D coll; // 胶囊碰撞体
    [HideInInspector] public PhysicsCheck physicsCheck;
    [HideInInspector] public PlayerAnimation playerAnimation;
    [HideInInspector] public PlayerInputController inputControl;
    public Vector2 inputDirection; // 输入方向

    [Header("基本参数")]
    public float speed; // 移动速度
    public float jumpForce; // 跳跃力度
    public float hurtForce; // 受伤力度
    public float accDelta; // 加速增量
    public float decDelta; // 减速增量

    // 根据一些参数计算出来的状态
    [Header("状态")]
    public bool isCrouch; // 是否蹲下
    public bool isHurt;
    public bool isDead;
    public bool isAttack;

    [Header("物理材质")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    private float runSpeed; // 跑步速度（等于 speed）
    private Vector2 originalOffset; // 胶囊碰撞体原始偏移值
    private Vector2 originalSize; // 胶囊碰撞体原始大小
    private float walkSpeed => speed / 2.5f; // 每次都会执行

    #region 生命周期函数
    private void Awake()
    {
        inputControl = new PlayerInputController();

        // 初始化组件变量的方法
        // 方法1: 在 Unity 界面中将 RigidBody2D 拖给 Script 的 rb 变量
        // 方法2: 在脚本中通过 GetComponent
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        playerAnimation = GetComponent<PlayerAnimation>();

        // 记录胶囊碰撞体的原始偏移值和大小, 用于蹲下时修改碰撞体大小和位移
        originalOffset = coll.offset;
        originalSize = coll.size;

        // 监听跳跃事件
        inputControl.Player.Jump.performed += Jump;

        // 监听攻击事件
        inputControl.Player.Attack.performed += PlayerAttack;

        #region 监听 shift 强制走路
        runSpeed = speed;
        // 按下 Shift 键时走路（将速度变为 walkSpeed)
        inputControl.Player.WalkButton.performed += ctx =>
        {
            if (physicsCheck.isGround) speed = walkSpeed;
        };
        // 松开 Shift 键时跑步 (将速度变为 runSpeed)
        inputControl.Player.WalkButton.canceled += ctx =>
        {
            if (physicsCheck.isGround) speed = runSpeed;
        };
        #endregion
    }

    private void Update()
    {
        // 播放完攻击动画后才能移动
        if (isAttack) return;

        inputDirection = inputControl.Player.Move.ReadValue<Vector2>();
        // CheckMaterial();
    }

    private void FixedUpdate()
    {
        if (isHurt) return;
        Move();
        CheckCrouch();
    }

    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }
    #endregion

    // 检测是否蹲下, 并修改碰撞体大小和位移 
    public void CheckCrouch()
    {
        isCrouch = inputDirection.y < -0.1f && physicsCheck.isGround;
        if (isCrouch)
        {
            // 修改碰撞体大小和位移
            coll.offset = new Vector2(-0.05f, 0.85f);
            coll.size = new Vector2(0.7f, 1.7f);
            rb.velocity = Vector2.zero; // 没有下蹲移动的动画, 所以下蹲时不允许移动
        }
        else
        {
            // 还原之前碰撞参数
            coll.size = originalSize;
            coll.offset = originalOffset;
        }
    }

    public void Move()
    {
        // 翻转
        int faceDirection = (int)transform.localScale.x; // 面朝方向
        if (inputDirection.x > 0) faceDirection = 1; // 正向
        if (inputDirection.x < 0) faceDirection = -1; // 反向
        transform.localScale = new Vector3(faceDirection, 1, 1);

        // 移动
        if (!isCrouch && !isAttack)
        {
            // 匀速移动
            rb.velocity = new Vector2(speed * inputDirection.x, rb.velocity.y);

            // 平滑移动（有加速度和减速度）
            // if (inputDirection.x != 0)
            // {
            //     rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x + inputDirection.x * accDelta, -speed, speed), rb.velocity.y);
            // }
            // else
            // {
            //     rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, 0, decDelta), rb.velocity.y);
            // }
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!physicsCheck.isGround) return;
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void PlayerAttack(InputAction.CallbackContext context)
    {
        // if (!physicsCheck.isGround) return;
        isAttack = true;
        playerAnimation.PlayAttack();
        rb.velocity = new Vector2(0, rb.velocity.y); // 攻击时停止移动
    }

    #region UnityEvent
    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.velocity = Vector2.zero;
        // 伤害来源在右边，向左弹开，反之亦然
        Vector2 dir = new Vector2(transform.position.x - attacker.position.x, 0).normalized;
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }

    public void PlayerDead()
    {
        isDead = true;
        inputControl.Player.Disable();
    }
    #endregion

    // blueHurt Event
    public void StopHurt()
    {
        isHurt = false;
    }

    // 平常使用有摩擦力的物理材质，攀爬时使用无摩擦力的物理材质
    // TODO: 存在问题，按方向键贴着墙跳的时候有摩擦力，跳不高
    // private void CheckMaterial()
    // {
    // coll.sharedMaterial = physicsCheck.isGround ? normal : wall;
    // }
}
