using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Chess_Usercontrol;

public class PlayGameController : MonoBehaviour {

	public enum GameMode
	{
		OnePlayer,
		TwoPlayer,
		Puzzle
	}

	public GameMode CurrentGameMode = GameMode.OnePlayer;

	public CanvasGroup Canvas;
	public GameObject Board;
	public Image Selecting;
	public List<Sprite> ListChess = new List<Sprite> ();
	public List<int> AddPosY = new List<int>();

	private List<CellBase> AllCell = new List<CellBase> ();
	private List<ChessBase> AllChess = new List<ChessBase>();
	private List<CellBase> TempCell = new List<CellBase>();
	private CellBase preCell;

	private MapData currentMap;
	private bool isSelecting = false;
	public ChessSide isWhite = ChessSide.White;
	public Vector2 _EnPassantVector2 = new Vector2();
	public int[,] _BoardState = new int[10, 10];
	public GameStatus CurrentStatus = GameStatus.NowPlaying;
	public Vector2 _EnPassantPoint = new Vector2();
	clsMove MyMove = new clsMove();
	public ArrayList arrMove = new ArrayList();
	private bool isThinking = false;
	public clsMove LastMove = null;
	public ArrayList arrFEN = new ArrayList();
	private bool isUndo = false;
	public GameDifficulty Difficult = GameDifficulty.Easy;

	void Awake() 
	{
		GetAllCell ();
	}

	public void ShowScreen()
	{
		Canvas.alpha = 1;
		Canvas.blocksRaycasts = true;
		InitNewMatch (null);
	}

	public void HideScreen()
	{
		Canvas.alpha = 0;
		Canvas.blocksRaycasts = false;
	}

	public void InitNewMatch(MapData newMap)
	{
		NewGame ();
		InitBoardState ();
	}

	private void GetAllCell()
	{
		AllCell.Clear ();
		var listCell = Board.GetComponentsInChildren<CellBase> ();
		foreach (var item in listCell) 
		{
			AllCell.Add (item);
		}
	}

	private void NewGame()
	{
		for (int i = 0; i < 10; i++)
			for (int j = 0; j < 10; j++)
				_BoardState [i , j] = 0;
//		Pawn = 0,//Tốt
//		Bishop = 1,//Tượng
//		Knight = 2,//Mã
//		Rook = 3,//Xe
//		Queen = 4,//Hậu
//		King = 5,//Vua

		string mapData = "1-0-0-3;1-7-0-3;1-1-0-2;1-6-0-2;1-2-0-1;1-5-0-1;1-3-0-5;1-4-0-4;" +
			"1-0-1-0;1-1-1-0;1-2-1-0;1-3-1-0;1-4-1-0;1-5-1-0;1-6-1-0;1-7-1-0;" + 
			"0-0-6-0;0-1-6-0;0-2-6-0;0-3-6-0;0-4-6-0;0-5-6-0;0-6-6-0;0-7-6-0;" + 
			"0-0-7-3;0-7-7-3;0-1-7-2;0-6-7-2;0-2-7-1;0-5-7-1;0-3-7-5;0-4-7-4;";

		var temp = mapData.Split (';');
		foreach (var item in temp) 
		{
			if (item.Length > 0) 
			{
				ChessBase newChess = new ChessBase ();
				var parts = item.Split ('-');
				newChess.IsWhite = parts [0] == "0" ? ChessSide.White : ChessSide.Black;
				newChess.Position = new Vector2(int.Parse(parts[1]),int.Parse(parts[2]));
				newChess.Type = (ChessPieceType)(int.Parse (parts [3]) + 1);
				int index = newChess.IsWhite == ChessSide.White ? 1 : 2;
				_BoardState[(int)newChess.Position.x + 1, (int)newChess.Position.y + 1] = (int)newChess.Type * 10 + index;
				AllChess.Add (newChess);
			}
		}

		currentMap = new MapData ();
		currentMap.IsWhiteFirst = true;
		currentMap.CurrentTime = 0;
		currentMap.ListAllChess.AddRange (AllChess);

		foreach (var cell in AllCell)
			cell.ClearView ();

		foreach (var item in currentMap.ListAllChess) 
		{
			foreach (var cell in AllCell) 
			{
				if ((int)item.Position.x == cell.PosX && (int)item.Position.y == cell.PosY) 
				{
					cell.SetView (item);
				}
			}
		}

		arrFEN = new ArrayList();
		arrFEN.Add(clsFEN.GetPiecePlacementString(this._BoardState));
	}

