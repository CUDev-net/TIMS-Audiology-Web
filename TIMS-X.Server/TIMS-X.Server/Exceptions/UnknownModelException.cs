using System;

namespace TIMS_X.Server.Exceptions;

public class UnknownModelException : Exception
{
	public UnknownModelException(Type type)
		: base(string.Format("Model Configurator does not have a configuration for type '{0}'", nameof(type)))
	{
	}
}