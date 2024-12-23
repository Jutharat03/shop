using System;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace shop
{
    public partial class newcustomer : Form
    {
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=stock;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }

        public newcustomer()
        {
            InitializeComponent();

            // อนุญาตเฉพาะตัวเลข และตัวอักษร backspace
            textBox3.KeyPress += new KeyPressEventHandler(textBox3_KeyPress);
            textBox3.TextChanged += new EventHandler(textBox3_TextChanged);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Code for combobox selection change event
        }

        private void button3_Click(object sender, EventArgs e)  //ปุ่มกดยืนยัน
        {
            this.Close();
            Form Form1 = new Form1();
            Form1.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // ตรวจสอบข้อมูลในแต่ละช่องว่าครบถ้วนหรือไม่
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("กรุณากรอกข้อมูลให้ครบทุกช่อง");
                return;
            }

            MySqlConnection conn = databaseConnection();    //กรอกให้ครบทุกช่องใน Textbox
            string name = textBox1.Text;
            string email = textBox2.Text;
            string tel = textBox3.Text;
            string username = textBox4.Text;
            string password = textBox5.Text;

            try
            {
                conn.Open();

                

                string query = "SELECT COUNT(*) FROM register WHERE name = @name";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", name);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count > 0)
                {
                    MessageBox.Show("Username ของคุณซ้ำ");
                }
                else
                {
                    MySqlCommand cmd1 = new MySqlCommand("INSERT INTO register (name, email, tel, username, password) VALUES (@name, @email, @tel, @username, @password)", conn);
                    cmd1.Parameters.AddWithValue("@name", name);
                    cmd1.Parameters.AddWithValue("@email", email);
                    cmd1.Parameters.AddWithValue("@tel", tel);
                    cmd1.Parameters.AddWithValue("@username", username);
                    cmd1.Parameters.AddWithValue("@password", password);

                    cmd1.ExecuteNonQuery();
                    MessageBox.Show("สมัครสมาชิกสำเร็จ");

                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
            this.Close();
            Form1 form1 = new Form1();
            form1.Show();

        }

        

        private void button2_Click(object sender, EventArgs e) //ปุ่มปิดหน้าจอ
        {
            this.Close();
            Form Form1 = new Form1();
            Form1.Show();
        }

        private void newcustomer_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            // อนุญาตเฉพาะตัวเลข และตัวอักษร backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            // จำกัดความยาวไม่เกิน 10 ตัวอักษร
            if (textBox3.Text.Length > 10)
            {
                textBox3.Text = textBox3.Text.Substring(0, 10);
                // ย้ายเคอร์เซอร์ไปท้ายข้อความ
                textBox3.SelectionStart = textBox3.Text.Length;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
