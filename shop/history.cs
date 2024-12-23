using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace shop
{
    public partial class history : Form
    {
        private MySqlConnection databaseConnection()  // เชือมต่อข้อมูลใน db
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=category;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        private MySqlConnection databaseConnection1()  // เชือมต่อข้อมูลใน db
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=stock;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        public history()
        {
            InitializeComponent();
        }

        private void comboBox1_ForeColorChanged(object sender, EventArgs e)
        {

        }
        private void showhistory()     // โชว์ตารางข้อมูลใน history ทั้งหมด
        {
            MySqlConnection conn = databaseConnection();
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM history", conn);
            DataTable dt = new DataTable();

            try
            {
                conn.Open();
                MySqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                dataGridView1.DataSource = dt;
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
       

        private void history_Load(object sender, EventArgs e)
        {
            showhistory();
            MySqlConnection conn = databaseConnection();
            MySqlCommand cmd = new MySqlCommand("SELECT id , customer FROM history ", conn);
            conn.Open();
            AutoCompleteStringCollection str_coll = new AutoCompleteStringCollection();
            MySqlDataReader myreader = cmd.ExecuteReader();
            while(myreader.Read())
            {
                str_coll.Add(myreader.GetString(1));
            }
            textBox1.AutoCompleteCustomSource = str_coll;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) // เริ่มตรงนี้
        {
            if (comboBox1.SelectedIndex == 0)
            {
                textBox1.Text = "username";  //กดเลือกชื่อในคอมโบบ็อก จะขึ้นแค่ให้กรอกชื่อ
                textBox3.Visible = false;
                textBox4.Visible = false;
                textBox5.Visible = false;
                textBox1.Visible = true;

            }
            else if (comboBox1.SelectedIndex == 1)
            {
                textBox3.Text = "31";  //กดเลือกชื่อวันโบบ็อก จะขึ้นแค่ให้กรอกวันเดือนปี
                textBox3.Visible = true;
                textBox4.Text = "01";
                textBox4.Visible = true;
                textBox5.Visible = true;
                textBox5.Text = "2024";
                textBox1.Visible = false;
            }
            else if(comboBox1.SelectedIndex == 2)
            {
                
                textBox3.Visible = false;    //กดเลือกเดือนโบบ็อก จะขึ้นแค่ให้กรอกเดือนปี
                textBox4.Visible = true;
                textBox4.Text = "01";
                textBox5.Visible = true;
                textBox5.Text = "2024";
                textBox1.Visible = false;

            }
            else if (comboBox1.SelectedIndex == 3)
            {
                textBox1.Text = "2024";    //กดเลือกปีโบบ็อก จะขึ้นแค่ให้กรอกปี
                textBox3.Visible = false;
                textBox4.Visible = false;
                textBox5.Visible = true;
                textBox5.Text = "2024";
                textBox1.Visible = false;

            }
            else if (comboBox1.SelectedIndex == 4)
            {
                textBox1.Text = "NO.bill";    //กดเลือกบิลโบบ็อก จะขึ้นแค่ให้กรอกบิล
                textBox3.Visible = false;
                textBox4.Visible = false;
                textBox5.Visible = false;
                textBox1.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (comboBox1.SelectedIndex == 0)// ปุ่มค้นหา username
            {
                MySqlConnection conn1 = databaseConnection1(); // ทำการเลือกข้อมูลและเลือกแถวโดยอิงจาก username
                MySqlCommand cmd1 = new MySqlCommand("SELECT email FROM register WHERE username = @username", conn1);
                cmd1.Parameters.AddWithValue("@username", textBox1.Text);  // เอาค่าใน textbox ไปใส่ใน username

                conn1.Open();
                MySqlDataReader reader1 = cmd1.ExecuteReader();

                if (reader1.Read())

                {
                    string email = reader1.GetString(0);

                    MySqlConnection conn = databaseConnection(); // ทำการเลือกข้อมูลและเลือกแถวโดยอิงจาก username
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM history WHERE email = @email", conn);
                    cmd.Parameters.AddWithValue("@email", email);  // เอาค่าใน textbox ไปใส่ใน username
                    DataTable dt = new DataTable();
                    try
                    {
                        conn.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        dt.Load(reader);
                        dataGridView2.DataSource = dt; // สร้างตารางแล้วเอา db เข้าตาราง
                    }
                    catch (MySqlException ex)  // ตรวจจับ error
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                    finally // ถ้าสำเร็จจะทำการปิด db
                    {
                        conn.Close();
                    }
                    int total = 0;  //ประกาศตัวแปร total ชนิด double เพื่อเก็บผลรวมของค่าที่จะคำนวณในลูปถัดไป
                    foreach (DataGridViewRow r in dataGridView2.Rows) //ดึงตารางจากข้อมูลมาเรื่อย ๆ
                    {
                        {
                            total += Convert.ToInt32(r.Cells["Product_total"].Value);// บวกคอลลัมProduct_totalในตารางไปเรื่อยๆ 
                        }


                    }
                    int vat = (total * 7) / 100;
                    textBox6.Text = (vat + total).ToString();
                    best_selling();
                }
                conn1.Close();

            }
            else if (comboBox1.SelectedIndex == 1)   // ค้นวันเดือนปี
            {
                //สร้างการเชื่อมต่อกับฐานข้อมูล:
                MySqlConnection conn = databaseConnection();  //เก็บข้อมูลเข้า cmd
                //สร้างคำสั่ง SQL เพื่อดึงข้อมูลจากตาราง history ตามวัน เดือน และปี ที่กำหนดใน textBox3, textBox4, และ textBox5 ตามลำดับ โดยใช้พารามิเตอร์ @day, @month, และ @year
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM history WHERE day = @day AND month = @month AND year = @year", conn);
                cmd.Parameters.AddWithValue("@day", textBox3.Text);
                cmd.Parameters.AddWithValue("@month", textBox4.Text);
                cmd.Parameters.AddWithValue("@year", textBox5.Text);
                //สร้าง DataTable เพื่อเก็บผลลัพธ์ของการคิวรีจากฐานข้อมูล
                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                    //ตั้งค่า DataGridView dataGridView2 เพื่อแสดงผลลัพธ์จาก DataTable
                    dataGridView2.DataSource = dt;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
                double total = 0;  //ประกาศตัวแปร total ชนิด double เพื่อเก็บผลรวมของค่าที่จะคำนวณในลูปถัดไป
                foreach (DataGridViewRow r in dataGridView2.Rows)
                {
                    {
                        // แปลงค่า Product total เป็นตัวเลขแล้วมาบวกเข้า tatal
                        total += Convert.ToInt32(r.Cells["Product_total"].Value);  // บวกคอลลัมProduct_totalในตารางไปเรื่อยๆ 
                    }

                }
                double vat = (total * 7) / 100;

                textBox6.Text = (vat + total).ToString();
                best_selling(); // โชว์สินค้าที่ขายดี

            }
            else if (comboBox1.SelectedIndex == 2)   // เลือกเดือน
            {
                MySqlConnection conn = databaseConnection();  //เลือกจาก history โดยอิงจากเดือนและปี
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM history WHERE month = @month AND year = @year", conn);
                cmd.Parameters.AddWithValue("@month", textBox4.Text);
                cmd.Parameters.AddWithValue("@year", textBox5.Text);
                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);  //ดึงมาจาก reader  และเก็บไว้ในตาราง db
                    dataGridView2.DataSource = dt;
                }
                catch (MySqlException ex)    // เช็ค error
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
                double total = 0;  //ประกาศตัวแปร total ชนิด double เพื่อเก็บผลรวมของค่าที่จะคำนวณในลูปถัดไป
                foreach (DataGridViewRow r in dataGridView2.Rows)
                {
                    {
                        // แปลงค่า Product total เป็นตัวเลขแล้วมาบวกเข้า tatal
                        total += Convert.ToInt32(r.Cells["Product_total"].Value);
                    }

                }
                double vat = (total * 7) / 100;

                textBox6.Text = (vat + total).ToString();
                best_selling();  // คำนวนและโชว์สินค้าขายดี

            }
            else if (comboBox1.SelectedIndex == 3)   //การเลือกปี
            {
                MySqlConnection conn = databaseConnection();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM history WHERE year = @year", conn);
                cmd.Parameters.AddWithValue("@year", textBox5.Text); // ใส่ปี
                DataTable dt = new DataTable();  //สร้างตัวแปรที่เอาไว้เรียกตาราง
                try
                {
                    conn.Open();
                    MySqlDataReader reader=cmd.ExecuteReader();
                    dt.Load(reader); //ดึงมาจาก reader  และเก็บไว้ในตาราง db
                    dataGridView2.DataSource = dt;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
                double total = 0;  //ประกาศตัวแปร total ชนิด double เพื่อเก็บผลรวมของค่าที่จะคำนวณในลูปถัดไป
                foreach (DataGridViewRow r in dataGridView2.Rows)
                {
                    {
                        // แปลงค่า Product total เป็นตัวเลขแล้วมาบวกเข้า tatal
                        total += Convert.ToInt32(r.Cells["Product_total"].Value);
                    }

                }
                double vat = (total * 7) / 100;

                textBox6.Text = (vat + total).ToString();
                best_selling();  // คำนวนและโชว์สินค้าขายดี


            }
            else if (comboBox1.SelectedIndex == 4)   // ค้นหาจากเลขของบิล
            {
                MySqlConnection conn = databaseConnection();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM history WHERE billnum = @billnum", conn);
                cmd.Parameters.AddWithValue("@billnum", textBox1.Text); //ให้ใส่เลขบิล
                DataTable dt = new DataTable(); //สร้างตัวแปรที่เอาไว้เรียกตาราง
                try
                {
                    conn.Open();  //ประกาศตัวแปร total ชนิด double เพื่อเก็บผลรวมของค่าที่จะคำนวณในลูปถัดไป
                    MySqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                    dataGridView2.DataSource = dt;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
                double total = 0;   //ประกาศตัวแปร total ชนิด double เพื่อเก็บผลรวมของค่าที่จะคำนวณในลูปถัดไป
                foreach (DataGridViewRow r in dataGridView2.Rows)
                {
                    {
                        // แปลงค่า Product total เป็นตัวเลขแล้วมาบวกเข้า tatal
                        total += Convert.ToDouble(r.Cells["Product_total"].Value);
                    }

                }
                double vat = (total * 7) / 100;

                textBox6.Text = (vat + total).ToString();
                best_selling(); // คำนวนและโชว์สินค้าขายดี
            }
        

        }
        private void best_selling()      //ฟังก์ชั่นของ bestselling ดึงข้อมูลจาก ตารางมาเช็คและเก็บไว้
        {
            // ประกาศตัวแปร productQuantities ซึ่งเป็น Dictionary ชนิดข้อมูล string เป็นกุญแจ (key) และ int เป็นค่า (value) จากนั้นก็สร้างอินสแตนซ์ของ Dictionary นี้
            Dictionary<string, int> productQuantities = new Dictionary<string, int>();

            // จะเป็นการวนลูปผ่านแต่ละแถวใน DataGridView dataGridView2 และประมวลผลข้อมูลในแต่ละแถว
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.Cells["Product_name"].Value != null && row.Cells["quantity"].Value != null) //เช็คว่ามีค่าในช่องชื่่อ กับจำนวน
                {
                    string product = row.Cells["Product_name"].Value.ToString(); //สร้างตัวแปรไว้เก็บชื่อ
                    int quantity = Convert.ToInt32(row.Cells["quantity"].Value);   //สร้างตัวแปรไว้เก็บจำนวน

                    // รวมปริมาณตามรหัสินค้า
                    if (productQuantities.ContainsKey(product))//เช็คว่ากล่องนี้มีชื่อยัง
                    {
                        productQuantities[product] += quantity;    
                    }
                    else
                    {
                        productQuantities[product] = quantity; //รวมจำนวนสินค้าที่ชื่อซ้ำกันของสินค้านั้น
                    }
                }
            }
            // ส่วนที่เอาไว้สร้างตาราง
            DataTable aggregatedDataTable = new DataTable();
            aggregatedDataTable.Columns.Add("Rank", typeof(int));
            aggregatedDataTable.Columns.Add("name", typeof(string));
            aggregatedDataTable.Columns.Add("TotalQuantity", typeof(int));

            //เพิ่ม DataTable ด้วยปริมาณรวม
            int rank = 0; 
            foreach (var kvp in productQuantities) // kvp คือของที่ยุใน productQuantities
            {
                aggregatedDataTable.Rows.Add (rank,kvp.Key, kvp.Value);
            }

            // ตั้งค่าแหล่งข้อมูลของ dataGridView3 ให้เป็น aggregatedDataTable ซึ่งเป็น DataTable ที่มีข้อมูลที่รวมกันแล้ว
            dataGridView3.DataSource = aggregatedDataTable;
            //เรียงลำดับแถวใน dataGridView3 ตามคอลัมน์ TotalQuantity ในลำดับจากมากไปน้อย
            dataGridView3.Sort(dataGridView3.Columns["TotalQuantity"], System.ComponentModel.ListSortDirection.Descending);
            for (int i = 0; i < dataGridView3.Rows.Count; i++)
            {
                dataGridView3.Rows[i].Cells["Rank"].Value = (i + 1).ToString();
            }
            //ประกาศตัวแปร rankLimit ชนิด int และกำหนดค่าเป็น 3 ซึ่งหมายถึงการแสดงผลเพียง 3 อันดับแรกเท่านั้น
            int rankLimit = 3;
            //ใช้ลูป for เพื่อวนลูปผ่านแต่ละแถวใน dataGridView3 โดยเริ่มจากอันดับที่กำหนด (rankLimit)
            for (int i = rankLimit; i < dataGridView3.Rows.Count; i++)
            {
                dataGridView3.Rows[i].Visible = false;
            }


        }
        //คลิก 2 ที ตารางโชว์ภาพ
        //ประกาศเมธอดที่ถูกเรียกใช้เมื่อมีการดับเบิลคลิกที่เซลล์ใน dataGridView3
        private void dataGridView3_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //แสดง pictureBox2 และซ่อน dataGridView3:
            pictureBox2.Visible = true;
            dataGridView3.Visible = false;
            //หาค่าของแถวที่ถูกเลือกใน dataGridView3
            int selectedRow = dataGridView3.CurrentCell.RowIndex;
            //ดึงค่าของเซลล์ที่ชื่อ name จากแถวที่ถูกเลือก:
            string select = dataGridView3.Rows[selectedRow].Cells["name"].Value.ToString();
            //สร้างการเชื่อมต่อกับฐานข้อมูลและเตรียมคำสั่ง SQL: และเตรียมคำสั่ง SQL เพื่อดึงข้อมูลภาพ (image) จากตาราง product ที่มีชื่อสินค้าตรงกับค่าที่เลือก
            MySqlConnection conn = databaseConnection();
            String sql = "SELECT image FROM product WHERE name = '" + select + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);

            conn.Open();
            MySqlDataReader reader = cmd.ExecuteReader();

            //วนลูปผ่านผลลัพธ์ของการคิวรีและแปลงข้อมูลภาพ:
            while (reader.Read ())
            {
                //แปลงข้อมูลภาพเป็น byte array
                byte[] productImageBytes = (byte[])reader["image"];
                // ตรวจสอบว่าข้อมูลภาพไม่เป็นค่าว่างและมีขนาดมากกว่า 0
                if (productImageBytes != null && productImageBytes.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream(productImageBytes))
                    {
                        pictureBox2.Image = new Bitmap(ms);
                        Console.WriteLine("add picture");
                    }
                }

            }
            conn.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)   
        {
            // คลิกรูปภาพแล้วรู้ภาพจะหายไปตารางจะขึ้นมาแทน
            pictureBox2.Visible = false;
            dataGridView3.Visible = true;

        }

        //เปิดไฟล์บิล ตาราง1
        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)  
        {
            int selectedRow = dataGridView1.CurrentCell.RowIndex;
            string pdf = dataGridView1.Rows[selectedRow].Cells["pdf"].Value.ToString();
            Process.Start(pdf);
        }

        //เปิดไฟล์บิล ตาราง2
        private void dataGridView2_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)  
        {
            int selectedRow = dataGridView2.CurrentCell.RowIndex;
            string pdf = dataGridView2.Rows[selectedRow].Cells["pdf"].Value.ToString();
            Process.Start(pdf);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Edit edit = new Edit();
            edit.Show();
            this.Hide();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
    
}
