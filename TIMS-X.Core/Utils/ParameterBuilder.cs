using System;
using System.Collections.Generic;
using System.Linq;

namespace TIMS_X.Core.Utils
{
	public class ParameterBuilder
	{
		private readonly List<Tuple<string, string>> _parameterList;

		public ParameterBuilder()
		{
			_parameterList = new List<Tuple<string, string>>();
		}

		public ParameterBuilder Add<T>(string name, T value)
		{
			if (value != null)
			{
				var param = string.Empty;
				if (typeof(T) == typeof(DateTime))
				{
					var dateValue = (DateTime)(object)value;
					param = dateValue.ToString("o");
				}
				else if (typeof(T) == typeof(DateTime?))
				{
					var dateValue = (DateTime?)(object)value;
					param = dateValue.Value.ToString("o");
				}
				else
				{
					param = value.ToString();
				}

				_parameterList.Add(new Tuple<string, string>(name, param));
			}

			return this;
		}

		public ParameterBuilder Add<T>(string name, T[] values)
		{
			if (values != null)
				_parameterList.AddRange(values.Select(value => new Tuple<string, string>(name, value.ToString())));

			return this;
		}

		public ParameterBuilder AddRange<T>(string name, IEnumerable<T> values)
		{
			if (values != null && values.Any())
				foreach (var value in values)
				{
					var param = string.Empty;
					if (typeof(T) == typeof(DateTime))
					{
						var dateValue = (DateTime)(object)value;
						param = dateValue.ToString("o");
					}
					else if (typeof(T) == typeof(DateTime?))
					{
						var dateValue = (DateTime?)(object)value;
						param = dateValue.Value.ToString("o");
					}
					else
					{
						param = value.ToString();
					}

					_parameterList.Add(new Tuple<string, string>(name, param));
				}

			return this;
		}

		public ParameterBuilder Replace<T>(string name, T value)
		{
			if (value != null)
			{
				var param = string.Empty;
				if (typeof(T) == typeof(DateTime))
				{
					var dateValue = (DateTime)(object)value;
					param = dateValue.ToString("o");
				}
				else if (typeof(T) == typeof(DateTime?))
				{
					var dateValue = (DateTime?)(object)value;
					param = dateValue.Value.ToString("o");
				}
				else
				{
					param = value.ToString();
				}

				var listItem = new Tuple<string, string>(name, param);
				var index = _parameterList.FindIndex(x => x.Item1 == name);
				if (index == -1)
					_parameterList.Add(listItem);
				else
					_parameterList[index] = listItem;
			}

			return this;
		}

		public override string ToString()
		{
			return string.Join("&", _parameterList.Select(x => x.Item1 + "=" + x.Item2));
		}
	}
}