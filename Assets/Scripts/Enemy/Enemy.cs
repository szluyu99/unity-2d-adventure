using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // [HideInInspector] 将在 Unity 面板中隐藏该变量
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public PhysicsCheck physicsCheck;

    [Header("基本参数")]
    public float normalSpeed;
    public float chaseSpeed;
    public float currentSpeed;
    public Vector3 faceDir; // 朝向
    public float hurtForce; // 受伤力度
    public float hurtWaitTime; // 受伤等待时间
    // public Transform attacker; // 攻击者

    [Header("撞墙等待")]
    public float waitTime; // 等待时间
    public float waitTimeCounter; // 等待时间计数器

    [Header("状态")]
    public bool isWait; // 是否等待
    public bool isHurt;
    public bool isDead;

    [Header("检测")]
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;
    public float lostTime; // 失去目标时间
    public float lostTimeCounter; // 失去目标时间计数器

    // protected 修饰符，外部无法访问, 子类可以访问
    // 状态机
    protected BaseState patrolState; // 巡逻状态
    protected BaseState chaseState; // 追逐状态
    private BaseState currentState; // 当前状态

    #region 生命周期
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();

        currentSpeed = normalSpeed;
        waitTimeCounter = waitTime;
        lostTimeCounter = lostTime;
    }

    private void OnEnable()
    {
        // 默认是巡逻状态
        currentState = patrolState;
        currentState.OnEnter(this);
    }

    private void Update()
    {
        // 获取当前朝向
        faceDir = new Vector3(-transform.localScale.x, 0, 0);

        currentState.LogicUpdate();
        TimeCounter();
    }

    private void FixedUpdate()
    {
        if (!isHurt && !isDead && !isWait) Move();

        currentState.PhysicsUpdate();
    }

    private void OnDisable()
    {
        currentState.OnExit();
    }
    #endregion

    // 加上 virtual 关键字，子类可以重写该方法
    public virtual void Move()
    {
        rb.velocity = new Vector2(currentSpeed * faceDir.x, rb.velocity.y);
    }

    // 掉转方向
    public void TurnDirection()
    {
        transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
        // 物体翻转时碰撞体不会翻转，偏移量也不会翻转，需要加上补偿值
        if (faceDir.x < 0)
        {
            physicsCheck.leftOffset.x += 0.4f;
            physicsCheck.rightOffset.x += 0.4f;
        }
        else
        {
            physicsCheck.leftOffset.x -= 0.4f;
            physicsCheck.rightOffset.x -= 0.4f;
        }
    }

    public void TimeCounter()
    {
        if (isWait)
        {
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0)
            {
                isWait = false;
                waitTimeCounter = waitTime;
                TurnDirection();
            }
        }

        if (DetectPlayer())
        {
            lostTimeCounter = lostTime; // 重置丢失时间计数器
        }
        else if (lostTimeCounter > 0)
        {
            lostTimeCounter -= Time.deltaTime;
        }
    }

    // 是否探测到前方的 attackLayer 层级的物体
    public bool DetectPlayer()
    {
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0, faceDir, checkDistance, attackLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset + new Vector3(-transform.localScale.x * checkDistance, 0), 0.2f);
    }

    public void SwitchState(EnemyState state)
    {
        BaseState newState = state switch
        {
            EnemyState.Patrol => patrolState,
            EnemyState.Chase => chaseState,
            _ => null
        };
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
    }

    #region 事件执行
    public void OnTakeDamage(Transform attacker)
    {
        // this.attacker = attacker;
        // 转身
        if (attacker.position.x - transform.position.x > 0) // 攻击者在右边
            transform.localScale = new Vector3(-1, 1, 1); // 默认面朝左, 受伤后面朝右
        if (attacker.position.x - transform.position.x < 0) // 攻击者在左边
            transform.localScale = new Vector3(1, 1, 1);

        isHurt = true;
        anim.SetTrigger("hurt");
        Vector2 dir = new Vector2(transform.position.x - attacker.position.x, 0).normalized; // 攻击者的方向
        rb.velocity = new Vector2(0, rb.velocity.y);
        StartCoroutine(OnHurt(dir)); // 开启协程
    }

    // 协程返回一个 IEnumerator 对象
    private IEnumerator OnHurt(Vector2 attackDir)
    {
        rb.AddForce(attackDir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(hurtWaitTime); // 等待固定时间
        isHurt = false;
        // 受伤后重置等待时间
        isWait = false;
        waitTimeCounter = waitTime;
    }

    public void OnDie()
    {
        rb.velocity = Vector2.zero;
        isDead = true;
        anim.SetBool("isDead", true);
        // 取消碰撞体, 防止死亡后还能攻击到人物（也可以采用修改图层的方式）
        GetComponent<CapsuleCollider2D>().enabled = false;
    }

    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }
    #endregion
}

