using System.Collections.Generic;
using System.Diagnostics;

namespace Gomoku
{
    static class GameAI
    {
        public delegate double EvaluateFunction(GameState gs);
        static readonly int[] d_x = new int[] { 1, -1, 0, 0, 1, -1, 1, -1 };
        static readonly int[] d_y = new int[] { 0, 0, 1, -1, 1, 1, -1, -1 };

        /// <summary>
        /// 评价单个点的价值，仅为实验使用，不含神经网络
        /// </summary>
        /// <param name="gs"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="type">棋子类型</param>
        /// <returns></returns>
        private static int EvaluatePointSimple(GameState gs, int x, int y, ChessPiece type)
        {
            int ans = 0;

            for (int i = 0; i < 4; i++)
            {
                if (x + d_x[i] >= 0 && x + d_x[i] < Settings.BOARD_SIZE
                    && y + d_y[i] >= 0 && y + d_y[i] < Settings.BOARD_SIZE)
                    ans += ChessPiece.EMPTY != gs.chessboard[x + d_x[i], y + d_y[i]] ? 1 : 0;
            }
            for (int i = 4; i < 8; i++)
            {
                if (x + d_x[i] >= 0 && x + d_x[i] < Settings.BOARD_SIZE
                    && y + d_y[i] >= 0 && y + d_y[i] < Settings.BOARD_SIZE)
                    ans += (ChessPiece.EMPTY != gs.chessboard[x + d_x[i], y + d_y[i]] ? 2 : 0);
            }
            return ans;

        }

        /// <summary>
        /// 使用不含神经网络的估价函数估计整个棋盘状态，仅用来测试
        /// MinMax正确性。
        /// </summary>
        /// <param name="gs">游戏状态</param>
        /// <param name="turn">评价的方面</param>
        /// <returns>返回状态的评估值</returns>
        public static double EvaluateFunctionWithoutNervus(GameState gs)
        {
            return EvaluatePointSimple(gs, gs.X, gs.Y, gs.LastPlayer());
        }

        /// <summary>
        /// MIN_MAX博弈的核心
        /// </summary>
        /// <param name="gs">游戏状态</param>
        /// <param name="depth">搜素深度</param>
        /// <param name="min">允许的最小值</param>
        /// <param name="max">允许的最大值</param>
        /// <param name="cp">评判的是哪一方</param>
        /// <param name="ef">可调整的评价函数</param>
        /// <returns></returns>
        public static double MinMax(GameState gs, int depth, double min, double max, ChessPiece cp, EvaluateFunction ef)
        {
            ChessPiece winner = gs.WhoIsWin();
            if (winner == cp)
            {
                return Settings.WIN_POINT;
            }
            else if (winner == GameState.AnotherPlayer(cp))
            {
                return Settings.LOSE_POINT;
            }

            if (depth == 0)
            {
                return ef(gs);
            }

            double value, temp_value;
            // 当前为极大节点
            if (gs.NextPlayer() == cp)
            {
                // 检查其所有子节点
                value = min;
                IEnumerator<GameState> ie_gs = gs.GetNextSiblingStates();
                while (ie_gs.MoveNext())
                {
                    if (IsNessarySearch(ie_gs.Current, ie_gs.Current.X, ie_gs.Current.Y))
                    {
                        temp_value = MinMax(ie_gs.Current, depth - 1, value, max, cp, ef);

                        // alpha pruning
                        if (temp_value > value)
                        {
                            value = temp_value;
                        }
                        if (value > max)
                        {
                            return max;
                        }
                    }
                }
                return value;
            }
            // 当前为极小节点
            else
            {
                // 检查其所有子节点
                value = max;
                IEnumerator<GameState> ie_gs = gs.GetNextSiblingStates();
                while (ie_gs.MoveNext())
                {
                    if (IsNessarySearch(ie_gs.Current, ie_gs.Current.X,
                        ie_gs.Current.Y))
                    {
                        // beta pruning
                        temp_value = MinMax(ie_gs.Current, depth - 1, min, value, cp, ef);
                        if (temp_value < value)
                        {
                            value = temp_value;
                        }
                        if (value < min)
                        {
                            return min;
                        }
                    }
                }
                return value;
            }

        }

        /// <summary>
        /// 搜索周围的8个方格确认是否有必要进行搜素
        /// </summary>
        /// <param name="gs">游戏状态</param>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <returns></returns>
        public static bool IsNessarySearch(GameState gs, int x, int y)
        {
            int tmpx = x - 1;
            int tmpy = y - 1;
            for (int i = 0; tmpx < Settings.BOARD_SIZE && i < 3; ++tmpx, ++i)
            {
                int ty = tmpy;
                for (int j = 0; ty < Settings.BOARD_SIZE && j < 3; ++ty, ++j)
                {
                    if (tmpx == x && ty == y)
                        continue;
                    if (tmpx >= 0 && ty >= 0 && gs.chessboard[tmpx, ty] != ChessPiece.EMPTY)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 使用AI放置棋子
        /// </summary>
        /// <param name="gs">游戏状态</param>
        /// <param name="ef">评价函数</param>
        public static void PlaceChessAI(GameState gs, EvaluateFunction ef)
        {
            IEnumerator<GameState> ie_gs = gs.GetNextSiblingStates();
            int x = 0, y = 0;
            double value = Settings.LOSE_POINT;
            double temp_value = 0;

            while (ie_gs.MoveNext())
            {
                if (IsNessarySearch(ie_gs.Current, ie_gs.Current.X, ie_gs.Current.Y))
                {
                    temp_value = MinMax(ie_gs.Current, Settings.SEARCH_DEPTH - 1, Settings.LOSE_POINT, Settings.WIN_POINT, gs.turn, ef);
                    if (temp_value > value)
                    {
                        x = ie_gs.Current.X;
                        y = ie_gs.Current.Y;
                        value = temp_value;
                    }
                }
            }
            //Debug.WriteLine("{0} {1} {2}", x, y, temp_value);
            gs.PlaceChess(x, y, gs.NextPlayer());

        }

        public static int ComputerVSComputer(EvaluateFunction fun1, EvaluateFunction fun2)
        {
            // 经过指定步数则为和棋
            int step = Settings.DRAW_STEPS;
            GameState gs = new GameState();
            ChessPiece winner = ChessPiece.EMPTY;
            while (step-- != 0)
            {
                winner = gs.WhoIsWin();
                if (winner == ChessPiece.EMPTY)
                {
                    if (step == Settings.DRAW_STEPS - 1)
                    {
                        gs.PlaceChessInCenter();
                        
                        Debug.WriteLine("{0} {1} {2}", gs.X, gs.Y, GameState.AnotherPlayer(gs.turn).ToString());
                    }
                    else if (step % 2 == 1)
                    {
                        PlaceChessAI(gs, fun1);
                        Debug.WriteLine("{0} {1} {2}", gs.X, gs.Y, GameState.AnotherPlayer(gs.turn).ToString());
                    }
                    else if (step % 2 == 0)
                    {
                        PlaceChessAI(gs, fun2);
                        Debug.WriteLine("{0} {1} {2}", gs.X, gs.Y, GameState.AnotherPlayer(gs.turn).ToString());
                    }
                }
                else if (winner == ChessPiece.CROSS)
                    return Settings.WIN_FITNESS;
                else if (winner == ChessPiece.NOUGHT)
                    return Settings.LOSE_FITNESS;
            }
            return Settings.DRAW_FITNESS;
        }

    }
}
