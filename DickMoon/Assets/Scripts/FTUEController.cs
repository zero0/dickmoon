using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FTUEController : MonoBehaviour
{
	public static readonly float CROSS_FADE_TIME = 0.3f;
	
	public CanvasGroup[] FTUEScreens;

	protected int index = 0;
	protected Coroutine crossFade;

	protected void SetScreen(int index)
	{
		if (this.index == 0)
		{
			return;
		}

		this.index = index;

		if (crossFade != null)
		{
			StopCoroutine(crossFade);
		}
		crossFade = StartCoroutine(Crossfade(index - 1, index));
	}

	protected IEnumerator Crossfade(int startIndex, int endIndex)
	{
		for (int i = 0; i < startIndex; i++)
		{
			FTUEScreens[i].alpha = 0f;
		}
		float crossFadeTime = 0f;
		while (crossFadeTime < CROSS_FADE_TIME)
		{
			float progress = Mathf.InverseLerp(0f, CROSS_FADE_TIME, crossFadeTime);
			FTUEScreens[startIndex].alpha = 1f - progress;
			FTUEScreens[endIndex].alpha = progress;
			yield return null;
			crossFadeTime += Time.deltaTime;
		}
		FTUEScreens[startIndex].alpha = 0f;
		FTUEScreens[endIndex].alpha = 1f;
	}

	public void Awake()
	{
		SetScreen(0);
	}

	public void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (++index < FTUEScreens.Length)
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
