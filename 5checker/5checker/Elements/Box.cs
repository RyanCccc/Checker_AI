using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace _5checker.Elements
{
    class Box : Panel
    {
        int hasCheck=0;
        public Box(int x,int y) : base() {
            this.BackColor = Color.Gray;
            this.Location = new Point(x, y);
            this.Width = 25;
            this.Height = 25;
            this.Click += new EventHandler(Box_Click);
        }

        void Box_Click(object sender, EventArgs e)
        {
            if (hasCheck == 0)
            {
                hasCheck = GameLogic.getLogic().turn;
                GameLogic.getLogic().recordClick(this.Location.X/25,this.Location.Y/25);
                this.Refresh();
            }
        }

        public void setCheck(int turn) {
            hasCheck = turn;
        }
        public Box(Point p)
            : base()
        {
            this.Location = p;
            this.BackColor = Color.Gray;
            this.Width = 25;
            this.Height = 25;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            draw(e.Graphics);
        }
        public void draw(Graphics g) {
            Pen p = new Pen(Brushes.Black, 3);
            g.DrawLine(p, new Point( Width / 2-1,0),new Point( Width / 2-1,Height));
            g.DrawLine(p, new Point(0, Height/2-1), new Point(Width, Height/2-1));
            if (hasCheck==1)
                g.FillEllipse(Brushes.Black, new Rectangle(0, 0, 25, 25));
            else if(hasCheck==2)
                g.FillEllipse(Brushes.White, new Rectangle(0, 0, 25, 25));
            
        }
    }
}
