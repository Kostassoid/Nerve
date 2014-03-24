namespace Kostassoid.Nerve.Core.Tools
{
	using System;
	using System.Linq;

	internal static class TypeEx
	{
		public static string BuildDescription(this Type type)
		{
			var name = type.Name;

			if (type.IsGenericType)
			{
				name = name.Remove(name.IndexOf('`'));
			}

			if (name.EndsWith("Operator"))
			{
				name = name.Substring(0, name.Length - "Operator".Length);
			}

			if (type.IsGenericType)
			{
				name += "[" + string.Join(",", type.GetGenericArguments().Select(BuildDescription).ToArray()) + "]";
				return name;
			}

			return name;
		}
		 
	}
}