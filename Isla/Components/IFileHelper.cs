using System;
using System.Linq;

namespace Isla.Components
{
	public interface IFileHelper
	{
		string[] ReadAllLines (string path);

		string ReadAllText(string path);

		void WriteAllLines(string path, string[] lines);
	}
}

