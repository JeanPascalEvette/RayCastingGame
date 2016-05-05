using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    private GameObject player;

    [SerializeField]
    private float rotateSpeed = 1f;

    // Use this for initialization
    void Start () {
        if (player == null)
            player = GameObject.Find("Player");

    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(0, Time.deltaTime * rotateSpeed, 0));
        RayCasting.CastRay(transform.position, transform.position + transform.forward * 10.0f, 0.01f);
	}
}
