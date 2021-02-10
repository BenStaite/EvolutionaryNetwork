using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 5f;
    public float stamina;

    public Vector3 velocity;
    public Rigidbody bod;
    public AgentBehaviour behaviour;

    public float x;
    public float z;

    public float rotationaccel = 0;

    public float forwardvel;
    public float sidevel;

    public bool bouncing;
    public float bounce = 0;
    public float bounceAmount = 1;
    public Vector3 BounceDir;
    public float rotation;
    public bool boost;

    // Start is called before the first frame update
    void Start()
    {
        boost = false;
        bouncing = false;
        stamina = 100;
        rotation = Random.Range(0f,360f);
    }

    public void BeginBounce(Collider obj)
    {
        Vector3 closestpoint = obj.ClosestPointOnBounds(transform.position);
        BounceDir = (closestpoint - transform.position).normalized;
        Vector3 veldiff = bod.velocity - obj.transform.root.gameObject.GetComponent<Rigidbody>().velocity;
        float dotprod = Vector3.Dot(veldiff, -BounceDir);
        Debug.Log(dotprod.ToString() + "  " + veldiff.ToString());
        dotprod = Mathf.Abs(dotprod);
        dotprod = Mathf.Clamp(dotprod, 0.5f, 10f);
        bounce = dotprod * bounceAmount;
        bod.AddForce(new Vector3(-BounceDir.x * bounce, 0f, -BounceDir.z * bounce));
    }

    // Update is called once per frame
    void Update()
    {
        //x = (Input.GetAxis("Horizontal") / 8f) * 0.9f;
        Vector3 move = new Vector3();
        z = 1;
        bool shift = behaviour.outputs[0]>0.5f;

        float mousex = behaviour.outputs[1] * Time.deltaTime;

        if (boost)
        {
            if (shift && stamina > 0)
            {
                stamina -= 1 * Time.deltaTime;
            }
            else
            {
                boost = false;
                stamina += 0.5f * Time.deltaTime;
            }
        }
        else
        {
            if (shift && stamina > 10)
            {
                boost = true;
                stamina -= 1 * Time.deltaTime;
            }
            else
            {
                stamina += 0.5f * Time.deltaTime;
            }
        }

        stamina = Mathf.Clamp(stamina, 0f, 100f);

        if (!(mousex == 0))
        {
            rotationaccel += mousex / 2f;
        }
        else
        {
            rotationaccel = rotationaccel * 0.8f;
        }

        rotationaccel = Mathf.Clamp(rotationaccel, -1.5f, 1.5f);
        rotation += rotationaccel;
        transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        move = transform.forward * z * speed;
        if (boost)
        {
            move = move * 2;
        }

        move = move * Time.deltaTime;


        //controller.Move(move * speed * Time.deltaTime);
        bod.AddForce(move);
        forwardvel = Vector3.Dot(bod.velocity, transform.forward);
        sidevel = Vector3.Dot(bod.velocity, transform.right);

        if (bounce > 0)
        {
            bounce = 0;
            BounceDir = new Vector3();
        }
    }
}
