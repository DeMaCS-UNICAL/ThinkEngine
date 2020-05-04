using System;

namespace EmbASP4Unity.it.unical.mat.embasp.languages.asp
{
	public class PredicateNotValidException : Exception
	{
    public PredicateNotValidException() : base("Value of predicate is not valid") { }
	}
}