using System;

namespace EmbASP4Unity.it.unical.mat.embasp.languages
{
	public class ObjectNotValidException : Exception
	{
		public ObjectNotValidException() : base("Value of the object is not valid") { }
	}
}