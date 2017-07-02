using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AiModeController : MonoBehaviour {

	public CanvasGroup Canvas;

	public void ShowScreen()
	{
		Canvas.alpha = 1;
		Canvas.blocksRaycasts = true;
	}

	public void HideScreen()
	{
		Canvas.alpha = 0;
		Canvas.blocksRaycasts = false;
	}

	public void OnSelectModeClick(int index)
	{
		if (index == 1) 
		{
			SceneManager.instance.Difficult = GameDifficulty.Easy;
		} 
		else if (index == 2) 
		{
			SceneManager.instance.Difficult = GameDifficulty.Normal;
		}
		else 
		{
			SceneManager.instance.Difficult = GameDifficulty.Hard;
		}
		HideScreen ();
		SceneManager.instance.PlayGameController.ShowScreen ();
	}
}
