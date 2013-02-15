using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
namespace _5checker
{
    class GameLogic
    {
        class Step
        {
            public Step(int turn, int weight, int x, int y)
            {
                this.turn = turn;
                this.weight[0] = weight;
                this.weight[1] = weight;
                this.x = x;
                this.y = y;
                weights[0] = new int[4];
                weights[1] = new int[4];
                for (int i = 0; i < 4; i++)
                {

                    weights[0][i] = 0;
                    weights[1][i] = 0;
                }
            }
            public int x;
            public int y;
            public int turn;
            public int[] weight = new int[2];
            public int[][] weights = new int[2][];
            public void clear()
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        weights[i][j] = 0;

                    }
                    weight[i] = 0;
                }
            }

            public void calcWeight(int turn)
            {
                foreach (int a in weights[turn - 1])
                {
                    if (a > weight[turn - 1])
                        weight[turn - 1] = a;
                }
            }
        }
        public int turn = 1;
        Checker gameBorad;
        Elements.Box[][] box = new _5checker.Elements.Box[28][];
        private static GameLogic logic;
        Step[][] MAP = new Step[28][];
        private GameLogic()
        {
            gameBorad = new Checker();
            createBox();
            for (int i = 0; i < 28; i++)
            {
                MAP[i] = new Step[28];
                for (int j = 0; j < 28; j++)
                {
                    MAP[i][j] = new Step(0, 0, i, j);
                }
            }
        }
        public static GameLogic getLogic()
        {
            if (logic == null)
                logic = new GameLogic();
            return logic;
        }
        private void createBox()
        {
            for (int i = 0; i < 28; i++)
            {
                box[i] = new _5checker.Elements.Box[28];
                for (int j = 0; j < 28; j++)
                {
                    box[i][j] = new Elements.Box(25 * i, 25 * j);
                }
                gameBorad.Controls.AddRange(box[i]);
            }
        }
        public void recordClick(int x, int y)
        {
            calcWeight();

            MAP[x][y].turn = turn;
            isWin(x, y, turn);
            switch (turn)
            {
                case 1:
                    turn = 2;
                    break;
                case 2:
                    turn = 1;
                    break;
            }
            myAI(x, y);
        }

        private void myAI(int x, int y)
        {
            calcWeight();
            int max = 0;
            Step potential = null;
            foreach (Step[] steps in MAP)
            {
                foreach (Step step in steps)
                {

                    if (step.turn == 0)
                    {
                        if (step.weight[turn - 1] >= max)
                        {
                            max = step.weight[turn - 1];
                            potential = step;
                        }

                    }
                }
            }
            int max_d = 0;
            Step potential_d = null;
            foreach (Step[] steps in MAP)
            {
                foreach (Step step in steps)
                {

                    if (step.turn == 0)
                    {

                        if (step.weight[oppositeTurn(turn) - 1] >= max_d)
                        {
                            max_d = step.weight[oppositeTurn(turn) - 1];
                            potential_d = step;
                        }

                    }
                }
            }
            List<Step> goodSteps;
            if (max_d > max)
            {
                //do defence
                goodSteps = new List<Step>();
                foreach (Step[] steps in MAP)
                {
                    foreach (Step step in steps)
                    {
                        if (step.turn == 0)
                        {
                            if (max_d == step.weight[oppositeTurn(turn) - 1])
                            {
                                if (max_d < 2)
                                {
                                    if (step.weight[oppositeTurn(turn) - 1] == max_d)
                                        goodSteps.Add(step);
                                }
                                else
                                {
                                    if (step.weight[oppositeTurn(turn) - 1] >= 2)
                                    {
                                        goodSteps.Add(step);
                                    }
                                } 
                            }
                        }
                    }
                }
                goodSteps = optimizeWeight(goodSteps, oppositeTurn(turn));
                goodSteps = optimizeWeight(goodSteps, turn);
            }
            else
            {
                //do offence
                goodSteps = new List<Step>();
                foreach (Step[] steps in MAP) {
                    foreach (Step step in steps) {
                        if (step.turn == 0)
                        {
                            if (max < 2)
                            {
                                if (step.weight[(turn) - 1] == max)
                                    goodSteps.Add(step);
                            }
                            else
                            {
                                if (step.weight[(turn) - 1] >= 2)
                                {
                                    goodSteps.Add(step);
                                }
                            } 
                        }
                    }
                }
                goodSteps=optimizeWeight(goodSteps, turn);
                goodSteps = optimizeWeight(goodSteps, oppositeTurn(turn));
            }
            Random r = new Random();
            int next = r.Next(goodSteps.Count);
            potential = goodSteps[next];
            MAP[potential.x][potential.y].turn = turn;
            box[potential.x][potential.y].setCheck(turn);
            box[potential.x][potential.y].Refresh();
            isWin(potential.x, potential.y, turn);
            switch (turn)
            {
                case 1:
                    turn = 2;
                    break;
                case 2:
                    turn = 1;
                    break;
            }
        }


        private List<Step> optimizeWeight(List<Step> groupstep, int turn)
        {
            int[][][] counts = new int[28][][];
            for (int i = 0; i < 28; i++)
            {
                counts[i] = new int[28][];
                for (int j = 0; j < 28; j++) {
                    counts[i][j] = new int[5];
                    for (int k = 0; k < 5; k++) {
                        counts[i][j][k] = 0;
                    }
                }
            }

            foreach (Step chosenStep in groupstep)
            {
                if (isWin(chosenStep.x, chosenStep.y, turn)) {
                    List<Step> n = new List<Step>();
                    n.Add(chosenStep);
                    return n;
                }
                MAP[chosenStep.x][chosenStep.y].turn = turn;
                calcWeight();

                foreach (Step[] steps in MAP)
                {
                    foreach (Step step in steps)
                    {
                        if (step.turn == 0)
                        {
                            counts[chosenStep.x][chosenStep.y][step.weight[turn - 1]]++;
                        }
                    }
                }

                MAP[chosenStep.x][chosenStep.y].turn = 0;
                calcWeight();
            }

            for (int i = 4; i > 0; i--) {
                int max=0;
                foreach (Step chosenStep in groupstep) {
                    if (max < counts[chosenStep.x][chosenStep.y][i]) {
                        max = counts[chosenStep.x][chosenStep.y][i];
                    }
                }
                if (max > 0) {
                    List<Step> mySteps = new List<Step>();
                    foreach (Step chosenStep in groupstep) {
                        if (counts[chosenStep.x][chosenStep.y][i] == max)
                            mySteps.Add(chosenStep);
                    }
                    return mySteps;
                }
            }
            return groupstep;
        }

        // judge if 'turn' wins
        private bool isWin(int x, int y, int turn)
        {
            if (MAP[x][y].weight[turn - 1] == 4)
            {
                gameBorad.Text = "End";
                return true;
            }
            return false;
        }


        //get opposite turn
        private int oppositeTurn(int x)
        {
            if (x == 1)
                return 2;
            else if (x == 2)
                return 1;
            else
                return -1;
        }





        // check horizonal weight
        private void checkH(Step step, int thisTurn)
        {
            if (step.turn != 0)
                return;
            int[] tem_weight = new int[5];
            for (int i = 0; i < 5; i++)
            {
                tem_weight[i] = 0;
            }
            for (int t = 0; t < 5; t++)
            {
                for (int i = 0 - t; i <= 4 - t; i++)
                {
                    if ((step.x + i) > 27 || (step.x + i) < 0)
                    {
                        tem_weight[t] = 0;
                        break;
                    }
                    if (MAP[step.x + i][step.y].turn == oppositeTurn(thisTurn))
                    {
                        tem_weight[t] = 0;
                        break;
                    }
                    else if (MAP[step.x + i][step.y].turn == thisTurn)
                        tem_weight[t]++;
                }
            }
            foreach (int w in tem_weight)
            {
                if (step.weights[thisTurn - 1][0] < w)
                {
                    step.weights[thisTurn - 1][0] = w;
                }
            }

            
            step.calcWeight(thisTurn);

        }


        // check horizonal weight
        private void checkV(Step step, int thisTurn)
        {
            if (step.turn != 0)
                return;
            int[] tem_weight = new int[5];
            for (int i = 0; i < 5; i++)
            {
                tem_weight[i] = 0;
            }
            for (int t = 0; t < 5; t++)
            {
                for (int i = 0 - t; i <= 4 - t; i++)
                {
                    if ((step.y + i) > 27 || (step.y + i) < 0)
                    {
                        tem_weight[t] = 0;
                        break;
                    }
                    if (MAP[step.x][step.y + i].turn == oppositeTurn(thisTurn))
                    {
                        tem_weight[t] = 0;
                        break;
                    }
                    else if (MAP[step.x][step.y + i].turn == thisTurn)
                        tem_weight[t]++;
                }
            }
            foreach (int w in tem_weight)
            {
                if (step.weights[thisTurn - 1][1] < w)
                {
                    step.weights[thisTurn - 1][1] = w;
                }
            }

            step.calcWeight(thisTurn);

        }


        // check horizonal weight
        private void checkLR(Step step, int thisTurn)
        {
            if (step.turn != 0)
                return;
            int[] tem_weight = new int[5];
            for (int i = 0; i < 5; i++)
            {
                tem_weight[i] = 0;
            }
            for (int t = 0; t < 5; t++)
            {
                for (int i = 0 - t; i <= 4 - t; i++)
                {
                    if ((step.x + i) > 27 || (step.x + i) < 0 || (step.y + i) > 27 || (step.y + i) < 0)
                    {
                        tem_weight[t] = 0;
                        break;
                    }
                    if (MAP[step.x + i][step.y + i].turn == oppositeTurn(thisTurn))
                    {
                        tem_weight[t] = 0;
                        break;
                    }
                    else if (MAP[step.x + i][step.y + i].turn == thisTurn)
                        tem_weight[t]++;
                }
            }
            foreach (int w in tem_weight)
            {
                if (step.weights[thisTurn - 1][2] < w)
                {
                    step.weights[thisTurn - 1][2] = w;
                }
            }
            step.calcWeight(thisTurn);

        }


        // check horizonal weight
        private void checkRL(Step step, int thisTurn)
        {
            if (step.turn != 0)
                return;
            int[] tem_weight = new int[5];
            for (int i = 0; i < 5; i++)
            {
                tem_weight[i] = 0;
            }
            for (int t = 0; t < 5; t++)
            {
                for (int i = 0 - t; i <= 4 - t; i++)
                {
                    if ((step.x - i) > 27 || (step.x - i) < 0 || (step.y + i) > 27 || (step.y + i) < 0)
                    {
                        tem_weight[t] = 0;
                        break;
                    }
                    if (MAP[step.x - i][step.y + i].turn == oppositeTurn(thisTurn))
                    {
                        tem_weight[t] = 0;
                        break;
                    }
                    else if (MAP[step.x - i][step.y + i].turn == thisTurn)
                        tem_weight[t]++;
                }
            }
            foreach (int w in tem_weight)
            {
                if (step.weights[thisTurn - 1][3] < w)
                {
                    step.weights[thisTurn - 1][3] = w;
                }
            }
            step.calcWeight(thisTurn);

        }

        //calc all the empty places' weights
        private void calcWeight()
        {
            foreach (Step[] steps in MAP)
            {
                foreach (Step step in steps)
                {

                    if (step.turn == 0)
                    {
                        step.clear();
                        checkH(step, 1);
                        checkV(step, 1);
                        checkLR(step, 1);
                        checkRL(step, 1);
                        checkH(step, 2);
                        checkV(step, 2);
                        checkLR(step, 2);
                        checkRL(step, 2);
                    }
                }
            }

        }
        private void gameOver()
        {
            for (int i = 0; i < 28; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    MAP[i][j].turn = 0;
                }
            }
            gameBorad.Refresh();
        }
        static void Main()
        {
            GameLogic myBrain = GameLogic.getLogic();
            Application.EnableVisualStyles();
            Application.Run(myBrain.gameBorad);
        }
    }
}
