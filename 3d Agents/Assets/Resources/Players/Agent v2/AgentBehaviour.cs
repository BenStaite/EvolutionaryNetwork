using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentBehaviour : MonoBehaviour
{
    public float[] info;
    public int[] Layers;
    public EfficientVision vision;
    public NeuralNetwork net;
    public float[] outputs;
    public int Kills;
    public NetHandler Master;
    public float fitness;
    public Color col;
    public Renderer body;
    public bool killed;
    public float timer;
    float[] prevOutputs;
    float health;
    float[] prevInfo;
    int outputIndex;
    public float timeAlive;
    public CharacterController controller;
    public Movement movement;
    public float recentDamage;
    public Rigidbody RB;
    public Vector3 start;
    public float distance;

    public Vector3 previousPos;

    // Start is called before the first frame update
    void Start()
    {
        previousPos = transform.position;
        distance = 0;
        start = transform.position;
        health = 100;
        timeAlive = 0;
        killed = false;
        Kills = 0;
        outputs = new float[3];
        prevOutputs = new float[5];
    }

    public void Init(NeuralNetwork net, Color c)
    {
        this.net = net;
        Layers = net.layers;
        col = c;
        body.material.color = c;
    }


    public void Kill()
    {
        if (!killed)
        {
            //net.AddFitness(-100f);
            //net.AddFitness(distance/10f);
            //net.AddFitness(-50f);
            //net.AddFitness(timeAlive / 10f);
            net.AddFitness(Kills);
            killed = true;
        }
        
        Destroy(gameObject);
    }

    public void WallHit(Collider wall)
    {
        Vector3 closestpoint = wall.ClosestPointOnBounds(transform.position);
        Vector3 Dir = (transform.position - closestpoint).normalized;
        Vector3 veldiff = RB.velocity - wall.transform.root.gameObject.GetComponent<Rigidbody>().velocity;
        float dotprod = Vector3.Dot(veldiff, Dir);
        float damage;
        damage = Mathf.Abs(dotprod) * 10;
        health -= damage;
        recentDamage = damage;
        net.AddFitness(-5);
        if (Master.KillOnTouch)
        {
            Kill();
        }
        else
        {
            if (health <= 0)
            {
                Kill();
            }
        }
    }

    public void hit(Collider attacker)
    {
        if (attacker.transform.root.tag == "Agent")
        {
            Vector3 closestpoint = attacker.ClosestPointOnBounds(transform.position);
            Vector3 Dir = (transform.position - closestpoint).normalized;
            Vector3 veldiff = RB.velocity - attacker.transform.root.gameObject.GetComponent<Rigidbody>().velocity;
            float dotprod = Vector3.Dot(veldiff, Dir);
            float damage;
            damage = Mathf.Abs(dotprod) * 10;
            health -= damage;
            recentDamage = damage;
            attacker.transform.root.GetComponent<AgentBehaviour>().net.damage += damage;
            attacker.transform.root.GetComponent<AgentBehaviour>().net.kills++;
            if (Master.KillOnTouch)
            {
                Kill();
            }
            else
            {
                if(health <= 0)
                {
                    Kill();
                }
            }
            
        }
        else if (attacker.transform.root.tag == "Player")
        {
            Vector3 closestpoint = attacker.ClosestPointOnBounds(transform.position);
            Vector3 Dir = (transform.position - closestpoint).normalized;
            Vector3 veldiff = RB.velocity - attacker.transform.root.gameObject.GetComponent<Rigidbody>().velocity;
            float dotprod = Vector3.Dot(veldiff, Dir);
            float damage;
            damage = Mathf.Abs(dotprod) * 10;
            health -= damage;
            recentDamage = damage;
            if (Master.KillOnTouch)
            {
                Kill();
            }
            else
            {
                if (health <= 0)
                {
                    Kill();
                }
            }
        }
        
    }

    public void ResetRandomly()
    {
        float x = 0;
        float z = 0;
        RaycastHit hit;
        bool placed = false;
        while (!placed)
        {
            x = UnityEngine.Random.Range(-120f, 120f);
            z = UnityEngine.Random.Range(-39f, 39f);
            if (Physics.Raycast(new Vector3(x, 3, z), -Vector3.up, out hit))
            {
                if (hit.collider.tag == "Floor")
                {
                    Collider[] detection = Physics.OverlapSphere(hit.point, 3f, LayerMask.GetMask("Player", "Wall"));
                    if (detection.Length == 0)
                    {
                        Debug.DrawLine(new Vector3(x, 3, z), hit.point, Color.green, 10f);
                        placed = true;
                    }
                    else
                    {
                        Debug.DrawLine(new Vector3(x, 3, z), hit.point, Color.grey, 10f);
                    }
                }
                else
                {
                    Debug.DrawLine(new Vector3(x, 3, z), hit.point, Color.grey, 2f);
                }
            }
            else
            {
                Debug.DrawLine(new Vector3(x, 3, z), hit.point, Color.grey, 2f);
            }
        }
        transform.position = new Vector3(x, 0.25f, z);
    }



    public void OnDestroy()
    {
        //Master.DestroyNet(net);
        Master.DestroyAgent(this);
    }

    // Update is called once per frame
    void Update()
    {
        distance += Vector3.Distance(transform.position, previousPos);
        previousPos = transform.position;
        timer += Time.deltaTime;
        if (timer >= 5){
            timeAlive++;
            //net.AddFitness(0.01f);
        }
        info = new float[vision.objectMax*3+4];
        for(int i = 0; i < vision.objectMax; i++)
        {
            int index = i * 3;
            info[index] = vision.Distances[i];
            info[index + 1] = vision.types[i];
            info[index + 2] = vision.angles[i];
        }
        info[vision.objectMax*3] = health;
        info[vision.objectMax*3 + 1] = movement.stamina;
        info[vision.objectMax*3 + 2] = movement.forwardvel;
        info[vision.objectMax*3 + 3] = movement.sidevel;

        if (Mathf.Abs(transform.position.y) > 5f)
        {
            ResetRandomly();
        }

        fitness = net.GetFitness();
        outputs = net.FeedForward(info);
        
        prevInfo = info;
    }
}
