using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;

namespace Chess_Usercontrol
{
    public static class clsPawn
    {
        //***Hàm trả về tất cả các nước đi có thể của 1 quân tốt, kể cả nước ăn quân***
        //EnPassant: Bắt Tốt Qua Đường
        private static int[,] PawnTable = new int[,]
		{
          //9  8   7   6   5   4   3   2   1  0
           {0, 0,  0,  0,  0,  0,  0,  0,  0, 0}, //0
           {0, 0,  0,  0,  0,  0,  0,  0,  0, 0}, //1
           {0, 50, 50, 50, 50, 50, 50, 50, 50, 0},//2
           {0, 10, 10, 20, 30, 30, 20, 10, 10, 0},//3
           {0, 5,  5, 10, 27, 27, 10,  5,  5, 0}, //4
           {0,-5, -5,-10, 25, 25, -5, -5,  0, 0}, //5
           {0, 5, -5,-10,  0,  0,-10, -5,  5, 0}, //6
           {0, 5, 10, 10,-25,-25, 10, 10,  5, 0}, //7
           {0, 0,  0,  0,  0,  0,  0,  0,  0, 0}, //8
           {0, 0,  0,  0,  0,  0,  0,  0,  0, 0}  //9
		};
        public static int GetPositionValue(Vector2 pos, ChessSide eSide)
        {
            int value = 0;
            if (eSide == ChessSide.Black)
            {

                value = PawnTable[(int)pos.y, (int)pos.x];
                //Tốt cánh xe bị trừ 15% giá trị
                if ((int)pos.x == 8 || (int)pos.x == 1)
                    value -= 15;
            }
            else
            {

                value = PawnTable[9 - (int)pos.y, 9 - (int)pos.x];
                if ((int)pos.x == 8 || (int)pos.x == 1)
                    value -= 15;
            }
            return value;
        }


