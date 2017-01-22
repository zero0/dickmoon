using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
	public static readonly float READ_TIME = 1.3f;
	
    public static int Score;
    private static int _nextScore;

	protected static ScoreManager instance;
	
	public FlyupView FlyupPrefab;
	public Text ScorePanel;
    public float scoreTextAnimSpeed;

	protected float expiryTime;

	public void Start()
	{
		instance = this;
        ClearScore();
		MakeScore(0, "Click to make waves");
	}

    public static void ClearScore()
    {
        Score = 0;
        _nextScore = 0;
    }

	public static void MakeScore(int score, string notifText)
	{
        _nextScore += score;
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

        int newScore = Mathf.FloorToInt(Mathf.Lerp( (float)Score, (float)_nextScore, Time.deltaTime * scoreTextAnimSpeed ) + 0.5f );
        if( newScore != Score )
        {
            Score = newScore;
            instance.ScorePanel.text = Score.ToString( "N0" );
        }
        else if( Score != _nextScore )
        {
            Score = _nextScore;
            instance.ScorePanel.text = Score.ToString( "N0" );
        }
	}
}