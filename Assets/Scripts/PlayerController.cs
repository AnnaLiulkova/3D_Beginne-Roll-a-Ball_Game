using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public GameObject winTextObject;
    public float speed = 0; 
    private int count;
    public TextMeshProUGUI countText;
    private Rigidbody rb; 
    private float movementX;
    private float movementY;

    void Start()
    {
        rb = GetComponent <Rigidbody>(); 
        count = 0; 
        SetCountText();
        winTextObject.SetActive(false);
    }

    void OnMove (InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>(); 
       
        movementX = movementVector.x; 
        movementY = movementVector.y; 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject); 
            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
        }
    }

    void SetCountText() 
    {
        countText.text =  "Count: " + count.ToString();
       
        if (count >= 12)
        {
           winTextObject.SetActive(true);
           Destroy(GameObject.FindGameObjectWithTag("Enemy"));
        }
    }

    void FixedUpdate() 
    {
        Vector3 movement = new Vector3 (movementX, 0.0f, movementY);
        rb.AddForce(movement * speed); 
    }

    void OnTriggerEnter (Collider other) 
    {
       if (other.gameObject.CompareTag("PickUp")) 
        {
           other.gameObject.SetActive(false);
           count = count + 1;
           SetCountText();
        }
    }
}
