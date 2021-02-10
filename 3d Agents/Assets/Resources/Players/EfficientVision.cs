using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EfficientVision : MonoBehaviour
{
    public float visonDistance = 10f;

    public float reactionTime = 0.1f;
    private float Timer;

    public float[] Distances;
    public float[] types;
    public float[] angles;
    public float visionangle = 0.51f;
    

    public int objectMax = 3;



    public class DetectedObject: IComparable<DetectedObject>
    {
        public Vector3 ClosestPoint;
        public float Distance;
        public Collider collider;
        public Transform root;
        public float angle;

        public DetectedObject(Collider c, Vector3 pos)
        {
            collider = c;
            root = c.transform.root;
            ClosestPoint = c.ClosestPointOnBounds(pos);
            Distance = Vector3.Distance(ClosestPoint, pos);
        }

        public int CompareTo(DetectedObject other)
        {
            return this.Distance.CompareTo(other.Distance);
        }
    }





    // Start is called before the first frame update
    void Start()
    {
        Timer = 0;
        Distances = new float[objectMax];
        types = new float[objectMax];
        angles = new float[objectMax];

    }

    int getHighestIndex(float[] distances)
    {
        float Highest = 0;
        int index = 0;
        for(int i = 0; i < distances.Length; i++)
        {
            if (distances[i] == -1f)
            {
                return i;
            }
            else if (distances[i] > Highest)
            {
                Highest = distances[i];
                index = i;
            }
        }
        return index;
    }


    float getHighestValue(float[] distances)
    {
        float highest = 1000f;
        int index = 0;
        for (int i = 0; i < distances.Length; i++)
        {
            if (distances[i] == -1f)
            {
                return 1000f;
            }
            else if (distances[i] < highest)
            {
                highest = distances[i];
                index = i;
            }
        }
        return highest;
    }

    int containsRoot(Collider[] colliders, Collider collider)
    {
        for(int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                if (colliders[i].transform.root.GetInstanceID() == collider.transform.root.GetInstanceID())
                {
                    return i;
                }
            }
        }
        return -1;
    }

    int containsRoot(DetectedObject[] ClosestObjects, DetectedObject obj)
    {
        for (int i = 0; i < ClosestObjects.Length; i++)
        {
            if (ClosestObjects[i] != null)
            {
                if (ClosestObjects[i].root.gameObject.GetInstanceID() == obj.root.gameObject.GetInstanceID())
                {
                    return i;
                }
            }
        }
        return -1;
    }


    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        if (Timer >= reactionTime)
        {
            Timer = 0;
            Collider[] colliders = Physics.OverlapSphere(transform.position, visonDistance,LayerMask.GetMask("Player","Wall"));
            Collider[] closest = new Collider[objectMax];
            float[] distances = new float[objectMax];
            for(int i = 0; i< objectMax; i++)
            {
                distances[i] = -1f;
                Distances[i] = 0;
                angles[i] = 0;
                types[i] = 0;
            }

            DetectedObject[] objects = new DetectedObject[colliders.Length];
            DetectedObject[] closestObjects = new DetectedObject[objectMax];

            for (int i = 0; i < colliders.Length; i++)
            {
                DetectedObject obj = new DetectedObject(colliders[i], transform.position);
                objects[i] = obj;
            }

            Array.Sort(objects);

            int index = 0;
            foreach(DetectedObject obj in objects)
            {
                if (index == objectMax)
                {
                    break;
                }
                if (obj != null && obj.root.gameObject.GetInstanceID() != transform.root.gameObject.GetInstanceID())
                {
                    Vector3 dirTopoint = (transform.position - obj.ClosestPoint).normalized;
                    float angle = Vector3.SignedAngle(-transform.forward, dirTopoint, Vector3.up) / 180f;
                    if (Math.Abs(angle) < visionangle)
                    {
                        int contains = containsRoot(closestObjects, obj);
                        if (contains != -1) //obj is alread in closest objects
                        {
                            //ignore 
                        }
                        else
                        {
                            closestObjects[index] = obj;
                            index++;
                            //add to closest
                        }
                    }
                }
            }

            for (int i = 0; i < objectMax; i++)
            {
                if (closestObjects[i] != null)
                {
                    DetectedObject obj = closestObjects[i];
                    Distances[i] = 1 - obj.Distance / visonDistance;

                    Vector3 dirTopoint = (transform.position - obj.ClosestPoint).normalized;
                    angles[i] = Vector3.SignedAngle(-transform.forward, dirTopoint, Vector3.up) / 180f;
                    angles[i] = angles[i] / visionangle;

                    if (obj.collider.tag == "Head")
                    {
                        Debug.DrawLine(transform.position, obj.ClosestPoint, Color.blue, 0.1f);
                        types[i] = -1;
                        //  Debug.Log(" ", col);
                    }
                    else if (obj.collider.tag == "Body")
                    {
                        Debug.DrawLine(transform.position, obj.ClosestPoint, Color.blue, 0.1f);
                        types[i] = 1;
                        // Debug.Log(" ", col);
                    }
                    else
                    {
                        Debug.DrawLine(transform.position, obj.ClosestPoint, Color.green, 0.1f);
                        types[i] = 0;
                        //Debug.Log(" ", col);
                    }
                }
            }
        }
    }
}
