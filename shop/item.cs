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
    public partial class item : UserControl

    {
        public item()
        {
            InitializeComponent();
        }
        private string _name_item;
        private int _price_item;
        private int _qty_item;
        private Image _picture_;
        private string _qty;
        private int qty_sel;

        public string name_item //ตัวชื่อ
        {
            get { return  _name_item; }
            set { _name_item = value; name.Text = value; }
        }
        public int price_item
        {
            get { return _price_item; }
            set { _price_item = value; }
        }
        public int qty_item   // 
        {
            get { return _qty_item; }
            set { _qty_item = value; }
        }
        public Image picture_item
        { 
            get { return _picture_; }
            set {  _picture_ = value; pictureBox1.Image = value; } 
        }
        public string qty_select
        {
            get { return qty_product.Text; }       //ต้องแก้ ไม่ให้ช่อง stock ว่าง
        }
        //public int qty_select
        //{
        //    get { return _qty; }
        //    set
        //    {
        //        _qty = value;
        //        // เมื่อต้องการรับค่าจาก TextBox ที่มีชื่อว่า qty_product
        //        // ให้แปลงค่าจาก TextBox เป็น int แล้วกำหนดให้กับ _qty
        //        if (int.TryParse(qty_product.Text, out int qtyFromTextBox))
        //        {
        //            _qty = qtyFromTextBox;
        //        }
        //        else
        //        {
        //            // กรณีที่ไม่สามารถแปลงได้ ให้ใช้ค่าเดิมหรือทำการจัดการตามที่เหมาะสม
        //            MessageBox.Show("Invalid quantity format");
        //        }
        //    }
        //}






        private void item_Load(object sender, EventArgs e)   // จัดวางตำแหน่ง
        {
            string price1 = price_item.ToString();
            price.Text = $"฿{price1}";
            string qty1 = qty_item.ToString();
            stock.Text = $" Stock : {qty1}";
            //int qty = Convert.ToInt32(qty_product.Text);

        }
        public event EventHandler buybutton;
        private void buy_Click(object sender, EventArgs e)
        {
            Console.WriteLine(qty_select);
            if (qty_product.Text != "")
            {
                //qty_sel = Convert.ToInt32(qty_product.Text);     ตรวจสอบค่าว่าง

                if (Convert.ToInt32(qty_product.Text) <= 0)
                {
                    MessageBox.Show("กรุณาเลือกขั้นต่ำตั้งแต่ 1 ชิ้นขึ้นไป");
                }
                else
                {
                    buybutton?.Invoke(this, e);
                    qty_sel = Convert.ToInt32(qty_product.Text);
                             
                }
            }
            else
            {
                MessageBox.Show("กรุณาใส่จำนวนสินค้าที่ต้องการ");
            }

        }
        private void item_MouseEnter(object sender, EventArgs e)
        {
            this.BackColor = Color.HotPink;
        }
        private void item_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = Color.White;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void stock_Click(object sender, EventArgs e)
        {

        }

        private void qty_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {

        }
    }
}
