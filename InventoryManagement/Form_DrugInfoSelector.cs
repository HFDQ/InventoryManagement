using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace InventoryManagement
{
	public class Form_DrugInfoSelector : Form
	{
		private IContainer components;

		private ToolStrip toolStrip1;

		private ToolStripLabel toolStripLabel1;

		private ToolStripTextBox toolStripTextBox1;

		private ToolStripButton toolStripButton1;

		private DataGridView dataGridView1;

		public event DrugSelectEventHandler OnDrugSelected;

		public SqlConnection con
		{
			get;
			private set;
		}

		public SqlCommand command
		{
			get;
			private set;
		}

		public string sqlConn
		{
			get;
			private set;
		}

		public SqlDataAdapter sda
		{
			get;
			private set;
		}

		public DataTable dt
		{
			get;
			private set;
		}

		public Form_DrugInfoSelector()
		{
			this.InitializeComponent();
			this.toolStripTextBox1.Focus();
			this.dataGridView1.CellDoubleClick += delegate(object s, DataGridViewCellEventArgs e)
			{
				if (this.OnDrugSelected != null)
				{
					DataGridViewRow row = this.dataGridView1.CurrentRow;
					this.OnDrugSelected(row);
				}
			};
			this.toolStripButton1.Click += delegate(object s, EventArgs e)
			{
				string keyword = this.toolStripTextBox1.Text.Trim();
				if (string.IsNullOrEmpty(keyword))
				{
					return;
				}
				string sql = string.Concat(new string[]
				{
					"select Id,ProductGeneralName as 品名\r\n,DictionarySpecificationCode as 规格\r\n,DictionaryDosageCode as 剂型\r\n,DictionaryMeasurementUnitCode as 计量单位\r\n,LicensePermissionNumber as 批准文号\r\n,druginfo.FactoryName as 生产企业\r\n,DrugInfo.Origin as 产地\r\nfrom DrugInfo where IsApproval=1 and( productGeneralName like '%",
					keyword,
					"%'  or  pinyin like '%",
					keyword,
					"%')"
				});
				this.command.CommandText = sql;
				this.sda = new SqlDataAdapter(this.command);
				this.dt = new DataTable();
				this.sda.Fill(this.dt);
				if (this.dt.Rows.Count <= 0)
				{
					MessageBox.Show("没查到，或已被删除！");
					return;
				}
				this.dataGridView1.DataSource = this.dt;
			};
		}

		public Form_DrugInfoSelector(string sqlconn) : this()
		{
			this.sqlConn = sqlconn;
			this.con = new SqlConnection(this.sqlConn);
			this.con.Open();
			this.command = new SqlCommand();
			this.command.Connection = this.con;
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripTextBox1,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(775, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(56, 22);
            this.toolStripLabel1.Text = "关键字：";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(68, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(36, 22);
            this.toolStripButton1.Text = "查询";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 25);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 30;
            this.dataGridView1.Size = new System.Drawing.Size(775, 425);
            this.dataGridView1.TabIndex = 1;
            // 
            // Form_DrugInfoSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 450);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.toolStrip1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form_DrugInfoSelector";
            this.Text = "Form_DrugInfoSelector";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
	}
}
