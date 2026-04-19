using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class BonusSpawner : MonoBehaviour
{
    public GameObject regularBonusPrefab;
    public GameObject superBonusPrefab;

    public float spawnAreaX = 9f;
    public float spawnAreaZ = 9f;
    public float spawnHeight = 0.5f;
    public float superSpawnRate = 15f; 

    [Header("Optimization")]
    public float checkRadius = 0.5f; 
    public LayerMask obstacleMask; 

    private bool isSpawning = true;
    private GameObject currentRegularBonus;
    
    private float nextRetryTime = 0f;

    void Start()
    {
        if (gameObject.name.Contains("(Clone)"))
        {
            Debug.LogWarning("Spawner detected on a Clone! Disabling to prevent crash.");
            enabled = false;
            return;
        }

        StartCoroutine(SpawnSuperBonuses());
    }

    void Update()
    {
        if (isSpawning && currentRegularBonus == null && Time.time >= nextRetryTime)
        {
            currentRegularBonus = SpawnSmart(regularBonusPrefab);
            
            if (currentRegularBonus == null)
            {
                nextRetryTime = Time.time + 1f;
            }
        }
    }

    IEnumerator SpawnSuperBonuses()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(superSpawnRate);
            SpawnSmart(superBonusPrefab);
        }
    }

    GameObject SpawnSmart(GameObject prefab)
    {
        if (prefab == null) return null;

        for (int i = 0; i < 30; i++)
        {
            float randomX = Random.Range(-spawnAreaX, spawnAreaX);
            float randomZ = Random.Range(-spawnAreaZ, spawnAreaZ);
            Vector3 randomPoint = new Vector3(randomX, 0f, randomZ);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 2.0f, NavMesh.AllAreas))
            {
                Vector3 finalPos = hit.position;
                
                finalPos.y = spawnHeight; 

                if (!Physics.CheckSphere(finalPos, checkRadius, obstacleMask))
                {
                    return Instantiate(prefab, finalPos, Quaternion.identity);
                }
            }
        }
        
        return null;
    }
    
   public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines(); 
    }
}