﻿using Collector.Interface;
using Newtonsoft.Json.Linq;
using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collector.Api
{
	public class ApiRequestParam : ICloneable
	{
		public ApiRequestParam(string Method, Dictionary<string, string> Params)
		{
			this.Method = Method;
			this.Params = Params;
		}

		public ApiRequestParam(Dictionary<string, string> Params)
		{
			this.Method = "";
			this.Params = Params;
		}
		public ApiRequestParam(string Method)
		{
			this.Method = Method;
			this.Params = new Dictionary<string, string>();
		}

		public ApiRequestParam()
		{
			this.Method = "";
			this.Params = new Dictionary<string, string>();
		}

		public string Method;
		public Dictionary<string,string> Params;

		public object Clone()
		{
			var newParams = new ApiRequestParam(
				this.Method,
				new Dictionary<string, string>( this.Params.Count )
			);

			foreach (var item in this.Params)
				newParams.Params.Add(item.Key, item.Value);

			return newParams;
		}
	}

	public abstract class BaseApiRequest : IApiRequest
	{
		protected IApi api;
		protected IDataExtractor dataExtractor;

		protected Dictionary<string, Dictionary<string, string>> requestParams;
		protected Dictionary<string, Type> objectTypeForMethods;
		protected Dictionary<string, int> batchSizes;
		protected Dictionary<string, int> itemsMaxCounts;
		protected Dictionary<string, ApiRequestType> requestTypes;


		//[Inject]
		//public TraceSource Trace { get; set; }

		public BaseApiRequest(IApi Api, IDataExtractor DataExtractor)
		{
			api = Api;
			dataExtractor = DataExtractor;

			requestParams = new Dictionary<string, Dictionary<string, string>>();
			objectTypeForMethods = new Dictionary<string, Type>();
			batchSizes = new Dictionary<string, int>();
			itemsMaxCounts = new Dictionary<string, int>();
			requestTypes = new Dictionary<string, ApiRequestType>();
		}

		public int GetRequestBatchSize(string Method)
		{
			if (!batchSizes.ContainsKey(Method))
				return 1;
			else
				return batchSizes[Method];
		}

		public int GetRequestItemsMaxCount(string Method)
		{
			if (!itemsMaxCounts.ContainsKey(Method))
				return 1;
			else
				return itemsMaxCounts[Method];
		}


		public ApiRequestType GetRequestType(string Method)
		{
			if (!requestTypes.ContainsKey(Method))
				return ApiRequestType.ObjectInfo;
			else
				return requestTypes[Method];
		}



		protected Type getObjectTypeForMethod(string Method)
		{
			if (!objectTypeForMethods.ContainsKey(Method))
				throw new NotSupportedException("Method " + Method + " not supported!");

			return objectTypeForMethods[Method];
		}

		protected Dictionary<string,string> getRequestParams(string method)
		{
			if (!requestParams.ContainsKey(method))
				return new Dictionary<string, string>();

			var reqOrigParam = requestParams[method];

			var result = new Dictionary<string, string>(reqOrigParam);

			return result;
		}

		protected abstract void setListParams(ApiRequestParam requestParam, int Offset, int Count);
		protected abstract void setIdParams(ApiRequestParam requestParam, string id);
		protected abstract void setIdParams(ApiRequestParam requestParam, List<string> ids);

		protected abstract object modifyResult(object Data, ApiRequestParam requestParam);

		protected ApiRequestParam createRequestParam(string Method)
		{
			var parameters = getRequestParams(Method);

			var req = new ApiRequestParam(Method, parameters);

			return req;
		}

		protected async Task<object> executeRequest(ApiRequestParam param)
		{
			var needObjectType = getObjectTypeForMethod(param.Method);

			var rawData = await api.ExecuteRequest(param.Method, param.Params);

			object result;
			var requestType = this.GetRequestType(param.Method);
			if (requestType == ApiRequestType.ObjectInfo)
			{
				result = dataExtractor.GetItem(rawData, needObjectType);
			}
			else
			{
				result = dataExtractor.GetItems(rawData, needObjectType);
			}

			return result;
		}

		public async Task<object> ExecuteRequest(string Method, string Id)
		{
			var param = createRequestParam(Method);

			setIdParams(param, Id);

			var result = await executeRequest(param);
			return modifyResult(result, param);
		}
		public async Task<object> ExecuteRequest(string Method, List<string> Ids)
		{
			var param = createRequestParam(Method);

			setIdParams(param, Ids);

			var result = await executeRequest(param);
			return modifyResult(result, param);
		}
		public async Task<object> ExecuteRequest(string Method, string Id, int Offset, int Count)
		{
			var param = createRequestParam(Method);

			List<object> resultList = new List<object>();
			List<object> result;

			var maxLimit = GetRequestItemsMaxCount(Method);
			var currentOffset = Offset;
			var currentCount = 0;

			while(true)
			{
				setIdParams(param, Id);
				setListParams(param, currentOffset, maxLimit);

				result = await executeRequest(param) as List<object>;

				if (result != null)
				{
					resultList.AddRange(result);

					currentCount = result.Count;

					if (currentCount < maxLimit || result.Count >= Count)
					{
						break;
					}

					currentOffset += currentCount;
				}
				else
				{
					break;
				}
			}

			return modifyResult(resultList, param);
		}
	}
}
