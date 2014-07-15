using System;
using System.IO;

namespace Isla.Components
{
	public class FileHelper : IFileHelper
	{
		#region IFileHelper implementation

		public string[] ReadAllLines (string path)
		{
			return File.ReadAllLines (path);
		}

		public string ReadAllText (string path)
		{
			return File.ReadAllText (path);
		}

		public void WriteAllLines (string path, string[] content)
		{
			File.WriteAllLines (path, content);
		}

		#endregion
	}
}

