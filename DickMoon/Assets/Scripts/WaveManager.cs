using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public GameObject wavePrefab;
    public float waveSpawnDelay;
    public Transform waveCenter;

    private float _waveSpawnTimer;

    public bool SpawnWave( Vector3 position, Transform pivot, float speed, float lifetime, float waveHeight )
    {
        if (_waveSpawnTimer > 0) return false;

        _waveSpawnTimer = waveSpawnDelay;

        GameObject go1 = GameObject.Instantiate(wavePrefab);
        GameObject go2 = GameObject.Instantiate(wavePrefab);

        go1.transform.SetParent(transform.parent, false);
        go2.transform.SetParent(transform.parent, false);
        go1.transform.localPosition = position;
        go2.transform.localPosition = position;

        WaveController w1 = go1.GetComponent<WaveController>();
        WaveController w2 = go1.GetComponent<WaveController>();

        w1.center = pivot;
        w2.center = pivot;
        w1.lifetime = lifetime;
        w2.lifetime = lifetime;
        w1.waveHeight = waveHeight;
        w2.waveHeight = waveHeight;

        w1.speed = speed;
        w2.speed = -speed;

        return true;
    }

    protected void Update()
    {
        if( _waveSpawnTimer > 0 )
        {
            _waveSpawnTimer -= Time.deltaTime;
        }

        if( Input.GetKeyDown( KeyCode.W ) )
        {
            SpawnWave(new Vector3(905, -597, 0), waveCenter, 100, 2, 4);
        }
    }
}
