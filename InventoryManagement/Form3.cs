using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace InventoryManagement
{
	public class Form3 : Form
	{
		private string sqlcon = string.Empty;

		private SqlConnection con;

		private IContainer components;

		private Label label1;

		private TextBox textBox1;

		private Button button1;

		private Label label2;

		public Form3(string dbpath)
		{
			this.InitializeComponent();
			base.FormClosing += new FormClosingEventHandler(this.Form3_FormClosing);
			this.sqlcon = dbpath;
			this.con = new SqlConnection(this.sqlcon);
			this.con.Open();
		}

		private void Form3_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.con.Close();
			this.con.Dispose();
			e.Cancel = false;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("真的要删除吗？", "提示", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
			{
				return;
			}
			if (MessageBox.Show("确定要删除吗？", "提示", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
			{
				return;
			}
			if (MessageBox.Show("再给你一次机会，一定要删除吗？", "提示", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
			{
				return;
			}
			this.label2.Text = "信息：";
			try
			{
				SqlCommand command = new SqlCommand();
				command.Connection = this.con;
				string q = "select * from purchaseorder where documentnumber = '" + this.textBox1.Text.Trim() + "' and deleted=0";
				command.CommandText = q;
				SqlDataAdapter sda = new SqlDataAdapter(command);
				DataTable dt = new DataTable();
				sda.Fill(dt);
				if (dt.Rows.Count <= 0)
				{
					MessageBox.Show("没查到，或已被删除！");
				}
				else
				{
					Guid pid = Guid.Parse(dt.Rows[0]["id"].ToString());
					SqlTransaction trans = this.con.BeginTransaction();
					command.Transaction = trans;
					string sql = " declare @pid uniqueIdentifier \n";
					sql = sql + " set @pid=(select Id from purchaseOrder where documentNumber='" + this.textBox1.Text.Trim() + "')\n";
					sql += " declare @piid uniqueIdentifier\n";
					object obj = sql;
					sql = string.Concat(new object[]
					{
						obj,
						" set @piid=(select Id from PurchaseInInventeryOrder where purchaseorderId='",
						pid,
						"')\n"
					});
					sql += " delete from drugInventoryRecord where PurchaseInInventeryOrderDetailId in (select Id from PurchaseInInventeryOrderDetail where PurchaseInInventeryOrderId=@piid)\n";
					sql += " delete from purchaseOrder where Id=@pid\n";
					command.CommandText = sql;
					command.ExecuteNonQuery();
					trans.Commit();
					MessageBox.Show("删除单据成功！");
				}
			}
			catch (Exception)
			{
				MessageBox.Show("删除单据出错！");
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.label1 = new Label();
			this.textBox1 = new TextBox();
			this.button1 = new Button();
			this.label2 = new Label();
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Location = new Point(1, 10);
			this.label1.Name = "label1";
			this.label1.Size = new Size(137, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "单号（完整采购单号）：";
			this.textBox1.Location = new Point(144, 6);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new Size(163, 21);
			this.textBox1.TabIndex = 1;
			this.button1.Location = new Point(211, 304);
			this.button1.Name = "button1";
			this.button1.Size = new Size(75, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "删除";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new EventHandler(this.button1_Click);
			this.label2.AutoSize = true;
			this.label2.Location = new Point(29, 39);
			this.label2.Name = "label2";
			this.label2.Size = new Size(41, 12);
			this.label2.TabIndex = 3;
			this.label2.Text = "信息：";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(319, 333);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.textBox1);
			base.Controls.Add(this.label1);
			base.Name = "Form3";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Form3";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
