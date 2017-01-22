using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static readonly float READ_TIME = 1.3f;
    public static readonly float LOG_BASE = 8; // Higher means slower speed ramp
    
    public static int Score { get { return instance._score; } set { instance._score = value; } }

    private int _score;
    private int _nextScore;

    public static float speedMultiplier
    {
        get
        {
            return Mathf.Log(Mathf.Max(instance._nextScore / 1000f, 0f) + LOG_BASE) / Mathf.Log(LOG_BASE);
        }
    }

    protected static ScoreManager instance;
    
    public FlyupView FlyupPrefab;
    public Text ScorePanel;
    public float scoreTextAnimSpeed;
    public Text flyUpText;

    protected float expiryTime;

    public void Start()
    {
        instance = this;
        ClearScore();
        SetNotificationText( "Click to make waves");
    }

    public static void ClearScore()
    {
        instance._score = 0;
        instance._nextScore = 0;
        instance.ScorePanel.text = "0";
    }

    public static int MakeScore(int score, string notifText)
    {
        if (Time.timeScale != 0.3f)
        {
            Time.timeScale = speedMultiplier;
        }

        int scaledScore = Mathf.CeilToInt(score * Time.timeScale);
        instance._nextScore += scaledScore;

        instance.SetNotificationText( notifText );

        return scaledScore;
    }

    public void SetNotificationText( string notifText )
    {
        FlyupPrefab.ScoreNotif.enabled = true;
        FlyupPrefab.ScoreNotif.text = notifText;
        expiryTime = READ_TIME;
    }

    public static void ShowFlyUpScore( int score, Vector3 startPosition )
    {
        GameObject go = GameObject.Instantiate( instance.flyUpText.gameObject );
        go.SetActive( true );

        Transform r = go.GetComponent< Transform >();
        r.SetParent( instance.transform );
        r.position = startPosition;

        Text t = go.GetComponent< Text >();
        t.text = ( score > 0 ? "+" : "" ) + score.ToString( "N0" );
    }

    public void Update()
    {
        if( expiryTime > 0f )
        {
            if ((expiryTime -= Time.deltaTime) <= 0f)
            {
                FlyupPrefab.ScoreNotif.enabled = false;
            }
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