using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gomoku
{
    class MultipleThread_MessageBox
    {
        int number;
        double fitness;
        public MultipleThread_MessageBox(int i, double fitness)
        {
            number = i;
            this.fitness = fitness;
        }
        public void MessageBox()
        {
            System.Windows.Forms.MessageBox.Show("第 " + number.ToString() + " 轮完成, 最佳适应度为 "+fitness.ToString());
        }
    }
}
