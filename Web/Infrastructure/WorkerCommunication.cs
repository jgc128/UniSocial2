﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Threading.Tasks;
using System.Web;
using Web.Models;
using Web.UniSocialService;
using Worker.Model;

namespace Web.Infrastructure
{
	class WorkerCommunication
	{
		string directory;
		//MessageQueue queue;
		//MessageQueue queueResponse;

		UniSocialClient uniSocialClient;

		public WorkerCommunication()
		{
			var appSettings = ConfigurationManager.AppSettings;

			directory = appSettings["fileDirectory"];

			uniSocialClient = new UniSocialClient();

			//string queueName = appSettings["inputQueue"];
			//string inputQueueName = appSettings["outputQueue"]; // relative to Worker

			//queue = new MessageQueue(queueName);
			//queue.Formatter = new BinaryMessageFormatter();
			//queueResponse = new MessageQueue(inputQueueName);
			//queueResponse.Formatter = new BinaryMessageFormatter();
		}

		public CollectTask CreateCollectTask(CollectForm collectForm)
		{
			var inputFilename = moveInputFile(collectForm.InputFile);
			var outpuFilename = Path.Combine(directory, "result-" + collectForm.InputFile.FileName);

			CollectTask ct = new CollectTask(collectForm.Network, collectForm.Method);
			ct.Input = new CollectTaskIOFile(inputFilename);
			ct.Output = new CollectTaskIOFile(outpuFilename);

			return ct;
		}

		public void SendTaskToQueue(CollectTask ct)
		{
			uniSocialClient.StartNewTask(ct);
			//queue.Send(ct);
		}

		string moveInputFile(HttpPostedFileBase inputFile)
		{
			var fileName = inputFile.FileName;
			var fullName = Path.Combine(directory, fileName);

			inputFile.SaveAs(fullName);

			return fullName;
		}


		public async Task<int> GetCollectTaskCount()
		{
			//queue.Send("taskcount");

			//var res = queueResponse.Receive();

			//return (int)res.Body;

			return await uniSocialClient.GetCurrentTaskCountAsync();
		}
	
	}
}