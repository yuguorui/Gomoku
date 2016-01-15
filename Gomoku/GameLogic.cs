using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gomoku
{
    public enum ChessPiece
    {
        EMPTY,
        NOUGHT,
        CROSS
    };

    class GameState
    {
        public ChessPiece[,] chessboard;
        public ChessPiece turn;
        private int[] last_modify;

        public int X
        {
            get { return last_modify[0]; }
            set { last_modify[0] = value; }
        }

        public int Y
        {
            get { return last_modify[1]; }
            set { last_modify[1] = value; }
        }


        public GameState()
        {
            chessboard = new ChessPiece[Settings.BOARD_SIZE, Settings.BOARD_SIZE];
            turn = ChessPiece.CROSS;
            last_modify = new int[2] { -1, -1 };
        }
        public GameState(GameState gs)
        {
            chessboard = (ChessPiece[,])gs.chessboard.Clone();
            turn = gs.turn;
            last_modify = (int[])gs.last_modify.Clone();
        }

        /// <summary>
        /// 迭代器，用于找到当前状态的所有子节点
        /// </summary>
        /// <returns></returns>
        public IEnumerator<GameState> GetNextSiblingStates()
        {
            var newGameState = new GameState(this);
            int tmp_x = -1, tmp_y = -1;

            ChessPiece chess = newGameState.turn;

            for (int i = 0; i < Settings.BOARD_SIZE; i++)
            {
                for (int j = 0; j < Settings.BOARD_SIZE; j++)
                {
                    if (chessboard[i, j] == ChessPiece.EMPTY)
                    {
                        if (tmp_x != -1)
                            newGameState.chessboard[tmp_x, tmp_y] = ChessPiece.EMPTY;
                        newGameState.PlaceChess(i, j, chess);
                        tmp_x = i;
                        tmp_y = j;
                        yield return newGameState;
                    }
                }
            }
        }

        /// <summary>
        /// 清空棋盘，重新开始
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < chessboard.GetLength(0); i++)
            {
                for (int j = 0; j < chessboard.GetLength(1); j++)
                {
                    chessboard[i, j] = ChessPiece.EMPTY;
                }
            }
            turn = ChessPiece.CROSS;
        }

        /// <summary>
        /// 得到棋子类型
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public ChessPiece GetPieceType(int x, int y)
        {
            return chessboard[x, y];
        }

        /// <summary>
        /// 先判断是否为棋子的合法放置位置
        /// 若合法将棋子放置在逻辑的棋盘上，并修改上次更改的棋子
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="cp">当前选手</param>
        public bool PlaceChess(int x, int y, ChessPiece cp)
        {
            if (chessboard[x, y] == ChessPiece.EMPTY)
            {
                chessboard[x, y] = cp;
                X = x;
                Y = y;

                turn = AnotherPlayer(cp);

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 获知下个选手
        /// </summary>
        /// <returns></returns>
        public ChessPiece NextPlayer()
        {
            return turn;
        }

        /// <summary>
        /// 判断当前是否有人取胜
        /// </summary>
        /// <returns>判断结果</returns>
        private bool IsWin()
        {
            // 若还未下过子，则直接返回false
            if(last_modify[0]==-1)
            {
                return false;
            }

            int count = 0;
            //vertical
            for (int i = 0; i < Settings.BOARD_SIZE - 1; i++)
            {
                if (chessboard[i, Y] ==
                         chessboard[i + 1, Y]
                    && chessboard[i, Y] != ChessPiece.EMPTY)
                {
                    count++;
                }
                else
                {
                    count = 0;
                }

                if (count >= 4)
                {
                    return true;
                }
            }


            //horizontal
            for (int i = 0; i < Settings.BOARD_SIZE - 1; i++)
            {
                if (chessboard[X, i] ==
                         chessboard[X, i + 1]
                    && chessboard[X, i] != ChessPiece.EMPTY)
                {
                    count++;
                }
                else
                {
                    count = 0;
                }

                if (count >= 4)
                {
                    return true;
                }
            }


            //diagonal left
            count = 0;
            for (int i = X, j = Y;
                i < Settings.BOARD_SIZE && j < Settings.BOARD_SIZE && i >= 0 && j >= 0;
                i--, j--)
            {
                if (chessboard[i, j] ==
                     chessboard[X, Y] && chessboard[i, j] != ChessPiece.EMPTY)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            for (int i = X, j = Y;
                i < Settings.BOARD_SIZE && j < Settings.BOARD_SIZE && i >= 0 && j >= 0;
                i++, j++)
            {
                if (chessboard[i, j] ==
                     chessboard[X, Y])
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            if (count >= 6)
            {
                return true;
            }

            //diagonal right
            count = 0;
            for (int i = X, j = Y;
                i < Settings.BOARD_SIZE && j < Settings.BOARD_SIZE && i >= 0 && j >= 0;
                i--, j++)
            {
                if (chessboard[i, j] ==
                     chessboard[X, Y] && chessboard[i, j] != ChessPiece.EMPTY)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            for (int i = X, j = Y;
                i < Settings.BOARD_SIZE && j < Settings.BOARD_SIZE && i >= 0 && j >= 0;
                i++, j--)
            {
                if (chessboard[i, j] ==
                     chessboard[X, Y])
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            if (count >= 6)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 根据当前的状态判断是谁赢了
        /// </summary>
        /// <returns>胜者</returns>
        public ChessPiece WhoIsWin()
        {
            if (!IsWin())
            {
                return ChessPiece.EMPTY;
            }
            if (turn == ChessPiece.CROSS)
            {
                return ChessPiece.NOUGHT;
            }
            else
            {
                return ChessPiece.CROSS;
            }
        }

        /// <summary>
        /// 重新开始游戏
        /// </summary>
        public void RestartGame()
        {
            Clear();
        }

        /// <summary>
        /// 得到另一位选手
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public static ChessPiece AnotherPlayer(ChessPiece cp)
        {
            return cp == ChessPiece.CROSS ? ChessPiece.NOUGHT : ChessPiece.CROSS;
        }


        public ChessPiece LastPlayer()
        {
            return AnotherPlayer(turn);
        }

        // 将棋盘转化为一维double数组以便神经网络处理
        public static implicit operator double[] (GameState gs)
        {
            double[] ans = new double[Settings.MAX_CHESSES];
            int xlen = gs.chessboard.GetLength(0);
            for (int i = 0; i < xlen; i++)
            {
                for (int j = 0; j < gs.chessboard.GetLength(1); j++)
                {
                    if (gs.turn == gs.chessboard[i, j])
                    {
                        ans[i * xlen + j] = 1;
                    }
                    else if (gs.chessboard[i, j] == ChessPiece.EMPTY)
                    {
                        ans[i * xlen + j] = 0;
                    }
                    else
                    {
                        ans[i * xlen + j] = -1;
                    }
                }
            }
            return ans;
        }

        /// <summary>
        /// 在棋盘的最中心放置一颗黑子
        /// </summary>
        public void PlaceChessInCenter()
        {
            PlaceChess(7, 7, ChessPiece.CROSS);
        }
    }
}
