using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class Swarm : MonoBehaviour
{
    [SerializeField] [Range(0.1f, 5)] private float speed = 1;
    [SerializeField] [Range(0, 2)] private float rotationDuration = 0.3f;
    [SerializeField] [Range(1, 4)] private int controlSharpness = 1;
    [SerializeField] [Min(0)] private float idleAngularSpeed;
    [SerializeField] private Vector3 stretchFactor = new Vector3(0.8f, 1.1f, 1);
    [SerializeField] private Vector3 stretchOffset = new Vector3(1,1,0);

    private Vector3 _effectiveStretchFactor = Vector3.one;
    
    private Rigidbody2D _rigidbody2D;
    //private TweenerCore<Quaternion, Quaternion, NoOptions> _rotationTweener;
    private TweenerCore<Quaternion, Vector3, QuaternionOptions> _rotationTweener;

    void Awake()
    {
        Hub.Register<Swarm>(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");

        var movementDirection = Vector2.ClampMagnitude(new Vector2(
            Mathf.Abs(Mathf.Pow(x, controlSharpness)) * Mathf.Sign(x),
            Mathf.Abs(Mathf.Pow(y, controlSharpness)) * Mathf.Sign(y)), 1);
        _rigidbody2D.velocity = movementDirection * speed;
        
        if(_rotationTweener != null && _rotationTweener.IsActive())
        {
            _rotationTweener.Kill();
        }
        
        if (new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg - 90f;
            var rot = Quaternion.AngleAxis(angle, Vector3.forward);

            var vec3End = new Vector3(rot.eulerAngles.x, rot.eulerAngles.y, rot.eulerAngles.z);

            _rotationTweener = transform.DORotate(vec3End, rotationDuration, RotateMode.Fast);

            //transform.rotation = rot;
            // if (_rotationTweener == null)
            // {

            //_rotationTweener = transform.DORotateQuaternion(rot, rotationDuration);
            // }
            // else
            // {
            //     _rotationTweener = _rotationTweener.ChangeEndValue(rot, rotationDuration);
            // }
            _effectiveStretchFactor = stretchFactor;
        }
        else
        {
            // Idle
            transform.Rotate(0,0, idleAngularSpeed * Time.deltaTime);
            _effectiveStretchFactor = Vector3.one;
        }
    }

    public Vector3 GetStretchFactor() => _effectiveStretchFactor;
    public Vector3 GetStretchOffset() => stretchOffset;

}   