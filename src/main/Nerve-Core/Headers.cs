namespace Kostassoid.Nerve.Core
{
	using System.Collections.Generic;
	using System.Linq;

	using Tools.Collections;

	/// <summary>
	///   Headers dictionary.
	/// </summary>
	public class Headers
	{
		// ReSharper disable InconsistentNaming
		private static readonly Headers _empty = new Headers();
		// ReSharper restore InconsistentNaming

		readonly IImmutableLinkedList<KeyValuePair<string, object>> _items
			= ImmutableLinkedList<KeyValuePair<string, object>>.Empty;

		/// <summary>
		///   Creates a new empty Headers object.
		/// </summary>
		private Headers()
		{
		}

		/// <summary>
		///   Creates a new Headers object wih content copied from another dictionary.
		/// </summary>
		/// <param name="headers">Source dictionary to copy from.</param>
		private Headers(IEnumerable<KeyValuePair<string, object>> headers)
		{
			_items = ImmutableLinkedList<KeyValuePair<string, object>>.FromEnumerable(headers);
		}

		/// <summary>
		/// Returns an empty Headers object.
		/// </summary>
		public static Headers Empty
		{
			get
			{
				return _empty;
			}
		}

		/// <summary>
		/// Returns items collection.
		/// </summary>
		public IEnumerable<KeyValuePair<string, object>> Items
		{
			get
			{
				return _items;
			}
		}

		/// <summary>
		/// Returns a new Headers object with added data.
		/// </summary>
		/// <param name="key">Key</param>
		/// <param name="value">Value</param>
		/// <returns></returns>
		public Headers With(string key, object value)
		{
			var newItems = _items
				.Where(i => i.Key != key)
				.Union(new[] { new KeyValuePair<string, object>(key, value) });

			return new Headers(newItems);
		}

		/// <summary>
		/// Removes item using key.
		/// </summary>
		/// <param name="key">Key</param>
		/// <returns>Headers without item.</returns>
		public Headers Without(string key)
		{
			var newItems = _items.Where(i => i.Key != key);

			return new Headers(newItems);
		}

		/// <summary>
		/// Returns headers value using key, or null if not present.
		/// </summary>
		/// <param name="key">Headers key</param>
		/// <returns></returns>
		public object this[string key]
		{
			get
			{
				var found = _items.FirstOrDefault(i => i.Key == key);
				return !found.Equals(default(KeyValuePair<string, object>)) ? found.Value : null;
			}
		}
	}
}