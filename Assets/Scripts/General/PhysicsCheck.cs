using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    public CapsuleCollider2D coll;

    [Header("检测参数")]
    public bool manual; // 是否手动设置碰撞体左右侧偏移
    public Vector2 bottomOffset; // 底部偏移
    public Vector2 leftOffset; // 左侧偏移
    public Vector2 rightOffset; // 右侧偏移
    public float checkRadius; // 检测范围
    public LayerMask groundLayer; // 地面层级


    [Header("状态")]
    public bool isGround; // 是否在地面上
    public bool touchLeftWall; // 撞左墙
    public bool touchRightWall; // 撞右墙

    private void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();

        if (!manual)
        {
            leftOffset = new Vector2(-coll.size.x / 2 + coll.offset.x, coll.bounds.size.y / 2);
            rightOffset = new Vector2(coll.size.x / 2 + coll.offset.x, coll.bounds.size.y / 2);
        }
    }

    private void Update()
    {
        Check();
    }

    // 绘制检测范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(leftOffset.x, leftOffset.y), checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(rightOffset.x, leftOffset.y), checkRadius);
    }

    public void Check()
    {
        // 检测地面: 需要乘上方向 localScale.x, 使得翻转时偏移量也跟着翻转
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRadius, groundLayer);
        // 墙体判断
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(leftOffset.x, leftOffset.y), checkRadius, groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(rightOffset.x, rightOffset.y), checkRadius, groundLayer);
    }
}
