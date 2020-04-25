using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{

    [SerializeField] private Transform targetTransform;
    [SerializeField] private Vector3 relativeTargetPosition;
    [SerializeField] private float movementSpeed = 3;
    [SerializeField] private float rotationSpeed = 5;

    public int index;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // var diff = relativeTargetPosition - transform.localPosition;
        // diff.Normalize();
        // float rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        // var targetRotation = Quaternion.Euler(0f, 0f, rotZ - 90);
        var targetPosition = 
            targetTransform.position +
            targetTransform.rotation * relativeTargetPosition; 
        //Vector3.Lerp(targetPosition, transform.position, movementSpeed * Time.deltaTime);
        //transform.rotation = Quaternion.Slerp(transform.rotation, TargetPosition.transform.rotation, rotationSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * movementSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, Time.deltaTime * 100);
        
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

    public void SetTargetPosition(Transform targetTransform, Vector3 relativeTargetPosition)
    {
        this.targetTransform = targetTransform;
        this.relativeTargetPosition = relativeTargetPosition;
    }

    public void Eaten()
    {
        SimplyRemove();
    }
}