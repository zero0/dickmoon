using UnityEngine;
using System.Collections;

public class WaveController : MonoBehaviour
{
    public float lifetime = 5;
    public float speed = 1;
    public float waveHeight = 1;
    public float heightDecayOverTime = 0;
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

        // TODO (justin): do we want this?
        waveHeight -= heightDecayOverTime * Time.deltaTime;

        transform.localScale = Vector3.one * Mathf.Max(0, waveHeight);

        if( waveHeight < 0 )
        {
            lifetime = 0;
        }

        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            GameObject.Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        coll.gameObject.SendMessage("HitByWave", waveHeight, SendMessageOptions.DontRequireReceiver);

        waveHeight -= 1;
    }
}