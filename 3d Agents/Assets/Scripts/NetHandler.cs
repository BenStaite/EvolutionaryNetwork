using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NetHandler : MonoBehaviour
{
    public GameObject AgentObject;
    public int netnum;
    public int agentnum;
    public bool KillOnTouch;
    public List<NeuralNetwork> nets;
    private List<AgentBehaviour> agents;
    public int populationSize;
    public bool Replenishing;
    public float[] fitness;
    public int[] kills;
    public Color[] colors;
    public int gens;
    public float mutationMod = 100f;
    private int[] layers = new int[] {19,25,10,2}; //9 input and 3 output
    public float averageFitness;
    public int limit = 4;
    public int resetNum = 6;
    public bool savebest = false;
    public float killScale = 0.5f;
    public int trainNum = 3;
    
    // Start is called before the first frame update
    void Start()
    {
        gens = 0;
        agents = new List<AgentBehaviour>();
        nets = new List<NeuralNetwork>();
        kills = new int[populationSize];
        fitness = new float[populationSize];
        colors = new Color[populationSize];
        for (int i = 0; i < populationSize; i++)
        {
            AddFreshAgent();
        }
    }

    void ReplenishAgents()
    {
        nets.Sort();
        int chunks = populationSize / limit;
        for (int i = 0; i < limit; i++)
        {
            nets[(populationSize - 1) - i] = new NeuralNetwork(nets[(populationSize - 1) - i], true);
            for (int j = 0; j < chunks-1; j++)
            {
                int breedrand = i;
                while(breedrand == i)
                {
                    breedrand = Random.Range(0, limit);
                }
                //nets[i + (j*limit)] = new NeuralNetwork(nets[(populationSize - 1) - i], false);
                nets[i + (j * limit)] = new NeuralNetwork(nets[(populationSize - 1) - i], nets[populationSize-1 - breedrand]);
                nets[i + (j*limit)].Mutate(mutationMod);
            }
        }

        nets[0] = new NeuralNetwork(layers);
        nets[0].color = Color.white;
        nets[1] = new NeuralNetwork(layers);
        nets[1].color = Color.black;

        foreach (NeuralNetwork net in nets)
        {
            float timer = 0;
            AddAgent(net);
            while(timer < 2)
            {
                timer += Time.deltaTime;
            }
        }

    }

    public void DestroyNet(NeuralNetwork n)
    {
        nets.Remove(n);
    }

    public void DestroyAgent(AgentBehaviour a)
    {
        agents.Remove(a);
    }


    void AddAgent(NeuralNetwork net)
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
                    //Debug.DrawLine(new Vector3(x, 3, z), hit.point, Color.grey, 2f);
                }
            }
            else
            {
                //Debug.DrawLine(new Vector3(x, 3, z), hit.point, Color.grey, 2f);
            }
        }
        AgentBehaviour agent = ((GameObject)Instantiate(AgentObject, new Vector3(x, 0.25f, z), Quaternion.Euler(0, Random.Range(0, 360), 0))).GetComponent<AgentBehaviour>();
        //nets.Add(nets[i]);
        agent.Init(net, net.color);
        agent.Master = this;
        agents.Add(agent);
    }


    void AddFreshAgent()
    {
        NeuralNetwork net = new NeuralNetwork(layers);
        nets.Add(net);
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
                        Debug.DrawLine(new Vector3(x, 3, z), hit.point, Color.green, 2f);
                        placed = true;
                    }
                    else
                    {
                        Debug.DrawLine(new Vector3(x, 3, z), hit.point, Color.grey, 2f);
                    }
                }
                else
                {
                   // Debug.DrawLine(new Vector3(x, 3, z), hit.point, Color.grey, 2f);
                }
            }
            else
            {
              //  Debug.DrawLine(new Vector3(x, 3, z), hit.point, Color.grey, 2f);
            }
        }


        AgentBehaviour agent = ((GameObject)Instantiate(AgentObject, new Vector3(x, 0.25f, z), Quaternion.Euler(0,Random.Range(0,360),0))).GetComponent<AgentBehaviour>();
        agent.Init(net, net.color);
        agent.Master = this;
        agents.Add(agent);
    }

    public void saveBest()
    {
        StreamWriter outf = new StreamWriter("BestNet.txt");
        for (int i = 0; i < nets[populationSize - 1].weights.Length; i++)
        {
            for (int j = 0; j < nets[populationSize - 1].weights[i].Length; j++)
            {
                for (int k = 0; k < nets[populationSize - 1].weights[i][j].Length; k++)
                {
                    outf.WriteLine(nets[populationSize - 1].weights[i][j][k].ToString());
                    Debug.Log(nets[populationSize - 1].weights[i][j][k].ToString());
                }
            }
        }
        outf.Flush();
        outf.Close();
    }

    // Update is called once per frame
    void Update()
    {
        netnum = nets.Count;
        agentnum = agents.Count;

        if (agents.Count < resetNum)
        {
            if (gens % trainNum == 0 && gens != 0)
            {
                Replenishing = true;
                foreach (AgentBehaviour a in agents)
                {
                    a.Kill();
                }
                foreach (NeuralNetwork n in nets)
                {
                    n.AddFitness(n.kills * killScale);
                    n.AddFitness(n.distance / 100f);
                    n.AddFitness(n.timeAlive / 1000f);
                }
                nets.Sort();
                float totalFitness = 0;
                foreach (NeuralNetwork net in nets)
                {
                    int index = nets.IndexOf(net);
                    fitness[index] = net.GetFitness();
                    totalFitness += fitness[index];
                    colors[index] = net.color;
                    kills[index] = net.kills;
                }
                if (savebest)
                {
                    saveBest();
                }
                averageFitness = totalFitness / populationSize;
                ReplenishAgents();
            }
            else
            {
                foreach (AgentBehaviour a in agents)
                {
                    a.Kill();
                }
                nets.Sort();
                float totalFitness = 0;
                foreach (NeuralNetwork n in nets)
                {
                    int index = nets.IndexOf(n);
                    fitness[index] = n.GetFitness();
                    totalFitness += fitness[index];
                    colors[index] = n.color;
                    kills[index] = n.kills;
                    averageFitness = (totalFitness / populationSize);
                    AddAgent(n);
                }
            }
            gens++;
        }
    }
}
