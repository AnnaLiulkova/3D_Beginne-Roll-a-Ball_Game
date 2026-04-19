using UnityEngine;

public class SuperStarRotator : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Rotate (new Vector3 (0, 100, 0) * Time.deltaTime);
    }
}