	public void SelectingThisCell(CellBase cell)
	{

		if (CurrentStatus != GameStatus.NowPlaying || isThinking)
			return;

		if (CurrentGameMode == GameMode.TwoPlayer || isWhite == ChessSide.White) 
		{
			if (cell.CurrentType != null && cell.CellStatus == CellBase.CellType.None && cell.CurrentType.IsWhite == isWhite) {
				if (preCell != null) {
					foreach (var item in TempCell) {
						item.ClearView ();
					}
				}
				TempCell.Clear ();
				Selecting.color = new Color (1, 1, 1, 1);
				Selecting.transform.position = cell.gameObject.transform.position;
				isSelecting = true;
				var temp = FindAllPossibleMove (cell);
				ShowTempMove (cell, temp);
				preCell = cell;
			} 
			else if (cell.CurrentType != null && cell.CellStatus == CellBase.CellType.Temp && cell.CurrentType.IsWhite == isWhite) 
			{
				ChessBase chess = cell.CurrentType;
				foreach (var item in TempCell) 
				{
					item.ClearView ();
				}
				TempCell.Clear ();

				if (preCell != null && preCell.CurrentType != null) 
				{
					LastMove = new clsMove (new Vector2(preCell.PosX + 1, preCell.PosY + 1),
						new Vector2(cell.PosX + 1, cell.PosY + 1));
					int index2 = preCell.CurrentType.IsWhite == ChessSide.White ? 1 : 2;
					_BoardState [(int)preCell.CurrentType.Position.x + 1, (int)preCell.CurrentType.Position.y + 1] = 0;
					preCell.ClearPreCell ();
					preCell.ClearView ();
					preCell = null;
				}

				cell.SetView (chess);
				Selecting.color = new Color (1, 1, 1, 0);

				arrMove = clsChessEngine.FindAllLegalMove(_BoardState, MyMove.CurPos, cell.CurrentType.Type);

				if (_BoardState [(int)cell.CurrentType.Position.x + 1, (int)cell.CurrentType.Position.y + 1] / 10 == 6) 
				{
					EndGame ();
				} 
				else 
				{
					int index = cell.CurrentType.IsWhite == ChessSide.White ? 1 : 2;
					_BoardState [(int)cell.CurrentType.Position.x + 1, (int)cell.CurrentType.Position.y + 1] = (int)cell.CurrentType.Type * 10 + index;
					isWhite = isWhite == ChessSide.Black ? ChessSide.White : ChessSide.Black;
				}
				arrFEN.Add(clsFEN.GetPiecePlacementString(this._BoardState));
			}
		} 
		else 
		{
			ComputerMove ();
		}
	}

	public void UndoClick ()
	{
		isUndo = true;
		arrFEN.RemoveAt(arrFEN.Count - 1);
	}

	private void ComputerMove()
	{
		isThinking = true;
		StartCoroutine (ComputerThinking());
	}

	IEnumerator ComputerThinking()
	{
		string strFen = clsFEN.GetFENWithoutMoveNumber (this);
		MyMove = clsChessEngine.ReadFromBook(strFen);
		if (MyMove == null)
		{
			MyMove = new clsMove(new Vector2(), new Vector2());
			arrMove = clsChessEngine.GenerateMove(this._BoardState, this.arrFEN, ChessSide.Black, ref MyMove, Difficult);
			if (Difficult == GameDifficulty.Hard)
				clsChessEngine.WriteToBook(strFen, MyMove);
		}
		else
		{
			ChessPieceType eType = (ChessPieceType)(this._BoardState[(int)MyMove.CurPos.x, (int)MyMove.CurPos.y] / 10);
			arrMove = clsChessEngine.FindAllLegalMove(this._BoardState, MyMove.CurPos, eType);
		}
		LastMove = MyMove;
		yield return null;
		AiMove ();
		Debug.LogError (MyMove.CurPos + "   " + MyMove.NewPos);
		isThinking = false;
		isWhite = ChessSide.White;
		arrFEN.Add(clsFEN.GetPiecePlacementString(this._BoardState));
	}