        public static ArrayList FindAllPossibleMove(int[,] State, Vector2 pos)//, bool EnPassant)
        {
            //Từ vị trí ban đầu quân tốt có thể di chuyển về phía trước 1 hoặc 2 ô các vị trí còn lại : 1 ô
            //Nước di chuyển 2 ô có thể kích hoạt trạng thái "Bắt Tốt Qua Đường(Enpassant)"
            //Trạng thái Enpassant cỉ có hiệu lực trong 1 Nước cờ
            //(nếu đối phương ko ăn quân trong lượt đó thì trạng thái Enpassant sẽ mất hiệu lực)
            ArrayList arrMove = new ArrayList();
            /*
             * Nếu ô phía trước không bị cản thì có thể di chuyển
             */
            int Side = State[(int)pos.x, (int)pos.y] % 10;//Chẵn(2) là quân trắng, lẻ(1) là quân đen
            if (Side == 2)//Quân Trắng
            {
                if (State[(int)pos.x, (int)pos.y + 1] == 0)
                {
                    arrMove.Add(new Vector2((int)pos.x, (int)pos.y + 1));

					if ((int)pos.y == 2 && State[(int)(int)pos.x, (int)(int)pos.y + 2] == 0)//Vi tri ban dau
                        arrMove.Add(new Vector2((int)pos.x, (int)pos.y + 2));
                    //Phong Cấp
                    if ((int)pos.y == 7)
                    {
                        arrMove.Add(new Vector2((int)pos.x, (int)pos.y + 1));
                        arrMove.Add(new Vector2((int)pos.x, (int)pos.y + 1));
                        arrMove.Add(new Vector2((int)pos.x, (int)pos.y + 1));
                    }
                }
                //Ăn quân
                //Nếu đường chéo ở 2 ô trước mặt là quân đen
				if (State[(int)(int)pos.x - 1, (int)(int)pos.y + 1] % 10 == 1)
                {
                    arrMove.Add(new Vector2((int)pos.x - 1, (int)pos.y + 1));
                    if ((int)pos.y == 7)
                    {
                        arrMove.Add(new Vector2((int)pos.x - 1, (int)pos.y + 1));
                        arrMove.Add(new Vector2((int)pos.x - 1, (int)pos.y + 1));
                        arrMove.Add(new Vector2((int)pos.x - 1, (int)pos.y + 1));
                    }
                }
                if (State[(int)pos.x + 1, (int)pos.y + 1] % 10 == 1)
                {
                    arrMove.Add(new Vector2((int)pos.x + 1, (int)pos.y + 1));
                    if ((int)pos.y == 7)
                    {
                        arrMove.Add(new Vector2((int)pos.x + 1, (int)pos.y + 1));
                        arrMove.Add(new Vector2((int)pos.x + 1, (int)pos.y + 1));
                        arrMove.Add(new Vector2((int)pos.x + 1, (int)pos.y + 1));
                    }
                }


            }
            else if (Side == 1)
            {
				if (State[(int)(int)pos.x, (int)(int)pos.y - 1] == 0)
                {
                    arrMove.Add(new Vector2((int)pos.x, (int)pos.y - 1));

					if ((int)pos.y == 7 && State[(int)(int)pos.x, (int)(int)pos.y - 2] == 0)//Vi tri ban dau
                        arrMove.Add(new Vector2((int)pos.x, (int)pos.y - 2));

                    if ((int)pos.y == 2)
                    {
                        arrMove.Add(new Vector2((int)pos.x, (int)pos.y - 1));
                        arrMove.Add(new Vector2((int)pos.x, (int)pos.y - 1));
                        arrMove.Add(new Vector2((int)pos.x, (int)pos.y - 1));
                    }

                }
                //Ăn quân
                //Nếu đường chéo ở 2 ô trước mặt là quân Trắng
				if (State[(int)(int)pos.x - 1, (int)(int)pos.y - 1] % 10 == 2)
                {
                    arrMove.Add(new Vector2((int)pos.x - 1, (int)pos.y - 1));
                    if ((int)pos.y == 2)
                    {
                        arrMove.Add(new Vector2((int)pos.x - 1, (int)pos.y - 1));
                        arrMove.Add(new Vector2((int)pos.x - 1, (int)pos.y - 1));
                        arrMove.Add(new Vector2((int)pos.x - 1, (int)pos.y - 1));
                    }
                }

				if (State[(int)(int)pos.x + 1, (int)(int)pos.y - 1] % 10 == 2)
                {
                    arrMove.Add(new Vector2((int)pos.x + 1, (int)pos.y - 1));
                    if ((int)pos.y == 2)
                    {
                        arrMove.Add(new Vector2((int)pos.x + 1, (int)pos.y - 1));
                        arrMove.Add(new Vector2((int)pos.x + 1, (int)pos.y - 1));
                        arrMove.Add(new Vector2((int)pos.x + 1, (int)pos.y - 1));
                    }
                }


            }

			Vector2 pt = SceneManager.instance.PlayGameController._EnPassantVector2;
            if (pt != new Vector2())//Nếu có tọa đọ bắt tốt
            {
                if ((int)pos.y == 4 && Side == 1)//Nếu là quân tốt đen
                {
                    if (new Vector2((int)pos.x - 1, 3) == pt)//Đường chéo phải
                    {
                        arrMove.Add(SceneManager.instance.PlayGameController._EnPassantVector2);
                    }

                    if (new Vector2((int)pos.x + 1, 3) == pt)//Đường chéo trái
                    {
                        arrMove.Add(SceneManager.instance.PlayGameController._EnPassantVector2);
                    }
                }

                if ((int)pos.y == 5 && Side == 2)
                {
                    if (new Vector2((int)pos.x - 1, 6) == pt)
                    {
                        arrMove.Add(SceneManager.instance.PlayGameController._EnPassantVector2);
                    }

                    if (new Vector2((int)pos.x + 1, 6) == pt)
                    {
                        arrMove.Add(SceneManager.instance.PlayGameController._EnPassantVector2);
                    }
                }
            }
            return arrMove;

        }
        public static Vector2 GetEnPassantVector2(int[,] State, Vector2 pos)
        {
            if ((int)pos.y == 4)
                return new Vector2((int)pos.x, (int)pos.y - 1);
            if ((int)pos.y == 5)
                return new Vector2((int)pos.x, (int)pos.y + 1);
            return new Vector2();
        }

        //Nếu chơi với máy hì máy sẽ chon quân Hậu và không hieern thị form chọn
        //Nếu đóng Form thì mặc định sẽ chọn quân hậu

		public static bool Promotion(ChessBase UcPawn, ChessPieceType PromoteTo)
        {

			if ((UcPawn.Position.y == 7) || (UcPawn.Position.y == 0))
            {

//                Bitmap queen = clsImageProcess.GetChessPieceBitMap(UcPawn.Side, ChessPieceType.Queen, UcPawn.Style);
//                Bitmap root = clsImageProcess.GetChessPieceBitMap(UcPawn.Side, ChessPieceType.Rook, UcPawn.Style);
//                Bitmap knight = clsImageProcess.GetChessPieceBitMap(UcPawn.Side, ChessPieceType.Knight, UcPawn.Style);
//                Bitmap bishop = clsImageProcess.GetChessPieceBitMap(UcPawn.Side, ChessPieceType.Bishop, UcPawn.Style);

//                if (PromoteTo == ChessPieceType.Null)
//                {
//                    Form f = new frmPromotion(queen, root, knight, bishop);
//                    f.ShowDialog();
//                    UcPawn.Type = frmPromotion.Type;
//                }
//                else
//                {
//                    UcPawn.Type = PromoteTo;
//                }
//                Bitmap image = clsImageProcess.GetChessPieceBitMap(UcPawn.Side, UcPawn.Type, UcPawn.Style);
//                UcPawn.Image = image;
                return true;
            }
            return false;
        }

    }
}
