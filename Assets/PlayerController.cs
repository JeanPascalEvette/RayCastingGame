using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float moveSpeed;
    private Rigidbody rb;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();

    }
	
	// Update is called once per frame
	void Update () {
        HandleInputs();	
	}

    void HandleInputs()
    {
        Vector3 vel = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            vel.z = -moveSpeed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            vel.z = moveSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            vel.x = moveSpeed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            vel.x = -moveSpeed;
        }
        rb.velocity = vel;
    }
}