	private void AiMove ()
	{
		if(MyMove != null) 
		{
			CellBase curCell = new CellBase(), newCell = new CellBase();
			foreach(var item in AllCell)
			{
				if (item.PosX == (int)MyMove.CurPos.x - 1 && item.PosY == 8 - (int)MyMove.CurPos.y) 
				{
					curCell = item;
				}

				if (item.PosX == (int)MyMove.NewPos.x - 1 && item.PosY == 8 - (int)MyMove.NewPos.y) 
				{
					newCell = item;
				}
			}

			if (curCell.CurrentType != null) 
			{
				newCell.SetView (curCell.CurrentType);
				_BoardState [(int)curCell.CurrentType.Position.x + 1, (int)curCell.CurrentType.Position.y + 1] = 0;
				curCell.ClearView ();
				_BoardState [(int)newCell.CurrentType.Position.x + 1, (int)newCell.CurrentType.Position.y + 1] = (int)newCell.CurrentType.Type * 10 + 1;
			}
		}
	}

	private void ShowTempMove(CellBase cell, ArrayList array)
	{
		if (cell.CurrentType != null) 
		{
			foreach (var item in array) {
				foreach (var cellTemp in AllCell) {
					if (item.ToString() == (new Vector2(cellTemp.PosX + 1, cellTemp.PosY + 1)).ToString()) {
						cellTemp.TempView (cell.CurrentType);
						TempCell.Add (cellTemp);
						break;
					}
				}
			}
		}
	}

	private ArrayList FindAllPossibleMove(CellBase cell)
	{	
		if (cell.CurrentType != null) 
		{
			switch ((int)cell.CurrentType.Type) 
			{
			case 1:
				return clsPawn.FindAllPossibleMove (_BoardState, new Vector2 (cell.PosX + 1, cell.PosY + 1));
			case 2:
				return clsBishop.FindAllPossibleMove (_BoardState, new Vector2 (cell.PosX + 1, cell.PosY + 1));
			case 3:
				return clsKnight.FindAllPossibleMove (_BoardState, new Vector2 (cell.PosX + 1, cell.PosY + 1));
			case 4:
				return clsRook.FindAllPossibleMove (_BoardState, new Vector2 (cell.PosX + 1, cell.PosY + 1));
			case 5:
				return clsQueen.FindAllPossibleMove (_BoardState, new Vector2 (cell.PosX + 1, cell.PosY + 1));
			case 6:
				return clsKing.FindAllPossibleMove (_BoardState, new Vector2 (cell.PosX + 1, cell.PosY + 1));
				default: break;
			}
		}
		return new ArrayList ();
	}

	private void InitBoardState()
	{
		/*
            * Qui ước:             
            *  + Cạnh bàn cờ:-1
            *  + Không có quân cờ:0
            *  + Quân Tốt Đen:   11, Quân Tốt Trắng:   12
            *  + Quân Tượng Đen: 21, Quân Tượng Trắng: 22
            *  + Quân Mã Đen:    31, Quân Mã Trắng:    32
            *  + Quân Xe Đen:    41, Quân Xe Trắng:    42
            *  + Quân Hậu Đen:   51, Quân Hậu Trắng:   52
            *  + Quân Vua Đen:   61, Quân Vua Trắng:   62
            * 
            * Phần tử có tọa độ (x,y) có trạng thái là _BoardState[x,y]
            */

		for (int i = 0; i < 10; i++)
		{
			_BoardState[0, i] = -1;
			_BoardState[i, 0] = -1;
			_BoardState[9, i] = -1;
			_BoardState[i, 9] = -1;
		}
		//*****Còn lại là 0********         
	}

	private void EndGame()
	{
		if (isWhite == ChessSide.White)
			CurrentStatus = GameStatus.WhiteWin;
		else
			CurrentStatus = GameStatus.BlackWin;
	}
}
