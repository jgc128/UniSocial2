﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worker.Repository
{
	interface IRepository : IDisposable
	{
		IEnumerable<string> GetInputData();

		void WriteResult(object Object);
	}
}
