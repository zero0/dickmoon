using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SpaceShuttleController : MonoBehaviour {

	public void HitByWave( float waveHeight )
	{
		GetComponentInChildren<Image>().enabled = false;
		ScoreManager.MakeScore(-10000, "OH NO!  Not the space program!");
		Time.timeScale = 0.3f;
		StartCoroutine(WaitThenLose());
	}

	protected IEnumerator WaitThenLose()
	{
		yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Lose");
        Time.timeScale = 1f;
	}
}
