using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarChaseState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        currentEnemy.anim.SetBool("isRun", true);
    }

    public override void LogicUpdate()
    {
        // 目标丢失则进入巡逻状态
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(EnemyState.Patrol);
        }

        if (!currentEnemy.physicsCheck.isGround || // 走到悬崖边缘
        (currentEnemy.faceDir.x < 0 && currentEnemy.physicsCheck.touchLeftWall) || // 走到左边墙壁
        (currentEnemy.faceDir.x > 0 && currentEnemy.physicsCheck.touchRightWall)) // 走到右边墙壁
        {
            currentEnemy.TurnDirection();
        }
    }

    public override void PhysicsUpdate()
    {
    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("isRun", false);
    }

}
