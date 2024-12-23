using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Primitives;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace shop
{
    public partial class category : Form
    {
        private MySqlConnection con;
        private byte[] imageBytes;
        double total = 0;
        double all = 0;
        private string _user;

        public string QR_total    //ดึงค่าส่งไปหน้า QR
        {
            get { return textBox3.Text; }       
        }
        public string user_name  // เอา username มาไว้หน้า category
        {
            get { return _user; }
            set { _user = value; user.Text = value; }
        }

        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=category;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }

        public category()
        {
            InitializeComponent();
            con = databaseConnection();
            
        }

                                              

        // กำหนดขนาดของปุ่ม
        private const int BUTTON_WIDTH = 170;
        private const int BUTTON_HEIGHT = 170;
        private const int BUTTON_PADDING = 40;

         bool Overstock = false;

        
        private void LoadProductButtons(string query = "SELECT id, name, quantity, price, image FROM product")
        {
            int buttonCount = 0;  //เก็บจำนวนปุ่มที่ถูกสร้างขึ้น
            int currentRow = 0;  //เก็บค่าของแถวปัจจุบันที่ปุ่มถูกแสดงอยู่
            using (MySqlConnection connection = databaseConnection())
            {
                MySqlCommand command = new MySqlCommand(query, connection);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int productId = reader.GetInt32(0);   //อ่านค่าจากคอลัมน์แรกในผลลัพธ์ของ query ซึ่งมีลำดับเป็น 0
                        string productName = reader.GetString(1);  //อ่านค่าจากคอลัมน์ที่สองในผลลัพธ์ของ query ซึ่งมีลำดับเป็น 1
                        decimal productPrice = reader.GetDecimal(3);
                        byte[] productImageBytes = (byte[])reader["image"];  //อ่านจาก pic เก็บไว้ในตัวแปร productImageBytes

                        // สร้างอ็อบเจกต์ของคลาส PictureBox 
                        PictureBox bg_product = new PictureBox();
                        bg_product.Width = BUTTON_WIDTH;
                        bg_product.Height = BUTTON_HEIGHT;
                        bg_product.SizeMode = PictureBoxSizeMode.StretchImage;
                        bg_product.Image = Image.FromStream(new MemoryStream(productImageBytes));
                        bg_product.Location = new Point((buttonCount % 3) * (BUTTON_WIDTH + BUTTON_PADDING),
                        currentRow * (BUTTON_HEIGHT + BUTTON_PADDING));
                        bg_product.Size = new Size(BUTTON_WIDTH + 20, BUTTON_HEIGHT + 50);
                        bg_product.BackColor = Color.White;

                        // ปุ่มเลือกซื้อ
                        System.Windows.Forms.Button productButton = new System.Windows.Forms.Button();
                        productButton.Size = new Size(100, 25);
                        productButton.Text = productName;
                        productButton.Tag = productId;
                        productButton.Click += ProductButton_Click;  //เชื่อมโยงเหตุการณ์คลิก
                        productButton.ForeColor = Color.Red;
                        productButton.BackColor = Color.White;

                        // กำหนดตำแหน่งปุ่ม
                        productButton.Location = new Point(bg_product.Left + (bg_product.Width - productButton.Width) / 2, bg_product.Top + bg_product.Height - 30);
                        panel2.Controls.Add(bg_product);
                        panel2.Controls.Add(productButton);

                        panel2.AutoScroll = true;

                        //เพิ่มจำนวนปุ่มและการเพิ่มแถวในการจัดวาง
                        buttonCount++;
                        if (buttonCount % 3 == 0)
                        {
                            currentRow++;
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)  //จัดการข้อผิดพลาด
                {
                    MessageBox.Show("Error: " + ex.Message);  //ตามด้วยข้อความข้อผิดพลาด
                }
            }
        }

        private void ProductButton_Click(object sender, EventArgs e)           
        {
            System.Windows.Forms.Button clickedButton = sender as System.Windows.Forms.Button;
            int productId = (int)clickedButton.Tag;
            string productName = clickedButton.Text;

            // เนื่องจาก ProductPrice ควรอยู่ใน ProductButton หรือวิธีอื่นในการเข้าถึงราคาจริง
            decimal productPrice = 0;

            // ส่งข้อมูลไปยังเมธอดที่จะใช้ในการอัพเดท DataGridView
            AddProductToCart(productId, productName, productPrice);
        }

        private void AddProductToCart(int productId, string productName, decimal productPrice)
        {
            try
            {
                // ดำเนินการเพิ่มสินค้าลงในตะกร้าของลูกค้าที่นี่
                // เพิ่มข้อมูลลงในตาราง carts ในฐานข้อมูล
                string connectionString = "server=localhost;user=root;password=;database=category";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    // ตรวจสอบว่ามีสินค้าอยู่ในตะกร้าแล้วหรือไม่
                    MySqlCommand checkCommand = new MySqlCommand("SELECT COUNT(*) FROM carts WHERE id = @productId", connection);
                    checkCommand.Parameters.AddWithValue("@productId", productId);
                    int existingCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (existingCount > 0)
                    {
                        // หากสินค้ามีอยู่ในตะกร้าแล้ว ให้ปรับปรุงจำนวนและราคารวม
                        MySqlCommand updateCommand = new MySqlCommand("UPDATE carts SET qty = qty + 1, totalprice = price * qty WHERE id = @productId", connection);
                        updateCommand.Parameters.AddWithValue("@productId", productId);
                        updateCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        // หากสินค้ายังไม่มีในตะกร้า ให้เพิ่มรายการใหม่
                        MySqlCommand priceCommand = new MySqlCommand("SELECT price FROM product WHERE id = @productId", connection);
                        priceCommand.Parameters.AddWithValue("@productId", productId);
                        decimal retrievedPrice = (decimal)priceCommand.ExecuteScalar();
                        MySqlCommand insertCommand = connection.CreateCommand();
                        insertCommand.CommandText = "INSERT INTO carts (id, name, qty, price, totalprice) VALUES (@productId, @productName, 1, @retrievedPrice, @retrievedPrice)";
                        insertCommand.Parameters.AddWithValue("@productId", productId);
                        insertCommand.Parameters.AddWithValue("@productName", productName);
                        insertCommand.Parameters.AddWithValue("@retrievedPrice", retrievedPrice);
                        insertCommand.ExecuteNonQuery();
                    }
                }
                // อัพเดท DataGridView
                showCarts();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void showCarts()
        {
            try
            {
                string connectionString = "server=localhost;user=root;password=;database=category";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("SELECT name, qty, price, totalprice FROM carts", connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command); //เอาจากsql เข้า adapter
                    DataTable dataTable = new DataTable();  //  กำหนดตัวแปร data grid
                    adapter.Fill(dataTable);     // เข้ามูลจาก db เติมเข้าใน datagrid

                    // ล้างรายการและคอลัมน์ที่มีอยู่
                    dataGridView1.Columns.Clear();
                    dataGridView1.Rows.Clear();
                    // เพิ่มคอลัมน์ใน DataGridView

                    // Set column widths
                    dataGridView1.Columns["ProductName"].Width = 144; // Set width of product name column
                    dataGridView1.Columns["ProductQuantity"].Width = 45; // Set width of product quantity column
                    dataGridView1.Columns["ProductPrice"].Width = 45; // Set width of product price column
                    dataGridView1.Columns["ProductTotal"].Width = 45; // Set width of product total column
                    dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // คอลัมน์ที่ 2
                    dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // คอลัมน์ที่ 3
                    dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // คอลัมน์ที่ 41
                    decimal subtotal = 0; // ยอดรวมราคาสินค้าก่อนคิดภาษี
                    foreach (DataRow row in dataTable.Rows)
                    {
                        string productName = row["name"].ToString();
                        int productQuantity = Convert.ToInt32(row["qty"]); // จำนวนสินค้า
                        decimal productPrice = Convert.ToDecimal(row["price"]);
                        decimal productTotal = Convert.ToDecimal(row["totalprice"]); // ราคารวมสินค้า

                        // เพิ่มรายการสินค้าลงใน DataGridView
                        dataGridView1.Rows.Add(productName, productQuantity, productPrice, productTotal);

                        // เพิ่มราคารวมสินค้าลงในยอดรวม
                        subtotal += productTotal; // เพิ่มราคารวมสินค้าทุกรายการ
                    }
                    // คำนวณภาษีมูลค่าเพิ่ม (VAT) 7%
                    decimal vat = subtotal * 0.07m;

                    // คำนวณยอดรวมทั้งสิ้น (รวมภาษีแล้ว)
                    decimal total = subtotal + vat;

                    // แสดงยอดรวมและภาษีมูลค่าเพิ่ม (VAT) ใน TextBox
                    textBox1.Text = subtotal.ToString("N2");
                    textBox2.Text = vat.ToString("N2");
                    textBox3.Text = total.ToString("N2");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                // ลบข้อมูลในตาราง carts หลังจากแสดงบิลใบเสร็จ
                //string connectionString = "server=localhost;user=root;password=;database=admin";
                //using (MySqlConnection connection = new MySqlConnection(connectionString))
                //{
                //    connection.Open();
                //    MySqlCommand deleteCommand = new MySqlCommand("DELETE FROM carts", connection);
                //    deleteCommand.ExecuteNonQuery();
                //}
            }
        }

        private void AddProductToCart(int productId, string productName)
        {
            try
            {
                // ดำเนินการเพิ่มสินค้าลงในตะกร้าของลูกค้าที่นี่
                // เพิ่มข้อมูลลงในตาราง carts ในฐานข้อมูล
                string connectionString = "server=localhost;user=root;password=;database=category";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    // ตรวจสอบว่ามีสินค้าอยู่ในตะกร้าแล้วหรือไม่
                    MySqlCommand checkCommand = new MySqlCommand("SELECT COUNT(*) FROM carts WHERE id = @productId", connection);
                    checkCommand.Parameters.AddWithValue("@productId", productId);
                    int existingCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (existingCount > 0)
                    {
                        // หากสินค้ามีอยู่ในตะกร้าแล้ว ให้ปรับปรุงจำนวนและราคารวม
                        MySqlCommand updateCommand = new MySqlCommand("UPDATE carts SET qty = qty + 1, totalprice = price * qty WHERE id = @productId", connection);
                        updateCommand.Parameters.AddWithValue("@productId", productId);
                        updateCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        // หากสินค้ายังไม่มีในตะกร้า ให้เพิ่มรายการใหม่
                        MySqlCommand priceCommand = new MySqlCommand("SELECT price FROM product WHERE id = @productId", connection);
                        priceCommand.Parameters.AddWithValue("@productId", productId);
                        decimal retrievedPrice = (decimal)priceCommand.ExecuteScalar();

                        MySqlCommand insertCommand = connection.CreateCommand();
                        insertCommand.CommandText = "INSERT INTO carts (id, name, qty, price, totalprice) VALUES (@productId, @productName, 1, @retrievedPrice, @retrievedPrice)";
                        insertCommand.Parameters.AddWithValue("@productId", productId);
                        insertCommand.Parameters.AddWithValue("@productName", productName);
                        insertCommand.Parameters.AddWithValue("@retrievedPrice", retrievedPrice);
                        insertCommand.ExecuteNonQuery();
                    }
                }
                // อัพเดท DataGridView
                showCart();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void showCart()
        {
            try
            {
                // เขียนโค้ดเพื่อแสดงรายการสินค้าในตะกร้า
            }
            catch (Exception ex)   // เก็บ error
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)  //ปุ่มปิดหน้าจอ และล้างข้อมูลสินค้าในตระกร้า
        {
            if (dataGridView1.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["id"].Value != null)
                    {
                        //int deleteId = Convert.ToInt32(row.Cells["id"].Value);
                        string deletename = row.Cells["name"].Value.ToString();
                        int deleteqty = Convert.ToInt32(row.Cells["qty"].Value);

                        IncreaseProductQuantity(deletename, deleteqty);
                    }
                }
                showitem();

            }
            else
            {
                MessageBox.Show("ไม่มีข้อมูลใน DataGridView");
            }
            this.Close();
            Form form1 = new Form1();
            form1.Show();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void showitem(string query = "SELECT id, name, quantity, price, image FROM product")  //แสดงรูปภาพ ///เริ่มตรงนี้//
        {
            Console.WriteLine("add product");

            flowLayoutPanel1.Controls.Clear();   //เคลียหน้า panel แล้วดึง itemมาแทน
            using (MySqlConnection connection = databaseConnection())
            {
                MySqlCommand command = new MySqlCommand(query, connection);

                try
                {
                    connection.Open();  // ดึง item มา
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())     // คือการเอาข้อมูลมาไว้ใน item
                    {
                        item i = new item(); // กำหนดตัวแปรหน้า item 
                        i.name_item = reader.GetString(1);  //อ่านค่าจากคอลัมน์ที่สองในผลลัพธ์ของ query ซึ่งมีลำดับเป็น 1
                        i.price_item = reader.GetInt32(3);   // อ่านค่าราคา ใน product
                        i.qty_item = reader.GetInt32(2);     // อ่านค่าจำนวนของสินค้า ใน product
                        i.buybutton += new EventHandler(add_cart); //ใส่ฟังก์ชั่นปุ่มหน้าไอเท็มทำให้ปุ่มทำงานได้
                        byte[] productImageBytes = (byte[])reader["image"];  //อ่านจาก pic เก็บไว้ในตัวแปร productImageBytes
                        if (productImageBytes != null && productImageBytes.Length > 0)   // ถ้ามีรูปภาพใน ดาต้าเบสก็จะทำงาน
                        {
                            using (MemoryStream ms = new MemoryStream(productImageBytes))    //ดึงรูปภาพในดาต้าเบสมาไว้picturboxในไอเท็ม
                            {
                                i.picture_item = new Bitmap(ms);
                                
                            }
                        }
                       
                            flowLayoutPanel1.Controls.Add(i); //เอาไอเท็มใส่  flowLayoutPanel1
                    }
                }
                catch (Exception ex)  //จัดการข้อผิดพลาด
                {
                    MessageBox.Show("Error: " + ex.Message);  //ตามด้วยข้อความข้อผิดพลาด
                }
            }
        }
        protected void add_cart(Object sender, EventArgs e) //ปุ่มเพิ่มเข้าตระกร้า
        {
            Overstock = false;  //สินค้าไม่เกินสต๊อคจะเพิ่มเข้าตระกร้าได้ 
            item selected_item = (item)sender; // ประกาศตัวแปร

            string nameproduct = selected_item.name_item;
            Console.WriteLine(nameproduct);        // ดึงชื่อจากไอเท็ม

            int qtyproduct = Int32.Parse(selected_item.qty_select);// ดึงจำนวนจากไอเท็ม
            Console.WriteLine(qtyproduct);

            ReduceProductQuantity(nameproduct, qtyproduct); // ดึงจำนวนสต๊อคจากไอเท็ม

            showitem();
            if (Overstock == false) //เช็คของให้สินค้าถ้าตัดได้ 
            {


                MySqlConnection conn = databaseConnection();//เอาเข้าตระกร้า
              
                String sql = "INSERT INTO carts (name, qty, price,totalprice ,username) VALUES(@name, @quantity, @price, @totalprice ,@user) ";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@name", nameproduct); //กำหนดค่า
                cmd.Parameters.AddWithValue("@quantity", qtyproduct);
                cmd.Parameters.AddWithValue("@price", ((item)sender).price_item);
                cmd.Parameters.AddWithValue("@totalprice", (qtyproduct * ((item)sender).price_item));
                cmd.Parameters.AddWithValue("@user",user_name);

                try
                {
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();  //เพิ่มตารางใน db
                    if (rows > 0)  //เพิ่มได้สำเร็จ  
                    {
                       
                        MessageBox.Show("เพิ่มข้อมูลสำเร็จ");
                        dataGridView1.Controls.Clear();
                        showEquipment();

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
            else      //ตัดสต็อคไม่ได้ก็จะหยุด
            {
                return;
            }
        }
        private void showEquipment()      //โชว์ตาราง
        {

            dataGridView1.Controls.Clear();
            MySqlConnection conn = databaseConnection();
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM carts WHERE username =@user", conn); //อิงจากชื่อเพราะมีหลายคนซื้อ
            cmd.Parameters.AddWithValue("@user", user.Text);
            DataTable dt = new DataTable();

            try
            {
                conn.Open();
                MySqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                dataGridView1.DataSource = dt;
                dataGridView1.AutoGenerateColumns = false; // ซ่อนไอดี
                dataGridView1.Columns["id"].Visible = false;

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
            total_func();  // คิดยอดรวม
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
                            MessageBox.Show("สินค้าเกินจำนวนที่มีใน stock: " );
                            Overstock = true;
                            return;
                        }
                            

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reducing product quantity: " + ex.Message);
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

        private void button1_Click(object sender, EventArgs e)  //ปุ่มลบสิ่งที่เลือก
        {   
            //ตรวจสอบว่าเซลล์ถูกเลือกหรือไม่
            if (dataGridView1.SelectedCells.Count > 0)
            {
                //หาค่าของแถวที่มีเซลล์ที่ถูกเลือกอยู่
                int selectedRow = dataGridView1.CurrentCell.RowIndex;
                //ดึงค่า id ของแถวที่ถูกเลือกและแปลงเป็นตัวเลข
                int deleteId = Convert.ToInt32(dataGridView1.Rows[selectedRow].Cells["id"].Value);
                //ดึงค่า name แปลงเป็นตัวหนังสือ
                string deletename = (dataGridView1.Rows[selectedRow].Cells["name"].Value).ToString();
                int deleteqty = Convert.ToInt32(dataGridView1.Rows[selectedRow].Cells["qty"].Value);

                IncreaseProductQuantity(deletename,deleteqty);
                showitem();
                MySqlConnection conn = databaseConnection();
                String sql = "DELETE FROM carts WHERE id = '" + deleteId + "'";  //ดึงข้อมูล
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                conn.Close();
                if (rows > 0)
                {
                    MessageBox.Show("ลบข้อมูลสำเร็จ");
                    showEquipment();


                }
            }
        }

        private void button2_Click(object sender, EventArgs e)  //ลบทั้งหมด   
        {
            //ตรวจสอบว่า DataGridView มีแถวอย่างน้อยหนึ่งแถวหรือไม่ ถ้าไม่มีแถว โค้ดในบล็อกนี้จะไม่ถูกรัน
            if (dataGridView1.Rows.Count > 0)
            {
                // foreach เพื่อวนลูปผ่านแต่ละแถวใน DataGridView
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    //ตรวจสอบว่าคอลัมน์ id ของแถวนั้นไม่เป็น null หรือไม่ 
                    if (row.Cells["id"].Value != null)
                    {
                        //ถ้าคอลัมน์ id มีค่า โค้ดนี้จะดึงค่าจากคอลัมน์ name และ qty ในแถวและแปลงเป็นชนิดข้อมูลฃ (string และ int ตามลำดับ)
                        string deletename = row.Cells["name"].Value.ToString();
                        int deleteqty = Convert.ToInt32(row.Cells["qty"].Value);

                        //ฟังก์ชันนี้ถูกเรียกเพื่อเพิ่มจำนวนสินค้าตามค่าที่ดึงมาได้ (deletename และ deleteqty)
                        IncreaseProductQuantity(deletename, deleteqty);
                    }
                }
                //เรียกฟังก์ชัน showitem() เพื่อแสดงหรืออัปเดตข้อมูลบางอย่างก่อนที่จะเริ่มกระบวนการลบข้อมูล
                showitem();

                // เชื่อต่อฐานข้อมูล
                MySqlConnection conn = databaseConnection();
                //สร้างคำสั่ง SQL สำหรับลบข้อมูล:
                String sql = "DELETE FROM carts";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                try
                {
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (rows > 0)
                    {
                        //ลบข้อมูล
                        MessageBox.Show("ลบข้อมูลทั้งหมดสำเร็จ");
                        showEquipment();
                        textBox1.Clear();
                        textBox2.Clear();
                        textBox3.Clear();
                        total = 0;
                    }
                    else
                    {
                        MessageBox.Show("ไม่มีข้อมูลให้ลบ");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
                }
            else
            {
                MessageBox.Show("ไม่มีข้อมูลใน DataGridView");
            }
        }
        private void total_func()     // คิดยอดรวมทั้งหมด
        {
            total = 0;
            if (dataGridView1.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["id"].Value != null)
                    {
                        
                        total += Convert.ToInt32(row.Cells["totalprice"].Value);
                       
                       
                    }
                }

            }
            Console.WriteLine(total);
            textBox1.Text = total.ToString();
            double vat = ((total * 7) / 100);
            textBox2.Text = vat.ToString("F2");
            double discount = ((( vat  + total) * 5) / 100);
            textBox4.Text = discount.ToString("F2");
            all = (total + vat) - discount;
            textBox3.Text = all.ToString("F2");
            //Console.WriteLine(price_QR);    //ยอดรวมหน้า QR
            Console.WriteLine("textbox3"+(textBox3.Text).ToString());

        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)    // ตรวจสอบข้อมูลให้ไม่มีช่องว่าง
        {

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {


        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            MyQR myQR = new MyQR
            {
                qr_total = all,    // รวมยอดหน้า QR
                user_qr = user_name
            };
            myQR.Show();
            this.Close();
            

        }

        private void category_Load(object sender, EventArgs e)
        {
            dataGridView1.Controls.Clear();
            showEquipment();
            showitem();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            showEquipment();
        }

        private void user_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private string nnn;
        private int pr = 0;
        private int qqq = 0;
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = null;
            foreach (DataGridViewCell selectedCell in dataGridView1.SelectedCells)
            {
                cell = selectedCell;
                break;
            }
            if (cell != null)
            {
                // คลิกชื่อสินค้าแล้วเปิดหน้าต่างเปลี่ยนจำนวน
                DataGridViewRow row = cell.OwningRow;
                 pr = Convert.ToInt32(row.Cells[3].Value);
                 nnn = row.Cells[1].Value.ToString();
                qqq = Convert.ToInt32(row.Cells[2].Value);

                textBox5.Text = qqq.ToString();
                panel3.Visible = true;
                // etc.
            }
            
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
        private void editUpdateProductQuantity(string nameproduct, int quantityToReduce)
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
        private void editIncreaseProductQuantity(string nameproduct, int qtyproduct)      //เอาจำนวนที่กรอกเพิ่มจำนวน stock ลูกค้าคืนของ
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
                        // เรียกใช้ฟังก์ชัน editUpdateProductQuantity เพื่ออัปเดตจำนวนสต็อกในฐานข้อมูล
                        editUpdateProductQuantity(nameproduct, quantityToIncrease);
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
                        updateCmd.Parameters.AddWithValue("@quantity", textBox5.Text);
                        updateCmd.Parameters.AddWithValue("@totalprice", pr * Convert.ToInt32(textBox5.Text));
                        updateCmd.Parameters.AddWithValue("@username", user.Text);
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
        private void editReduceProductQuantity(string nameproduct, int qtyproduct)    //เอาจำนวนที่กรอกมาลบจำนวน stock   เป็นการเอาเข้าตระกร้าแล้วก็ตัดสต๊อค
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
                            editUpdateProductQuantity(nameproduct, quantityToReduce);
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
                        updateCmd.Parameters.AddWithValue("@quantity", textBox5.Text);
                        updateCmd.Parameters.AddWithValue("@totalprice", pr * Convert.ToInt32(textBox5.Text));
                        updateCmd.Parameters.AddWithValue("@username", user.Text);
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
        private void button5_Click_1(object sender, EventArgs e)      //ปุ่มรีเฟรช
        {


            int qq = int.Parse(textBox5.Text);
            int pdq = qqq;


            if (qq < pdq)
            {

                int nq = pdq - qq;
            

                /*  Overstock = false;  //สินค้าไม่เกินสต๊อคจะเพิ่มเข้าตระกร้าได้ */
                editIncreaseProductQuantity(nnn, nq);


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
                MySqlConnection conn = databaseConnection();
                MySqlCommand cmd = new MySqlCommand("SELECT qty FROM carts WHERE username =@user AND name = @name",  conn); //อิงจากชื่อเพราะมีหลายคนซื้อ
                cmd.Parameters.AddWithValue("@user", user.Text);
                cmd.Parameters.AddWithValue("@name", nnn);
                int count = 0;
                conn.Open();

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    count = reader.GetInt32(0);
                }
                conn.Close();

                if (count <= 0)
                    {
                        
                             MySqlConnection conn1 = databaseConnection();
                        MySqlCommand cmd1 = new MySqlCommand("DELETE FROM carts WHERE  username =@user AND name = @name", conn1); //อิงจากชื่อเพราะมีหลายคนซื้อ
                        cmd1.Parameters.AddWithValue("@user", user.Text);
                        cmd1.Parameters.AddWithValue("@name", nnn);
                        conn1.Open();
                        int rows = cmd1.ExecuteNonQuery();
                        conn1.Close();
                        if (rows > 0)
                        {
                            MessageBox.Show("ลบข้อมูลสำเร็จ");
                            showEquipment();


                        }
                    }
                

                panel3.Visible = false;

                dataGridView1.Controls.Clear();
                showEquipment();
                showitem();

            }
            else if (qq > pdq)
            {
                //MessageBox.Show("new < old");
                int nq = qq - pdq;
                //MessageBox.Show(nq.ToString());

                editReduceProductQuantity(nnn, nq);
                panel3.Visible = false;

                dataGridView1.Controls.Clear();
                showEquipment();
                showitem();




            }


        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}