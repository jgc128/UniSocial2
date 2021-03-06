﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collector.Interface
{
	public interface IApi
	{
		Task<JObject> ExecuteRequest(string Method, Dictionary<string, string> Params);
	}
}
