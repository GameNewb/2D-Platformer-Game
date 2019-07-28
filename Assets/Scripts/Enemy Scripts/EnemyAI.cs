using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Transform target;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    public Transform enemyGFX;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rigidBody = GetComponent<Rigidbody2D>();

        StartCoroutine(UpdatePath());

        // InvokeRepeating("UpdatePath", 0f, .5f);
        
    }

    IEnumerator UpdatePath()
    {
        while (true)
        {
            seeker.StartPath(rigidBody.position, target.position, OnPathComplete);
            yield return new WaitForSeconds(0.5f);
        }

        // if (seeker.IsDone()) 
        //     seeker.StartPath(rigidBody.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            // Reset waypoint from new path
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (path == null) { return; }

        // Total amount of waypoints
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        // Move enemy

        // Precision of current waypoint - current precision
        // Normalizing it to length of 1
        Vector2 direction = ((Vector2) path.vectorPath[currentWaypoint] - rigidBody.position).normalized;
        Vector2 force = direction * speed * Time.fixedDeltaTime;

        rigidBody.AddForce(force);

        float distance = Vector2.Distance(rigidBody.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // Desired enemy movement in given path
        if (rigidBody.velocity.x >= 0.01f)
        {
            enemyGFX.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (rigidBody.velocity.x <= -0.01f)
        {
            enemyGFX.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
