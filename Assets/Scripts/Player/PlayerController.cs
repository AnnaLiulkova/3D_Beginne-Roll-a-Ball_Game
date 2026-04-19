using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections; 

public class PlayerController : MonoBehaviour
{
    public GameObject winTextObject;
    public float speed = 10f; 
    private int count;
    public TextMeshProUGUI countText;
    private Rigidbody rb; 
    private float movementX;
    private float movementY;

    [Header("Fall Death Setting")]
    public float deathThresholdY = -1.0f; 

    [Header("Explosion VFX")]
    public ParticleSystem playerExplosionParticles; 

    private bool isGameOver = false; 
    private PlayerInput playerInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        playerInput = GetComponent<PlayerInput>();
        rb.sleepThreshold = 0f; 
        
        count = 0; 
        SetCountText();
        winTextObject.SetActive(false);
        
        if (playerExplosionParticles != null) playerExplosionParticles.gameObject.SetActive(false);
    }

    void OnMove(InputValue movementValue)
    {
        if (isGameOver) return; 

        Vector2 movementVector = movementValue.Get<Vector2>(); 
        movementX = movementVector.x; 
        movementY = movementVector.y; 
    }

    void FixedUpdate() 
    {
        if (isGameOver) return; 

        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed); 
    }

    void Update()
    {
        if (!isGameOver && transform.position.y < deathThresholdY)
        {
            StartCoroutine(PlayerDeathRoutine());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isGameOver)
        {
            StartCoroutine(PlayerDeathRoutine());
        }
    }

void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("PickUp") && !isGameOver) 
        {
            Destroy(other.gameObject);
            count += 1;
            SetCountText();
        }

        if (other.gameObject.CompareTag("SuperBonus") && !isGameOver)
        {
            Destroy(other.gameObject);

            count += 5;
            SetCountText();
        }
    }

    void SetCountText() 
    {
        countText.text = "Count: " + count.ToString();
    }

    IEnumerator PlayerDeathRoutine()
    {
        isGameOver = true;
        rb.isKinematic = true; 
        if (playerInput != null) playerInput.DeactivateInput(); 

        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (enemy != null)
        {
            EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
            if (enemyMovement != null) enemyMovement.Stop();
        }

        BonusSpawner spawner = FindFirstObjectByType<BonusSpawner>();
        if (spawner != null) spawner.StopSpawning();

        if (playerExplosionParticles != null)
        {
            playerExplosionParticles.gameObject.SetActive(true); 
            playerExplosionParticles.transform.position = transform.position;
            playerExplosionParticles.Play();
        }

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<SphereCollider>().enabled = false;

        yield return new WaitForSeconds(0.5f);
        
        winTextObject.SetActive(true);
        winTextObject.GetComponent<TextMeshProUGUI>().text = "Game Over!\nScore: " + count.ToString();
    }
}