using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.IO;
using UnityEngine;

namespace Chess_Usercontrol
{
	public static class clsChessEngine
	{
		private enum PieceValue
		{
			Pawn = 100,
			Bishop = 330,
			Knight = 320,
			Rook = 500,
			Queen = 900,
			King = 10000
		}

		static int MaxDepth = 5;
		static ChessSide Myside, OppSide;
		static clsMove MyBestMove = new clsMove();
		// static long NodeCount = 0;

		public static IEnumerator InitBook()
		{
			TextAsset leveltext = Resources.Load("book") as TextAsset;
			textInBook = leveltext.text;
			lineText = textInBook.Split(new [] { '\r', '\n' });
			yield return null;
			isFirstTime = false;
		}

		#region "Game Logic"
		public static ArrayList FindAllPossibleMove(int[,] arrState, Vector2 CurPos, ChessPieceType eType)
		{
			ArrayList arrMove = new ArrayList();

			if (eType == ChessPieceType.Pawn)
			{
				arrMove = clsPawn.FindAllPossibleMove(arrState, CurPos);
			}
			if (eType == ChessPieceType.Knight)
			{
				arrMove = clsKnight.FindAllPossibleMove(arrState, CurPos);
			}

			if (eType == ChessPieceType.Bishop)
			{
				arrMove = clsBishop.FindAllPossibleMove(arrState, CurPos);
			}

			if (eType == ChessPieceType.Rook)
			{
				arrMove = clsRook.FindAllPossibleMove(arrState, CurPos);
			}

			if (eType == ChessPieceType.Queen)
			{
				arrMove = clsQueen.FindAllPossibleMove(arrState, CurPos);
			}

			if (eType == ChessPieceType.King)
			{
				arrMove = clsKing.FindAllPossibleMove(arrState, CurPos);
//				ChessSide eSide = (ChessSide)(arrState[(int)CurPos.x, (int)CurPos.y] % 10);
//				clsKing.AddCastlingVector2(arrState, eSide, arrMove);

			}
			return arrMove;
		}

		/// <summary>
		/// Trả về độ di động của Side cần kiểm tra
		/// </summary>
		static int Mobility(ChessSide eSide, int[,] BoardState)
		{

			int intSide = 0;
			if (eSide == ChessSide.White)
			{
				intSide = 2;
			}
			else
			{
				intSide = 1;
			}
			int intMobility = 0;
			for (int y = 1; y <= 8; y++)
				for (int x = 1; x <= 8; x++)
					if (BoardState[x, y] > 0)
					{
						int side = BoardState[x, y] % 10;
						if (side == intSide)
						{
							int intType = BoardState[x, y] / 10;
							intMobility += FindAllLegalMove(BoardState, new Vector2(x, y), (ChessPieceType)intType).Count;
						}
					}
			return intMobility;
		}

		/// <summary>
		/// Trả về độ di động của phe ta trừ phe địch
		/// </summary>
		static int Mobility(int[,] BoardState)
		{

		    int intSide = 0;
		    if (Myside == ChessSide.White)
		    {
		        intSide = 2;
		    }
		    else
		    {
		        intSide = 1;
		    }
		    int intMobility = 0;
		    for (int y = 1; y <= 8; y++)
		        for (int x = 1; x <= 8; x++)
		            if (BoardState[x, y] > 0)
		            {
		                int side = BoardState[x, y] % 10;

		                int intType = BoardState[x, y] / 10;
		                if (side == intSide)
		                {
							intMobility += FindAllLegalMove(BoardState, new Vector2(x, y), (ChessPieceType)intType).Count;
		                }
		                else
		                {
							intMobility -= FindAllLegalMove(BoardState, new Vector2(x, y), (ChessPieceType)intType).Count;
		                }
		            }
		    return intMobility;
		}

