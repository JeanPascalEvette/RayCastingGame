using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour {


    public float viewRadius;

    [Range(0,360)]
    public float viewAngle;

    GameObject listObstacles;
    GameObject player;
    
    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDistanceThreshold;
    public float pollingFrequency;

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;


    public List<Transform> VisibleTargets = new List<Transform>();

    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";

        if(viewMeshFilter == null)
        {
            GameObject meshChild = new GameObject("View Visualisation");
            meshChild.transform.parent = transform;
            meshChild.transform.localPosition = Vector3.zero;
            meshChild.transform.localRotation = Quaternion.identity;
            var vmf = meshChild.AddComponent<MeshFilter>();
            var mr = meshChild.AddComponent<MeshRenderer>();
            mr.receiveShadows = false;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.sharedMaterial = (Material)Resources.Load("Materials/LoS");
            viewMeshFilter = vmf;
        }

        viewMeshFilter.mesh = viewMesh;



        listObstacles = GameObject.Find("Obstacles");
        player = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(FindTargetsWithDelay(.2f));
    }

    void LateUpdate()
    {
        DrawFieldOfView();
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;


        List<Vector3> viewPoints = new List<Vector3>();
        RayCasting.ViewCastInfo oldViewCast = new RayCasting.ViewCastInfo();
        for(int i = 0; i <= stepCount; i ++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            RayCasting.ViewCastInfo newViewCast = ViewCast(angle);

            if(i>0)
            {
                bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
                {
                    RayCasting.EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                        viewPoints.Add(edge.pointA);
                    if (edge.pointB != Vector3.zero)
                        viewPoints.Add(edge.pointB);
                }

            }

            viewPoints.Add(newViewCast.endPoint);
            //Debug.DrawLine(transform.position, newViewCast.endPoint, Color.blue);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    RayCasting.EdgeInfo FindEdge(RayCasting.ViewCastInfo minViewCast, RayCasting.ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2.0f;
            RayCasting.ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.endPoint;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.endPoint;
            }

        }
        return new RayCasting.EdgeInfo(minPoint, maxPoint);
    }

    RayCasting.ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RayCasting.ViewCastInfo vci = RayCasting.ViewCast(transform, dir, viewRadius, pollingFrequency);
        vci.angle = globalAngle;
        return vci;
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        VisibleTargets.Clear();
        Collider[] targetsInRange = GetListOfTargetsInRange();
        foreach (Collider target in targetsInRange)
        {
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                if (RayCasting.CheckRay(transform, target.transform, pollingFrequency))
                    VisibleTargets.Add(target.transform);
            }
        }
    }


    Collider[] GetListOfTargetsInRange()
    {
        List<Collider> targetsInRange = new List<Collider>();
        /*for (int i = 0; i < listObstacles.transform.childCount; i++)
        {
            if (Vector3.Distance(transform.position, listObstacles.transform.GetChild(i).position) < viewRadius)
            {
                 targetsInRange.Add(listObstacles.transform.GetChild(i).GetComponent<Collider>());
            }
        }
        */
        if (Vector3.Distance(transform.position, player.transform.position) < viewRadius)
        {
            targetsInRange.Add(player.GetComponent<Collider>());
        }
        return targetsInRange.ToArray();
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool isAngleGlobal)
    {
        if (!isAngleGlobal)
            angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
       
}
