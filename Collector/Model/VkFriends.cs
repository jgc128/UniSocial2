﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collector.Model
{
	public class VkFriends
	{
		public long UserId { get; set; }

		public List<long> Friends { get; set; }
	}
}
