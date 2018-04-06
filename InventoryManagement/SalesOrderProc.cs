using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace InventoryManagement
{
	public class SalesOrderProc : Form
	{
		private IContainer components;

		public SalesOrderProc()
		{
			this.InitializeComponent();
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
			base.SuspendLayout();
			base.AutoScaleDimensions = new SizeF(9f, 18f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(1117, 790);
			base.Name = "SalesOrderProc";
			this.Text = "SalesOrderProc";
			base.ResumeLayout(false);
		}
	}
}
