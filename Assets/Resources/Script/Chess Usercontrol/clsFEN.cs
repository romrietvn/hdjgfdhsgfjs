﻿using System;
using System.Collections.Generic;
using System.Text;
namespace Chess_Usercontrol
{
    public class clsFEN
    {
        public static string DefaultFENstring = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

		public static string GetFEN(PlayGameController Board)
        {
            string FEN = "";

            string strActiveSide = "";
            string strCastling = "";
            string strEnPassant = "-";
            int intHalfMoveClock = 0;
            int intFullMoveNumBer = 1;

            string strPiecePlacemen = GetPiecePlacementString(Board._BoardState);
			UnityEngine.Debug.LogError (strPiecePlacemen);
			if (Board.isWhite == ChessSide.White)
                strActiveSide = "w";
            else
                strActiveSide = "b";

			if (Board._EnPassantPoint != new UnityEngine.Vector2())
            {
                //strEnPassant = Board.arrChessCell[UcChessBoard._EnPassantPoint.X, UcChessBoard._EnPassantPoint.Y].Name.ToLower();
            }

			if (Board.KINGsideCastling == true)
                strCastling += "K";

			if (Board.QUEENsideCastling == true)
                strCastling += "Q";

			if (Board.kingsideCastling == true)
                strCastling += "k";

			if (Board.queensideCastling == true)
                strCastling += "q";

            if (strCastling == "")
                strCastling = "-";
            FEN = strPiecePlacemen + " " + strActiveSide + " " + strCastling + " " + strEnPassant + " " + intHalfMoveClock + " " + intFullMoveNumBer;

            return FEN;
        }

		public static string GetFENWithoutMoveNumber(PlayGameController Board)
        {
            string FEN = "";

            string strActiveSide = "";
            string strCastling = "";
            string strEnPassant = "-";

            string strPiecePlacemen = GetPiecePlacementString(Board._BoardState);

			if (Board.isWhite == ChessSide.White)
                strActiveSide = "w";
            else
                strActiveSide = "b";

			if (Board._EnPassantPoint != new UnityEngine.Vector2())
            {
				switch ((int)Board._EnPassantPoint.x) 
				{
					case 1:
						strEnPassant = "a";
						break;
					case 2:
						strEnPassant = "b";
						break;
					case 3:
						strEnPassant = "c";
						break;
					case 4:
						strEnPassant = "d";
						break;
					case 5:
						strEnPassant = "e";
						break;
					case 6:
						strEnPassant = "f";
						break;
					case 7:
						strEnPassant = "g";
						break;
					case 8:
						strEnPassant = "h";
						break;
				}
				strEnPassant += (9 - (int)Board._EnPassantPoint.y);
            }

			            if (Board.KINGsideCastling == true)
                strCastling += "K";

			            if (Board.QUEENsideCastling == true)
                strCastling += "Q";

			            if (Board.kingsideCastling == true)
                strCastling += "k";

			            if (Board.queensideCastling == true)
                strCastling += "q";

            if (strCastling == "")
                strCastling = "-";
            FEN = strPiecePlacemen + " " + strActiveSide + " " + strCastling + " " + strEnPassant;

            return FEN;
        }

		public static void SetFEN(PlayGameController Board, string strFEN)
        {
			strFEN = GetFEN(Board);
            strFEN = strFEN.Trim();
            string[] s = strFEN.Split(' ');
            if(s.Length ==1)
            {
                SetPiecePlacement(Board._BoardState, strFEN);
                return;
            }

            string strPiecePlacemen = s[0];
            string strActiveSide = s[1];
            string strCastling = s[2];
            string strEnPassant = s[3];

            SetPiecePlacement(Board._BoardState, strPiecePlacemen);

			Board.isWhite = strActiveSide.ToUpper () == "W" ? ChessSide.White : ChessSide.Black;

			Board.KINGsideCastling = false;
			Board.kingsideCastling = false;
			Board.QUEENsideCastling = false;
			Board.queensideCastling = false;

            if (strCastling != "-")
            {
                foreach (char c in strCastling)
                {
                    switch (c)
                    {
					case 'Q': Board.QUEENsideCastling = true; break;
					case 'K': Board.KINGsideCastling = true; break;

					case 'q': Board.queensideCastling = true; break;
					case 'k': Board.kingsideCastling = true; break;
                    }
                }
            }
//            if (strEnPassant != "-")
//            {
//                //UcChessCell cell = (UcChessCell)Board.Controls.Find(strEnPassant, true)[0];
//                //UcChessBoard._EnPassantPoint = new Point(cell.PositionX, cell.PositionY);
//            }
//            else
//                UcChessBoard._EnPassantPoint = new Point();
			Board._EnPassantPoint = new UnityEngine.Vector2();

        }

