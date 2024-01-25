using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage; // 攻击伤害
    public float attackRange; // 攻击范围
    public float attackRate; // 攻击速率

    // other 是被攻击对象的碰撞体
    private void OnTriggerStay2D(Collider2D other)
    {
        // this 是攻击者, 将攻击者的信息传递给被攻击者
        other.GetComponent<Character>()?.TakeDamage(this);
    }
}