		public static ArrayList FindAllLegalMove(int[,] arrState, Vector2 CurPos, ChessPieceType eType)
		{
			ArrayList arrPossibleMove = FindAllPossibleMove(arrState, CurPos, eType);

			if (arrPossibleMove.Count == 0)
				return arrPossibleMove;

			ArrayList arrLegalMove = new ArrayList();

			//Những nước đi làm cho quân Vua phe mình bị chiếu được xem là không hợp lệ
			int[,] arrNewState = new int[10, 10];
			Array.Copy(arrState, arrNewState, arrState.Length);
			ChessSide eSide = (ChessSide)(arrState[(int)CurPos.x, (int)CurPos.y] % 10);
			foreach (Vector2 p in arrPossibleMove)
			{
				int tmp = arrNewState[(int)p.x, (int)p.y];//Quân cờ tại vị trí mới
				arrNewState[(int)p.x, (int)p.y] = (int)eType * 10 + (int)eSide;//Thay quân cờ tại vị trí mới
				arrNewState[(int)CurPos.x, (int)CurPos.y] = 0;//Xóa quân cờ tại vị trí cũ

				if (clsKing.IsChecked(arrNewState, eSide) == false)
				{
					arrLegalMove.Add(p);
				}
				arrNewState[(int)CurPos.x, (int)CurPos.y] = arrNewState[(int)p.x, (int)p.y];//Cho quân cờ quay lại vị trí cũ
				arrNewState[(int)p.x, (int)p.y] = tmp;//Trả lại quân cờ nằm ở vị trí mới    
			}
			return arrLegalMove;
		}

		public static bool CanMove(ArrayList arrLegalMove, Vector2 NewPos)
		{

			foreach (Vector2 p in arrLegalMove)//Kiểm tra vị trí cần đến có trong danh sách có thể đi hay không
			{
				if (p == NewPos)
				{
					return true;
				}
			}
			return false;
		}
		/*
         *Hàm kiểm tra 1 phe có còn Nước Đi Hợp Lệ hay không
         * 1. Duyệt tất cả các quân cờ.
	     * 2. Với mỗi quân cờ tìm tất cả các bước đi hợp lệ
         * 3. Nếu tồn tại ít nhất 1 nước đi hợp lệ thì trả vè true còn lại trả về false
         */
		public static bool LegalMoveAvaiable(int[,] arrState, ChessSide eSide)
		{
			int intSide = (int)eSide;
			for (int y = 1; y <= 8; y++)
				for (int x = 1; x <= 8; x++)
				{
					if (arrState[x, y] > 0 && arrState[x, y] % 10 == intSide)
					{
						ChessPieceType eType = (ChessPieceType)(arrState[x, y] / 10);
						if (FindAllLegalMove(arrState, new Vector2(x, y), eType).Count > 0)
							return true;
					}
				}
			return false;
		}
		//Kiểm tra Chiếu Bí, Hòa Cờ(chưa xét trường hợp bất biến 3 lần) 
		//1. Kiểm tra quân đối phương có còn nước đi hợp lệ hay không nếu không ->2.
		//2. Kiểm tra quân vua đối phương có bị chiếu hay không nếu có-> THUA, nếu không ->HÒA

		//code change 30/07/2010 sửa lại đối số để phục vụ việc kiểm tra bất biến 3 lần.
		public static GameStatus GetGameStatus(int[,] arrState,ArrayList arrFEN, ChessSide eSide)//eSide: phe của quân vừa di chuyển
		{
			ChessSide eOppSide;//Phe đối phương

			if (eSide == ChessSide.White)
				eOppSide = ChessSide.Black;
			else
				eOppSide = ChessSide.White;

			if(CheckThreefoldRepetition (arrFEN ))
				return GameStatus.Draw;

			if (CheckInsufficientMaterial(arrState, eSide) == true)
				return GameStatus.Draw;

			if (LegalMoveAvaiable(arrState, eOppSide) == true)//Phe đối phương còn nước đi hợp lệ
			{
				return GameStatus.NowPlaying;
			}
			else
			{
				if (clsKing.IsChecked(arrState, eOppSide) == false)//Quân vua dối phương không bị chiếu
				{
					return GameStatus.Draw;
				}
				else
				{
					if (eSide == ChessSide.Black)
						return GameStatus.BlackWin;
					else
						return GameStatus.WhiteWin;
				}
			}
		}
		//Kiểm tra truòng hợp hòa cờ do cả 2 bên không đủ quân để chiếu bí đối phương

