using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusBar : MonoBehaviour
{
    public Image healthImage; // 血条
    public Image healthDelayImage; // 延迟血条
    public Image powerImage; // 蓝条

    private void Update()
    {
        // 实现血条延迟减少的效果
        if (healthDelayImage.fillAmount > healthImage.fillAmount)
        {
            healthDelayImage.fillAmount -= Time.deltaTime * 0.3f;
        }
    }

    /// <summary>
    /// 设置 Health 的当前血量百分比
    /// </summary>
    /// <param name="percentage">血量百分比: currentHeath / maxHealth </param>
    public void SetHealth(float percentage)
    {
        healthImage.fillAmount = percentage;
    }
}
