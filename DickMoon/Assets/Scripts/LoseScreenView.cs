using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoseScreenView : MonoBehaviour
{
    public Text finalScore;

    protected void Start()
    {
        finalScore.text = ScoreManager.Score.ToString( "N0" );
    }

    protected void Update()
    {
        if( Input.GetMouseButtonDown( 0 ) )
        {
            OnRestartClicked();
        }
    }

    public void OnRestartClicked()
    {
        ScoreManager.ClearScore();
        SceneManager.LoadScene( "Play" );
    }
}
