using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace shop
{
    public partial class qry : Form
    {
        private string _product;
        public string pd 
        {
            get { return _product; }
            set { _product = value; Namep.Text = value; }
        }
        private int _q;
        public int pdq 
        {
            get { return _q; }
            set { _q = value; textBox1.Text = value.ToString(); }
        }
        private int price;
        public int pr
        {
            get { return price; }
            set { price = value; }
        }
        private string usern;
        public string us
        {
            get { return usern; }
            set { usern = value; }
        }

        public qry()
        {
            InitializeComponent();
        }
        bool Overstock = false;
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=category;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }


        private void Namep_Click(object sender, EventArgs e)
        {

        }

        private void qry_Load(object sender, EventArgs e)
        {

        }
        private void ReduceProductQuantity(string nameproduct, int qtyproduct)    //เอาจำนวนที่กรอกมาลบจำนวน stock   เป็นการเอาเข้าตระกร้าแล้วก็ตัดสต๊อค
        {
            // สตริงคำสั่ง SQL เพื่อเลือกจำนวนสินค้าจากฐานข้อมูล
            string selectQuery = "SELECT quantity FROM product WHERE name = '" + nameproduct + "'";  //ดึงจำนวนสินค้าจากชื่อสต๊อค

            using (MySqlConnection conn = databaseConnection())
            {
                try
                {
                    conn.Open();
                    MySqlCommand selectCmd = new MySqlCommand(selectQuery, conn);
                    MySqlDataReader reader = selectCmd.ExecuteReader();   // มีจำนวนสินค้าจากชื่อ

                    while (reader.Read())
                    {
                        // แปลงจำนวนสินค้าที่ได้จากฐานข้อมูลเป็น int
                        int stock = Convert.ToInt32(reader["quantity"]);
                        // ตรวจสอบว่าจำนวนสินค้าที่ต้องการลดมีอยู่ในสต็อกพอหรือไม่
                        if (stock >= qtyproduct)
                        {
                            Console.WriteLine(stock);
                            // คำนวณจำนวนสินค้าที่เหลือหลังจากลดแล้ว
                            int quantityToReduce = stock - qtyproduct; //ตัดสต๊อค

                            Console.WriteLine(quantityToReduce);
                            // อัพเดทจำนวนสินค้าในฐานข้อมูล
                            UpdateProductQuantity(nameproduct, quantityToReduce);
                        }
                        else
                        {
                            MessageBox.Show("สินค้าเกินจำนวนที่มีใน stock: ");
                            Overstock = true;
                            return;
                        }


                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reducing product quantity: " + ex.Message);
                }

                string updateQuery = "UPDATE carts SET qty =  @quantity, totalprice = @totalprice WHERE name = @name AND username = @username";
                using (MySqlConnection conne = databaseConnection())
                {
                    try
                    {       
                        conne.Open();
                        MySqlCommand updateCmd = new MySqlCommand(updateQuery, conne);
                        updateCmd.Parameters.AddWithValue("@quantity", textBox1.Text);
                        updateCmd.Parameters.AddWithValue("@totalprice", pr * Convert.ToInt32(textBox1.Text));
                        updateCmd.Parameters.AddWithValue("@username", us);
                        updateCmd.Parameters.AddWithValue("@name", nameproduct);
                        updateCmd.ExecuteNonQuery();


;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error updating product quantity: " + ex.Message);
                    }
                }
            }
        }
        private void IncreaseProductQuantity(string nameproduct, int qtyproduct)      //เอาจำนวนที่กรอกเพิ่มจำนวน stock ลูกค้าคืนของ
        {
            string selectQuery = "SELECT quantity FROM product WHERE name = '" + nameproduct + "'";

            using (MySqlConnection conn = databaseConnection())
            {
                try
                {
                    conn.Open();
                    // สร้างและรันคำสั่ง SQL เพื่อดึงจำนวนสต็อกปัจจุบันของสินค้า
                    MySqlCommand selectCmd = new MySqlCommand(selectQuery, conn);
                    MySqlDataReader reader = selectCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        // แปลงค่าจำนวนสต็อกจากฐานข้อมูลเป็นตัวเลขจำนวนเต็ม
                        int stock = Convert.ToInt32(reader["quantity"]);
                        Console.WriteLine(stock);
                        // คำนวณจำนวนสต็อกใหม่โดยการบวกจำนวนที่ต้องการคืนค่าสต๊อค
                        int quantityToIncrease = stock + qtyproduct;
                        Console.WriteLine(quantityToIncrease);
                        // เรียกใช้ฟังก์ชัน UpdateProductQuantity เพื่ออัปเดตจำนวนสต็อกในฐานข้อมูล
                        UpdateProductQuantity(nameproduct, quantityToIncrease);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reducing product quantity: " + ex.Message);
                }
                string updateQuery = "UPDATE carts SET qty =  @quantity, totalprice = @totalprice WHERE name = @name AND username = @username";

                using (MySqlConnection conne = databaseConnection())
                {
                    try
                    {
                        conne.Open();
                        MySqlCommand updateCmd = new MySqlCommand(updateQuery, conne);
                        updateCmd.Parameters.AddWithValue("@quantity", textBox1.Text);
                        updateCmd.Parameters.AddWithValue("@totalprice", pr * Convert.ToInt32(textBox1.Text));
                        updateCmd.Parameters.AddWithValue("@username", us);
                        updateCmd.Parameters.AddWithValue("@name", nameproduct);


                        updateCmd.ExecuteNonQuery();

                        ;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error updating product quantity: " + ex.Message);
                    }
                }
            }
        }
        private void UpdateProductQuantity(string nameproduct, int quantityToReduce)
        {
       
            string updateQuery = "UPDATE product SET quantity =  @quantity WHERE name = '" + nameproduct + "'";

            using (MySqlConnection conn = databaseConnection())
            {
                try
                {
                    conn.Open();
                    MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@quantity", quantityToReduce);



                    updateCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating product quantity: " + ex.Message);
                }
            }
        }
        private void Update(string nameproduct, int quantityToReduce)  //อัปปเดต
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
           
            
            int qq = int.Parse(textBox1.Text);
           

            if ( qq < pdq)    
            {
             
                int nq = pdq - qq;
              /*  Overstock = false;  //สินค้าไม่เกินสต๊อคจะเพิ่มเข้าตระกร้าได้ */
                IncreaseProductQuantity(Namep.Text, nq);


                //if (Overstock == false) //เช็คของให้สินค้าถ้าตัดได้ 
                //{


                //    MySqlConnection conn = databaseConnection();//เอาเข้าตระกร้า

                //    String sql = "INSERT INTO carts (name, qty, price,totalprice ,username) VALUES(@name, @quantity, @price, @totalprice ,@user) ";
                //    MySqlCommand cmd = new MySqlCommand(sql, conn);

                //    cmd.Parameters.AddWithValue("@name", nameproduct); //กำหนดค่า
                //    cmd.Parameters.AddWithValue("@quantity", qtyproduct);
                //    cmd.Parameters.AddWithValue("@price", ((item)sender).price_item);
                //    cmd.Parameters.AddWithValue("@totalprice", (qtyproduct * ((item)sender).price_item));
                //    cmd.Parameters.AddWithValue("@user", user_name);

                //    try
                //    {
                //        conn.Open();
                //        int rows = cmd.ExecuteNonQuery();  //เพิ่มตารางใน db
                //        if (rows > 0)  //เพิ่มได้สำเร็จ  
                //        {

                //            MessageBox.Show("เพิ่มข้อมูลสำเร็จ");
                //            dataGridView1.Controls.Clear();
                //            showEquipment();

                //        }
                //    }
                //    catch (MySqlException ex)
                //    {
                //        MessageBox.Show("Error: " + ex.Message);
                //    }
                //    finally
                //    {
                //        conn.Close();
                //    }




                //}
                //else      //ตัดสต็อคไม่ได้ก็จะหยุด
                //{
                //    return;
                //}
                this.Close();



            }
            else if ( qq > pdq ) 
            {
                MessageBox.Show("new < old");
                int nq = qq- pdq;
                ReduceProductQuantity(Namep.Text, nq);
                this.Close();

              


            }
          
          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

// รีไม่ได้ 
