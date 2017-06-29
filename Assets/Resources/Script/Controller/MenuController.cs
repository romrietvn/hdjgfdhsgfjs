using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuController : MonoBehaviour {

	public CanvasGroup Canvas;

	public void ShowMenu()
	{
		Canvas.alpha = 1;
		Canvas.blocksRaycasts = true;
	}

	public void HideMenu()
	{
		Canvas.alpha = 0;
		Canvas.blocksRaycasts = false;
	}

	public void OnSingleModeClick()
	{
		HideMenu ();
		SceneManager.instance.AiModeController.ShowScreen ();
	}

	public void OnTwoModeClick()
	{
		
	}

	public void OnPuzzleModeClick()
	{
		
	}

	public void OnSettingClick()
	{
		
	}

	public void OnRateClick()
	{
		
	}

	public void OnShareClick()
	{
		
	}

	public void OnMoreGameClick()
	{
		
	}

	public void OnFacebookClick()
	{
		
	}
}
