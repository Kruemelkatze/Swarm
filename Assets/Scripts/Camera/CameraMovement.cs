using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] [Range(-1, 5)] private float verticalSpeed = 1f;

    public bool maxTopForTopOfViewport = true;
    public Transform maxTop;
    public bool maxBottomForBottomOfViewport = true;
    public Transform maxBottom;

    [SerializeField] private float depthPercentage = 0;

    void Awake()
    {
        Hub.Register(this);
    }

    void Start()
    {
        cam = cam ? cam : Camera.main;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var lCam = cam ? cam : Camera.main;
        var p = lCam.ViewportToWorldPoint(new Vector2(0.5f, 1));
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(p, 0.1F);

        p = lCam.ViewportToWorldPoint(new Vector2(0.5f, 0));
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(p, 0.1F);
    }
#endif


    void Update()
    {
        var pos = transform.position;

        var viewPortTop = cam.ViewportToWorldPoint(Vector2.up);
        var verticalDiff = viewPortTop.y - pos.y;
        float topRef = 0, bottomRef = 0;

        var nextY = pos.y - verticalSpeed * Time.deltaTime;
        if (maxTop)
        {
            topRef = maxTop.position.y - (maxTopForTopOfViewport ? verticalDiff : 0);
            nextY = Mathf.Min(nextY, topRef);
        }

        if (maxBottom)
        {
            bottomRef = maxBottom.position.y + (maxBottomForBottomOfViewport ? verticalDiff : 0);
            nextY = Mathf.Max(nextY, bottomRef);
        }

        if (maxTop && maxBottom)
        {
            var length = topRef - bottomRef;
            if (length > 0)
            {
                var distance = topRef - nextY;
                depthPercentage = Mathf.Abs(distance / length);
            }
            else
            {
                depthPercentage = 0;
            }
        }

        transform.position = new Vector3(pos.x, nextY, pos.z);
    }

    public float GetDepthPercentage() => depthPercentage;
}