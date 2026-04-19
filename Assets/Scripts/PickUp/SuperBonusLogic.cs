using UnityEngine;
using System.Collections;

public class SuperBonusLogic : MonoBehaviour
{
    [Header("Timer Settings")]
    public float lifeTime = 10.0f; 
    public float blinkStartPercent = 0.7f; 
    
    private float timer = 0f;
    private bool isBlinking = false;
    private MeshRenderer starRenderer;

    void Start()
    {
        starRenderer = GetComponentInChildren<MeshRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (!isBlinking && timer >= lifeTime * blinkStartPercent)
        {
            isBlinking = true;
            StartCoroutine(BlinkCoroutine());
        }
        if (timer >= lifeTime)
        {
            Destroy(gameObject); 
        }
    }

    IEnumerator BlinkCoroutine()
    {
        float blinkInterval = 0.2f; 

        while (timer < lifeTime)
        {
            if (starRenderer != null)
            {
                starRenderer.enabled = !starRenderer.enabled;
            }
            yield return new WaitForSeconds(blinkInterval);
        }
        
        if (starRenderer != null) starRenderer.enabled = false;
    }
}