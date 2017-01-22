using UnityEngine;
using System.Collections;

public class MonumentController : MonoBehaviour
{
    public float health = 3;
    public int score;
    public string destroyText;

    public void HitByWave( float waveHeight )
    {
        health -= waveHeight;

        if( health <= 0 )
        {
			WaveSound.Play();
            transform.Rotate( 0, 0, -80 );
            GetComponent< BoxCollider2D >().enabled = false;
            ScoreManager.MakeScore(score, destroyText);
        }
    }
}
