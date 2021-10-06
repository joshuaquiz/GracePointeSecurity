using System;
using System.Collections.Generic;
using System.Linq;

namespace GracePointeSecurity.Library
{
	public sealed class Logging
	{
        private const string TopItem = "...";

		private List<string> _logs = new List<string>();

		private readonly Action<string> _onLogAdded;

		public Logging(Action<string> onLogAdded)
		{
			_onLogAdded = onLogAdded;
		}

		public void AddLog(string str)
		{
			_logs.Add(str);
			if (_logs.Count == 2000)
			{
				_logs = _logs.Skip(1).ToList();
				_logs[0] = TopItem;
			}

			_onLogAdded(string.Join(Environment.NewLine, _logs));
		}
    }
}