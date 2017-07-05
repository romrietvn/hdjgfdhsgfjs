using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellBase : MonoBehaviour 
{
	public enum CellType
	{
		None,
		Selecting,
		Temp
	}

	public Image image;
	public ChessBase CurrentType;
	public PlayGameController MainController;
	public MapEditorController MapEditor;

	public int PosX = 0;
	public int PosY = 0;
	private RectTransform rect;
	private ChessBase PreChess;
	public CellType CellStatus = CellType.None;
	private bool firstTime = true;

	void Awake()
	{
		image.raycastTarget = false;
		rect = image.rectTransform;
	}

	public void SetView(ChessBase chess)
	{
		image.color = new Color (1, 1, 1, 1);
		CurrentType = chess;
		chess.Position = new Vector2 (PosX, PosY);
		image.sprite = MainController.ListChess [GetChess (chess)];
		image.SetNativeSize ();
		int temp = (int)chess.Type - 1;
		if (temp < MainController.AddPosY.Count)
			rect.localPosition = new Vector2 (0 , MainController.AddPosY[temp]);
	}

	public void ClearView()
	{
		image.color = new Color (0, 0, 0, 0);
		CurrentType = null;
		CellStatus = CellType.None;
		firstTime = true;
		if (PreChess != null) {
			SetView (PreChess);
		}
	}

	public void ClearPreCell() 
	{
		PreChess = null;
	}

	public void TempView(ChessBase chess) 
	{
		if (firstTime) 
		{
			firstTime = false;
			PreChess = CurrentType;
		}
		CurrentType = chess;
		image.color = new Color (1, 1, 1, 0.5f);
		image.sprite = MainController.ListChess [GetChess (chess)];
		CellStatus = CellType.Temp;
		image.SetNativeSize ();
	}

	public void OnCurrentChessClick()
	{
		if (MainController != null)
			MainController.SelectingThisCell (this);
		else
			MapEditor.OnCellClick (this);
	}

	private int GetChess(ChessBase chess)
	{
		int index = (int)chess.Type - 1;
		if (chess.IsWhite == ChessSide.White)
			index += 6;
		return index;
	}

	public void SetViewEditor(ChessBase chess)
	{
		image.color = new Color (1, 1, 1, 1);
		CurrentType = chess;
		chess.Position = new Vector2 (PosX, PosY);
		image.sprite = MainController.ListChess [GetChess (chess)];
		image.SetNativeSize ();
		int temp = (int)chess.Type - 1;
		if (temp < MainController.AddPosY.Count)
			rect.localPosition = new Vector2 (0 , MainController.AddPosY[temp]);
	}
}
