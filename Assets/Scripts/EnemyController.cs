using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    private GameObject player;

    [SerializeField]
    private float rotateSpeed = 1f;

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
    }
    
    
}
