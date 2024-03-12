﻿namespace Domain.Exceptions;

public class IncorrectPasswordException : ApplicationException
{
	public IncorrectPasswordException()
	{
	}

	public IncorrectPasswordException(string message)
		: base(message)
	{
	}

	public IncorrectPasswordException(string message, Exception exception)
		: base(message, exception)
	{

	}
}
