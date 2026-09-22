/*
	Icod.DirTree
	Cross-platform command-line tool to report subdirectory structure as text tree.
	Copyright( C) 2026  Timothy J.Bruce<uniblab@hotmail.com>
*/

/*
	This program is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option ) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program.If not, see<https://www.gnu.org/licenses/>.
*/

namespace Icod.Collections.Immutable {

	/// <summary>
	/// Defines an immutable last-in, first-out collection.
	/// </summary>
	/// <typeparam name="T">The type of item stored in the stack.</typeparam>
	public interface IStack<T> : System.Collections.Generic.ICollection<T> {

		/// <summary>
		/// Gets a value indicating whether the stack contains no items.
		/// </summary>
		/// <value><see langword="true"/> when the stack is empty; otherwise, <see langword="false"/>.</value>
		System.Boolean IsEmpty {
			get;
		}

		/// <summary>
		/// Returns a stack with <paramref name="item"/> added to the top.
		/// </summary>
		/// <param name="item">The item to add to the returned stack.</param>
		/// <returns>A stack whose top item is <paramref name="item"/>.</returns>
		IStack<T> Push( T item );

		/// <summary>
		/// Returns a stack without the current top item.
		/// </summary>
		/// <returns>The stack tail below the current top item.</returns>
		/// <exception cref="System.InvalidOperationException">The stack is empty.</exception>
		IStack<T> Pop();

		/// <summary>
		/// Returns the current top item without removing it.
		/// </summary>
		/// <returns>The item at the top of the stack.</returns>
		/// <exception cref="System.InvalidOperationException">The stack is empty.</exception>
		T Peek();

		/// <summary>
		/// Returns a stack with the current top item duplicated.
		/// </summary>
		/// <returns>A stack whose two top items are both the original top item.</returns>
		/// <exception cref="System.InvalidOperationException">The stack is empty.</exception>
		IStack<T> Duplicate();

		/// <summary>
		/// Returns a stack with the top <paramref name="count"/> items copied above the current top item.
		/// </summary>
		/// <param name="count">The number of top items to copy.</param>
		/// <returns>A stack containing the copied items followed by the original stack contents.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
		/// <exception cref="System.ArgumentException"><paramref name="count"/> is greater than the number of items in the stack.</exception>
		IStack<T> Copy( System.Int32 count );

		/// <summary>
		/// Returns a stack with the top two items exchanged.
		/// </summary>
		/// <returns>A stack whose top two items have swapped positions.</returns>
		/// <exception cref="System.InvalidOperationException">The stack contains fewer than two items.</exception>
		IStack<T> Exchange();

		/// <summary>
		/// Returns a stack with the item order reversed.
		/// </summary>
		/// <returns>A stack containing the same items in the opposite order.</returns>
		IStack<T> Reverse();

		/// <summary>
		/// Returns a stack with all items rotated by <paramref name="shift"/> positions.
		/// </summary>
		/// <param name="shift">The number of positions to rotate the stack.</param>
		/// <returns>A stack containing the same items in rotated order.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">The stack is empty.</exception>
		IStack<T> Rotate( System.Int32 shift );

		/// <summary>
		/// Returns a stack with the top <paramref name="count"/> items rotated by <paramref name="shift"/> positions.
		/// </summary>
		/// <param name="count">The number of top items to rotate.</param>
		/// <param name="shift">The number of positions to rotate the selected items.</param>
		/// <returns>A stack whose selected top segment has been rotated.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="count"/> is less than one.</exception>
		/// <exception cref="System.ArgumentException"><paramref name="count"/> is greater than the number of items in the stack.</exception>
		IStack<T> Rotate( System.Int32 count, System.Int32 shift );

		/// <summary>
		/// Determines whether the stack contains an item equal to <paramref name="item"/> using <paramref name="comparer"/>.
		/// </summary>
		/// <param name="item">The item to locate.</param>
		/// <param name="comparer">The comparer used to compare stack items.</param>
		/// <returns><see langword="true"/> when a matching item is present; otherwise, <see langword="false"/>.</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
		System.Boolean Contains( T item, System.Collections.Generic.IEqualityComparer<T> comparer );

		/// <summary>
		/// Returns an enumerator that yields items from top to bottom.
		/// </summary>
		/// <returns>An enumerator over the stack contents.</returns>
		new System.Collections.Generic.IEnumerator<T> GetEnumerator();

	}

}
