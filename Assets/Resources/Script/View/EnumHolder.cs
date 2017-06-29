using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStatus
{
	NowPlaying = 0,
	BlackWin = 1,
	WhiteWin = 2,
	Draw = 3
}

public enum ChessSide
{
	Black = 1,//Đen
	White = 2//Trắng
}

public enum ChessPieceType
{
	Pawn = 1,//Tốt
	Bishop = 2,//Tượng
	Knight = 3,//Mã
	Rook = 4,//Xe
	Queen = 5,//Hậu
	King = 6,//Vua
	Null = 7
}

public enum GameDifficulty
{
	Easy = 1,
	Normal = 2,
	Hard = 3
}
