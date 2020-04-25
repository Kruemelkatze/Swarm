using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{

    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Vector3 targetPosition;
    

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetColor(Color c)
    {
        _spriteRenderer.color = c;
    }

    public void SimplyRemove()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void SetTargetPosition(Vector3 transformPosition)
    {
        targetPosition = targetPosition;
    }
}