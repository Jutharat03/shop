namespace shop
{
    partial class item
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(item));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.name = new System.Windows.Forms.Label();
            this.price = new System.Windows.Forms.Label();
            this.stock = new System.Windows.Forms.Label();
            this.buy = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.qty_product = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(192)))), ((int)(((byte)(216)))));
            this.pictureBox1.Location = new System.Drawing.Point(136, 34);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(173, 144);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // name
            // 
            this.name.AutoSize = true;
            this.name.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(156)))), ((int)(((byte)(194)))));
            this.name.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.name.Location = new System.Drawing.Point(370, 49);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(140, 46);
            this.name.TabIndex = 1;
            this.name.Text = "NAME";
            this.name.MouseEnter += new System.EventHandler(this.item_MouseEnter);
            this.name.MouseLeave += new System.EventHandler(this.item_MouseLeave);
            // 
            // price
            // 
            this.price.AutoSize = true;
            this.price.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(156)))), ((int)(((byte)(194)))));
            this.price.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.price.Location = new System.Drawing.Point(394, 104);
            this.price.Name = "price";
            this.price.Size = new System.Drawing.Size(92, 32);
            this.price.TabIndex = 3;
            this.price.Text = "label1";
            this.price.MouseEnter += new System.EventHandler(this.item_MouseEnter);
            this.price.MouseLeave += new System.EventHandler(this.item_MouseLeave);
            // 
            // stock
            // 
            this.stock.AutoSize = true;
            this.stock.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(156)))), ((int)(((byte)(194)))));
            this.stock.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.stock.Location = new System.Drawing.Point(586, 152);
            this.stock.Name = "stock";
            this.stock.Size = new System.Drawing.Size(53, 20);
            this.stock.TabIndex = 4;
            this.stock.Text = "label1";
            this.stock.Click += new System.EventHandler(this.stock_Click);
            this.stock.MouseEnter += new System.EventHandler(this.item_MouseEnter);
            this.stock.MouseLeave += new System.EventHandler(this.item_MouseLeave);
            // 
            // buy
            // 
            this.buy.BackColor = System.Drawing.Color.Transparent;
            this.buy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.buy.Location = new System.Drawing.Point(378, 143);
            this.buy.Name = "buy";
            this.buy.Size = new System.Drawing.Size(127, 39);
            this.buy.TabIndex = 5;
            this.buy.Text = "BUY NOW";
            this.buy.UseVisualStyleBackColor = false;
            this.buy.Click += new System.EventHandler(this.buy_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(759, 205);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 6;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click_1);
            this.pictureBox2.MouseEnter += new System.EventHandler(this.item_MouseEnter);
            this.pictureBox2.MouseLeave += new System.EventHandler(this.item_MouseLeave);
            // 
            // qty_product
            // 
            this.qty_product.Location = new System.Drawing.Point(512, 152);
            this.qty_product.Name = "qty_product";
            this.qty_product.Size = new System.Drawing.Size(68, 22);
            this.qty_product.TabIndex = 7;
            // 
            // item
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DeepPink;
            this.Controls.Add(this.qty_product);
            this.Controls.Add(this.buy);
            this.Controls.Add(this.stock);
            this.Controls.Add(this.price);
            this.Controls.Add(this.name);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pictureBox2);
            this.Name = "item";
            this.Size = new System.Drawing.Size(759, 205);
            this.Load += new System.EventHandler(this.item_Load);
            this.MouseEnter += new System.EventHandler(this.item_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.item_MouseLeave);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label name;
        private System.Windows.Forms.Label price;
        private System.Windows.Forms.Label stock;
        private System.Windows.Forms.Button buy;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TextBox qty;
        private System.Windows.Forms.TextBox qty_product;
    }
}
