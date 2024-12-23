using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Win32;
using MySql.Data.MySqlClient;

namespace shop
{
    public partial class Form1 : Form
    {
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;passwode=;database=stock;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)   //ปุ่มปิดหน้าจอ
        {
            this.Close();
            Form form1 = new Form1();
            form1.Show();
        }

        private void button3_Click(object sender, EventArgs e) //ปุ่ม login
        {
            // ตรวจสอบว่าช่อง Username และ Password ไม่ว่างเปล่า
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("กรุณากรอก Username และ Password ให้ครบถ้วน", "ข้อความแจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MySqlConnection conn = databaseConnection();

            try
            {
                conn.Open();
                string input = textBox1.Text; // รับค่า Username or Password
                string password = textBox2.Text;

                string query = "SELECT COUNT(*) FROM register WHERE username = @Input AND password = @Password";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Input", input);
                cmd.Parameters.AddWithValue("@Password", password);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count > 0)
                {
                    MessageBox.Show("เข้าสู่ระบบสำเร็จ!", "ข้อความแจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // ปิด Login
                    this.Hide();
                    category g = new category
                    {
                        user_name = input
                    };


                    g.Show();
                }
                else
                {
                    MessageBox.Show("กรุณากรอกข้อมูลให้ถูกต้อง", "ข้อความแจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message, "ข้อความแจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e) //ปุ่ม customer
        {
            newcustomer newcustomer = new newcustomer();
            newcustomer.Show();
            this.Hide();
        }



        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e) //ปุ่มแอดมิน
        {
            loginadmin loginadmin = new loginadmin();
            loginadmin.Show();
            this.Hide();
        }
    }
}
