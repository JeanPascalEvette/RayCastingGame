using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    private GameObject player;

	// Use this for initialization
	void Start () {
        if (player == null)
            player = GameObject.Find("Player");

    }
	
	// Update is called once per frame
	void Update () {
	    
	}
}
