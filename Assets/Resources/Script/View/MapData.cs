using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData 
{
	public bool IsWhiteFirst;
	public List<ChessBase> ListAllChess = new List<ChessBase>();
	public long CurrentTime = 0;

	public MapData()
	{
		
	}
}
