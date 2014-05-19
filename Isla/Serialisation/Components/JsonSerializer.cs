using System;
using ServiceStack.Text;
using Isla.Logging;

namespace Isla.Serialisation.Components
{
	public class JsonSerializer: IJsonSerializer
	{
		public JsonSerializer ()
		{
		}

		#region IJsonSerializer implementation

		public string Serialize (TimedInvocation invocation)
		{
			using (JsConfig.With (timeSpanHandler: TimeSpanHandler.StandardFormat)) {
				var serialiser = new JsonStringSerializer ();
				var serialisedInvocation = serialiser.SerializeToString (invocation);
				return serialisedInvocation;
			}
		}

		#endregion
	}
}