		public static bool CheckInsufficientMaterial(int[,] BoardState, ChessSide eSide)
		{
			bool insufficientMaterial = true;
			int intSide = 0;
			if (eSide == ChessSide.White)
			{
				intSide = 2;
			}
			else
			{
				intSide = 1;
			}

			int MyKnightCount = 0;
			int OppKnightCount = 0;

			bool MyBlackBishopAvailable = false;
			bool MyWhiteBishopAvailable = false;
			bool OppBlackBishopAvailable = false;
			bool OppWhiteBishopAvailable = false;

			for (int y = 1; y <= 8; y++)
				for (int x = 1; x <= 8; x++)
					if (BoardState[x, y] > 0)
					{
						int side = BoardState[x, y] % 10;

						if (side == intSide)
						{
							int intType = BoardState[x, y] / 10;

							switch (intType)
							{
							case 1: insufficientMaterial = false; break;
							case 2: if ((x + y) % 2 == 1) MyWhiteBishopAvailable = true; else MyBlackBishopAvailable = true; break;
							case 3: MyKnightCount++; break;
							case 4: insufficientMaterial = false; break;
							case 5: insufficientMaterial = false; break;
							}
						}
						else
						{
							int intType = BoardState[x, y] / 10;
							switch (intType)
							{
							case 1: insufficientMaterial = false; break;
							case 2: if ((x + y) % 2 == 1) OppWhiteBishopAvailable = true; else OppBlackBishopAvailable = true; break;
							case 3: OppKnightCount++; break;
							case 4: insufficientMaterial = false; break;
							case 5: insufficientMaterial = false; break;
							}
						}
					}
			if (OppKnightCount > 1 || MyKnightCount > 1)
				insufficientMaterial = false;
			if ((MyKnightCount >= 1 && (MyBlackBishopAvailable || MyWhiteBishopAvailable)) || (OppKnightCount >= 1 && (OppBlackBishopAvailable || OppWhiteBishopAvailable)))
				insufficientMaterial = false;
			if (MyBlackBishopAvailable && MyWhiteBishopAvailable)
			{
				insufficientMaterial = false;
			}
			if (OppBlackBishopAvailable && OppWhiteBishopAvailable)
			{
				insufficientMaterial = false;
			}

			if ((OppWhiteBishopAvailable && MyBlackBishopAvailable) || (OppBlackBishopAvailable && MyWhiteBishopAvailable))
				insufficientMaterial = false;

			return insufficientMaterial;
		}

		//Code change ngày 29/07/2010
		//Kiểm tra trường hợp hòa cờ do bất biến 3 lần
		//arrFen chứa danh sách các FEN, mỗi khi trạng thái bàn cờ thay đổi thì trạng thái mới sẽ lưu vào ArrFEN
		public static bool CheckThreefoldRepetition(ArrayList arrFEN)
		{
			int n=arrFEN.Count;
			for (int i = 0; i < n - 1; i++)
			{
				int count = 1;
				string s = arrFEN[i].ToString();
				for (int j = i + 1; j < n; j++)
				{
					if (s == arrFEN[j].ToString())
					{
						count++;                        
					}

				}
				if (count >= 3)
				{                    
					return true;                    
				}
			}
			return false;
		}

		#endregion

		#region "Game AI"

		#region "Giải Thuật MiniMax"
		#region "Evalute(hàm đánh giá thế cờ)"

