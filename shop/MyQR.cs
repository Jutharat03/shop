using System;
using System.Collections.Generic;
using System.ComponentModel;
using Saladpuk.PromptPay.Contracts;
using Saladpuk.PromptPay.Contracts.Models;
using Saladpuk.PromptPay.Facades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing.QrCode;
using ZXing;
using ZXing.Common;
using System.Windows.Media.Media3D;
using ZXing.Rendering;
using PdfSharp.Drawing.Layout;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net.Mail;
using System.Windows.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace shop
{
    public partial class MyQR : Form
    {
        public MyQR()
        {
            InitializeComponent();
        }
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=category;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        private MySqlConnection databaseConnection1()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=stock;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }

        private double _qr_total;  //รับค่า total จาก category
        public double qr_total           
        {                                        
            get { return _qr_total; }   
            set { _qr_total = value; }           
        }                                       
        private string _un;                //รับค่า ชื่อลูกค้า จาก category
        public string user_qr              
        {                                    
            get { return _un; }              
            set { _un = value;label1.Text = value; }           
        }                                          
        private void MyQR_Load(object sender, EventArgs e)  // MyQR ถูกโหลด
        {
            Console.WriteLine("qr total" + qr_total);    //ใช้คำสั่ง Console.WriteLine เพื่อแสดงค่า qr_total ในคอนโซลเพื่อการตรวจสอบและดีบัก

            double total_qr = Convert.ToDouble(qr_total);  //แปลงค่า doble แสดงผลในคอนโทรล tatal
            //total_qr เป็นสตริงในรูปแบบทศนิยม 2 ตำแหน่ง และแสดงผลในคอนโทรล total
            total.Text = total_qr.ToString("F2");

            // ตั้งค่าหมายเลขการรับเงิน
            string QR = PPay.DynamicQR.MobileNumber("0995043944").Amount(total_qr).CreateCreditTransferQrCode();
            ZXing.BarcodeWriter barcodeWriter = new ZXing.BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,// รูปแบบบาร์โค้ด
                Options = new EncodingOptions
                {
                    Width = 129,
                    Height = 129,
                    PureBarcode = true   // โชว์แค่ QR ไม่มีข้อความ
                },
            };

            // Create the bitmap for the QR code and display it in the PictureBox
            Bitmap bc = barcodeWriter.Write(QR);
            pictureBox2.Image = bc;

        }
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
            category category = new category();
            category.Show();
        }

        private void total_Click(object sender, EventArgs e)
        {

        }
        string productName;
        string ame;
        int quantity;
        double totalbill;
        string pdf;
        DateTime date_bill;
        string day;
        string month;
        string year;
        private void button1_Click(object sender, EventArgs e)       //ปริ้นใบเสร็จ PDF
        {
            MySqlConnection conn = databaseConnection();
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM carts WHERE username = @username ", conn);
            cmd.Parameters.AddWithValue("@username", user_qr);

            conn.Open();
            MySqlDataReader reader = cmd.ExecuteReader();

            string tick = DateTime.Now.Ticks.ToString(); //ตัวเลขสุ่มสำหรับเลขใบเสร็จและชื่อไฟล์pdf
            string charge_date = DateTime.Now.ToString("dd MMMM yyyy"); //วันที่ปริ้นใบเสร็จ
            string charge_time = DateTime.Now.ToString("HH:mm:ss");

            const string address = "999 Moo 9,\n" + //ที่อยู่ใส่ในใบเสร็จ
            "Mueang Amnatcharoen  District,\n" +
            "Amnatcharoen 37000";

            //ใช้ pdfsharp สร้างpdf โหลดใน nuget
            PdfDocument document = new PdfDocument();
            PdfSharp.Pdf.PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            //ฟ้อนต์ตัวอักษร
            XFont font = new XFont("Arial", 14, XFontStyleEx.Regular);
            XTextFormatter tf = new XTextFormatter(gfx);

            //เนื้อหาในpdf
            gfx.DrawString($"No. : {tick}", font, XBrushes.Black, new XPoint(360, 70)); //เลขที่ใบเสร็จ
            gfx.DrawString($"Charge Date : {charge_date}", font, XBrushes.Black, new XPoint(360, 90)); //วันที่ปริ้นใบเสร็จ
            DrawImage(gfx, "C:\\xampp\\mysql\\data\\EYE\\โลโก้.png", 25, 20, 300, 150);      // logo  //โลโก้เอามาจากไฟล์ในเครื่อง
            gfx.DrawString($"Charge time : {charge_time}", font, XBrushes.Black, new XPoint(360, 110));
            gfx.DrawString($"Customer : {user_qr}", font, XBrushes.Black, new XPoint(360, 130));
            tf.Alignment = XParagraphAlignment.Left;//ทำให้ ที่อยู่ ชิดซ้าย
            tf.DrawString(address, font, XBrushes.Black, new XRect(40, 150, 250, 220), XStringFormats.TopLeft); //ที่อยู่
                                                                                                                              //gfx.DrawString($"Customer Name : {customer_qr}", font, XBrushes.Black, new XPoint(40, 190));                      //ชื่อลูกค้า
                                                                                                                              //gfx.DrawString($"Email Address : {email_qr}", font, XBrushes.Black, new XPoint(40, 210));                         //อีเมลลูกค้า

            //ตาราง 1บรรทัดคือ1เส้น
            gfx.DrawLine(new XPen(XColor.FromName("Black")), new XPoint(40, 250), new XPoint(570, 250));
            gfx.DrawLine(new XPen(XColor.FromName("Black")), new XPoint(40, 250), new XPoint(40, 740));
            gfx.DrawLine(new XPen(XColor.FromName("Black")), new XPoint(40, 740), new XPoint(570, 740));
            gfx.DrawLine(new XPen(XColor.FromName("Black")), new XPoint(570, 250), new XPoint(570, 740));
            gfx.DrawLine(new XPen(XColor.FromName("Black")), new XPoint(40, 280), new XPoint(570, 280));
            gfx.DrawLine(new XPen(XColor.FromName("Black")), new XPoint(40, 600), new XPoint(570, 600));

            //รายละเอียดในตาราง
            int space = 0;

            gfx.DrawString($"Product", font, XBrushes.Black, new XPoint(110, 270));
            gfx.DrawString($"Quantity", font, XBrushes.Black, new XPoint(370, 270));
            gfx.DrawString($"Total", font, XBrushes.Black, new XPoint(500, 270));
            while (reader.Read())
            {
                gfx.DrawString($"{reader.GetString(1)}", font, XBrushes.Black, new XPoint(110, 310 + space));      //ปรับตำแหน่งชื่อสินค้าและยอด ใน PDF
                gfx.DrawString($"{reader.GetString(2)}", font, XBrushes.Black, new XPoint(390, 310 + space));
                gfx.DrawString($"{reader.GetString(4)}", font, XBrushes.Black, new XPoint(510, 310 + space));
                space += 20;
            }
            conn.Close();
            MySqlCommand cmd1 = new MySqlCommand("SELECT SUM(totalprice) FROM carts  WHERE username = @username", conn);
            cmd1.Parameters.AddWithValue("@username", user_qr);

            conn.Open();
            double result = Convert.ToInt32(cmd1.ExecuteScalar());  // ยอดรวมหน้า PDF
            conn.Close();
            double vatt = (result * 7) / 100;
            double discount = ((vatt + result) * 5) / 100;
            double totall = (result + vatt) - discount;
            gfx.DrawString($"Sub Total", font, XBrushes.Black, new XPoint(70, 620));
            gfx.DrawString($"{result.ToString("F2")}", font, XBrushes.Black, new XPoint(500, 620));

            gfx.DrawString($"Vat 7%", font, XBrushes.Black, new XPoint(70, 650));
            gfx.DrawString($"{vatt.ToString("F2")}", font, XBrushes.Black, new XPoint(500, 650));

            gfx.DrawString($"discount 5%", font, XBrushes.Black, new XPoint(70, 680));
            gfx.DrawString($"{discount.ToString("F2")}", font, XBrushes.Black, new XPoint(500, 680));

            gfx.DrawString($"Total", font, XBrushes.Black, new XPoint(70, 710));
            gfx.DrawString($"{totall.ToString("F2")}", font, XBrushes.Black, new XPoint(500, 710));
            

            //เซฟไฟล์ลงเครื่อง
            string filename = @"d:\bill\" + tick + ".pdf";
            document.Save(filename);

            ////เก็บบิลเข้าดาต้าเบส

            MySqlConnection con2 = databaseConnection();
            MySqlCommand cmd2 = new MySqlCommand("SELECT * FROM carts", con2);
            MySqlConnection con3 = databaseConnection1();
            MySqlCommand cmd3 = new MySqlCommand("SELECT email FROM register WHERE username = @username", con3);
            cmd3.Parameters.AddWithValue("@username", user_qr);
            con2.Open();
            MySqlDataReader reader2 = cmd2.ExecuteReader();
            con3.Open();
            MySqlDataReader reader3 = cmd3.ExecuteReader();
            string email;
            while (reader3.Read()) ;

            {
                email = reader3.GetString(0);   

            }
            con3.Close();


            while (reader2.Read())
            {

                productName = reader2.GetString(1);  //อ่านค่าจากคอลัมน์ที่สองในผลลัพธ์ของ query ซึ่งมีลำดับเป็น 1
                quantity = reader2.GetInt32(2);
                totalbill = reader2.GetInt32(4);
                pdf = filename;
                date_bill = DateTime.Now;
                day = DateTime.Now.ToString("dd");
                month = DateTime.Now.ToString("MM");
                year = DateTime.Now.ToString("yyyy");


                MySqlConnection con = databaseConnection();
                String sql = "INSERT INTO history (customer , Product_name , quantity , Product_total , pdf , date,billnum,day,month,year,email) VALUES(@customer, @Product_name, @quantity, @Product_total,@pdf,@date,@billnum,@day,@month,@year,@email)";
                MySqlCommand cmd4 = new MySqlCommand(sql, con);

                cmd4.Parameters.AddWithValue("@customer", user_qr);
                cmd4.Parameters.AddWithValue("@Product_name", productName);
                cmd4.Parameters.AddWithValue("@quantity", quantity);
                cmd4.Parameters.AddWithValue("@Product_total", totalbill);
                cmd4.Parameters.AddWithValue("@pdf", pdf);
                cmd4.Parameters.AddWithValue("@date", date_bill);  
                cmd4.Parameters.AddWithValue("@billnum", tick);
                cmd4.Parameters.AddWithValue("@day", day);
                cmd4.Parameters.AddWithValue("@month", month);
                cmd4.Parameters.AddWithValue("@year", year);
                cmd4.Parameters.AddWithValue("@email", email);     //




                try  // รันเพื่อเอาข้อมูลเข้าดาต้าเบส
                {
                    con.Open();
                    int kkkkk = cmd4.ExecuteNonQuery(); // เอาข้อมูลเข้าดาต้าเบส
     
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }

            }

            MessageBox.Show("ชำระเงินสำเร็จ");

            con2.Close();
                                                              
            MySqlConnection conndel = databaseConnection();    //ลบข้อมูลในตะกร้า
            String sqldel = "DELETE FROM carts WHERE username = @user"; // Corrected SQL statement
            MySqlCommand cmddel = new MySqlCommand(sqldel, conndel);
            cmddel.Parameters.AddWithValue("@user", user_qr);

            // ลบข้อมูลในตระกร้า
            conndel.Open();
            cmddel.ExecuteNonQuery();
            conndel.Close();

            //MessageBox.Show("1"+ user);
            //MessageBox.Show("2" + productName);
            //MessageBox.Show("3" + quantity.ToString());
            //MessageBox.Show("4" + totalbill.ToString());
            //MessageBox.Show("5" + pdf.ToString());
            //MessageBox.Show("6" + date_bill.ToString());


            //conn.Open();
            //MySqlDataReader reader4 = cmd.ExecuteReader();
            //while (reader4.Read())
            //{
            //    MySqlConnection con = databaseConnection();
            //    String sql = "INSERT INTO history (customer , Product_name , quantity , Product_total , pdf , date) VALUES(@customer, @Product_name, @quantity, @Product_total,@pdf,@date)";
            //    MySqlCommand cmd3 = new MySqlCommand(sql, con);

            //    cmd3.Parameters.AddWithValue("@customer", user);
            //    cmd3.Parameters.AddWithValue("@Product_name", productName);
            //    cmd3.Parameters.AddWithValue("@quantity", quantity);
            //    cmd3.Parameters.AddWithValue("@Product_total", totalbill);
            //    cmd3.Parameters.AddWithValue("@pdf", pdf);
            //    cmd3.Parameters.AddWithValue("@date", date_bill);

            //    try
            //    {
            //        con.Open();
            //        int kkkkk = cmd3.ExecuteNonQuery();
            //        if (kkkkk > 0)
            //        {
            //            MessageBox.Show("เพิ่มข้อมูลสำเร็จ");

            //        }
            //    }
            //    catch (MySqlException ex)
            //    {
            //        MessageBox.Show("Error: " + ex.Message);
            //    }
            //    finally
            //    {
            //        con.Close();
            //    }
            //}
            //เปิดไฟล์pdf
            Process.Start(filename);

        }



    
            void DrawImage(XGraphics gfx, string jpegSamplePath, int x, int y, int width, int height)  //เลือกรูปภาพโลโก้
            {
                XImage image = XImage.FromFile(jpegSamplePath);
                gfx.DrawImage(image, x, y, width, height);
            }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            category category = new category();
            category.Show();
        }
    }
}