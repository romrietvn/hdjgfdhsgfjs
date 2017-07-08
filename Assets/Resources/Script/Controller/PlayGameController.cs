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

	public GameMode CurrentGameMode = GameMode.TwoPlayer;

	public CanvasGroup Canvas;
	public GameObject Board;
	public Image Selecting;
	public List<Sprite> ListChess = new List<Sprite> ();
	public List<int> AddPosY = new List<int>();

	private List<CellBase> AllCell = new List<CellBase> ();
	private List<ChessBase> AllChess = new List<ChessBase>();
	private List<CellBase> TempCell = new List<CellBase>();
	private CellBase preCell, currentCell;

	private MapData currentMap;
	public ChessSide isWhite = ChessSide.Black;
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
	public GameDifficulty Difficult = GameDifficulty.Normal;

	public bool kingsideCastling = true;//Nhập thành gần, quân đen
	public bool queensideCastling = true;//Nhập thành xa, quân đen

	public bool KINGsideCastling = true;//Nhập thành gần, quân trắng        
	public bool QUEENsideCastling = true;//Nhập thành xa, quân trắng
	public int CurrentLevel = 0;
	public List<Vector2> CurrentMove = new List<Vector2>();
	public List<Vector2> NextMove = new List<Vector2>();

	void Awake() 
	{
		GetAllCell ();
	}

	public void ShowScreen()
	{
		Canvas.alpha = 1;
		Canvas.blocksRaycasts = true;
		CurrentGameMode = SceneManager.instance.ModeGamePlay;
		Difficult = SceneManager.instance.Difficult;
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
		while (clsChessEngine.isFirstTime) 
		{
			CurrentStatus = GameStatus.Pause;
		}
		CurrentStatus = GameStatus.NowPlaying;
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
		CurrentMove.Clear ();
		NextMove.Clear ();
		for (int i = 0; i < 10; i++)
			for (int j = 0; j < 10; j++)
				_BoardState [i , j] = 0;
//		Pawn = 0,//Tốt
//		Bishop = 1,//Tượng
//		Knight = 2,//Mã
//		Rook = 3,//Xe
//		Queen = 4,//Hậu
//		King = 5,//Vua
		string mapData = "";
		CurrentLevel = SceneManager.instance.CurrentLevel;
		if (CurrentLevel < SceneManager.instance.LevelData.Count)
			mapData = SceneManager.instance.LevelData [CurrentLevel];
		else mapData = SceneManager.instance.LevelData [0];

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
				int index = newChess.IsWhite == ChessSide.White ? 2 : 1;
				_BoardState[ 8 - (int)newChess.Position.x, (int)newChess.Position.y + 1] = (int)newChess.Type * 10 + index;
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

		if (CurrentGameMode != GameMode.Puzzle) 
		{
			kingsideCastling = true;//Nhập thành gần, quân đen
			queensideCastling = true;//Nhập thành xa, quân đen

			KINGsideCastling = true;//Nhập thành gần, quân trắng        
			QUEENsideCastling = true;//Nhập thành xa, quân trắng
		} 
		else 
		{
			kingsideCastling = false;//Nhập thành gần, quân đen
			queensideCastling = false;//Nhập thành xa, quân đen

			KINGsideCastling = false;//Nhập thành gần, quân trắng        
			QUEENsideCastling = false;//Nhập thành xa, quân trắng
		}
		clsFEN.SetFEN(this, "");

		arrFEN = new ArrayList();
		arrFEN.Add(clsFEN.GetPiecePlacementString(this._BoardState));
	}

	public void SelectingThisCell(CellBase cell)
	{

		if (CurrentStatus != GameStatus.NowPlaying || isThinking || isUndo)
			return;

		if (CurrentGameMode == GameMode.TwoPlayer || isWhite == ChessSide.Black) 
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
					_BoardState [ 8 - (int)preCell.PosX, (int)preCell.PosY + 1] = 0;
					preCell.ClearPreCell ();
					preCell.ClearView ();

					CurrentMove.Add (new Vector2(preCell.PosX, preCell.PosY));
					NextMove.Add (new Vector2(cell.PosX, cell.PosY));

					preCell = null;
				}

				cell.SetView (chess);
				Selecting.color = new Color (1, 1, 1, 0);

				//arrMove = clsChessEngine.FindAllLegalMove(_BoardState, MyMove.CurPos, cell.CurrentType.Type);
				currentCell = cell;
				if (_BoardState [8 - (int)cell.PosX, (int)cell.PosY + 1] / 10 == 6) 
				{
					EndGame ();
				} 
				else 
				{
					int index = cell.CurrentType.IsWhite == ChessSide.White ? 2 : 1;
					int X = 8 - (int)cell.PosX;
					int Y = (int)cell.PosY + 1;
					_BoardState [ X, Y] = (int)cell.CurrentType.Type * 10 + index;

					if (currentCell.CurrentType.Position.y == 0 && currentCell.CurrentType.Type == ChessPieceType.Pawn) 
					{
						//PromotePawn (4);
						Debug.LogError("Show popup chon co de phong cap \n 1: Tinh - 2: Ma - 3: Xe - 4: Hau");
					}

					isWhite = isWhite == ChessSide.Black ? ChessSide.White : ChessSide.Black;

					arrFEN.Add(clsFEN.GetPiecePlacementString(this._BoardState));
					if (CurrentGameMode != GameMode.TwoPlayer) {
						ComputerMove ();
					}
				}
			}
		} 
	}

	public void UndoClick ()
	{
		if (CurrentGameMode == GameMode.Puzzle && isWhite == ChessSide.Black && CurrentMove.Count > 0 && !isUndo) 
		{
			isUndo = true;
			StartCoroutine (delayUndo());
		}
	}

	IEnumerator delayUndo() 
	{
		Undo ();
		arrFEN.RemoveAt (arrFEN.Count - 1);
		yield return new WaitForSeconds (0.5f);
		Undo ();
		arrFEN.RemoveAt (arrFEN.Count - 1);
		isUndo = false;
	}

	private void Undo()
	{
		if (CurrentMove.Count > 0) 
		{
			int X = (int)CurrentMove [CurrentMove.Count - 1].x;
			int Y = (int)CurrentMove [CurrentMove.Count - 1].y;
			CurrentMove.RemoveAt (CurrentMove.Count - 1);

			int X2 = (int)NextMove [NextMove.Count - 1].x;
			int Y2 = (int)NextMove [NextMove.Count - 1].y;
			NextMove.RemoveAt (NextMove.Count - 1);

			CellBase curCell = new CellBase(), newCell = new CellBase();
			foreach(var item in AllCell)
			{
				if (item.PosX == X && item.PosY == Y) 
				{
					curCell = item;
				}

				if (item.PosX == X2 && item.PosY == Y2) 
				{
					newCell = item;
				}
			}

			if (newCell.CurrentType != null) 
			{
				curCell.SetView (newCell.CurrentType);

				_BoardState [8 - (int)curCell.PosX, (int)curCell.PosY + 1] = 0;

				_BoardState [8 - (int)newCell.CurrentType.Position.x, (int)newCell.CurrentType.Position.y + 1] = (int)newCell.CurrentType.Type * 10 + 2;
				newCell.ClearPreCell ();
				newCell.ClearView ();
//						Debug.LogError (MyMove.CurPos + "   " + MyMove.NewPos);
//						for (int i = 0; i < 10; i++) {
//							string tmp = "";
//							for (int j = 0; j < 10; j++)
//								tmp += _BoardState [i, j] + " ";
//							Debug.LogError (tmp);
//						}
			}
		}
	}

	private void ComputerMove()
	{
		isThinking = true;
		StartCoroutine (ComputerThinking());
	}

	IEnumerator ComputerThinking()
	{
		string strFen = clsFEN.GetFENWithoutMoveNumber (this);
		MyMove = null;
		yield return MyMove = clsChessEngine.ReadFromBook(strFen);
		Debug.LogError (strFen);
		if (MyMove == null)
		{
			MyMove = new clsMove(new Vector2(), new Vector2());
			arrMove = clsChessEngine.GenerateMove(this._BoardState, this.arrFEN, ChessSide.Black, ref MyMove, Difficult);
			Debug.LogError (MyMove.CurPos + "   " + MyMove.NewPos);

//			if (Difficult == GameDifficulty.Hard)
//				clsChessEngine.WriteToBook(strFen, MyMove);
		}
		else
		{
			ChessPieceType eType = (ChessPieceType)(this._BoardState[(int)MyMove.CurPos.x, (int)MyMove.CurPos.y] / 10);
			arrMove = clsChessEngine.FindAllLegalMove(this._BoardState, MyMove.CurPos, eType);
		}
		LastMove = MyMove;
		while (clsChessEngine.isProcessing)
			yield return new WaitForEndOfFrame();
		AiMove ();
//		Debug.LogError (MyMove.CurPos + "   " + MyMove.NewPos);
//		for (int i = 0; i < 10; i++) {
//			string tmp = "";
//			for (int j = 0; j < 10; j++)
//				tmp += _BoardState [i, j] + " ";
//			Debug.LogError (tmp);
//		}
		isThinking = false;
		isWhite = ChessSide.Black;
		arrFEN.Add(clsFEN.GetPiecePlacementString(this._BoardState));
	}

	private void AiMove ()
	{
		if(MyMove != null) 
		{
			CellBase curCell = new CellBase(), newCell = new CellBase();
			foreach(var item in AllCell)
			{
				if (item.PosX == 8 - (int)MyMove.CurPos.x && item.PosY == (int)MyMove.CurPos.y - 1) 
				{
					curCell = item;
				}

				if (item.PosX == 8 - (int)MyMove.NewPos.x && item.PosY == (int)MyMove.NewPos.y - 1) 
				{
					newCell = item;
				}
			}

			if (curCell.CurrentType != null && curCell.CurrentType.IsWhite == ChessSide.White) 
			{
				newCell.SetView (curCell.CurrentType);
				_BoardState [8 - (int)curCell.PosX, (int)curCell.PosY + 1] = 0;
				curCell.ClearPreCell ();
				curCell.ClearView ();

				CurrentMove.Add (new Vector2(curCell.PosX, curCell.PosY));
				NextMove.Add (new Vector2(newCell.PosX, newCell.PosY));
				currentCell = newCell;
				_BoardState [8 - (int)newCell.CurrentType.Position.x, (int)newCell.CurrentType.Position.y + 1] = (int)newCell.CurrentType.Type * 10 + 2;

				if (newCell.CurrentType.Position.y == 7 && newCell.CurrentType.Type == ChessPieceType.Pawn) 
				{
					PromotePawn (4);
				}
			}
		}
	}

	private void ShowTempMove(CellBase cell, ArrayList array)
	{
		if (cell.CurrentType != null) 
		{
			foreach (var item in array) {
				foreach (var cellTemp in AllCell) {
					if (item.ToString() == (new Vector2( 8 - cellTemp.PosX, cellTemp.PosY + 1)).ToString()) {
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
				return clsPawn.FindAllPossibleMove (_BoardState, new Vector2 (8 - cell.PosX, cell.PosY + 1));
			case 2:
				return clsBishop.FindAllPossibleMove (_BoardState, new Vector2 (8 - cell.PosX, cell.PosY + 1));
			case 3:
				return clsKnight.FindAllPossibleMove (_BoardState, new Vector2 (8 - cell.PosX, cell.PosY + 1));
			case 4:
				return clsRook.FindAllPossibleMove (_BoardState, new Vector2 (8 - cell.PosX, cell.PosY + 1));
			case 5:
				return clsQueen.FindAllPossibleMove (_BoardState, new Vector2 (8 - cell.PosX, cell.PosY + 1));
			case 6:
				return clsKing.FindAllPossibleMove (_BoardState, new Vector2 (8 - cell.PosX, cell.PosY + 1));
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
		Debug.LogError ("End Game");
		if (isWhite == ChessSide.White)
			CurrentStatus = GameStatus.WhiteWin;
		else
			CurrentStatus = GameStatus.BlackWin;
	}

	public void PromotePawn(int type) 
	{
		ChessPieceType newType = (ChessPieceType)(type + 1);
		if (currentCell != null && currentCell.CurrentType != null) 
		{
			currentCell.CurrentType.Type = newType;
			currentCell.SetView (currentCell.CurrentType);
			int index = currentCell.CurrentType.IsWhite == ChessSide.White ? 2 : 1;
			_BoardState [8 - (int)currentCell.CurrentType.Position.x, (int)currentCell.CurrentType.Position.y + 1] = (int)currentCell.CurrentType.Type * 10 + index;
		}
	}
}
