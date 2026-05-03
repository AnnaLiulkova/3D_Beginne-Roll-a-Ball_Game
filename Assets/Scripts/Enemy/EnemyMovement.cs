using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent navMeshAgent;
    private bool isStopped = false;

    [Header("Death Effects")]
    public ParticleSystem explosionParticles;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (explosionParticles != null)
        {
            explosionParticles.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (player != null && !isStopped && navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(player.position);
        }
    }

    public void Stop()
    {
        isStopped = true;

        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();              
            navMeshAgent.velocity = Vector3.zero;  
        }
    }

    public void Die()
    {
        Stop();
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        if (explosionParticles != null)
        {
            explosionParticles.transform.parent = null;
            explosionParticles.gameObject.SetActive(true);
            explosionParticles.transform.position = transform.position;
            explosionParticles.Play();
        }

        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }

        foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>())
        {
            r.enabled = false;
        }

        foreach (Collider c in GetComponentsInChildren<Collider>())
        {
            c.enabled = false;
        }

        yield return new WaitForSeconds(1.5f);

        Destroy(gameObject);
    }
}