using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace GroceryStore
{
    public partial class Form1 : Form
    {
        private Form2 f2 = new Form2();

        public Form1()
        {
            InitializeComponent();

            f2.FormClosed += new FormClosedEventHandler(f2c);
        }

        private void f2c(object sender, FormClosedEventArgs e)
        {
            this.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("預設帳號：admin\n預設密碼：admin");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private int _failedLoginCounter = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            var username = textBox1.Text;
            var password = textBox2.Text;

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("請輸入帳號");
                textBox1.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("請輸入密碼");
                textBox2.Focus();
                return;
            }

            Login(username, password);
        }

        private void Login(string username, string password)
        {
            try
            {
                string constring = @"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = gsl.accdb; Jet OLEDB:Database Password = admin;";

                using (OleDbConnection conDataBase = new OleDbConnection(constring))
                {
                    using (OleDbCommand cmdDataBase = conDataBase.CreateCommand())
                    {
                        cmdDataBase.CommandText =
                            "SELECT * FROM gsl WHERE username = @username AND password = @password";

                        cmdDataBase.Parameters.AddRange(new OleDbParameter[]
                        {
                            new OleDbParameter("username", username),
                            new OleDbParameter("password", password)
                        }
                        );

                        if (conDataBase.State != ConnectionState.Open)
                            conDataBase.Open();

                        var numberOrResults = 0;

                        using (OleDbDataReader myReader = cmdDataBase.ExecuteReader())
                        {
                            while (myReader != null && myReader.Read())
                            {
                                numberOrResults++;
                            }
                        }

                        if (numberOrResults == 1)
                        {
                            this.Hide();
                            f2.ShowDialog();
                        }

                        else if (numberOrResults > 1)
                        {
                            MessageBox.Show("帳號或密碼重複");
                            _failedLoginCounter++;
                        }
                        
                        else if (numberOrResults == 0)
                        {
                            MessageBox.Show("帳號或密碼錯誤");
                            _failedLoginCounter++;
                        }
                    }

                }

                if (_failedLoginCounter >= 3)
                {
                    MessageBox.Show("已嘗試次數過多");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button1_Click(sender, e);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }
    }
}
