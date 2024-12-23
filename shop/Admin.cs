using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace shop
{
    public partial class Admin : Form
    {
        private MySqlConnection con;
        private byte[] imageBytes;

        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=category;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }

        public Admin()
        {
            InitializeComponent();
            con = databaseConnection();
        }

        private const int BUTTON_WIDTH = 170;
        private const int BUTTON_HEIGHT = 170;
        private const int BUTTON_PADDING = 40;

        private void Admin_Load(object sender, EventArgs e)
        {
            showEquipment(); // Load data into DataGridView when the form loads
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid.")
            {
                object value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)dataGridView1.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)dataGridView1.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)    //ปุ่มเพิ่มข้อมูล
        {
            if (imageBytes != null && imageBytes.Length > 0)
            {
                MySqlConnection conn = databaseConnection();
                String sql = "INSERT INTO product (name, quantity, price, image) VALUES(@name, @quantity, @price, @image)";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@name", textBox1.Text);
                cmd.Parameters.AddWithValue("@quantity", textBox2.Text);
                cmd.Parameters.AddWithValue("@price", textBox3.Text);
                cmd.Parameters.AddWithValue("@image", imageBytes);


                try
                {
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("เพิ่มข้อมูลสำเร็จ");
                        showEquipment();

                        textBox1.Clear();
                        textBox2.Clear();
                        textBox3.Clear();
                        textBoxpic.Clear();
                    }
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
            else
            {
                MessageBox.Show("กรุณากรอกข้อมูลให้ครบ");
            }
        }

        private void showEquipment()    // โชว์สินค้าทั้งหมดใน dataGridView
        {
            MySqlConnection conn = databaseConnection();
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM product", conn);
            DataTable dt = new DataTable();

            try
            {
                conn.Open();      //อ่านข้อมูลใน sql
                MySqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                dataGridView1.DataSource = dt;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: " + ex.Message);  //เอาไว้โชว์ error
            }
            finally
            {
                conn.Close();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)    // กำหนดไฟล์รูปภาพ
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.gif)|*.jpg; *.jpeg; *.png; *.gif";

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string imagePath = openFileDialog1.FileName;// โชว์ชื่อไฟล์รูปที่เลือก
                    imageBytes = File.ReadAllBytes(imagePath);
                    textBoxpic.Text = Path.GetFileName(imagePath);
                }
            }
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
                textBox2.Text = row.Cells["quantity"].Value.ToString();
                textBox3.Text = row.Cells["price"].Value.ToString();
                textBoxpic.Text = row.Cells["image"].Value.ToString();
                // etc.
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {


        }

        private void button3_Click(object sender, EventArgs e) // ปุ่มลบข้อมูล
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int selectedRow = dataGridView1.CurrentCell.RowIndex;
                int deleteId = Convert.ToInt32(dataGridView1.Rows[selectedRow].Cells["id"].Value);
                MySqlConnection conn = databaseConnection();
                String sql = "DELETE FROM product WHERE id = '" + deleteId + "'"; // เลือกไอดีที่ต้องกาารลบ
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                conn.Close();    //ปิด db
                if (rows > 0)
                {
                    MessageBox.Show("ลบข้อมูลสำเร็จ");
                    showEquipment();

                    // ล้างข้อมูลใน TextBox หลังจากลบข้อมูลสำเร็จ
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                    textBoxpic.Clear();
                }
            }
            else
            {
                MessageBox.Show("กรุณาเลือกข้อมูลที่ต้องการจะลบ");
            }
        }


        private void button4_Click(object sender, EventArgs e)      //ปุ่มแก้ไข
        {
            int selectedRow = dataGridView1.CurrentCell.RowIndex;
            int editId = Convert.ToInt32(dataGridView1.Rows[selectedRow].Cells["id"].Value);

            MySqlConnection conn = databaseConnection();
            String sql = "UPDATE product SET name = '" + textBox1.Text + "' ,quantity = '" + textBox2.Text + "',price ='" + textBox3.Text + "'  WHERE id = '" + editId + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);

            conn.Open();
            int rows = cmd.ExecuteNonQuery();   //รันแล้วก็ส่งค่าไป db
            conn.Close();
            if (rows > 0)
            {
                MessageBox.Show("แก้ไขข้อมูลสำเร็จ");
                

                // ล้างข้อมูลใน TextBox หลังจากแก้ไขข้อมูลสำเร็จ

            
            }
            if (textBoxpic.Text.Length > 1)
            {
                MySqlConnection conn2 = databaseConnection();
                String sql1 = "UPDATE product SET image = @image WHERE id = '" + editId + "' ";
                MySqlCommand cmd1 = new MySqlCommand(sql1, conn2);
                cmd1.Parameters.AddWithValue("@image",imageBytes);
                conn2.Open();
                int cc = cmd1.ExecuteNonQuery();
                if (cc > 0)
                { 
                    conn2.Close();
                }
                
            }
            showEquipment();
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBoxpic.Clear();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
            Edit edit = new Edit();
            edit.Show();
        }


    }
}