		//Code change ngày 30/07/2010
		//Thêm đối số để phục vụ cho việc xác định các nước đi bất biến 3 lần
		private static int Evaluate(int[,] BoardState,ArrayList arrFEN)
		{
			if (CheckThreefoldRepetition(arrFEN))
				return 0;

			int intSide = 0;
			ChessSide eSide = Myside;
			ChessSide s = OppSide;
			bool insufficientMaterial = true;
			if (eSide == ChessSide.White)
			{
				intSide = 2;
			}
			else
			{
				intSide = 1;
			}
			int value = 0;

			int MyKnightCount = 0;
			int OppKnightCount = 0;

			bool bEndGame = IsEndGame(BoardState, Myside);

			bool MyBlackBishopAvailable = false;
			bool MyWhiteBishopAvailable = false;
			bool OppBlackBishopAvailable = false;
			bool OppWhiteBishopAvailable = false;

			/*
            * Nếu x+y là số lẻ thì đó là ô Trắng
            * Nếu x+y là số chẵn thì đó là ô Đen
            */

			for (int y = 1; y <= 8; y++)
				for (int x = 1; x <= 8; x++)
					if (BoardState[x, y] > 0)
					{
						int side = BoardState[x, y] % 10;

						if (side == intSide)
						{
							int intType = BoardState[x, y] / 10;

							switch (intType)
							{
							case 1: value += (int)PieceValue.Pawn; value += clsPawn.GetPositionValue(new Vector2(x, y), Myside); insufficientMaterial = false; break;
							case 2: value += (int)PieceValue.Bishop; if ((x + y) % 2 == 1) MyWhiteBishopAvailable = true; else MyBlackBishopAvailable = true; value += clsBishop.GetPositionValue(new Vector2(x, y), Myside); break;
							case 3: value += (int)PieceValue.Knight; value += clsKnight.GetPositionValue(new Vector2(x, y), Myside); MyKnightCount++; break;
							case 4: value += (int)PieceValue.Rook; value += clsRook.GetPositionValue(new Vector2(x, y), Myside); insufficientMaterial = false; break;
							case 5: value += (int)PieceValue.Queen; value += clsQueen.GetPositionValue(new Vector2(x, y), Myside); insufficientMaterial = false; break;
							case 6: value += clsKing.GetPositionValue(new Vector2(x, y), Myside, bEndGame); break;

							}
						}
						else
						{
							int intType = BoardState[x, y] / 10;
							switch (intType)
							{
							case 1: value -= (int)PieceValue.Pawn; value -= clsPawn.GetPositionValue(new Vector2(x, y), OppSide); insufficientMaterial = false; break;
							case 2: value -= (int)PieceValue.Bishop; if ((x + y) % 2 == 1) OppWhiteBishopAvailable = true; else OppBlackBishopAvailable = true; value -= clsBishop.GetPositionValue(new Vector2(x, y), OppSide); break;
							case 3: value -= (int)PieceValue.Knight; value -= clsKnight.GetPositionValue(new Vector2(x, y), OppSide); OppKnightCount++; break;
							case 4: value -= (int)PieceValue.Rook; value -= clsRook.GetPositionValue(new Vector2(x, y), OppSide); insufficientMaterial = false; break;
							case 5: value -= (int)PieceValue.Queen; value -= clsQueen.GetPositionValue(new Vector2(x, y), OppSide); insufficientMaterial = false; break;
							case 6: value -= clsKing.GetPositionValue(new Vector2(x, y), OppSide, bEndGame); break;

							}
						}
					}
			/*
             * Việc sở hữu 2 quân tượng sẽ làm tăng lợi thế cho bên sở hữu nó.
             * Bên cạnh đó quân Tượng rất hiệu quả khi EndGame(Tàn Cuộc), còn quân Mã thì ngược lại
             */

			//ván cờ không hòa do insufficientMaterial khi
			//1 trong 2 bên còn Tốt hoặc xe, hoặc Hậu
			//1 trong 2 bên còn 2 mã hoặc 2 tượng khác màu(ô)
			//1 trong 2 bên còn 1 mã và 1 tượng
			//Mỗi bên còn 1 quân tượng khác màu(ô)
			if (OppKnightCount > 1 || MyKnightCount > 1)
				insufficientMaterial = false;
			if ((MyKnightCount >= 1 && (MyBlackBishopAvailable || MyWhiteBishopAvailable)) || (OppKnightCount >= 1 && (OppBlackBishopAvailable || OppWhiteBishopAvailable)))
				insufficientMaterial = false;
			if (MyBlackBishopAvailable && MyWhiteBishopAvailable)
			{
				value += 10;
				insufficientMaterial = false;
			}
			if (OppBlackBishopAvailable && OppWhiteBishopAvailable)
			{
				value -= 10;
				insufficientMaterial = false;
			}
			if ((OppWhiteBishopAvailable && MyBlackBishopAvailable) || (OppBlackBishopAvailable && MyWhiteBishopAvailable))
				insufficientMaterial = false;

			if (insufficientMaterial == true)
				return 0;

			if (bEndGame == true)
			{
				if (MyKnightCount >= 1)
					value -= 10;
				if (OppKnightCount >= 1)
					value += 10;
				if (MyBlackBishopAvailable || MyWhiteBishopAvailable)
					value += 10;
				if (MyBlackBishopAvailable || MyWhiteBishopAvailable)
					value -= 10;
			}

			//value += Mobility(BoardState);

			return value;
		}
		/* 
        * Speelman considers that endgames are positions in which each player has 13
        * or fewer points in material (not counting the king)- wikipedia
        * 
        * Khi giá trị quân cờ của mỗi đối thủ nhỏ hơn hoặc bằng 13(khoảng 1350 điểm)=>EndGame
        */


