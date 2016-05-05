using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class RayCasting
{
    static GameObject[] defaultColliders = VisionCollider.GetListVisionObjects();

    public static Vector3[] CastRay(Vector3 startPost, Vector3 endPos, float pollingFrequency, GameObject[] listColliders = null)
    {
        if (listColliders == null)
            listColliders = defaultColliders;
        List<Vector3> hits = new List<Vector3>();
        Vector3 direction = (endPos - startPost);
        float numSteps = direction.magnitude / pollingFrequency;
        Vector3 previousPos = startPost;
        Vector3 currentPos = startPost;
        GameObject col;
        for (int i = 0; i < (int)(numSteps); i++)
        {
            previousPos = currentPos;
            currentPos += direction * (1.0f/numSteps);
            col = TestCollision(currentPos, listColliders);
            if (col != null)
            {
                hits.Add(currentPos);
                Debug.DrawLine(previousPos, currentPos, Color.red);
                var vc = col.GetComponent<VisionCollider>();
                if (vc != null && !vc.isSeeThrough())
                    break;
                
            }
            else
            {
                Debug.DrawLine(previousPos, currentPos, Color.green);
            }
        }

        return hits.ToArray();
    }


    public static GameObject TestCollision(Vector3 position, GameObject[] listColliders)
    {
        foreach(GameObject collider in listColliders)
        {
            var bounds = collider.GetComponent<Collider>().bounds;
            bool checkX = position.x < bounds.max.x && position.x > bounds.min.x;
            bool checkY = position.y < bounds.max.y && position.y > bounds.min.y;
            bool checkZ = position.z < bounds.max.z && position.z > bounds.min.z;
            if (checkX && checkY && checkZ)
                return collider;
        }
        return null;
    }
}

