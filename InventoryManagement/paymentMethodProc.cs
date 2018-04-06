using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace InventoryManagement
{
	public class paymentMethodProc : Form
	{
		private string sqlcon = string.Empty;

		private SqlConnection con;

		private Guid SelectedSalesOrderId = Guid.Empty;

		private IContainer components;

		private Label label1;

		private TextBox textBox1;

		private Button button1;

		private Label label2;

		private ComboBox comboBox1;

		private Button button2;

		private Label label3;

		private ComboBox comboBox2;

		private Label label4;

		private ComboBox comboBox3;

		private Label label5;

		private ComboBox comboBox4;

		public paymentMethodProc(string dbpath)
		{
			this.InitializeComponent();
			this.sqlcon = dbpath;
			this.con = new SqlConnection(this.sqlcon);
			this.con.Open();
			SqlCommand command = new SqlCommand();
			command.Connection = this.con;
			string sql = string.Empty;
			Action iniComb = delegate
			{
				sql = "select * from PaymentMethod";
				command.CommandText = sql;
				SqlDataAdapter sda = new SqlDataAdapter(command);
				DataTable dt = new DataTable();
				sda.Fill(dt);
				this.comboBox1.DisplayMember = "Name";
				this.comboBox1.ValueMember = "Id";
				this.comboBox1.DataSource = dt;
			};
			Action iniComb2 = delegate
			{
				sql = "select * from PurchaseUnit order by Name";
				command.CommandText = sql;
				SqlDataAdapter sda = new SqlDataAdapter(command);
				DataTable dt = new DataTable();
				sda.Fill(dt);
				this.comboBox2.DisplayMember = "Name";
				this.comboBox2.ValueMember = "Id";
				this.comboBox2.DataSource = dt;
			};
			Action iniComb3 = delegate
			{
				sql = "select [User].Id,Employee.Name as Name from [User] join Employee on [User].EmployeeId =Employee.Id where [User].deleted=0 order by Employee.Name";
				command.CommandText = sql;
				SqlDataAdapter sda = new SqlDataAdapter(command);
				DataTable dt = new DataTable();
				sda.Fill(dt);
				this.comboBox3.DisplayMember = "Name";
				this.comboBox3.ValueMember = "Id";
				this.comboBox3.DataSource = dt;
			};
			Action iniComb4 = delegate
			{
				sql = "select Employee.Name as Id,Employee.Name as Name from [User] join Employee on [User].EmployeeId =Employee.Id where [User].deleted=0 order by Employee.Name";
				command.CommandText = sql;
				SqlDataAdapter sda = new SqlDataAdapter(command);
				DataTable dt = new DataTable();
				sda.Fill(dt);
				this.comboBox4.DisplayMember = "Name";
				this.comboBox4.ValueMember = "Id";
				this.comboBox4.DataSource = dt;
			};
			iniComb();
			iniComb2();
			iniComb3();
			iniComb4();
			this.button2.Click += delegate(object s, EventArgs e)
			{
				if (this.textBox1.Text.Trim().Length <= 0)
				{
					MessageBox.Show("请输入单号！");
					return;
				}
				sql = "select SalesOrder.Id , Salesorder.SalerName,Salesorder.payMentMethodID,PurchaseUnitId,CreateUserId  from salesorder where ordercode='" + this.textBox1.Text.Trim() + "'";
				command.CommandText = sql;
				SqlDataAdapter sda = new SqlDataAdapter(command);
				DataTable dt = new DataTable();
				sda.Fill(dt);
				if (dt.Rows.Count <= 0)
				{
					MessageBox.Show("没有查到该单据！请检查销售单号！");
					this.SelectedSalesOrderId = Guid.Empty;
					return;
				}
				this.comboBox1.SelectedValue = dt.Rows[0]["payMentMethodID"];
				this.comboBox2.SelectedValue = dt.Rows[0]["PurchaseUnitId"];
				this.comboBox3.SelectedValue = dt.Rows[0]["CreateUserId"];
				this.comboBox4.SelectedValue = dt.Rows[0]["SalerName"];
				this.SelectedSalesOrderId = (Guid)dt.Rows[0]["Id"];
			};
			this.button1.Click += delegate(object s1, EventArgs e2)
			{
				if (this.SelectedSalesOrderId == Guid.Empty)
				{
					MessageBox.Show("请先查询一下！");
					return;
				}
				DialogResult dlgre = MessageBox.Show("您确定需要修改销售单信息吗？", "提示", MessageBoxButtons.OKCancel);
				if (dlgre == DialogResult.OK)
				{
					try
					{
						string SalerName = this.comboBox4.SelectedValue.ToString();
						SqlTransaction trans = this.con.BeginTransaction();
						command.Transaction = trans;
						command.CommandText = string.Concat(new object[]
						{
							"update SalesOrder set SalerName='",
							SalerName,
							"', payMentMethodID='",
							this.comboBox1.SelectedValue,
							"',CreateUserId='",
							this.comboBox3.SelectedValue,
							"',purchaseUnitId='",
							this.comboBox2.SelectedValue,
							"',updateUserId='",
							this.comboBox3.SelectedValue,
							"' where Id='",
							this.SelectedSalesOrderId,
							"'"
						});
						command.ExecuteNonQuery();
						command.CommandText = string.Format("update Delivery set delivery.ReceivingCompasnyID=t.PurchaseUnitId,DeliveryAddress=t2.ReceiveAddress from (select * from SalesOrder where Id='{0}')t, (select * from PurchaseUnit where Id='{1}')t2 where OrderID=t.Id", this.SelectedSalesOrderId, this.comboBox2.SelectedValue);
						command.ExecuteNonQuery();
						trans.Commit();
						trans.Dispose();
						MessageBox.Show("修改成功！");
						this.SelectedSalesOrderId = Guid.Empty;
					}
					catch (Exception)
					{
						MessageBox.Show("修改失败！");
					}
				}
			};
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
			this.comboBox1 = new ComboBox();
			this.button2 = new Button();
			this.label3 = new Label();
			this.comboBox2 = new ComboBox();
			this.label4 = new Label();
			this.comboBox3 = new ComboBox();
			this.label5 = new Label();
			this.comboBox4 = new ComboBox();
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Location = new Point(90, 73);
			this.label1.Name = "label1";
			this.label1.Size = new Size(170, 18);
			this.label1.TabIndex = 0;
			this.label1.Text = "输入完整销售单号：";
			this.textBox1.Location = new Point(257, 63);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new Size(381, 28);
			this.textBox1.TabIndex = 1;
			this.button1.Location = new Point(548, 382);
			this.button1.Name = "button1";
			this.button1.Size = new Size(90, 47);
			this.button1.TabIndex = 2;
			this.button1.Text = "确定";
			this.button1.UseVisualStyleBackColor = true;
			this.label2.AutoSize = true;
			this.label2.Location = new Point(153, 129);
			this.label2.Name = "label2";
			this.label2.Size = new Size(98, 18);
			this.label2.TabIndex = 0;
			this.label2.Text = "结算方式：";
			this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new Point(257, 121);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new Size(380, 26);
			this.comboBox1.TabIndex = 3;
			this.button2.Location = new Point(669, 63);
			this.button2.Name = "button2";
			this.button2.Size = new Size(75, 28);
			this.button2.TabIndex = 4;
			this.button2.Text = "查询";
			this.button2.UseVisualStyleBackColor = true;
			this.label3.AutoSize = true;
			this.label3.Location = new Point(153, 199);
			this.label3.Name = "label3";
			this.label3.Size = new Size(98, 18);
			this.label3.TabIndex = 0;
			this.label3.Text = "购货单位：";
			this.comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBox2.FormattingEnabled = true;
			this.comboBox2.Location = new Point(257, 196);
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new Size(380, 26);
			this.comboBox2.TabIndex = 3;
			this.label4.AutoSize = true;
			this.label4.Location = new Point(162, 275);
			this.label4.Name = "label4";
			this.label4.Size = new Size(80, 18);
			this.label4.TabIndex = 0;
			this.label4.Text = "开票员：";
			this.comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBox3.FormattingEnabled = true;
			this.comboBox3.Location = new Point(258, 267);
			this.comboBox3.Name = "comboBox3";
			this.comboBox3.Size = new Size(380, 26);
			this.comboBox3.TabIndex = 3;
			this.label5.AutoSize = true;
			this.label5.Location = new Point(162, 342);
			this.label5.Name = "label5";
			this.label5.Size = new Size(80, 18);
			this.label5.TabIndex = 0;
			this.label5.Text = "销售员：";
			this.comboBox4.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBox4.FormattingEnabled = true;
			this.comboBox4.Location = new Point(258, 339);
			this.comboBox4.Name = "comboBox4";
			this.comboBox4.Size = new Size(380, 26);
			this.comboBox4.TabIndex = 3;
			base.AutoScaleDimensions = new SizeF(9f, 18f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(816, 504);
			base.Controls.Add(this.button2);
			base.Controls.Add(this.comboBox4);
			base.Controls.Add(this.comboBox3);
			base.Controls.Add(this.comboBox2);
			base.Controls.Add(this.comboBox1);
			base.Controls.Add(this.label5);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.textBox1);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Name = "paymentMethodProc";
			this.Text = "销售单处理";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
