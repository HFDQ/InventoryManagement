using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace InventoryManagement
{
	public class Form2 : Form
	{
		private XmlNodeList _nodeList;

		private XmlDocument _doc;

		private IContainer components;

		private TextBox textBox1;

		private Label label1;

		private TextBox textBox2;

		private Label label2;

		private Button button1;

		private TextBox textBox3;

		private Label label3;

		private TextBox textBox4;

		private Label label4;

		public Form2(XmlNodeList nodeList, XmlDocument doc)
		{
			this.InitializeComponent();
			this._nodeList = nodeList;
			this._doc = doc;
			this.textBox1.Text = this._nodeList[0].Attributes["address"].Value.ToString();
			this.textBox2.Text = this._nodeList[0].Attributes["database"].Value.ToString();
			this.textBox3.Text = this._nodeList[0].Attributes["name"].Value.ToString();
			this.textBox4.Text = this._nodeList[0].Attributes["pw"].Value.ToString();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this._nodeList[0].Attributes["address"].Value = this.textBox1.Text.Trim();
			this._nodeList[0].Attributes["database"].Value = this.textBox2.Text.Trim();
			this._nodeList[0].Attributes["name"].Value = this.textBox3.Text.Trim();
			this._nodeList[0].Attributes["pw"].Value = this.textBox4.Text.Trim();
			this._doc.Save(AppDomain.CurrentDomain.BaseDirectory + "XMLFile1.xml");
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
			this.textBox1 = new TextBox();
			this.label1 = new Label();
			this.textBox2 = new TextBox();
			this.label2 = new Label();
			this.button1 = new Button();
			this.textBox3 = new TextBox();
			this.label3 = new Label();
			this.textBox4 = new TextBox();
			this.label4 = new Label();
			base.SuspendLayout();
			this.textBox1.Location = new Point(89, 10);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new Size(236, 21);
			this.textBox1.TabIndex = 0;
			this.label1.AutoSize = true;
			this.label1.Location = new Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new Size(77, 12);
			this.label1.TabIndex = 1;
			this.label1.Text = "数据库地址：";
			this.textBox2.Location = new Point(89, 37);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new Size(236, 21);
			this.textBox2.TabIndex = 0;
			this.label2.AutoSize = true;
			this.label2.Location = new Point(13, 40);
			this.label2.Name = "label2";
			this.label2.Size = new Size(77, 12);
			this.label2.TabIndex = 1;
			this.label2.Text = "数据库名称：";
			this.button1.Location = new Point(331, 10);
			this.button1.Name = "button1";
			this.button1.Size = new Size(75, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "保存";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new EventHandler(this.button1_Click);
			this.textBox3.Location = new Point(89, 64);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new Size(92, 21);
			this.textBox3.TabIndex = 0;
			this.label3.AutoSize = true;
			this.label3.Location = new Point(25, 67);
			this.label3.Name = "label3";
			this.label3.Size = new Size(65, 12);
			this.label3.TabIndex = 1;
			this.label3.Text = "登陆账号：";
			this.textBox4.Location = new Point(230, 64);
			this.textBox4.Name = "textBox4";
			this.textBox4.Size = new Size(95, 21);
			this.textBox4.TabIndex = 0;
			this.label4.AutoSize = true;
			this.label4.Location = new Point(190, 67);
			this.label4.Name = "label4";
			this.label4.Size = new Size(41, 12);
			this.label4.TabIndex = 1;
			this.label4.Text = "密码：";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(417, 91);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.textBox4);
			base.Controls.Add(this.textBox3);
			base.Controls.Add(this.textBox2);
			base.Controls.Add(this.textBox1);
			base.Name = "Form2";
			this.Text = "Form2";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
