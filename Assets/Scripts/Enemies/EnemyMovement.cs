using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] [Range(5, 100)] private int triggerAtDepth = 5;
    [SerializeField] [Range(-180, 180)] private float degreeStart = 0;
    [SerializeField] [Min(0)] private float speedVariation = 1;
    [SerializeField] private float movementSpeed = 3;

    private float verticalSpeed = 1f;
    private bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        movementSpeed = movementSpeed + Random.Range(-speedVariation, speedVariation);
        
        transform.rotation = Quaternion.Euler(0f, 0f, degreeStart);

        var verticalCameraSpeed = Hub.Get<CameraMovement>().GetVerticalSpeed();

        verticalSpeed = -1 * (verticalCameraSpeed / movementSpeed);

    }

    // Update is called once per frame
    void Update()
    {
        var currentSwarmDepth = Hub.Get<DepthController>().GetSwarmDepthMeters();

        if (!started && currentSwarmDepth < triggerAtDepth)
        {
            return;
        }

        started = true;

        var pos = transform.position;

        // Movement on cos curve?
        
        Vector3 diff = transform.up;

        Vector3 dir = new Vector3(0, verticalSpeed, 0);

        transform.position += (diff + dir) * Time.deltaTime * movementSpeed;

        outOfBounds();
    }

    void outOfBounds()
    {
        var x = transform.position.x;
        var y = transform.position.y;

        if (x < -10 || x > 10 || y > 10 || y < -100)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
