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
        public VisionCollider vcFound;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _endPoint, float _distance, float _angle, VisionCollider _vcFound) { hit = _hit;  endPoint = _endPoint; distance = _distance; angle = _angle; vcFound = _vcFound; }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB) { pointA = _pointA; pointB = _pointB; }
    }

    public static ViewCastInfo ViewCast(Vector3 startPos, Vector3 dir, float length, float pollingFrequency, VisionCollider currentMedium = null)
    {
        ViewCastInfo hitFound = new ViewCastInfo(false, startPos + dir * length, length, 0f, null);
        float minDist = length;
        foreach(var collider in defaultColliders)
        {
            if (currentMedium != null && collider.GetComponent<VisionCollider>() != null && currentMedium == collider.GetComponent<VisionCollider>())
                continue;

            var vecIntersection = Vector3.zero;
            var flFraction = 0.0f;


            var startPoint = startPos;
            var endPoint = (startPos + dir * length);

            var minPt = (collider.GetComponent<Renderer>().bounds.min);
            var maxPt = (collider.GetComponent<Renderer>().bounds.max);

            if (LineAABBIntersection(minPt, maxPt, startPoint, endPoint, ref vecIntersection, ref flFraction))
        {
                if (minDist > flFraction * length)
            {
                    minDist = flFraction * length;
                    hitFound = new ViewCastInfo(true, vecIntersection, flFraction * length, 0f, collider.GetComponent<VisionCollider>());
                }
            }
        }
        return hitFound;


        
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


    static bool ClipLine(int d, Vector3 minPt, Vector3 maxPt, Vector3 v0, Vector3 v1, ref float f_low, ref float f_high)
    {
	    // f_low and f_high are the results from all clipping so far. We'll write our results back out to those parameters.

	    // f_dim_low and f_dim_high are the results we're calculating for this current dimension.
	    float f_dim_low, f_dim_high;

        // Find the point of intersection in this dimension only as a fraction of the total vector http://youtu.be/USjbg5QXk3g?t=3m12s
        f_dim_low = (minPt[d] - v0[d])/(v1[d] - v0[d]);
	    f_dim_high = (maxPt[d] - v0[d])/(v1[d] - v0[d]);

        // Make sure low is less than high
        if (f_dim_high < f_dim_low)
        {
            var temp = f_dim_high;
            f_dim_high = f_dim_low;
            f_dim_low = temp;
        }

	    // If this dimension's high is less than the low we got then we definitely missed. http://youtu.be/USjbg5QXk3g?t=7m16s
	    if (f_dim_high<f_low)
		    return false;

	    // Likewise if the low is less than the high.
	    if (f_dim_low > f_high)
		    return false;

	    // Add the clip from this dimension to the previous results http://youtu.be/USjbg5QXk3g?t=5m32s
	    f_low = Mathf.Max(f_dim_low, f_low);
        f_high = Mathf.Min(f_dim_high, f_high);

	    if (f_low > f_high)
		    return false;

        return true;
    }

    // Find the intersection of a line from v0 to v1 and an axis-aligned bounding box http://www.youtube.com/watch?v=USjbg5QXk3g
    static bool LineAABBIntersection(Vector3 minPt, Vector3 maxPt, Vector3 v0, Vector3 v1, ref Vector3 vecIntersection, ref float flFraction)
    {
	    float f_low = 0;
        float f_high = 1;

	    if (!ClipLine(0, minPt, maxPt, v0, v1, ref f_low, ref f_high))
		    return false;

	    if (!ClipLine(1, minPt, maxPt, v0, v1, ref f_low, ref f_high))
		    return false;

	    if (!ClipLine(2, minPt, maxPt, v0, v1, ref f_low, ref f_high))
		    return false;

	    // The formula for I: http://youtu.be/USjbg5QXk3g?t=6m24s
	    Vector3 b = v1 - v0;
        vecIntersection = v0 + b* f_low;

        flFraction = f_low;

	    return true;
    }

    // Work in here
    public static Transform TestCollision(Vector3 position, GameObject[] listColliders)
    {
        foreach(GameObject collider in listColliders)
        {
            if(collider.GetComponent<Collider>().bounds.Contains(position))
                return collider.transform;
        }
        return null;
    }
}

