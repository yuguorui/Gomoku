namespace Gomoku
{
    using System.Drawing;
    using System;
    static class Settings
    {
        // 逻辑棋盘边缘像素值（计算棋子位置）
        public const int EDGE_PIXEL = 50 - 25 / 2;
        // 两个棋子之间相隔距离
        public const int GRID_PIXEL = 25;
        // 最大可放置棋子的位置
        public const int MAX_PIXEL = 400;
        // 棋盘边缘像素值
        public const int MARGIN_PIXEL = 50;
        // 棋盘像素值大小
        public const int BITMAP_SIZE = 450;
        // 棋盘大小
        public const int BOARD_SIZE = 15;
        // 最多棋子数目
        public const int MAX_CHESSES = BOARD_SIZE * BOARD_SIZE;

        // x棋子图像
        public static Image CROSS = Properties.Resources.cross;
        // o棋子图像
        public static Image NOUGHT = Properties.Resources.nought;

        // 透明颜色
        public static Color CLEAR_COLOR = Color.FromArgb(0, 0, 0, 0);

        /////////////////////////////////////////////////////////////

        public const double WIN_POINT = double.MaxValue;
        public const double LOSE_POINT = double.MinValue;

        /////////////////////////////////////////////////////////////

        // 极大极小树搜索深度
        public static int SEARCH_DEPTH = 2;

        // 机器走棋和棋判定步数
        public const int DRAW_STEPS = 150;

        // 神经网络拓扑结构
        public static int[] NETWORK_STRUCT = { 225, 64, 1 };

        // 神经网络竞争胜利、平局、失败得到适应度
        public const int WIN_FITNESS = 1;
        public const int DRAW_FITNESS = 0;
        public const int LOSE_FITNESS = -1;

        // 神经网络进行的比赛数目
        public const int GAME_MATCHES = 5;

        // 神经网络的种群大小
        public const int POPULATION = 15;

        // 遗传算法进化的轮数
        public const int EPOCH = 10;

        // 是否使用神经网络作为评估函数
        public static bool IS_NEURO = true;

        // 存储神经网络信息的文件名
        public static String DATA_FILENAME = "data.bin";

    }
}
