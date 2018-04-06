using System;

namespace InventoryManagement
{
	public class inventoryInfo
	{
		public Guid druginfoid
		{
			get;
			set;
		}

		public string drugname
		{
			get;
			set;
		}

		public string drugFactory
		{
			get;
			set;
		}

		public string drugSpecific
		{
			get;
			set;
		}

		public string drugDosage
		{
			get;
			set;
		}

		public int validPerid
		{
			get;
			set;
		}

		public string batchNumber
		{
			get;
			set;
		}

		public DateTime outValidDate
		{
			get;
			set;
		}

		public decimal quantity
		{
			get;
			set;
		}

		public decimal price
		{
			get;
			set;
		}

		public Guid inventoryid
		{
			get;
			set;
		}

		public string Origin
		{
			get;
			set;
		}

		public Guid PurchaseInInventoryDetailId
		{
			get;
			set;
		}
	}
}
