using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
	public static readonly float READ_TIME = 1.3f;
	
	public static int Score;

	protected static ScoreManager instance;
	
	public FlyupView FlyupPrefab;
	public Text ScorePanel;

	protected float expiryTime;

	public void Start()
	{
		instance = this;
		MakeScore(0, "Click to make waves");
	}

	public static void MakeScore(int score, string notifText)
	{
		Score += score;
		instance.ScorePanel.text = Score.ToString( "N0" );
		instance.FlyupPrefab.ScoreNotif.text = notifText;
		instance.FlyupPrefab.ScoreNotif.enabled = true;
		instance.expiryTime = READ_TIME;
	}

	public void Update()
	{
		if ((expiryTime -= Time.deltaTime) <= 0f)
		{
			FlyupPrefab.ScoreNotif.enabled = false;
		}
	}
}