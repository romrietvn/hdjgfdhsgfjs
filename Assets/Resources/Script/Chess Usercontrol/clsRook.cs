﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;
namespace Chess_Usercontrol
{
    public static class clsRook
    {
        //***Hàm trả về tất cả các nước đi có thể của 1 quân xe, kể cả nước ăn quân***
        static int Side;
        static ArrayList arrMove;


        private static int[,] RookTable = new int[,]
		{
          //9  8   7   6   5   4   3   2   1  0
           {0,   0,  0,  0,  0,  0,  0,  0,  0,   0}, //0

           {0,  0,  0,  0,  0,  0,  0,  0,  0,   0},
           {0,  5,  15, 15, 15, 15, 15, 15, 5,   0},
           {0, -5,  0,  0,  0,  0,  0,  0, -5,   0},
           {0, -5,  0,  0,  0,  0,  0,  0, -5,   0},
           {0, -5,  0,  0,  0,  0,  0,  0, -5,   0},
           {0, -5,  0,  0,  0,  0,  0,  0, -5,   0},
           {0, -5,  0,  0,  0,  0,  0,  0, -5,   0},
           {0,  0,  5,  0,  5,  5,  0,  0,  0,   0},
            
           {0,   0,  0,  0,  0,  0,  0, 0,  0,  0}  //9
		};

        public static int GetPositionValue(Vector2 pos, ChessSide eSide)
        {
            if (eSide == ChessSide.Black)
            {
				return RookTable[(int)pos.y, (int)pos.x];
            }
            else
            {
				return RookTable[9 - (int)pos.y, 9 - (int)pos.x];
            }
        }
       
        public static ArrayList FindAllPossibleMove(int[,] State, Vector2 pos)//, bool EnPassant)
        {
            //Từ vị trí ban đầu quân xe có thể di chuyển về 4 phía theo trục đứng và ngang

            arrMove = new ArrayList();

			Side = State[(int)pos.x, (int)pos.y] % 10;//Chẵn(0) là quân trắng, lẻ(1) là quân đen

            chessloop(State, pos, 0, 1);
            chessloop(State, pos, 1, 0);
            chessloop(State, pos, 0, -1);
            chessloop(State, pos, -1, 0);

            return arrMove;
        }

        static void chessloop(int[,] State, Vector2 pos, int indexx, int indexy)
        {
            int stop = 0;
			int x = (int)pos.x;
			int y = (int)pos.y;
            while (stop == 0)
            {
                int state = State[x += indexx, y += indexy];

                if (state == 0)
                {
                    arrMove.Add(new Vector2(x, y));
                }
                else if (state == -1)
                {
                    stop = 1;
                }
                else
                {
                    if (state % 10 != Side)
                    {
                        arrMove.Add(new Vector2(x, y));
                    }
                    stop = 1;
                }

            }
        }

    }
}
