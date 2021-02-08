using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathDetection : MonoBehaviour
{
    public Movement movement;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log(collision.gameObject);
        if (collision.gameObject.transform.root.GetInstanceID() != transform.root.GetInstanceID())
        {
            if (collision.gameObject.tag != "Floor")
            {
                //Debug.Log(collision.gameObject);
                movement.BeginBounce(collision);
            }
            if (collision.gameObject.tag == "Wall")
            {
                transform.parent.GetComponent<AgentBehaviour>().WallHit(collision);
            }
            else if (collision.gameObject.tag == "Head")
            {
                transform.parent.GetComponent<AgentBehaviour>().hit(collision);
            }
        }
    }
}
