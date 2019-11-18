using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ReactionTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            speed = ConfigurationManager.AppSettings["speed"].Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            update = ConfigurationManager.AppSettings["update"].Split(',').Select(x => Convert.ToInt32(x)).ToArray();

            Reset();
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            Process.Start("https://gitee.com/yokeqi/game_reaction");
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    Reset();
                    break;
                case Keys.Space:
                    StartOrPause();
                    break;
                case Keys.Left:
                case Keys.Up:
                case Keys.Right:
                case Keys.Down:
                    Tap((int)keyData - 37);
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private int goal = 0, error = 0;
        private int level = 0;
        private int[] speed;
        private int[] update;
        private void Reset()
        {
            goal = error = 0;
            UpdateScore();

            level = 0;
            UpdateLevel();
        }
        private void StartOrPause()
        {
            if (tmrFPS.Enabled)
                tmrFPS.Stop();
            else
                tmrFPS.Start();
        }
        private void Tap(int val)
        {
            if (_curFX == -1)
                return;

            if (val == _curFX)
                goal++;
            else
                error++;

            UpdateScore();
        }


        private Random _rnd = new Random(DateTime.Now.Millisecond);
        private int _curFX = -1;
        private void tmrFPS_Tick(object sender, EventArgs e)
        {
            picLeft.Visible = picTop.Visible = picRight.Visible = picBottom.Visible = false;
            int val = -1;
            do
            {
                val = _rnd.Next(0, 21) % 4;
            } while (val == _curFX);
            _curFX = val;
            switch (_curFX)
            {
                case 0:
                    picLeft.Visible = true;
                    break;
                case 1:
                    picTop.Visible = true;
                    break;
                case 2:
                    picRight.Visible = true;
                    break;
                case 3:
                    picBottom.Visible = true;
                    break;
            }
        }

        private void UpdateScore()
        {
            lblGoal.Text = goal.ToString();
            lblError.Text = error.ToString();
            var total = goal - error;
            lblTotal.Text = Math.Abs(total).ToString("0000");
            lblTotal.ForeColor = (total == 0) ? Color.Gray : (total > 0) ? Color.Red : Color.Green;

            if (total > update[level])
            {
                level++;
                UpdateLevel();
            }
            else if (level > 0 && total < update[level - 1])
            {
                level--;
                UpdateLevel();
            }
        }

        private void UpdateLevel()
        {
            lblLevel.Text = $"Lv：{level + 1}，Speed：{speed[level]}";
            tmrFPS.Interval = speed[level];
        }
    }
}
