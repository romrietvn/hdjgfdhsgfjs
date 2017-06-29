using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {

	public const string IS_WHITE_FIRST = "IS_WHITE_FIRST";
	public const string DIFFICULT = "DIFFICULT";
	public const string MODEGAMEPLAY = "MODEGAMEPLAY";

	public enum  ModePlay
	{
		One,
		Two,
		Puzzle		
	}

	public enum ModeDifficult 
	{
		Ez,
		Medium,
		Hard
	}

	public static SceneManager instance;
	public MenuController MenuController;
	public AiModeController AiModeController;
	public PlayGameController PlayGameController;

	public bool IsWhiteFirst = true;
	public ModeDifficult Difficult = ModeDifficult.Ez;
	public ModePlay ModeGamePlay = ModePlay.One;

	void Awake() 
	{
		if (instance == null)
		{
			instance = this;
		}
		DontDestroyOnLoad(this.gameObject);
	}

	// Use this for initialization
	void Start () 
	{
		MenuController.ShowMenu();
		AiModeController.HideScreen();
		PlayGameController.HideScreen ();
	}
}