		private static bool IsEndGame(int[,] BoardState, ChessSide eMySide)
		{
			int intSide = 0;
			if (eMySide == ChessSide.White)
			{
				intSide = 2;
			}
			else
			{
				intSide = 1;
			}

			int MyScore = 0;
			int OppScore = 0;

			for (int y = 1; y <= 8; y++)
				for (int x = 1; x <= 8; x++)
					if (BoardState[x, y] > 0)
					{
						int side = BoardState[x, y] % 10;

						if (side == intSide)
						{
							int intType = BoardState[x, y] / 10;

							switch (intType)
							{
							case 1: MyScore += (int)PieceValue.Pawn; break;
							case 2: MyScore += (int)PieceValue.Bishop; break;
							case 3: MyScore += (int)PieceValue.Knight; break;
							case 4: MyScore += (int)PieceValue.Rook; break;
							case 5: MyScore += (int)PieceValue.Queen; break;
							}
						}
						else
						{
							int intType = BoardState[x, y] / 10;
							switch (intType)
							{
							case 1: OppScore += (int)PieceValue.Pawn; break;
							case 2: OppScore += (int)PieceValue.Bishop; break;
							case 3: OppScore += (int)PieceValue.Knight; break;
							case 4: OppScore += (int)PieceValue.Rook; break;
							case 5: OppScore += (int)PieceValue.Queen; break;
							}
						}
					}

			return MyScore <= 1350 && OppScore <= 1350;

		}

		#endregion

		#region "Hàm Alpha Beta"


		//Code change ngày 30/07/2010
		//Thêm đối số để phục vụ cho việc xác định các nước đi bất biến 3 lần
		public static int AlphaBeta(int[,] BoardState,ArrayList arrFEN, int depth, ChessSide eSide, int Alpha, int Beta)
		{
			//NodeCount++;
			if (depth == 0)
			{
				return Evaluate(BoardState,arrFEN);
			}

			int best = -100000;
			clsMove bestmove = new clsMove();
			ArrayList arrMoves = Successors(BoardState, eSide);

			if (arrMoves.Count == 0)
			{
				//Quân Vua mình bị chiếu và người hết cờ là mình => trừ điểm mình

				if (clsKing.IsChecked(BoardState, Myside) == true)
				{
					if (eSide == Myside)
						return -10000 - depth;
				}
				//Quân Vua đối phươn bị chiếu và người hết cờ là đối phương => trừ điểm đối phương
				if (clsKing.IsChecked(BoardState, OppSide) == true)
				{
					if (eSide == OppSide)
						return -10000 - depth;
				}

				return 0;
			}
			//Sắp xếp các nước đi nhằm tăng hiệu quả của hàm Alpha Beta
			int intPromotionType = 2;
			if (depth > 1)
			{

				foreach (clsMove m in arrMoves)
				{
					int[,] State = new int[10, 10];
					Array.Copy(BoardState, State, BoardState.Length);

					Vector2 c = m.CurPos;
					Vector2 n = m.NewPos;

					int value = State[(int)c.x, (int)c.y];

					if (value / 10 == 1)
					{
						if (value % 10 == 1 && n.y == 1)
						{
							value = intPromotionType * 10 + 1;
							intPromotionType++;
							State[(int)c.x, (int)c.y] = value;
						}
						if (value % 10 == 2 && n.y == 8)
						{
							value = intPromotionType * 10 + 2;
							intPromotionType++;
							State[(int)c.x, (int)c.y] = value;
						}
					}
					if (intPromotionType == 6)
						intPromotionType = 2;
					TryMove(State, m);
					int Score = AlphaBeta(State,arrFEN , 0, eSide, -Beta, -Alpha);
					m.Score = Score;
				}
				Sort(arrMoves, eSide);
			}
			intPromotionType = 5;


			while (arrMoves.Count > 0 && best < Beta)
			{

				int[,] State = new int[10, 10];
				Array.Copy(BoardState, State, BoardState.Length);


				clsMove Move = (clsMove)arrMoves[arrMoves.Count - 1];

				Vector2 c = Move.CurPos;
				Vector2 n = Move.NewPos;

				int x = State[(int)c.x, (int)c.y];

				if (x / 10 == 1)
				{
					if (x % 10 == 1 && n.y == 1)
					{
						x = intPromotionType * 10 + 1;
						Move.PromoteTo = (ChessPieceType)intPromotionType;
						intPromotionType--;
						State[(int)c.x, (int)c.y] = x;
					}
					if (x % 10 == 2 && n.y == 8)
					{
						x = intPromotionType * 10 + 2;
						Move.PromoteTo = (ChessPieceType)intPromotionType;
						intPromotionType--;
						State[(int)c.x, (int)c.y] = x;
					}
					if (intPromotionType == 1)
						intPromotionType = 5;
				}

				TryMove(State, Move);
				arrMoves.RemoveAt(arrMoves.Count - 1);
				//Đã đi thử xong
				ArrayList arrFENNew = new ArrayList();
				arrFENNew.AddRange(arrFEN);

				ChessSide s;
				if (eSide == ChessSide.White)
					s = ChessSide.Black;
				else
					s = ChessSide.White;

				if (best > Alpha)
					Alpha = best;

				int value = -AlphaBeta(State,arrFENNew , depth - 1, s, -Beta, -Alpha);

				if (value > best)
				{
					best = value;
					bestmove = Move;
				}
			}

			if (depth == MaxDepth)
			{
				bestmove.Score = best;
				MyBestMove = bestmove;
			}

			return best;
		}
		#endregion

