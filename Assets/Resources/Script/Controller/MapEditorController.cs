using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapEditorController : MonoBehaviour 
{
	private List<CellBase> AllCell = new List<CellBase> ();
	public GameObject Board;
	public ChessEdit lassCell;

	void Start()
	{
		AllCell.Clear ();
		var listCell = Board.GetComponentsInChildren<CellBase> ();
		foreach (var item in listCell) 
		{
			AllCell.Add (item);
		}
	}

	public void OnCellClick(CellBase cell)
	{
		if (lassCell != null) 
		{
			ChessBase temp = new ChessBase ();
			temp.IsWhite = lassCell.IsWhite ? ChessSide.Black : ChessSide.White;
			temp.Position = new Vector2 (cell.PosX, cell.PosY);
			temp.Type = (ChessPieceType)(lassCell.Type);
			cell.CurrentType = temp;
			lassCell = null;
		}
	}

	public void OnChessClick(ChessEdit cell)
	{
		lassCell = cell;
		if (lassCell == null) 
		{
			cell.OnSelected ();
		}
	}

	public void OnSaveMap()
	{
		try
		{
			string temp ="";
			foreach(var item in AllCell)
			{
				if (item.CurrentType != null)
				{
					if (item.CurrentType.IsWhite == ChessSide.Black)
						temp += "0-";
					else temp += "1-";
					temp += item.PosX + "-" + item.PosY + "-";
					temp += (int)item.CurrentType.Type + ";";
				}
			}

			StreamWriter writer = new StreamWriter(Application.dataPath + "/Resources/level.txt");
			writer.WriteLine(temp);
			writer.Close();
		}
		catch
		{

		}
	}

	public void OnResetMap()
	{
		
	}
}
