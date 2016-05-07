using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float moveSpeed ;
    private Rigidbody rb;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();

    }
	
	// Update is called once per frame
	void Update () {
        HandleInputs();	
	}

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + rb.velocity * Time.fixedDeltaTime);
    }

    void HandleInputs()
    {
        rb.velocity = new Vector3(-Input.GetAxisRaw("Horizontal"), 0, -Input.GetAxisRaw("Vertical")).normalized * moveSpeed;
    }
}
