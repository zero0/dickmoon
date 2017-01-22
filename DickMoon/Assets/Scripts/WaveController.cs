using UnityEngine;
using System.Collections;

public class WaveController : MonoBehaviour
{
    public float lifetime = 5;
    public float speed = 1;
    public float waveHeight = 1;
    public Transform center = null;

    protected void Start()
    {
    }

    protected void Update()
    {
        if (center != null)
        {
            Vector3 dir = Vector3.Normalize(center.position - transform.position);
            Vector3 tangent = Vector3.Cross(dir, Vector3.forward * Mathf.Sign(speed));
            transform.right = tangent;
            transform.position += (tangent * Mathf.Abs(speed) * Time.deltaTime);

        }

        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            GameObject.Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        Debug.Log("Collision Enter " + gameObject.name + " -> " + coll.gameObject.name);
    }

    void OnTriggerStay2D(Collider2D coll)
    {
        Debug.Log("Collision Enter " + gameObject.name + " -> " + coll.gameObject.name);
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        Debug.Log("Collision Exit " + gameObject.name + " -> " + coll.gameObject.name);
    }
}