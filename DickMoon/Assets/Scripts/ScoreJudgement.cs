using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreJudgement : MonoBehaviour
{
	public Text JudgementArea;

	public void Start()
	{
		if (ScoreManager.Score < -5000)
		{
			JudgementArea.text = "Try avoiding the space center";
		}
		else if (ScoreManager.Score < 0)
		{
			JudgementArea.text = "Meh";
		}
		else if (ScoreManager.Score < 10000)
		{
			JudgementArea.text = "They'll build another one... Someday...";
		}
		else if (ScoreManager.Score < 30000)
		{
			JudgementArea.text = "Yeah!  That'll show 'em!";
		}
		else if (ScoreManager.Score < 100000)
		{
			JudgementArea.text = "Woah!  That's a high score!";
		}
		else
		{
			JudgementArea.text = "How did you do that?";
		}
	}
}