		#region "Hàm Tạo Trạng Thái Bàn Cờ Từ 1 Nước Đi Hợp Lệ"
		static void TryMove(int[,] State, clsMove Move)
		{

			Vector2 c = Move.CurPos;
			Vector2 n = Move.NewPos;

			int value = State[(int)c.x,(int) c.y];
			//Nhập Thành
			if (value / 10 == 6)
			{
				if (Math.Abs(n.x - c.x) == 2)
				{
					if (n.x == 7)
					{
						int tmp = State[8, (int)n.y];
						State[8, (int)n.y] = 0;
						State[6, (int)n.y] = tmp;
					}
					if (n.x == 3)
					{
						int tmp = State[1, (int)n.y];
						State[1, (int)n.y] = 0;
						State[4, (int)n.y] = tmp;
					}
				}
			}


			State[(int)n.x, (int)n.y] = value;
			State[(int)c.x, (int)c.y] = 0;

		}
		#endregion

		# region "Successors(Hàm Tìm Tất Cả Các Nước Đi Hợp Lệ)"
		 private static ArrayList Successors(int[,] BoardState, ChessSide eSide)
		{

			int intSide = 0;
			if (eSide == ChessSide.White)
			{
				intSide = 1;
			}
			else
			{
				intSide = 2;
			}

			ArrayList arrMoves = new ArrayList();
			for (int y = 1; y <= 8; y++)
				for (int x = 1; x <= 8; x++)
					if (BoardState[x, y] > 0)
					{
						int side = BoardState[x, y] % 10;
						if (side == intSide)
						{
							int intType = BoardState[x, y] / 10;
							ArrayList arr = FindAllLegalMove(BoardState, new Vector2(x, y), (ChessPieceType)intType);
							foreach (Vector2 p in arr)
							{
								clsMove Move = new clsMove(new Vector2(x, y), p);
								arrMoves.Add(Move);
							}
						}
					}
			return arrMoves;
		}

		#endregion

		//sắp xếp các nước đi để tăng hiệu suất hàm alpha beta
		private static void Sort(ArrayList arrMoves, ChessSide eSide)
		{
			int[] a = new int[arrMoves.Count];
			int[] b = new int[arrMoves.Count];

			for (int i = 0; i < arrMoves.Count; i++)
			{
				clsMove m = (clsMove)arrMoves[i];
				a[i] = m.Score;
				b[i] = i;
			}

			QuickSort(a, b, 0, arrMoves.Count - 1);


			ArrayList arr = new ArrayList();
			arr.AddRange(arrMoves);
			if (eSide == OppSide)
				Array.Reverse(b);

			for (int i = 0; i < arr.Count; i++)
			{
				arrMoves[i] = arr[b[i]];
			}

		}
		//Test thử quick sort 
		private static void QuickSort(int[] a, int[] b, int l, int r)
		{
			int i, j;
			int x;
			x = a[(l + r) / 2];
			i = l;
			j = r;
			do
			{
				while (a[i] < x)
					i++;
				while (a[j] > x)
					j--;
				if (i <= j)
				{
					int tmp = a[i];
					a[i] = a[j];
					a[j] = tmp;

					tmp = b[i];
					b[i] = b[j];
					b[j] = tmp;
					i++;
					j--;
				}
			}
			while (i < j);
			if (l < j)
				QuickSort(a, b, l, j);
			if (i < r)
				QuickSort(a, b, i, r);

		}

