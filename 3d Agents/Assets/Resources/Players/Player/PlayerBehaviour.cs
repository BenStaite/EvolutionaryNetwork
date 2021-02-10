using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public int Kills;
    public float health;
    public float recentDamage;
    public CharacterController controller;
    public float[] info;
    public EfficientVision vision;
    public Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        Kills = 0;
        health = 100;
    }

    public void WallHit(Collider wall)
    {
        Vector3 closestpoint = wall.ClosestPointOnBounds(transform.position);
        Vector3 Dir = (transform.position - closestpoint).normalized;
        Vector3 veldiff = body.velocity - wall.transform.root.gameObject.GetComponent<Rigidbody>().velocity;
        float dotprod = Vector3.Dot(veldiff, Dir);
        float damage;
        damage = Mathf.Abs(dotprod)* 10;
        if (damage > 1f)
        {
            health -= damage;
            recentDamage = damage;
        }
        if (health <= 0)
        {
            //Kill();
        }
        Debug.Log(damage.ToString());
    }

    public void hit(Collider attacker)
    {
        Vector3 closestpoint = attacker.ClosestPointOnBounds(transform.position);
        Vector3 Dir = (transform.position - closestpoint).normalized;
        Vector3 veldiff = body.velocity - attacker.transform.root.gameObject.GetComponent<Rigidbody>().velocity;
        float dotprod = Vector3.Dot(veldiff, Dir);
        float damage;
        damage = Mathf.Abs(dotprod) * 10;
        health -= damage;
        recentDamage = damage;
        attacker.transform.root.GetComponent<AgentBehaviour>().net.damage += damage;

        if (health <= 0)
        {
            attacker.transform.root.GetComponent<AgentBehaviour>().net.kills++;
            Kill();
        }
    }

    public void Kill()
    {
        Destroy(gameObject,5f);
    }

}
