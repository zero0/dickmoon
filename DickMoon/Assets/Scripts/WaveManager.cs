using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public GameObject wavePrefab;
    public float waveSpawnDelay;
    public Transform waveCenter;
    public Transform moon;
    public ChunkLoader chunkLoader;

    private float _waveSpawnTimer;

    public bool SpawnWave( Vector3 position, Transform pivot, float speed, float lifetime, float waveHeight, bool bothDirections )
    {
        if (_waveSpawnTimer > 0) return false;

        _waveSpawnTimer = waveSpawnDelay;

        // spawn forward wave
        CreateWave( position, pivot, -speed, lifetime, waveHeight );

        // spawn backwards wave if needed
        if( bothDirections )
        {
            CreateWave( position, pivot, speed, lifetime, waveHeight );
        }

        return true;
    }

    private void CreateWave( Vector3 position, Transform pivot, float speed, float lifetime, float waveHeight )
    {
        GameObject go1 = GameObject.Instantiate(wavePrefab);

        go1.transform.SetParent(transform.parent, false);
        go1.transform.position = position;

        WaveController w1 = go1.GetComponent<WaveController>();
        w1.center = pivot;
        w1.lifetime = lifetime;
        w1.waveHeight = waveHeight;
        w1.speed = speed;
    }

    protected void Update()
    {
        if( _waveSpawnTimer > 0 )
        {
            _waveSpawnTimer -= Time.deltaTime;
        }

        if( Input.GetKeyDown( KeyCode.W ) )
        {
            //Vector3 p, n;
            //if( TryMoonPlanetIntersection( moon.transform.position, moon.transform.right, waveCenter.position, 3000, out p, out n ) )
            //{
                SpawnWave( transform.position, waveCenter, 300, 2, 4, false);
            //}
        }
    }

    public static bool TryMoonPlanetIntersection(Vector3 moonPosition, Vector3 moonDirection, Vector3 planetCenter, float planetRadius, out Vector3 intersectionPoint, out Vector3 intersectionNormal)
    {
        intersectionPoint = Vector3.zero;
        intersectionNormal = Vector3.zero;

        Vector3 m = planetCenter - moonPosition;
        Vector3 d = moonDirection;

        float b = 2 * Vector3.Dot(m, d);

        float c = Vector3.Dot(m, m) - (planetRadius * planetRadius);

        float disc = (b * b) - (4 * c);

        if( disc < 0 )
        {
            return false;
        }

        disc = Mathf.Sqrt(disc);

        float t = (-b - disc) / 2f;
        if( t < 0 )
        {
            t = (-b + disc) / 2f;
        }

        if( t < 0 )
        {
            return false;
        }

        intersectionPoint = moonPosition + t * moonDirection;
        intersectionNormal = (intersectionPoint - planetCenter).normalized;

        //float b = Vector3.Dot(m, d);
        //float c = Vector3.Dot(m, m) - (planetRadius * planetRadius);
        //
        //if (c > 0 && b > 0) return false;
        //
        //float discr = b * b - c;
        //
        //if( discr < 0 )
        //{
        //    return false;
        //}
        //
        //float t = -b - Mathf.Sqrt(discr);
        //
        //if( t < 0 )
        //{
        //    t = 0;
        //}
        //intersectionPoint = planetCenter + t * d;

        //float pDotD = Vector3.Dot(m, d);
        //float rSqr = planetRadius * planetRadius;
        //
        //if (pDotD < 0 || Vector3.Dot(m, m) < rSqr)
        //{
        //    return false;
        //}
        //
        //Vector3 a = m - d * pDotD;
        //float aDotA = Vector3.Dot(a, a);
        //
        //if (aDotA > rSqr)
        //{
        //    return false;
        //}
        //
        //float h = Mathf.Sqrt(rSqr - aDotA);
        //
        //Vector3 i = a - h * d;
        //
        //intersectionPoint = planetCenter + i;
        //intersectionNormal = i / planetRadius;

        return true;
    }
}
