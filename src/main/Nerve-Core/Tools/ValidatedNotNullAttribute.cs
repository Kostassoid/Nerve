namespace Kostassoid.Nerve.Core.Tools
{
	using System;

	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	internal sealed class ValidatedNotNullAttribute : Attribute
	{
	}
}