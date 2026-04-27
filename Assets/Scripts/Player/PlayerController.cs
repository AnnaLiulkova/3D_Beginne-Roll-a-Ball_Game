using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public float speed = 10f; 
    public bool canJump = false; 
    public float jumpForce = 5f;
    public float deathThresholdY = -1.0f; 

    [Header("Visuals & Skins")]
    public ParticleSystem playerExplosionParticles; 
    public Material[] skinMaterials; 

    private Rigidbody rb; 
    private float movementX;
    private float movementY;
    private bool isGrounded = true;
    private PlayerInput playerInput;
    private bool isDead = false;

   void Start()
    {
        rb = GetComponent<Rigidbody>(); 
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        playerInput = GetComponent<PlayerInput>();
        rb.sleepThreshold = 0f; 
        
        if (playerExplosionParticles != null) playerExplosionParticles.gameObject.SetActive(false);

        int selectedSkin = PlayerPrefs.GetInt("PlayerSkin", 0);
        SetSkin(selectedSkin); 
    }

    public void SetSkin(int index)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null && skinMaterials != null && index >= 0 && index < skinMaterials.Length)
        {
            renderer.material = skinMaterials[index];
        }
    }

    void OnMove(InputValue movementValue)
    {
        if (isDead || (GameManager.Instance != null && GameManager.Instance.isGameOver)) return; 

        Vector2 movementVector = movementValue.Get<Vector2>(); 
        movementX = movementVector.x; 
        movementY = movementVector.y; 
    }

    void Update()
    {
        if (isDead || GameManager.Instance == null || GameManager.Instance.isGameOver) return;

        if (transform.position.y < deathThresholdY)
        {
            Die("You fell!\nGame Over!");
        }

        if (canJump && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate() 
    {
        if (isDead || (GameManager.Instance != null && GameManager.Instance.isGameOver)) return; 

        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed); 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; 
        }

        if (collision.gameObject.CompareTag("Enemy") && !isDead)
        {
            Die("You were caught!\nGame Over!");
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if (isDead || (GameManager.Instance != null && GameManager.Instance.isGameOver)) return;

        if (other.CompareTag("PickUp")) 
        {
            AudioManager.Instance.PlayPickupSuper();
            Destroy(other.gameObject);
            GameManager.Instance.AddScore(1); 
        }
        else if (other.CompareTag("SuperBonus"))
        {
            AudioManager.Instance.PlayPickupNormal();
            Destroy(other.gameObject);
            GameManager.Instance.AddScore(5); 
        }
    }

    public void Die(string message)
    {
        if (isDead) return; 
        isDead = true;
        rb.isKinematic = true; 
        if (playerInput != null) playerInput.DeactivateInput(); 

        if (playerExplosionParticles != null)
        {
            playerExplosionParticles.transform.parent = null;
            playerExplosionParticles.gameObject.SetActive(true); 
            playerExplosionParticles.transform.position = transform.position;
            playerExplosionParticles.Play();
        }

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<SphereCollider>().enabled = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGameOver(message);
        }
    }
}