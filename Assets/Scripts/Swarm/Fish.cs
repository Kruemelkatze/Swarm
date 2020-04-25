using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Fish : MonoBehaviour
{

    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform lookAtTransform;
    
    [SerializeField] private Vector3 relativeTargetPosition;
    [SerializeField] private float movementSpeed = 3;
    [SerializeField] private float rotationSpeed = 5;

    [SerializeField] [Min(0)] private float speedVariation = 1;
    
    public int index;

    public bool isEnabled = true;
    // Start is called before the first frame update
    void Start()
    {
        movementSpeed = movementSpeed + Random.Range(-speedVariation, speedVariation);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnabled)
        {
            return;
        }
        var pos = transform.position;
        
        // Pos
        var targetPosition = 
            targetTransform.position +
            targetTransform.rotation * relativeTargetPosition;
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

    public void SetTarget(Transform targetTransform, Vector3 relativeTargetPosition, Transform lookAtTransform)
    {
        this.targetTransform = targetTransform;
        this.relativeTargetPosition = relativeTargetPosition;
        this.lookAtTransform = lookAtTransform;
    }

    public void Eaten()
    {
        SimplyRemove();
    }
}