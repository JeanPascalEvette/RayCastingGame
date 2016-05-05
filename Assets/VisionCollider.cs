using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisionCollider : MonoBehaviour {

    private static List<GameObject> ListVisionobjects;

    [SerializeField]
    private bool seeThrough = false;
	// Use this for initialization
	void Start () {
        if (ListVisionobjects == null)
            ListVisionobjects = new List<GameObject>();
        if (!ListVisionobjects.Contains(gameObject))
            ListVisionobjects.Add(gameObject);
	}

    public static GameObject[] GetListVisionObjects()
    {
        return ListVisionobjects.ToArray();
    }

    public bool isSeeThrough()
    {
        return seeThrough;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
