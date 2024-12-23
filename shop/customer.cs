using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace shop
{
    public partial class customer : Form
    {
        public customer()
        {
            InitializeComponent();
        }
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=stock;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
        private void showEquipment()
        {
            MySqlConnection conn = databaseConnection();
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM register", conn);
            DataTable dt = new DataTable();

            try
            {
                conn.Open();
                MySqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                dataGridView1.DataSource = dt;
                dataGridView1.AutoGenerateColumns = false; // ซ่อนไอดี
                dataGridView1.Columns["id"].Visible = false;
                dataGridView1.Columns["name"].DisplayIndex = 1;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)  // เพิ่มข้อมูลลูกค้า หรือสมัครสมาชิกใหม่
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

            MySqlConnection conn = databaseConnection();

            try
            {
                conn.Open();

                string name = textBox1.Text;
                string email = textBox2.Text;
                string tel = textBox3.Text;
                string username = textBox4.Text;
                string password = textBox5.Text;

                string query = "SELECT COUNT(*) FROM register WHERE name = @name";//เช็คว่าซ้ำไหม
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", name);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count > 0)
                {
                    MessageBox.Show("Username ของคุณซ้ำ");
                }
                else      //ถ้าไม่ซ้ำมาตรงนี้ต่อ
                {
                    MySqlCommand cmd1 = new MySqlCommand("INSERT INTO register (name, email, tel, username, password) VALUES (@name, @email, @tel, @username, @password)", conn);
                    cmd1.Parameters.AddWithValue("@name", name);
                    cmd1.Parameters.AddWithValue("@email", email);
                    cmd1.Parameters.AddWithValue("@tel", tel);
                    cmd1.Parameters.AddWithValue("@username", username);
                    cmd1.Parameters.AddWithValue("@password", password);

                    cmd1.ExecuteNonQuery();
                    MessageBox.Show("สมัครสมาชิกสำเร็จ");
                    showEquipment();   //อัปเดตว่าเพิ่มสำเร็จแล้ว

                    // ล้างข้อมูลใน TextBox หลังจากเพิ่มข้อมูลสำเร็จ
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                    textBox4.Clear();
                    textBox5.Clear();


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
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void customer_Load(object sender, EventArgs e)
        {
            showEquipment();
        }

        private void button4_Click(object sender, EventArgs e) // ปุ่มแก้ไขข้อมูลลูกค้า
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
            int selectedRow = dataGridView1.CurrentCell.RowIndex;
            int editId = Convert.ToInt32(dataGridView1.Rows[selectedRow].Cells["id"].Value);

            MySqlConnection conn = databaseConnection();
            String sql = "UPDATE register SET name = '" + textBox1.Text + "' ,email = '" + textBox2.Text + "',tel ='" + textBox3.Text + "' ,username = '" + textBox4.Text + "'  ,password ='" + textBox5.Text + "' WHERE id = '" + editId + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);

            conn.Open();
            int rows = cmd.ExecuteNonQuery();
            conn.Close();
            if (rows > 0)
            {
                MessageBox.Show("แก้ไขข้อมูลสำเร็จ");
                showEquipment();

                // ล้างข้อมูลใน TextBox หลังจากแก้ไขข้อมูลสำเร็จ
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
            }
        }

        private void button3_Click(object sender, EventArgs e)  //ลบข้อมูล
        {
            if (dataGridView1.SelectedCells.Count > 0)    //เลือกแถว
            {
                int selectedRow = dataGridView1.CurrentCell.RowIndex;
                int deleteId = Convert.ToInt32(dataGridView1.Rows[selectedRow].Cells["id"].Value);  // เลือกแถวใน datagrid
                MySqlConnection conn = databaseConnection();
                String sql = "DELETE FROM register WHERE id = '" + deleteId + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                conn.Close();
                if (rows > 0)
                {
                    MessageBox.Show("ลบข้อมูลสำเร็จ");
                    showEquipment();

                    // ล้างข้อมูลใน TextBox หลังจากลบข้อมูลสำเร็จ
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                    textBox4.Clear();
                    textBox5.Clear();

                }
            }
            else
            {
                MessageBox.Show("กรุณาเลือกข้อมูลที่ต้องการจะลบ");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
            Edit edit = new Edit();
            edit.Show();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)    // ตรวจสอบข้อมูลให้ไม่มีช่องว่าง
        {
            DataGridViewCell cell = null;
            foreach (DataGridViewCell selectedCell in dataGridView1.SelectedCells)
            {
                cell = selectedCell;
                break;
            }
            if (cell != null)
            {
                DataGridViewRow row = cell.OwningRow;
                textBox1.Text = row.Cells["name"].Value.ToString();
                textBox2.Text = row.Cells["email"].Value.ToString();
                textBox3.Text = row.Cells["tel"].Value.ToString();
                textBox4.Text = row.Cells["username"].Value.ToString();
                textBox5.Text = row.Cells["password"].Value.ToString();
                // etc.
            }
        }
    }

}
