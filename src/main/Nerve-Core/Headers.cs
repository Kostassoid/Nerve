namespace Kostassoid.Nerve.Core
{
	using System.Collections.Generic;

	/// <summary>
	///   Headers dictionary.
	/// </summary>
	public class Headers : Dictionary<string, object>
	{
		#region Constructors and Destructors

		/// <summary>
		///   Creates a new empty Headers object.
		/// </summary>
		public Headers()
		{
		}

		/// <summary>
		///   Creates a new Headers object wih content copied from another dictionary.
		/// </summary>
		/// <param name="templateHeaders">Source dictionary to copy from.</param>
		public Headers(IDictionary<string, object> templateHeaders)
			: base(templateHeaders)
		{
		}

		#endregion

		#region Public Properties

		/// <summary>
		///   Returns a new empty Headers object.
		/// </summary>
		public static Headers Empty
		{
			get
			{
				return new Headers();
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///   Clone current Headers object.
		/// </summary>
		/// <returns></returns>
		public Headers Clone()
		{
			return new Headers(this);
		}

		#endregion
	}
}