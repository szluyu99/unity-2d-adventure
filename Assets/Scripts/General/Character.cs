using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("基本属性")]
    public float maxHealth; // 最大血量
    public float currentHealth; // 当前血量

    [Header("受伤无敌")]
    public float invulnerableDuration; // 受伤无敌时间
    public float invulnerableCounter; // 受伤无敌计数器
    public bool invulnerable; // 是否受伤无敌

    // 在 Unity 面板中可以给事件挂载一系列的函数
    public UnityEvent<Transform> OnTakeDamage; // 受伤事件, 依赖于攻击者的方向
    public UnityEvent OnDie; // 死亡事件
    public UnityEvent<Character> OnHealthChange; // 血量改变事件

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChange?.Invoke(this); // 触发血量改变要执行的事件
    }

    private void Update()
    {
        CheckInvulnerable();
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Water")) 
        {
            currentHealth = 0;
            OnHealthChange?.Invoke(this); // 触发血量改变要执行的事件
            OnDie?.Invoke(); // 触发死亡要执行的事件
        }
    }

    // 受到来自 attacker 的攻击
    public void TakeDamage(Attack attacker)
    {
        if (invulnerable) return;

        if (currentHealth > attacker.damage)
        {
            currentHealth -= attacker.damage;
            TriggerInvulnerable();
            OnTakeDamage?.Invoke(attacker.transform); // 触发受伤后要执行的事件
        }
        else
        {
            currentHealth = 0;
            OnDie?.Invoke(); // 触发死亡要执行的事件
        }

        OnHealthChange?.Invoke(this); // 触发血量改变要执行的事件
    }

    /// <summary>
    /// 检测无敌时间
    /// </summary>
    private void CheckInvulnerable()
    {
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }
    }

    /// <summary>
    /// 触发受伤无敌
    /// </summary>
    private void TriggerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration; // 重置计数器
        }
    }
}
