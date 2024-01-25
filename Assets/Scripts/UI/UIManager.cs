using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 用于管理 UI 和游戏逻辑之间的交互
public class UIManager : MonoBehaviour
{
    // 人物血条
    public PlayerStatusBar playerStatusBar;

    // [Header("事件监听")]
    // public CharacterEventSO healthEvent;

    // private void OnEnable()
    // {
    // healthEvent.OnEventRaised += OnHealthEvent;
    // }

    // private void OnDisable()
    // {
    // healthEvent.OnEventRaised -= OnHealthEvent;
    // }

    public void OnHealthEvent(Character character)
    {
        float percentage = character.currentHealth / character.maxHealth;
        playerStatusBar.SetHealth(percentage); // 更新血条
    }
}
