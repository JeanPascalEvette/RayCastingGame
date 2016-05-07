using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class RayCasting
{
    static GameObject[] defaultColliders = VisionCollider.GetListVisionObjects();

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 endPoint;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _endPoint, float _distance, float _angle) { hit = _hit;  endPoint = _endPoint; distance = _distance; angle = _angle; }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB) { pointA = _pointA; pointB = _pointB; }
    }

    public static ViewCastInfo ViewCast(Transform startObj, Vector3 dir, float length, float pollingFrequency)
    {
        float numSteps = length / pollingFrequency;
        Vector3 previousPos = startObj.position;
        Vector3 currentPos = startObj.position;
        Transform col;
        for (int i = 0; i < (int)(numSteps); i++)
        {
            previousPos = currentPos;
            currentPos += dir * (length / numSteps);
            col = TestCollision(currentPos, defaultColliders);
            if (col != null && col != startObj)
            {
                return new ViewCastInfo(true, currentPos, (length / numSteps) * i, 0f);
            }
        }
        return new ViewCastInfo(false, startObj.position + dir * length, length, 0f);
    }

    public static bool CheckRay(Transform startObj, Transform endObj, float pollingFrequency)
    {
        Vector3 direction = (endObj.position - startObj.position);
        float numSteps = direction.magnitude / pollingFrequency;
        Vector3 previousPos = startObj.position;
        Vector3 currentPos = startObj.position;
        Transform col;
        for (int i = 0; i < (int)(numSteps); i++)
        {
            previousPos = currentPos;
            currentPos += direction * (1.0f / numSteps);
            col = TestCollision(currentPos, defaultColliders);
            if (col != null && col != startObj && col != endObj)
            {
                return false;
            }
        }
        return true;

    }

    public static Vector3[] CastRay(Vector3 startPost, Vector3 endPos, float pollingFrequency, out GameObject visionBreaker, GameObject[] listColliders = null)
    {
        visionBreaker = null;
        if (listColliders == null)
            listColliders = defaultColliders;
        List<Vector3> hits = new List<Vector3>();
        Vector3 direction = (endPos - startPost);
        float numSteps = direction.magnitude / pollingFrequency;
        Vector3 previousPos = startPost;
        Vector3 currentPos = startPost;
        Transform col;
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
                {
                    visionBreaker = col.gameObject;
                    break;
                }
            }
            else
            {
                Debug.DrawLine(previousPos, currentPos, Color.green);
            }
        }

        return hits.ToArray();
    }


    public static Transform TestCollision(Vector3 position, GameObject[] listColliders)
    {
        foreach(GameObject collider in listColliders)
        {
            var bounds = collider.GetComponent<Collider>().bounds;
            bool checkX = position.x < bounds.max.x && position.x > bounds.min.x;
            bool checkY = position.y < bounds.max.y && position.y > bounds.min.y;
            bool checkZ = position.z < bounds.max.z && position.z > bounds.min.z;
            if (checkX && checkY && checkZ)
                return collider.transform;
        }
        return null;
    }
}

