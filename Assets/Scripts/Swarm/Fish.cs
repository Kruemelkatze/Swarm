using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Fish : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform lookAtTransform;

    [SerializeField] private Transform splitTargetTransform;
    [SerializeField] private Transform splitLookAtTransform;

    [SerializeField] private Vector3 relativeTargetPosition;
    [SerializeField] private float movementSpeed = 3;
    [SerializeField] private float rotationSpeed = 5;

    [SerializeField] [Min(0)] private float speedVariation = 1;

    public int index;

    private Swarm _swarm;

    // Start is called before the first frame update
    void Start()
    {
        movementSpeed = movementSpeed + Random.Range(-speedVariation, speedVariation);
        _swarm = Hub.Get<Swarm>();
    }

    // Update is called once per frame
    void Update()
    {
        var pos = transform.position;

        var effectiveTarget = _swarm.IsSplit ? splitTargetTransform : targetTransform;

        // Pos
        var targetPosition =
            effectiveTarget.position +
            effectiveTarget.rotation * Vector3.Scale(relativeTargetPosition, _swarm.GetStretchFactor());
        transform.position = Vector3.Lerp(pos, targetPosition, Time.deltaTime * movementSpeed);

        // Rot
        //var diff = lookAtTransform.position - pos;
        var diff = transform.position - pos;
        diff.Normalize();
        float rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        var targetRotation = Quaternion.Euler(0f, 0f, rotZ - 90);

        //transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, Time.deltaTime * rotationSpeed);
        transform.rotation = targetRotation;
    }

    public void SetColor(Color c)
    {
        GetComponent<SpriteRenderer>().color = c;
    }

    public void SimplyRemove()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void SetTarget(Transform targetTransform, Transform lookAtTransform, Transform splitTargetTransform,
        Transform splitLookAtTransform, Vector3 relativeTargetPosition)
    {
        this.targetTransform = targetTransform;
        this.relativeTargetPosition = relativeTargetPosition;
        this.lookAtTransform = lookAtTransform;
        this.splitTargetTransform = splitTargetTransform;
        this.splitLookAtTransform = splitLookAtTransform;
    }

    public void Eaten()
    {
        SimplyRemove();
    }
}