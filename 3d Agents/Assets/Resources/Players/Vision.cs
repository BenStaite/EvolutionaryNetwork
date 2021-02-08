using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour
{
    public float visonDistance;

    public float reactionTime = 0.1f;
    private float Timer;

    public int Rays;
    public float[] Distances;
    public float[] isHeads;
    public float[] isBodys;
    public float[] isWalls;

    // Start is called before the first frame update
    void Start()
    {
        Rays = 24;
        Distances = new float[Rays];
        isHeads = new float[Rays];
        isBodys = new float[Rays];
        isWalls = new float[Rays];
        Timer = 0;
    }


    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        if (Timer >= reactionTime)
        {
            Timer = 0;
            for (int i = 0; i < Rays; i++)
            {
                RaycastHit ray;
                float angle = (i * 15f);
                float length = (Mathf.Abs(180 - angle)/180)+0.4f;
                Vector3 dir = Vector3.Normalize(Quaternion.AngleAxis(angle, Vector3.up) * transform.forward);
                if (Physics.Raycast(transform.position, dir, out ray, visonDistance*length))
                {
                    if (ray.collider.transform.parent != null)
                    {
                        if (ray.collider.transform.parent.GetInstanceID() != transform.parent.GetInstanceID())
                        {
                            Distances[i] = ray.distance / visonDistance * length;
                            Debug.DrawRay(transform.position, dir * ray.distance, Color.blue, reactionTime);
                            if (ray.collider.gameObject.tag == "Head")
                            {
                                isHeads[i] = 1f;
                                isWalls[i] = 0f;
                                isBodys[i] = 0f;
                            }
                            else if (ray.collider.gameObject.tag == "Body")
                            {
                                isBodys[i] = 1f;
                                isHeads[i] = 0f;
                                isWalls[i] = 0f;
                            }
                            else
                            {
                                isWalls[i] = 1f;
                                isBodys[i] = 0f;
                                isHeads[i] = 0f;
                            }
                        }
                        else
                        {
                            isBodys[i] = 0f;
                            isWalls[i] = 0f;
                            isHeads[i] = 0f;
                            Distances[i] = 0f;
                            Debug.DrawRay(transform.position, dir * (visonDistance * length), Color.red, reactionTime);
                        }
                    }
                }
                else
                {
                    isBodys[i] = 0f;
                    isWalls[i] = 0f;
                    isHeads[i] = 0f;
                    Distances[i] = 0f;
                    Debug.DrawRay(transform.position, dir * (visonDistance * length), Color.red, reactionTime);
                }
            }
        }
    }
}
