using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static readonly float READ_TIME = 1.3f;
    public static readonly float LOG_BASE = 8; // Higher means slower speed ramp
    
    public static int Score { get { return _score; } set { _score = value; } }

    private static int _score;
    private static int _nextScore;

    public static float speedMultiplier
    {
        get
        {
            return Mathf.Log(Mathf.Max(_nextScore / 1000f, 0f) + LOG_BASE) / Mathf.Log(LOG_BASE);
        }
    }

    private static ScoreManager _instance = null;
    protected static ScoreManager instance
    {
        get
        {
            if( _instance == null )
            {
                _instance = FindObjectOfType< ScoreManager >();
            }
            return _instance;
        }
    }
    
    public FlyupView FlyupPrefab;
    public Text ScorePanel;
    public float scoreTextAnimSpeed;
    public Text flyUpText;

    protected float expiryTime;

    public void Start()
    {
        ScorePanel.text = "0";
        ClearScore();
        SetNotificationText( "Click to make waves");
    }

    public void OnDestroy()
    {
        _instance = null;
    }

    public static void ClearScore()
    {
        _score = 0;
        _nextScore = 0;
    }

    public static int MakeScore(int score, string notifText)
    {
        if (Time.timeScale != 0.3f)
        {
            Time.timeScale = speedMultiplier;
        }

        int scaledScore = Mathf.CeilToInt(score * Time.timeScale);
        _nextScore += scaledScore;

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