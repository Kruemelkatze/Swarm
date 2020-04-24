using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothRandomMovement : MonoBehaviour
{
    [SerializeField] private Vector3 currentTarget;
    private Vector3 _velocity;
    public float smoothTime = 0.3f;

    public Vector2 randomRange = new Vector2(0.5f, 0.5f);
    public float distanceToSwitch = 0.05f;

    public bool enabledMovement = true;

    // Use this for initialization
    void Start()
    {
        currentTarget = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!enabledMovement) 
            return;
        
        if (Vector3.Distance(transform.position, currentTarget) < distanceToSwitch)
        {
            Vector2 random = Random.insideUnitCircle * randomRange;
            currentTarget = new Vector3(random.x, random.y, currentTarget.z);
        }

        transform.position = Vector3.SmoothDamp(transform.position, currentTarget, ref _velocity, smoothTime);
    }
}