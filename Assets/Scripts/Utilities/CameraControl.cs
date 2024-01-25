using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
    // 相机震源
    public CinemachineImpulseSource impulseSource;
    [HideInInspector]public CinemachineConfiner2D confiner2D;

    private void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    // TODO: 场景切换后更改
    private void Start()
    {
        GetNewCameraBounds();
    }

    private void GetNewCameraBounds()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Bounds");
        if (obj == null) return;
        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();
        confiner2D.InvalidateCache();
    }

    public void ShakeCamera()
    {
        impulseSource.GenerateImpulse();
    }
}
