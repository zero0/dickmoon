using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FTUEController : MonoBehaviour
{
	public Image FTUEScreen;

	public Sprite[] Screens;

	protected int index = 0;

	protected void SetScreen(int index)
	{
		this.index = index;
		FTUEScreen.sprite = Screens[index];
	}

	public void Awake()
	{
		SetScreen(0);
	}

	public void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (++index < Screens.Length)
			{
				SetScreen(index);
			}
			else
			{
				SceneManager.LoadScene("Play");
			}
		}
	}
}
