using System;
using Test.Isla.Serialisation.Components;
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

		#endregion
	}
}