        public static void SetPiecePlacement(int[,] BoardState, string strPieceplacement)
        {
            string[] s = strPieceplacement.Split('/');            

            for (int i = 0; i < 8; i++)
            {
                int index = 1;
                foreach (char c in s[i])
                {
                    if (char.IsDigit(c))
                    {
                        int index1 = Convert.ToInt32(c) - 48 + index;
                        for (; index < index1; index++)
                        {
                            BoardState[index, 8 - i] = 0;
                        }
                    }
                    else
                    {
                        switch (c)
                        {
                            case 'P': BoardState[index, 8 - i] = 12; break;
                            case 'B': BoardState[index, 8 - i] = 22; break;
                            case 'N': BoardState[index, 8 - i] = 32; break;
                            case 'R': BoardState[index, 8 - i] = 42; break;
                            case 'Q': BoardState[index, 8 - i] = 52; break;
                            case 'K': BoardState[index, 8 - i] = 62; break;

                            case 'p': BoardState[index, 8 - i] = 11; break;
                            case 'b': BoardState[index, 8 - i] = 21; break;
                            case 'n': BoardState[index, 8 - i] = 31; break;
                            case 'r': BoardState[index, 8 - i] = 41; break;
                            case 'q': BoardState[index, 8 - i] = 51; break;
                            case 'k': BoardState[index, 8 - i] = 61; break;
                        }
                        index++;
                    }
                }
            }

            for (int i = 0; i < 10; i++)
            {
                BoardState[0, i] = -1;
                BoardState[i, 0] = -1;
                BoardState[9, i] = -1;
                BoardState[i, 9] = -1;
            }

        }

        public static string GetPiecePlacementString(int[,] BoardState)
        {
            string strPiecePlacemen = "";
            for (int y = 8; y >= 0; y--)
            {
                int count = 0;
                for (int x = 1; x <= 8; x++)
                {
                    if (BoardState[x, y] == 0)
                        count++;
                    else
                    {
                        if (count > 0)
                        {
                            strPiecePlacemen += count;
                            count = 0;
                        }
                        int type = (BoardState[x, y] / 10);
                        int side = (BoardState[x, y] % 10);

                        if (side == 1)
                        {
                            switch (type)
                            {
                                case 1: strPiecePlacemen += "p"; break;
                                case 2: strPiecePlacemen += "b"; break;
                                case 3: strPiecePlacemen += "n"; break;
                                case 4: strPiecePlacemen += "r"; break;
                                case 5: strPiecePlacemen += "q"; break;
                                case 6: strPiecePlacemen += "k"; break;
                            }
                        }
                        else
                        {
                            switch (type)
                            {
                                case 1: strPiecePlacemen += "P"; break;
                                case 2: strPiecePlacemen += "B"; break;
                                case 3: strPiecePlacemen += "N"; break;
                                case 4: strPiecePlacemen += "R"; break;
                                case 5: strPiecePlacemen += "Q"; break;
                                case 6: strPiecePlacemen += "K"; break;
                            }
                        }
                    }
                }
                if (count > 0)
                {
                    strPiecePlacemen += count;
                    count = 0;
                }
                if (y > 0)
                    strPiecePlacemen += "/";
            }
            return strPiecePlacemen;
        }
    }
}
