using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace InventoryManagement
{
	public class Form1 : Form
	{
		private string sqlConn = string.Empty;

		private DataSet dsSql = new DataSet();

		private SqlConnection oleConnection;

		private DataTable dt = new DataTable();

		private SqlDataAdapter oa;

		private SqlDataAdapter oaDrugInventory;

		private DataTable dtDrugInventory = new DataTable();

		private DataSet dsDrugInventory = new DataSet();

		private IContainer components;

		private ToolStrip toolStrip1;

		private ToolStripTextBox toolStripTextBox1;

		private ToolStripButton toolStripButton1;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripButton toolStripButton2;

		private ToolStripLabel toolStripLabel1;

		private DataGridView dataGridView1;

		private ToolStripSeparator toolStripSeparator2;

		private ToolStripButton toolStripButton3;

		private ToolStripButton toolStripButton4;

		private ToolStripButton toolStripButton5;

		private ToolStripSeparator toolStripSeparator3;

		private ToolStripLabel toolStripLabel2;

		private ToolStripTextBox toolStripTextBox2;

		private ToolStripButton toolStripButton6;

		private DataGridViewTextBoxColumn Column1;

		private DataGridViewTextBoxColumn Column2;

		private DataGridViewTextBoxColumn Column3;

		private DataGridViewTextBoxColumn Column4;

		private DataGridViewTextBoxColumn Column5;

		private DataGridViewTextBoxColumn Column6;

		private DataGridViewTextBoxColumn Column12;

		private DataGridViewTextBoxColumn Column7;

		private DataGridViewTextBoxColumn Column8;

		private DataGridViewTextBoxColumn Column9;

		private DataGridViewTextBoxColumn Column10;

		private DataGridViewTextBoxColumn Column11;

		public Form1()
		{
			this.InitializeComponent();
		}

		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			base.Validate();
			DataTable table2 = this.dt.GetChanges(DataRowState.Modified);
			if (table2 == null)
			{
				return;
			}
			SqlTransaction trans = this.oleConnection.BeginTransaction();
			SqlCommand command = new SqlCommand();
			command.Connection = this.oleConnection;
			command.Transaction = trans;
			foreach (DataRow dr in table2.Rows)
			{
				command.CommandText = string.Concat(new object[]
				{
					"declare @batchOrigin varchar(Max)\r\ndeclare @currentCansaleNumber decimal(18,4)\r\nset @currentCansaleNumber=",
					dr["CanSaleNum"],
					"\r\ndeclare @isvalid bit\r\nset @isvalid= (select(case when @currentCansaleNumber>0then 1 else 0 end))\r\ndeclare @batch varchar(Max)\r\nset @batch='",
					dr["BatchNumber"],
					"'\r\ndeclare @druginVId uniqueIdentifier\r\nset @druginVId='",
					dr["id1"],
					"'\r\ndeclare @purchasePrice decimal(18,4)\r\nset @purchasePrice=",
					dr["PurchasePricce"],
					"\r\ndeclare @piid uniqueIdentifier\r\nset @piid=(select PurchaseInInventeryOrderDetailId from DrugInventoryRecord where Id=@druginVId)\r\nset @batchOrigin=(select BatchNumber from DrugInventoryRecord where Id=@druginVId)\r\ndeclare @druginfoId uniqueIdentifier\r\nset @druginfoId=(select DrugInfoId from DrugInventoryRecord where Id=@druginVId)\r\ndeclare @CurrentInventoryCount decimal(18,4)\r\nset @CurrentInventoryCount=(select InInventoryCount+(@currentCansaleNumber-CanSaleNum) from DrugInventoryRecord where Id=@druginVId)\r\nselect @CurrentInventoryCount\r\ndeclare @OutValidDate datetime\r\nset @OutValidDate='",
					dr["outValidDate"],
					"'\r\nbegin try\r\n begin tran\r\nupdate DrugInventoryRecord set OutValidDate=@OutValidDate,BatchNumber=@batch,InInventoryCount=InInventoryCount+(@currentCansaleNumber-CanSaleNum),CanSaleNum=@currentCansaleNumber,PurchasePricce=@purchasePrice,Valid=@isvalid where Id=@druginVId\r\n\r\nupdate PurchaseInInventeryOrderDetail set OutValidDate=@OutValidDate,BatchNumber=@batch,ArrivalAmount=@CurrentInventoryCount,PurchasePrice=@purchasePrice\r\nwhere id =@piid\r\n\r\nupdate PurchaseCheckingOrderDetail set OutValidDate=@OutValidDate,BatchNumber=@batch,PurchasePrice=@purchasePrice,QualifiedAmount=@CurrentInventoryCount,ArrivalAmount=@CurrentInventoryCount,ReceivedAmount=@CurrentInventoryCount where PurchaseCheckingOrderId in(\r\nselect Id from PurchaseCheckingOrder\r\nwhere PurchaseOrderId in\r\n(select PurchaseOrderId from PurchaseInInventeryOrder where Id = (select PurchaseInInventeryOrderId from PurchaseInInventeryOrderDetail where Id=@piid))\r\n) and DrugInfoId=@druginfoId and BatchNumber=@batchOrigin\r\ncommit tran\r\nend try\r\nbegin catch\r\nrollback tran\r\nend catch"
				});
				command.ExecuteNonQuery();
			}
			trans.Commit();
			trans.Dispose();
			trans = null;
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			if (this.oleConnection == null)
			{
				MessageBox.Show("请执行连接到数据库!");
				return;
			}
			if (this.toolStripTextBox1.Text.Trim() == string.Empty)
			{
				return;
			}
			this.dataGridView1.DataSource = null;
			this.dt = null;
			this.dsSql.Tables.Clear();
			string sql = string.Concat(new string[]
			{
				"select * from druginfo join DrugInventoryRecord on druginfo.id=DrugInventoryRecord.druginfoid where (pinyin like '%",
				this.toolStripTextBox1.Text.Trim(),
				"%' or productgeneralname like '%",
				this.toolStripTextBox1.Text.Trim(),
				"%')"
			});
			if (!string.IsNullOrEmpty(this.toolStripTextBox2.Text.Trim()))
			{
				sql = sql + " and DrugInventoryRecord.batchnumber like '%" + this.toolStripTextBox2.Text.Trim() + "%'";
			}
			this.oa = new SqlDataAdapter(sql, this.oleConnection);
			this.oa.Fill(this.dsSql);
			new SqlCommandBuilder(this.oa);
			this.dt = this.dsSql.Tables[0];
			this.dataGridView1.DataSource = this.dt;
		}

		private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			MessageBox.Show("数据错误，请修改！");
		}

		private void toolStripButton3_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("确定删除？", "提示", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
			{
				return;
			}
			if (this.dataGridView1.Rows.Count <= 0)
			{
				return;
			}
			EnumerableRowCollection<DataRow> inventorys = this.dtDrugInventory.AsEnumerable();
			DataGridViewRow row = this.dataGridView1.CurrentRow;
			DataRow i = (from r in inventorys
			where r.Field<Guid>("id").Equals(Guid.Parse(row.Cells["Column11"].Value.ToString()))
			select r).FirstOrDefault<DataRow>();
			i.Delete();
			this.oaDrugInventory.Update(this.dtDrugInventory);
			this.dsDrugInventory.AcceptChanges();
			this.toolStripButton1_Click(sender, e);
			MessageBox.Show("更新成功！");
		}

		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
		}

		private void Form1_Load(object sender, EventArgs e)
		{
		}

		private void toolStripButton4_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.sqlConn))
			{
				return;
			}
			new Form3(this.sqlConn)
			{
				StartPosition = FormStartPosition.CenterParent
			}.ShowDialog();
		}

		private void toolStripButton5_Click(object sender, EventArgs e)
		{
			XmlNodeList nodeList = null;
			XmlDocument doc = new XmlDocument();
			try
			{
				string xmlFile = AppDomain.CurrentDomain.BaseDirectory + "XMLFile1.xml";
				doc.Load(xmlFile);
				XmlNode nod = doc.SelectSingleNode("Connection");
				nodeList = nod.ChildNodes;
			}
			catch (Exception)
			{
				MessageBox.Show("配置文件不存在！");
				return;
			}
			string ip = nodeList[0].Attributes["address"].Value.ToString();
			string name = nodeList[0].Attributes["name"].Value.ToString();
			string pw = nodeList[0].Attributes["pw"].Value.ToString();
			string database = nodeList[0].Attributes["database"].Value.ToString();
			this.sqlConn = string.Concat(new string[]
			{
				"Data Source=",
				ip,
				";Initial Catalog=",
				database,
				";User ID=",
				name,
				";Password=",
				pw,
				";Min Pool Size=1"
			});
			try
			{
				this.oleConnection = new SqlConnection(this.sqlConn);
				this.oleConnection.Open();
			}
			catch (Exception)
			{
				MessageBox.Show("请重新配置文件！");
				Form2 frm = new Form2(nodeList, doc);
				frm.ShowDialog();
				return;
			}
			this.dataGridView1.AutoGenerateColumns = false;
			string sql = "select * from DrugInventoryRecord";
			this.oaDrugInventory = new SqlDataAdapter(sql, this.oleConnection);
			this.oaDrugInventory.Fill(this.dsDrugInventory);
			this.dtDrugInventory = this.dsDrugInventory.Tables[0];
			new SqlCommandBuilder(this.oaDrugInventory);
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if ((this.toolStripTextBox1.Focused || this.toolStripTextBox2.Focused) && keyData == Keys.Return)
			{
				this.toolStripButton1_Click(null, null);
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void toolStripButton6_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.sqlConn))
			{
				return;
			}
			new paymentMethodProc(this.sqlConn)
			{
				StartPosition = FormStartPosition.CenterParent
			}.ShowDialog();
		}

		private void toolStripButton7_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.sqlConn))
			{
				return;
			}
			new paymentMethodProc(this.sqlConn)
			{
				StartPosition = FormStartPosition.CenterParent
			}.ShowDialog();
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
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox2 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton5,
            this.toolStripSeparator3,
            this.toolStripLabel1,
            this.toolStripTextBox1,
            this.toolStripLabel2,
            this.toolStripTextBox2,
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.toolStripButton2,
            this.toolStripSeparator2,
            this.toolStripButton3,
            this.toolStripButton4,
            this.toolStripButton6});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(913, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(84, 22);
            this.toolStripButton5.Text = "链接到数据库";
            this.toolStripButton5.Click += new System.EventHandler(this.toolStripButton5_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(68, 22);
            this.toolStripLabel1.Text = "药品拼音码";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 25);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(44, 22);
            this.toolStripLabel2.Text = "批号：";
            // 
            // toolStripTextBox2
            // 
            this.toolStripTextBox2.Name = "toolStripTextBox2";
            this.toolStripTextBox2.Size = new System.Drawing.Size(100, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(36, 22);
            this.toolStripButton1.Text = "查询";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(36, 22);
            this.toolStripButton2.Text = "保存";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(36, 22);
            this.toolStripButton3.Text = "删除";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(72, 22);
            this.toolStripButton4.Text = "采购单处理";
            this.toolStripButton4.Visible = false;
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // toolStripButton6
            // 
            this.toolStripButton6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton6.Name = "toolStripButton6";
            this.toolStripButton6.Size = new System.Drawing.Size(72, 22);
            this.toolStripButton6.Text = "销售单处理";
            this.toolStripButton6.Click += new System.EventHandler(this.toolStripButton6_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6,
            this.Column12,
            this.Column7,
            this.Column8,
            this.Column9,
            this.Column10,
            this.Column11});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 25);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(913, 386);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "Id";
            this.Column1.HeaderText = "品种ID";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 61;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "productGeneralName";
            this.Column2.HeaderText = "药品通用名";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 72;
            // 
            // Column3
            // 
            this.Column3.DataPropertyName = "FactoryName";
            this.Column3.HeaderText = "生产厂家";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 61;
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "DictionarySpecificationCode";
            this.Column4.HeaderText = "规格";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 51;
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "DictionaryDosageCode";
            this.Column5.HeaderText = "剂型";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Width = 51;
            // 
            // Column6
            // 
            this.Column6.DataPropertyName = "ValidPeriod";
            this.Column6.HeaderText = "有效期（月）";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            this.Column6.Width = 72;
            // 
            // Column12
            // 
            this.Column12.DataPropertyName = "Decription";
            this.Column12.HeaderText = "产地";
            this.Column12.Name = "Column12";
            this.Column12.Width = 51;
            // 
            // Column7
            // 
            this.Column7.DataPropertyName = "batchNumber";
            this.Column7.HeaderText = "批号";
            this.Column7.Name = "Column7";
            this.Column7.Width = 51;
            // 
            // Column8
            // 
            this.Column8.DataPropertyName = "outValidDate";
            this.Column8.HeaderText = "过期日";
            this.Column8.Name = "Column8";
            this.Column8.Width = 61;
            // 
            // Column9
            // 
            this.Column9.DataPropertyName = "CanSaleNum";
            this.Column9.HeaderText = "可销数量";
            this.Column9.Name = "Column9";
            this.Column9.Width = 61;
            // 
            // Column10
            // 
            this.Column10.DataPropertyName = "PurchasePricce";
            this.Column10.HeaderText = "采购价";
            this.Column10.Name = "Column10";
            this.Column10.Width = 61;
            // 
            // Column11
            // 
            this.Column11.DataPropertyName = "Id1";
            this.Column11.HeaderText = "库存ID";
            this.Column11.Name = "Column11";
            this.Column11.ReadOnly = true;
            this.Column11.Width = 61;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(913, 411);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
	}
}
