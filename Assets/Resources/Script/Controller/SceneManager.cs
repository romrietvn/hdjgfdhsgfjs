using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {

	public const string IS_WHITE_FIRST = "IS_WHITE_FIRST";
	public const string DIFFICULT = "DIFFICULT";
	public const string MODEGAMEPLAY = "MODEGAMEPLAY";

	public static SceneManager instance;
	public MenuController MenuController;
	public AiModeController AiModeController;
	public PlayGameController PlayGameController;

	public bool IsWhiteFirst = true;
	public GameDifficulty Difficult = GameDifficulty.Easy;
	public PlayGameController.GameMode ModeGamePlay = PlayGameController.GameMode.OnePlayer;
	public int MaxLevel = 0;
	public int CurrentLevel = 0;
	public List<string> LevelData = new List<string>();

	void Awake() 
	{
		if (instance == null)
		{
			instance = this;
		}
		InitLevel ();
		DontDestroyOnLoad(this.gameObject);
	}

	// Use this for initialization
	void Start () 
	{
		MenuController.ShowMenu();
		AiModeController.HideScreen();
		PlayGameController.HideScreen ();
		StartCoroutine (Chess_Usercontrol.clsChessEngine.InitBook());
	}

	void InitLevel() 
	{
		TextAsset leveltext = Resources.Load("level") as TextAsset;
		string textInBook = leveltext.text;
		string[] lineText = textInBook.Split(new [] { '\r', '\n' });
		LevelData.Clear ();
		for (int i = 0; i < lineText.Length; i++) 
		{
			LevelData.Add (lineText [i]);
		}
	}
}
