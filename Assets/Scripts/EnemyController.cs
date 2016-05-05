using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    private GameObject player;

    [SerializeField]
    private float rotateSpeed = 1f;
    [SerializeField]
    private float visionLength = 10.0f;
    [SerializeField]
    private float pollingFrequency = 0.01f;


    private bool playerInSight = false;

    private Material redMaterial;
    private Material greenMaterial;
    // Use this for initialization
    void Start () {
        if (player == null)
            player = GameObject.Find("Player");
    redMaterial = (Material)Resources.Load("Materials/Red");
    greenMaterial = (Material)Resources.Load("Materials/Green");

}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(0, Time.deltaTime * rotateSpeed, 0));
        DrawVision();
    }

    void DrawVision()
    {
        playerInSight = false;
        CastRay(30);
        CastRay(25);
        CastRay(20);
        CastRay(15);
        CastRay(10);
        CastRay(5);
        CastRay(0);
        CastRay(5);
        CastRay(10);
        CastRay(15);
        CastRay(20);
        CastRay(25);
        CastRay(30);
        if (playerInSight)
            GetComponent<Renderer>().sharedMaterial = redMaterial;
        else
            GetComponent<Renderer>().sharedMaterial = greenMaterial;
    }


    void CastRay(float rotation)
    {
        GameObject visionBreaker = null;
        RayCasting.CastRay(transform.position, transform.position + Quaternion.Euler(0, rotation, 0) * transform.forward * visionLength, pollingFrequency, out visionBreaker);

        if (visionBreaker != null && visionBreaker.tag == "Player")
            playerInSight = true;
    }
}
