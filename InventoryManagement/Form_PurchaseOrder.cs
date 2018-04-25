using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace InventoryManagement
{
    public class Form_PurchaseOrder : Form
    {
        private string sqlConn;

        private SqlDataAdapter sda;

        private DataTable dt;

        private Guid PurchaseOrderId = Guid.Empty;

        private Guid StoreId = Guid.Empty;

        private Guid CreateUserId = Guid.Empty;

        private int sequence;

        private ContextMenuStrip cms;

        private IContainer components;

        private ToolStrip toolStrip1;

        private ToolStripButton toolStripButton1;

        private ToolStripLabel toolStripLabel1;

        private ToolStripTextBox toolStripTextBox1;

        private ToolStripButton toolStripButton2;

        private ToolStripButton toolStripButton3;

        private Panel panel1;

        private Label label2;

        private Label label1;

        private GroupBox groupBox1;

        private DataGridView dataGridView1;

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

        public Form_PurchaseOrder()
        {
            this.InitializeComponent();
            this.cms = new ContextMenuStrip();
            this.cms.Items.Add("删除该条记录", null, delegate (object sender, EventArgs ex)
            {
                if (this.dataGridView1.CurrentRow == null)
                {
                    return;
                }
                if (MessageBox.Show("真的要删掉当前您选中的这个品种吗？", "提示", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    return;
                }
                DataRowView row = this.dataGridView1.CurrentRow.DataBoundItem as DataRowView;
                try
                {
                    string sql = "delete from purchaseorderdetail where id='" + row["purchaseorderdetailId"] + "'";
                    this.OpenCon();
                    this.command.CommandText = sql;
                    this.command.ExecuteNonQuery();
                    MessageBox.Show("删除成功！");
                    this.DisposeCOn();
                    this.toolStripButton2_Click(null, null);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                    return;
                }
                row.Row.Delete();
            });
            this.dataGridView1.CellMouseClick += delegate (object s, DataGridViewCellMouseEventArgs e)
            {
                if (e.Button != MouseButtons.Right)
                {
                    return;
                }
                this.cms.Show(Control.MousePosition.X, Control.MousePosition.Y);
            };
        }

        private void OpenCon()
        {
            this.con = new SqlConnection(this.sqlConn);
            this.con.Open();
            this.command = new SqlCommand();
            this.command.Connection = this.con;
        }

        private void DisposeCOn()
        {
            if (this.con != null)
            {
                this.con.Close();
                this.con.Dispose();
                this.con = null;
            }
            if (this.command != null)
            {
                this.command.Dispose();
                this.command = null;
            }
        }

        public Form_PurchaseOrder(string sqlConn) : this()
        {
            this.sqlConn = sqlConn;
            base.FormClosing += delegate (object s, FormClosingEventArgs e)
            {
                this.DisposeCOn();
            };
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.AutoGenerateColumns = true;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.CellEnter += delegate (object s, DataGridViewCellEventArgs e)
            {
                this.dataGridView1.ReadOnly = (!(this.dataGridView1.Columns[e.ColumnIndex].Name == this.dataGridView1.Columns["订购数量"].Name) && !(this.dataGridView1.Columns[e.ColumnIndex].Name == this.dataGridView1.Columns["采购价格"].Name) && !(this.dataGridView1.Columns[e.ColumnIndex].Name == this.dataGridView1.Columns["税率"].Name));
            };
        }

        private void HideColumns()
        {
            this.dataGridView1.Columns["Id"].Visible = false;
            this.dataGridView1.Columns["purchaseorderdetailId"].Visible = false;
            this.dataGridView1.Columns["drugInfoId"].Visible = false;
            this.dataGridView1.Columns["采购单号"].Visible = false;
            this.dataGridView1.Columns["供货单位"].Visible = false;
            this.dataGridView1.Columns["CreateUserId"].Visible = false;
            this.dataGridView1.Columns["sequence"].Visible = false;
            this.dataGridView1.Columns["StoreId"].Visible = false;
            this.dataGridView1.Columns["订购数量"].DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.Yellow,
                NullValue = 0
            };
            this.dataGridView1.Columns["采购价格"].DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.Yellow,
                NullValue = 0
            };
            this.dataGridView1.Columns["税率"].DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.Yellow,
                NullValue = 0
            };
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.toolStripTextBox1.Text.Trim()))
            {
                return;
            }
            string docNumber = this.toolStripTextBox1.Text.Trim();
            string sql = "select purchaseorder.Id,\r\npurchaseorder.DocumentNumber as 采购单号,\r\nSupplyUnit.Name as 供货单位,\r\ndruginfo.ProductGeneralName as 品名,\r\nDrugInfo.DictionaryDosageCode as 剂型,\r\ndruginfo.DictionarySpecificationCode as 规格,\r\ndruginfo.LicensePermissionNumber as 批准文号,\r\nDrugInfo.DictionaryMeasurementUnitCode as 计量单位,\r\nDrugInfo.FactoryName as 生产企业,DrugInfo.Origin as 产地,\r\nPurchaseOrderDetail.DrugInfoId as druginfoid,\r\nPurchaseOrderDetail.Amount as 订购数量,\r\nPurchaseOrderDetail.PurchasePrice as 采购价格,\r\nPurchaseOrderDetail.AmountOfTax as 税率,\r\nPurchaseOrderDetail.Id as purchaseorderdetailId,\r\npurchaseorderdetail.CreateUserId,\r\nsequence,\r\npurchaseorderdetail.StoreId\r\n from PurchaseOrder\r\njoin SupplyUnit on purchaseorder.SupplyUnitId =SupplyUnit.Id\r\njoin PurchaseOrderDetail on purchaseorder.Id =PurchaseOrderDetail.PurchaseOrderId\r\njoin druginfo on PurchaseOrderDetail.DrugInfoId =druginfo.Id\r\nwhere purchaseorder.DocumentNumber='" + docNumber + "' and purchaseorderdetail.deleted=0 order by purchaseorderdetail.sequence";
            this.OpenCon();
            this.command.CommandText = sql;
            this.sda = new SqlDataAdapter(this.command);
            this.dt = new DataTable();
            this.sda.Fill(this.dt);
            if (this.dt.Rows.Count <= 0)
            {
                MessageBox.Show("没查到，或已被删除！");
                return;
            }
            this.PurchaseOrderId = Guid.Parse(this.dt.Rows[0]["Id"].ToString());
            this.StoreId = Guid.Parse(this.dt.Rows[0]["StoreId"].ToString());
            this.CreateUserId = Guid.Parse(this.dt.Rows[0]["CreateUserId"].ToString());
            this.sequence = Convert.ToInt32(this.dt.Rows[this.dt.Rows.Count - 1]["sequence"]);
            this.dt.PrimaryKey = new DataColumn[]
            {
                this.dt.Columns["druginfoid"]
            };
            DataRow row = this.dt.Rows[0];
            this.label1.Text = "采购单号：" + row["采购单号"].ToString();
            this.label2.Text = "供货单位：" + row["供货单位"].ToString();
            this.dataGridView1.DataSource = this.dt;
            this.HideColumns();
            this.DisposeCOn();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Form_DrugInfoSelector frm = new Form_DrugInfoSelector(this.sqlConn);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show(this);
            frm.OnDrugSelected += delegate (DataGridViewRow row)
            {
                DataRow newRow = this.dt.NewRow();
                newRow["DruginfoId"] = Guid.Parse(row.Cells["Id"].Value.ToString());
                newRow["品名"] = row.Cells["品名"].Value.ToString();
                newRow["剂型"] = row.Cells["剂型"].Value.ToString();
                newRow["规格"] = row.Cells["规格"].Value.ToString();
                newRow["计量单位"] = row.Cells["计量单位"].Value.ToString();
                newRow["批准文号"] = row.Cells["批准文号"].Value.ToString();
                newRow["生产企业"] = row.Cells["生产企业"].Value.ToString();
                newRow["产地"] = row.Cells["产地"].Value.ToString();
                newRow["订购数量"] = 0;
                newRow["采购价格"] = 0;
                newRow["税率"] = 17;
                newRow["Id"] = this.dt.Rows[0]["Id"];
                newRow["purchaseorderdetailId"] = this.dt.Rows[0]["purchaseorderdetailId"];
                DataRow trow = this.dt.Rows.Find(newRow.Field<Guid>("druginfoid"));
                if (trow != null)
                {
                    MessageBox.Show("已存在该品种！不得重复添加！");
                    return;
                }
                this.dt.Rows.Add(newRow);
                this.dataGridView1.DataSource = null;
                this.dataGridView1.DataSource = this.dt;
                this.HideColumns();
            };
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            base.Validate();
            if (this.dt.Rows.Count <= 0)
            {
                MessageBox.Show("请至少选择一条品种记录！当前的采购单中，一条药品记录都没有，没法存储了！");
                return;
            }
            List<TranSqlModel> submitList = new List<TranSqlModel>();
            for (int i = 0; i < this.dt.Rows.Count; i++)
            {
                DataRow row = this.dt.Rows[i];
                row["sequence"] = i;
                if ((decimal)row["订购数量"] <= 0m)
                {
                    MessageBox.Show(row["品名"].ToString() + "的订购数量等于或小于0，请修改！");
                    return;
                }
                if ((decimal)row["采购价格"] <= 0m)
                {
                    MessageBox.Show(row["品名"].ToString() + "的采购价格等于或小于0，请修改！");
                    return;
                }
                if (row.RowState == DataRowState.Added)
                {
                    string s = "insert into purchaseorderdetail(Id,CreateUserId,UpdateUserId,CreateTime,UpdateTime,Amount,PurchasePrice,AmountOfTax,\r\nStoreId,DrugInfoId,PurchaseOrderId,sequence,deleted) values";
                    string ext = string.Format("(@Id{0},@CreateUserId{0},@UpdateUserId{0},@CreateTime{0},@UpdateTime{0},@Amount{0},@PurchasePrice{0},@AmountOfTax{0},\r\n@StoreId{0},@DrugInfoId{0},@PurchaseOrderId{0},@sequence{0},@deleted{0})", i);
                    s += ext;
                    List<SqlParameter> ParamList = new List<SqlParameter>();
                    ParamList.Add(new SqlParameter("@Id" + i, Guid.NewGuid()));
                    ParamList.Add(new SqlParameter("@CreateUserId" + i, this.CreateUserId));
                    ParamList.Add(new SqlParameter("@UpdateUserId" + i, this.CreateUserId));
                    ParamList.Add(new SqlParameter("@CreateTime" + i, DateTime.Now));
                    ParamList.Add(new SqlParameter("@UpdateTime" + i, DateTime.Now));
                    ParamList.Add(new SqlParameter("@Amount" + i, row["订购数量"]));
                    ParamList.Add(new SqlParameter("@PurchasePrice" + i, row["采购价格"]));
                    ParamList.Add(new SqlParameter("@AmountOfTax" + i, row["税率"]));
                    ParamList.Add(new SqlParameter("@StoreId" + i, this.StoreId));
                    ParamList.Add(new SqlParameter("@DrugInfoId" + i, row["DrugInfoId"]));
                    ParamList.Add(new SqlParameter("@PurchaseOrderId" + i, this.PurchaseOrderId));
                    ParamList.Add(new SqlParameter("@sequence" + i, i));
                    ParamList.Add(new SqlParameter("@deleted" + i, false));
                    TranSqlModel tm = new TranSqlModel
                    {
                        SqlText = s,
                        Parameters = ParamList
                    };
                    submitList.Add(tm);
                }
                if (row.RowState == DataRowState.Modified)
                {
                    string sql = string.Format("update purchaseorderdetail set purchaseprice=@purchaseprice{0},Amount=@Amount{0},AmountOfTax=@AmountOfTax{0},sequence={0} where Id=@Id{0}", i);
                    List<SqlParameter> ParamList2 = new List<SqlParameter>();
                    ParamList2.Add(new SqlParameter("@Id" + i, row["purchaseorderdetailId"]));
                    ParamList2.Add(new SqlParameter("@purchaseprice" + i, row["采购价格"]));
                    ParamList2.Add(new SqlParameter("@Amount" + i, row["订购数量"]));
                    ParamList2.Add(new SqlParameter("@AmountOfTax" + i, row["税率"]));
                    TranSqlModel tsm = new TranSqlModel
                    {
                        SqlText = sql,
                        Parameters = ParamList2
                    };
                    submitList.Add(tsm);
                }


                string sql1 = "update [PurchaseOrder]  set  TotalMoney=(select SUM( Amount*PurchasePrice) from PurchaseOrderDetail where PurchaseOrderId=@PurchaseOrderId) where id = =@PurchaseOrderId ";
                List<SqlParameter> ParamList1 = new List<SqlParameter>();
                ParamList1.Add(new SqlParameter("@PurchaseOrderId", this.PurchaseOrderId));
                TranSqlModel tsm1 = new TranSqlModel
                {
                    SqlText = sql1,
                    Parameters = ParamList1
                };
                submitList.Add(tsm1);
            }
            if (submitList.Count <= 0)
            {
                MessageBox.Show("您没有给这个采购单新增品种记录！保存是没有意义的！");
                return;
            }
            try
            {
                this.OpenCon();
                this.command.Transaction = this.con.BeginTransaction();
                foreach (TranSqlModel s2 in submitList)
                {
                    this.command.CommandText = s2.SqlText;
                    this.command.Parameters.AddRange(s2.Parameters.ToArray<SqlParameter>());
                    this.command.ExecuteNonQuery();
                }
                this.command.Transaction.Commit();
                MessageBox.Show("提交成功！");
                this.DisposeCOn();
            }
            catch (Exception exception)
            {
                this.command.Transaction.Rollback();
                MessageBox.Show(exception.Message);
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripTextBox1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(815, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(68, 22);
            this.toolStripLabel1.Text = "采购单号：";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(141, 25);
            this.toolStripTextBox1.Text = "CGD20171120001296";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(60, 22);
            this.toolStripButton2.Text = "1-》查找";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(84, 22);
            this.toolStripButton3.Text = "2-》增加品种";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(60, 22);
            this.toolStripButton1.Text = "3-》保存";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(815, 65);
            this.panel1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 38);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "供货单位：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "采购单号：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataGridView1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 90);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(815, 365);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(2, 16);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 30;
            this.dataGridView1.Size = new System.Drawing.Size(811, 347);
            this.dataGridView1.TabIndex = 0;
            // 
            // Form_PurchaseOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 455);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form_PurchaseOrder";
            this.Text = "采购单品种新增";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
