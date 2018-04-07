using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace InventoryManagement
{
	public class TranSqlModel
	{
		public string SqlText
		{
			get;
			set;
		}

		public ICollection<SqlParameter> Parameters
		{
			get;
			set;
		}
	}
}
