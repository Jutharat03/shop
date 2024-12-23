using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace shop
{
    public partial class Edit : Form
    {
        public Edit()
        {
            InitializeComponent();
        }

        private void Editproduct_Click(object sender, EventArgs e)  //ปุ่มไปหน้าแก้ไขข้อมูลสินค้า
        {
            this.Close();
            Admin admin = new Admin();
            admin.Show();
        }

        private void button5_Click(object sender, EventArgs e)  //ปุ่มปิดหน้าจอ
        {
            this.Close();
            Form form1 = new Form1();
            form1.Show();
        }

        private void Editcustomer_Click(object sender, EventArgs e)   //แก้ไขข้อมูลลูกค้า
        {
            this.Close();
            customer customer = new customer();
            customer.Show();

        }

        private void information_Click(object sender, EventArgs e)   // ข้อมูลผู้จัดทำ
        {    
            this.Close();
            information information = new information();
            information.Show();

        }

        private void button1_Click(object sender, EventArgs e)  // ประวัติ
        {
            this.Close();
            history history = new history();
            history.Show();

        }
    }
}
