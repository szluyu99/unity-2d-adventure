using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 野猪巡逻状态
public class BoarPatrolState : BaseState
{

    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
        currentEnemy.anim.SetBool("isWalk", true);
    }

    public override void LogicUpdate()
    {
        // 发现 player, 切换到追逐状态
        if (currentEnemy.DetectPlayer())
        {
            currentEnemy.SwitchState(EnemyState.Chase);
        }

        if (!currentEnemy.physicsCheck.isGround || // 走到悬崖边缘
            (currentEnemy.faceDir.x < 0 && currentEnemy.physicsCheck.touchLeftWall) || // 走到左边墙壁
            (currentEnemy.faceDir.x > 0 && currentEnemy.physicsCheck.touchRightWall)) // 走到右边墙壁
        {
            currentEnemy.isWait = true;
            currentEnemy.anim.SetBool("isWalk", false);
        }
        else
        {
            currentEnemy.anim.SetBool("isWalk", true);
        }

    }

    public override void PhysicsUpdate()
    {
    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("isWalk", false);
        Debug.Log("BoarPatrolState OnExit");
    }

}
