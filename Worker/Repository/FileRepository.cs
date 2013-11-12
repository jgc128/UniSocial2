﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Worker.Common;
using Worker.Repository;

namespace Worker.Repository
{
	class FileRepository : IRepository
	{
		ObjectFormatter formatter;
		string filename;

		public FileRepository(string Filename)
		{
			filename = Filename;
			formatter = new ObjectFormatter();
		}

		public void WriteResult(object Obj)
		{
			var stream = formatter.ToCSVStream(Obj);

			if (stream == null)
				return;

			using (var file = File.Open(filename, FileMode.Append, FileAccess.Write, FileShare.None))
			{
				stream.Seek(0, SeekOrigin.Begin);
				stream.CopyTo(file);
				file.Flush();
			}

			stream.Dispose();

			Thread.Sleep(500);
		}

		public IEnumerable<string> GetInputData()
		{
			return File.ReadLines(filename);
		}

		public void Dispose()
		{
			
		}
	}
}
