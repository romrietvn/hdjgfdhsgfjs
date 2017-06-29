using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using UnityEngine;

namespace Chess_Usercontrol
{
    public static class clsBishop
    {

        static int Side;
        static ArrayList arrMove;

        private static int[,] BishopTable = new int[,]
		{
          //9  8   7   6   5   4   3   2   1  0
           {0,   0,  0,  0,  0,  0,  0,  0,  0,   0}, //0

           {0,  -20,-10,-10,-10,-10,-10,-10,-20  ,0},
           {0,  -10,  0,  0,  0,  0,  0,  0,-10  ,0},
           {0,  -10,  0,  5, 10, 10,  5,  0,-10  ,0},
           {0,  -10,  5,  5, 10, 10,  5,  5,-10  ,0},
           {0,  -10,  0, 10, 10, 10, 10,  0,-10  ,0},
           {0,  -10, 10, 10, 10, 10, 10, 10,-10  ,0},            
           {0,  -10,  5,  0,  0,  0,  0,  5,-10  ,0},
           {0,  -20,-10,-40,-10,-10,-40,-10,-20  ,0},

           {0,   0,  0,  0,  0,  0,  0,  0,  0,   0}  //9
		};
		public static int GetPositionValue(Vector2 pos, ChessSide eSide)
        {
			if (eSide == ChessSide.White)
            {
				return BishopTable [(int)pos.x, (int)pos.y];
            }
            else
            {
				return BishopTable[9 - (int)pos.y, 9 - (int)pos.x];
            }
        }
		public static ArrayList FindAllPossibleMove(int[,] State, Vector2 pos)
        {

            arrMove = new ArrayList();

			Side = State[(int)pos.x, (int)pos.y] % 10;//Chẵn(0) là quân trắng, lẻ(1) là quân đen

            chessloop(State, pos, 1, 1);
            chessloop(State, pos, 1, -1);
            chessloop(State, pos, -1, -1);
            chessloop(State, pos, -1, 1);

            return arrMove;
        }


		static void chessloop(int[,] State, Vector2 pos, int indexx, int indexy)
        {
            int stop = 0;
			int x = (int)pos.x;
			int y = (int)pos.y;
            while (stop == 0)
            {
                x += indexx;
                y += indexy;
                
                int state = State[x, y];
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
