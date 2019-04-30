using System;

namespace EmbASP4Unity.it.unical.mat.embasp.languages
{
	public class IllegalAnnotationException : Exception
	{
		public IllegalAnnotationException() : base("bad annotation") { }
	}
}