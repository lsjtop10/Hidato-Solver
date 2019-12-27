using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using hidato_solver;

namespace Hidato_Solver_Gui_
{
    public partial class OptionDig : Form
    {
        public OptionDig()
        {
            InitializeComponent();

            textBox1.Enabled = true;
            textBox1.Enabled = true;
            textBox2.Text = HidatoSolver.NextUpdateSoconds.ToString();
            textBox1.Text = HidatoSolver.ProcessWaitTime.ToString();

            if (HidatoSolver.ShowAllProcess == true)
            {
                if(!(HidatoSolver.ProcessWaitTime == 0))
                {
                    checkBox1.Checked = true;
                    
                }

                checkBox1.Enabled = true;
                textBox1.Enabled = true;
                radioButton1.Checked = true;
                textBox2.Enabled = false;

            }
            else
            {
                checkBox1.Enabled = false;
                textBox1.Enabled = false;
                radioButton2.Checked = true;
            }
            //checkBox1.Enabled = false;
            //textBox1.Enabled = false;
            //radioButton2.Checked = true;
            //textBox2.Text = "5";

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            BoxChanged();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                HidatoSolver.ShowAllProcess = true;
                if(checkBox1.Checked)
                {
                    if(textBox1.Text == "")
                    {
                        checkBox1.Enabled = false;
                        textBox1.Enabled = false;

                        HidatoSolver.ProcessWaitTime = 0;
                    }
                    else
                    {
                        HidatoSolver.ProcessWaitTime = int.Parse(textBox1.Text);
                    }
                }
                else
                {
                    textBox1.Enabled = false;

                    HidatoSolver.ProcessWaitTime = 0;
                }
            }
            else if(radioButton2.Checked)
            {
                HidatoSolver.ShowAllProcess = false;
                
                if(textBox2.Text == "")
                {
                    MessageBox.Show("Error: Insert Value");
                    return;
                }
                else
                {
                    HidatoSolver.ShowAllProcess = false;
                    HidatoSolver.NextUpdateSoconds = int.Parse(textBox2.Text);
                }
            }
            if((Button)sender == OkButton)
            {
                Close();
            }
           
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            OkButton_Click(sender, e);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OptionDig_Click(object sender, EventArgs e)
        {

        }

        private void OptionDig_MouseDown(object sender, MouseEventArgs e)
        {

            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            BoxChanged();
        }

        private void BoxChanged()
        {

            if (radioButton1.Checked)
            {
                if (checkBox1.Checked)
                {
                    textBox1.Enabled = true;
                }
                else
                {
                    textBox1.Enabled = false;
                }

                checkBox1.Enabled = true;
                textBox2.Enabled = false;
            }
            else
            {
                checkBox1.Enabled = false;
                textBox2.Enabled = true;

            }

        }

       
    }
    
}
