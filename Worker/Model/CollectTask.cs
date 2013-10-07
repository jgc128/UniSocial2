﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;


namespace Worker.Model
{
	[Serializable()]
	[DataContract]
	[KnownType(typeof(CollectTaskIOFile))]
	public class CollectTask
	{
		protected string _method;
		protected string _socialNetworks;

		[DataMember]
		public string SocialNetwork { get { return _socialNetworks; } protected set { _socialNetworks = value; } }
		[DataMember]
		public string Method { get { return _method; } protected set { _method = value; } }

		[DataMember]
		public bool IsCompleted { get; set; }
		[DataMember]
		public string ErrorMessage { get; set; }

		[DataMember]
		public CollectTaskIO Input { get; set; }
		[DataMember]
		public CollectTaskIO Output { get; set; }

		public CollectTask(string Method)
		{
			_method = Method;

			IsCompleted = false;
		}
		public CollectTask(string SocialNetwork, string Method)
		{
			_socialNetworks = SocialNetwork;
			_method = Method;

			IsCompleted = false;
		}

		public override string ToString()
		{
			return _socialNetworks + ":" + _method;
		}
	}
}
