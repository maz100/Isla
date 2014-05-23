using System;
using System.Linq;

namespace Test.Isla.Serialisation.Components
{
	public interface IFileHelper
	{
		string[] ReadAllLines (string path);
	}
}

