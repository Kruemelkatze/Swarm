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
        var nextY = pos.y - verticalSpeed * Time.deltaTime;
        if (maxTop)
        {
            if (maxTopForTopOfViewport)
            {
                var p = cam.ViewportToWorldPoint(Vector2.up);
                var diff = p.y - pos.y;
                nextY = Mathf.Min(nextY, maxTop.position.y - diff);
            }
            else
            {
                nextY = Mathf.Min(nextY, maxTop.position.y);
            }
        }

        if (maxBottom)
        {
            if (maxBottomForBottomOfViewport)
            {
                var p = cam.ViewportToWorldPoint(Vector2.zero);
                var diff = p.y - pos.y;
                nextY = Mathf.Max(nextY, maxBottom.position.y - diff);
            }
            else
            {
                // y is negative!
                nextY = Mathf.Max(nextY, maxBottom.position.y);
            }
        }

        transform.position = new Vector3(pos.x, nextY, pos.z);
    }
}