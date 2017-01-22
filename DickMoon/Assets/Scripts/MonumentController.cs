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
            GameObject.Destroy(gameObject);
			ScoreManager.MakeScore(score, destroyText);
        }
    }
}
