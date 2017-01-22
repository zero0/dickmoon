using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public GameObject wavePrefab;
    public float waveSpawnDelay;

    private float _waveSpawnTimer;

    public bool SpawnWave( Vector3 position, Transform pivot, float speed, float lifetime )
    {
        if (_waveSpawnTimer > 0) return false;

        _waveSpawnTimer = waveSpawnDelay;

        GameObject go1 = GameObject.Instantiate(wavePrefab);
        GameObject go2 = GameObject.Instantiate(wavePrefab);

        WaveController w1 = go1.GetComponent<WaveController>();
        WaveController w2 = go1.GetComponent<WaveController>();

        w1.transform.position = w2.transform.position = position;

        w1.center = w2.center = pivot;

        w1.speed = speed;
        w2.speed = -speed;

        w1.lifetime = w2.lifetime = lifetime;

        return true;
    }

    protected void Update()
    {
        if( _waveSpawnTimer > 0 )
        {
            _waveSpawnTimer -= Time.deltaTime;
        }
    }
}
