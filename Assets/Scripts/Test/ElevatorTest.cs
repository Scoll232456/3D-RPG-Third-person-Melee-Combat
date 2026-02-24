using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorTest : MonoBehaviour
{
    Rigidbody rigidbody;
    float speed = 2f;
    Vector3 dir = Vector3.up;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.y > 20)
        { return; }
        var NewPos = transform.position + dir * speed * Time.fixedDeltaTime;
        rigidbody.MovePosition(NewPos);

    }
}