		#endregion

		#region "Hàm Tạo Nước Đi Hợp Lệ Ngẫu Nhiên"
		public static clsMove RandomMove(int[,] BoardState, ChessSide eSide)
		{
			ArrayList arrMoves = Successors(BoardState, eSide);
			if (arrMoves.Count == 0)
				return null;
			Debug.LogError (111);
			int value = UnityEngine.Random.Range (0, arrMoves.Count);
			return (clsMove)arrMoves[value];
		}
		#endregion

//		#region "Hàm Tạo Nước Đi Hợp Lệ Áp Dụng Giải Thuật MiniMax"
//
//		//Tìm trạng thái bàn cờ trong Opening Book nếu tìm được trong Opening book=> trả về nước đi trong Opening Book
		public static bool isFirstTime = true;
		static string textInBook = "";
		static string[] lineText;
		public static clsMove ReadFromBook(string strFEN)
		{
			if (lineText == null)
				return null;
			try
			{
				foreach(var tmp in lineText)
				{
					if (tmp.Contains(strFEN))
					{
						string[] strMove = tmp.Split(' ');
						int count = strMove.Length;
						count = UnityEngine.Random.Range (0, count);
						string result = strMove[count];

						clsMove Move = new clsMove(result);
						return Move;
					}	
				}
				return null;
			}
			catch (Exception ex)
			{
				return null;
			}

		}
//		//Nếu không tìm được trong Opening Book thì dùng Hàm Tìm Kiếm để tìm nước đi tốt nhất sau đó ghi nước đi này vào Opening Book
		public static void WriteToBook(string strFEN, clsMove Move)
		{
			try
			{
				StreamWriter writer = new StreamWriter(Application.dataPath + "/Resources/book.txt");
				writer.WriteLine(strFEN);
				writer.WriteLine(Move.ToString().ToLower());
				writer.Close();
			}
			catch (Exception ex)
			{
				
			}
		}
		//Code change ngày 30/07/2010
		//Thêm đối số để phục vụ cho việc xác định các nước đi bất biến 3 lần
		public static ArrayList GenerateMove(int[,] BoardState,ArrayList arrFEN, ChessSide eSide, ref clsMove Move, GameDifficulty eDifficult)
		{
			ArrayList arrMove = new ArrayList();
			//NodeCount = 0;         
			MyBestMove = new clsMove();
			if (eDifficult != GameDifficulty.Easy)
			{
				if (eDifficult == GameDifficulty.Normal)
					MaxDepth = 4;//Normal
				else
					MaxDepth = 6;//Hard

				Myside = eSide;
				if (Myside == ChessSide.Black)
					OppSide = ChessSide.White;
				else
					OppSide = ChessSide.Black;


				int alpha = -50000;
				int beta = 50000;

				if (Mobility(Myside, BoardState) + Mobility(OppSide, BoardState) < 30)
					MaxDepth = 6;

				AlphaBeta(BoardState,arrFEN , MaxDepth, eSide, -beta, -alpha);

				Move = MyBestMove;

				if (Move == null)
				{
					Move = RandomMove(BoardState, eSide);
					Move.PromoteTo = ChessPieceType.Queen;
				}
			}
			else
			{
				Move = RandomMove(BoardState, eSide);
				Move.PromoteTo = ChessPieceType.Queen;
			}
			if (Move == null)
				return arrMove;

			ChessPieceType eType = (ChessPieceType)(BoardState[(int)Move.CurPos.x, (int)Move.CurPos.y] / 10);
			arrMove = FindAllLegalMove(BoardState, Move.CurPos, eType);
			//MessageBox.Show(NodeCount.ToString ());
			return arrMove;

		}
	}
	#endregion

}
