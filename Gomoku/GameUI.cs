using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace Gomoku
{
    public partial class GameUI : Form
    {
        private Bitmap bBackGroud;  //棋盘图层
        private Bitmap bChess;      //棋子图层

        private Graphics gChess;
        private Graphics gBackGround;

        private volatile GameState gs;

        private Strategy strategy;
        private GameAI.EvaluateFunction NeuroEvaluateFunction;
        private bool enableNeuroSuccessful = false;

        public GameUI()
        {
            InitializeComponent();

            bBackGroud = new Bitmap(ChessBoard.ClientSize.Width, ChessBoard.ClientSize.Height);
            bChess = new Bitmap(ChessBoard.ClientSize.Width, ChessBoard.ClientSize.Height);

            gChess = Graphics.FromImage(bChess);
            gBackGround = Graphics.FromImage(bBackGroud);

            gs = new GameState();

            Initialization();
        }

        private void Initialization()
        {
            if (Settings.IS_NEURO && File.Exists(Settings.DATA_FILENAME))
            {
                strategy = (Strategy)Network.Load(Settings.DATA_FILENAME);
                NeuroEvaluateFunction = strategy.Compute;
                enableNeuroSuccessful = true;
            }
        }

        private void GameUI_Load(object sender, EventArgs e)
        {

        }

        private void Chessboard_Click(object sender, EventArgs e)
        {

        }

        private void Chessboard_MouseDown(object sender, MouseEventArgs e)
        {
            int x = Pixel2Index(e.X);
            int y = Pixel2Index(e.Y);

            if (gs.PlaceChess(x, y, gs.NextPlayer()))
            {
                DrawChess(x, y, gs.NextPlayer());
                Redraw();
                if (gs.WhoIsWin() != ChessPiece.EMPTY)
                {
                    Win(gs.WhoIsWin());
                    return;
                }

                if (enableNeuroSuccessful)
                {
                    GameAI.PlaceChessAI(gs, NeuroEvaluateFunction);
                }
                else
                {
                    GameAI.PlaceChessAI(gs, GameAI.EvaluateFunctionWithoutNervus);
                }

                DrawChess(gs.X, gs.Y, gs.NextPlayer());
                Redraw();
                if (gs.WhoIsWin() != ChessPiece.EMPTY)
                {
                    Win(gs.WhoIsWin());
                }
            }

            //MessageBox.Show(Pixel2Index(x).ToString() + ' ' + Pixel2Index(y).ToString());
        }

        /// <summary>
        /// 将点击得到的像素值转化为逻辑标号
        /// </summary>
        /// <param name="p">像素值</param>
        /// <returns></returns>
        private int Pixel2Index(int p)
        {
            if (p < Settings.EDGE_PIXEL)
                p = Settings.EDGE_PIXEL;
            if (p > Settings.MAX_PIXEL)
                p = Settings.MAX_PIXEL;
            return (p - Settings.EDGE_PIXEL) / Settings.GRID_PIXEL;
        }


        /// <summary>
        /// 将游戏中的逻辑值转换为画面上的像素值
        /// </summary>
        /// <param name="i">index</param>
        /// <returns></returns>
        private int Index2Pixel(int i)
        {
            return Settings.MARGIN_PIXEL + i * Settings.GRID_PIXEL;
        }

        /// <summary>
        /// 在指定位置放置棋子
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="cp">现在的玩家</param>
        private void DrawChess(int x, int y, ChessPiece cp)
        {
            if (cp == ChessPiece.CROSS)
                gChess.DrawImage(Settings.NOUGHT, Index2Pixel(x) - Settings.GRID_PIXEL / 2, Index2Pixel(y) - Settings.GRID_PIXEL / 2,
                    Settings.GRID_PIXEL, Settings.GRID_PIXEL);
            else
                gChess.DrawImage(Settings.CROSS, Index2Pixel(x) - Settings.GRID_PIXEL / 2, Index2Pixel(y) - Settings.GRID_PIXEL / 2,
                     Settings.GRID_PIXEL, Settings.GRID_PIXEL);
        }

        /// <summary>
        /// 重绘棋盘画面
        /// </summary>
        private void Redraw()
        {
            gBackGround.DrawImage(Properties.Resources.chessboard, 0, 0, Settings.BITMAP_SIZE, Settings.BITMAP_SIZE);
            gBackGround.DrawImage(bChess, 0, 0);

            ChessBoard.Image = bBackGroud;
            ChessBoard.Refresh();

        }

        private void ChessBoard_Paint(object sender, PaintEventArgs e)
        {
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 关于我们ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("于国瑞 1120131774\n蒋子洋 1120132031\n杜田野 1120131766", "关于我们");
        }

        private void Win(ChessPiece cp)
        {
            MessageBox.Show(cp.ToString() + " is winner.\nClick Yes to restart.");
            Restart();
        }

        private void Restart()
        {
            gChess.Clear(Settings.CLEAR_COLOR);
            Redraw();
            gs.RestartGame();
        }

        private void 开始游戏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Restart();
        }

        /// <summary>
        /// 为了保证类型安全创建的辅助类
        /// </summary>
        private class AIThreadClass
        {
            GameState game_state;
            GameAI.EvaluateFunction ef;
            public AIThreadClass(GameState gs, GameAI.EvaluateFunction eva_fun)
            {
                game_state = gs;
                ef = eva_fun;
            }
            public void RunThread()
            {
                GameAI.PlaceChessAI(game_state, ef);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingUI form = new SettingUI();
            form.Show();
        }

        private void 开始神经网络训练ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread learning = new Thread(delegate ()
            {
                GameAINeuroEvolutionaryLearning GANEL;
                if (File.Exists(Settings.DATA_FILENAME))
                {
                    FileStream file = new FileStream(Settings.DATA_FILENAME, FileMode.Open);
                    GANEL = new GameAINeuroEvolutionaryLearning(Settings.POPULATION, file);
                }
                else
                {
                    GANEL = new GameAINeuroEvolutionaryLearning(Settings.POPULATION);
                }
                for (int i = 0; i < Settings.EPOCH; i++)
                {
                    GANEL.RunEpoch();
                    MultipleThread_MessageBox box = new MultipleThread_MessageBox(i, GANEL.BestChromosome.Fitness);
                    Thread thread = new Thread(box.MessageBox);
                    thread.Start();
                }
                GANEL.BestChromosome.Save(Settings.DATA_FILENAME);
                GANEL.population[0].Save(Settings.DATA_FILENAME);
                
                MessageBox.Show("训练完成，数据已保存");
            });

            learning.Start();
        }
    }
}
