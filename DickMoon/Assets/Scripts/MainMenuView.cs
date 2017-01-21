using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuView : MonoBehaviour
{
	public void OnPlay()
	{
		SceneManager.LoadScene("Play");
	}
}