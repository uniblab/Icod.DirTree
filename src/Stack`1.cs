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
	/// Provides an immutable last-in, first-out stack.
	/// </summary>
	/// <typeparam name="T">The type of item stored in the stack.</typeparam>
	[System.Serializable]
	[System.Diagnostics.DebuggerDisplay( "Count = {Count}" )]
	public sealed class Stack<T> : IStack<T> {

		#region internal classes
		[System.Serializable]
		[System.Diagnostics.DebuggerDisplay( "Empty" )]
		private sealed class EmptyStack : IStack<T> {
			private static readonly System.Int32 theHashCode;

			static EmptyStack() {
				theHashCode = System.Reflection.Assembly.GetExecutingAssembly().GetType()!.AssemblyQualifiedName!.GetHashCode();
				unchecked {
					theHashCode += typeof( T )!.AssemblyQualifiedName!.GetHashCode();
				}
			}
			public EmptyStack() : base() {
			}

			public IStack<T> Empty {
				get {
					return this;
				}
			}
			public System.Int32 Count {
				get {
					return 0;
				}
			}
			public System.Boolean IsEmpty {
				get {
					return true;
				}
			}

			public System.Boolean IsReadOnly {
				get {
					return true;
				}
			}
			void System.Collections.Generic.ICollection<T>.Add( T item ) {
				throw new System.NotSupportedException();
			}
			void System.Collections.Generic.ICollection<T>.Clear() {
				throw new System.NotSupportedException();
			}
			public System.Boolean Contains( T item ) {
				return false;
			}
			public System.Boolean Contains( T item, System.Collections.Generic.IEqualityComparer<T> comparer ) {
				if ( null == comparer ) {
					throw new System.ArgumentNullException( "comparer" );
				}
				return false;
			}
			public void CopyTo( T[] array, System.Int32 arrayIndex ) {
				if ( arrayIndex < 0 ) {
					throw new System.ArgumentOutOfRangeException( "arrayIndex" );
				} else if ( null == array ) {
					throw new System.ArgumentNullException( "array" );
				} else if ( ( array.Length - arrayIndex ) < this.Count ) {
					throw new System.ArgumentException( null, "array" );
				}
				foreach ( T t in this ) {
					array[ arrayIndex++ ] = t;
				}
			}
			System.Boolean System.Collections.Generic.ICollection<T>.Remove( T item ) {
				throw new System.NotImplementedException();
			}

			public T Peek() {
				throw new System.InvalidOperationException();
			}
			public IStack<T> Pop() {
				throw new System.InvalidOperationException();
			}
			public IStack<T> Push( T item ) {
				return new Stack<T>( item, this );
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return this.GetEnumerator();
			}
			System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() {
				return this.GetEnumerator();
			}
			public System.Collections.Generic.IEnumerator<T> GetEnumerator() {
				yield break;
			}

			public IStack<T> Duplicate() {
				throw new System.InvalidOperationException();
			}
			public IStack<T> Copy( System.Int32 count ) {
				throw new System.InvalidOperationException();
			}
			public IStack<T> Exchange() {
				throw new System.InvalidOperationException();
			}
			public IStack<T> Reverse() {
				return this;
			}

			public IStack<T> Rotate( System.Int32 shift ) {
				throw new System.InvalidOperationException();
			}
			public IStack<T> Rotate( System.Int32 count, System.Int32 shift ) {
				throw new System.InvalidOperationException();
			}

			public sealed override System.Int32 GetHashCode() {
				return theHashCode;
			}
		}
		#endregion internal classes


		#region fields
		private static readonly IStack<T> theEmpty;

		private readonly T myValue;
		private readonly IStack<T> myTail;
		private readonly System.Int32 myCount;
		private readonly System.Int32 myHashCode;
		#endregion fields


		#region .ctor
		static Stack() {
			theEmpty = new EmptyStack();
		}

		private Stack( T value ) : this( value, theEmpty ) {
		}
		private Stack( T value, IStack<T> tail ) : base() {
			myValue = value;
			myTail = tail;
			myCount = 1 + tail.Count;
			myHashCode = theEmpty.GetHashCode();
			unchecked {
				myHashCode += tail.GetHashCode();
				if ( null != value ) {
					myHashCode += value.GetHashCode();
				}
			}
		}
		#endregion .ctor


		#region properties
		/// <summary>
		/// Gets the shared empty stack instance.
		/// </summary>
		/// <value>The empty immutable stack.</value>
		public static IStack<T> Empty {
			get {
				return theEmpty;
			}
		}
		/// <summary>
		/// Gets a value indicating whether this stack contains no items.
		/// </summary>
		/// <value><see langword="false"/> for non-empty stack instances.</value>
		public System.Boolean IsEmpty {
			get {
				return false;
			}
		}
		/// <summary>
		/// Gets the number of items in the stack.
		/// </summary>
		/// <value>The number of items reachable from this stack node.</value>
		public System.Int32 Count {
			get {
				return myCount;
			}
		}
		/// <summary>
		/// Gets a value indicating whether the collection is read-only.
		/// </summary>
		/// <value><see langword="true"/> because stack instances are immutable.</value>
		public System.Boolean IsReadOnly {
			get {
				return true;
			}
		}
		#endregion properties


		#region methods
		/// <summary>
		/// Returns the current top item without removing it.
		/// </summary>
		/// <returns>The item at the top of the stack.</returns>
		public T Peek() {
			return myValue;
		}
		/// <summary>
		/// Returns a stack without the current top item.
		/// </summary>
		/// <returns>The stack tail below the current top item.</returns>
		public IStack<T> Pop() {
			return myTail;
		}
		/// <summary>
		/// Returns a stack with <paramref name="item"/> added to the top.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <returns>A stack whose top item is <paramref name="item"/>.</returns>
		public IStack<T> Push( T item ) {
			return new Stack<T>( item, this );
		}

		void System.Collections.Generic.ICollection<T>.Add( T item ) {
			throw new System.NotSupportedException();
		}
		void System.Collections.Generic.ICollection<T>.Clear() {
			throw new System.NotSupportedException();
		}
		/// <summary>
		/// Determines whether the stack contains an item equal to <paramref name="item"/> using the default comparer.
		/// </summary>
		/// <param name="item">The item to locate.</param>
		/// <returns><see langword="true"/> when a matching item is present; otherwise, <see langword="false"/>.</returns>
		public System.Boolean Contains( T item ) {
			return this.Contains( item, System.Collections.Generic.EqualityComparer<T>.Default );
		}
		/// <summary>
		/// Determines whether the stack contains an item equal to <paramref name="item"/> using <paramref name="comparer"/>.
		/// </summary>
		/// <param name="item">The item to locate.</param>
		/// <param name="comparer">The comparer used to compare stack items.</param>
		/// <returns><see langword="true"/> when a matching item is present; otherwise, <see langword="false"/>.</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
		public System.Boolean Contains( T item, System.Collections.Generic.IEqualityComparer<T> comparer ) {
			if ( null == comparer ) {
				throw new System.ArgumentNullException( "comparer" );
			}
			foreach ( T t in this ) {
				if ( comparer.Equals( t, item ) ) {
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// Copies stack items to <paramref name="array"/> starting at <paramref name="arrayIndex"/>.
		/// </summary>
		/// <param name="array">The destination array.</param>
		/// <param name="arrayIndex">The zero-based destination index at which copying begins.</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is negative.</exception>
		/// <exception cref="System.ArgumentException">The destination array does not have enough remaining space.</exception>
		public void CopyTo( T[] array, System.Int32 arrayIndex ) {
			if ( arrayIndex < 0 ) {
				throw new System.ArgumentOutOfRangeException( "arrayIndex" );
			} else if ( null == array ) {
				throw new System.ArgumentNullException( "array" );
			} else if ( ( array.Length - arrayIndex ) < this.Count ) {
				throw new System.ArgumentException( null, "array" );
			}
			foreach ( T t in this ) {
				array[ arrayIndex++ ] = t;
			}
		}
		System.Boolean System.Collections.Generic.ICollection<T>.Remove( T item ) {
			throw new System.NotImplementedException();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}
		System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() {
			return this.GetEnumerator();
		}
		/// <summary>
		/// Returns an enumerator that yields stack items from top to bottom.
		/// </summary>
		/// <returns>An enumerator over the stack contents.</returns>
		public System.Collections.Generic.IEnumerator<T> GetEnumerator() {
			for ( IStack<T> stack = this; false == stack.IsEmpty; stack = stack.Pop() ) {
				yield return stack.Peek();
			}
		}

		/// <summary>
		/// Returns a stack with the current top item duplicated.
		/// </summary>
		/// <returns>A stack whose two top items are both the original top item.</returns>
		/// <exception cref="System.InvalidOperationException">The stack is empty.</exception>
		public IStack<T> Duplicate() {
			return this.Push( this.Peek() );
		}
		/// <summary>
		/// Returns a stack with the top <paramref name="count"/> items copied above the current top item.
		/// </summary>
		/// <param name="count">The number of top items to copy.</param>
		/// <returns>A stack containing the copied items followed by the original stack contents.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
		/// <exception cref="System.ArgumentException"><paramref name="count"/> is greater than the number of items in the stack.</exception>
		public IStack<T> Copy( System.Int32 count ) {
			if ( count < 0 ) {
				throw new System.ArgumentOutOfRangeException( "count" );
			} else if ( 0 == count ) {
				return this;
			} else if ( 1 == count ) {
				return this.Duplicate();
			} else if ( this.Count < count ) {
				throw new System.ArgumentException( "count is greater than number of elements in stack reference.", "count" );
			} else {
				T[] dupes = new T[ count ];
				IStack<T> probe = this;
				System.Int32 i = count;
				while ( 0 < i-- ) {
					dupes[ i ] = probe.Peek();
					probe = probe.Pop();
				}
				IStack<T> output = this;
				for ( System.Int32 j = 0; j < count; j++ ) {
					output = output.Push( dupes[ j ] );
				}
				return output;
			}
		}
		/// <summary>
		/// Returns a stack with the top two items exchanged.
		/// </summary>
		/// <returns>A stack whose top two items have swapped positions.</returns>
		/// <exception cref="System.InvalidOperationException">The stack contains fewer than two items.</exception>
		public IStack<T> Exchange() {
			if ( this.Count < 2 ) {
				throw new System.InvalidOperationException();
			}
			IStack<T> tail = this;
			T a = tail.Peek();
			tail = tail.Pop();
			T b = tail.Peek();
			tail = tail.Pop();
			return tail.Push( a ).Push( b );
		}

		/// <summary>
		/// Returns a stack with all items rotated by <paramref name="shift"/> positions.
		/// </summary>
		/// <param name="shift">The number of positions to rotate the stack.</param>
		/// <returns>A stack containing the same items in rotated order.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">The stack is empty.</exception>
		public IStack<T> Rotate( System.Int32 shift ) {
			return this.Rotate( this.Count, shift );
		}
		/// <summary>
		/// Returns a stack with the top <paramref name="count"/> items rotated by <paramref name="shift"/> positions.
		/// </summary>
		/// <param name="count">The number of top items to rotate.</param>
		/// <param name="shift">The number of positions to rotate the selected items.</param>
		/// <returns>A stack whose selected top segment has been rotated.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="count"/> is less than one.</exception>
		/// <exception cref="System.ArgumentException"><paramref name="count"/> is greater than the number of items in the stack.</exception>
		public IStack<T> Rotate( System.Int32 count, System.Int32 shift ) {
			if ( count < 1 ) {
				throw new System.ArgumentOutOfRangeException( "count" );
			} else if ( this.Count < count ) {
				throw new System.ArgumentException( "count is greater than number of elements in stack reference.", "count" );
			} else if ( 0 == shift ) {
				return this;
			}
			shift = -shift;
			System.Int32 j = shift % count;
			if ( 0 == j ) {
				return this;
			}

			T[] store = new T[ count ];
			IStack<T> probe = this;
			for ( System.Int32 i = count - 1; 0 <= i; i-- ) {
				store[ i ] = probe.Peek();
				probe = probe.Pop();
			}
			for ( System.Int32 i = 0; i < count; i++ ) {
				probe = probe.Push( store[ GetShiftedIndex( i, j, count ) ] );
			}

			return probe;
		}

		/// <summary>
		/// Returns a stack with the item order reversed.
		/// </summary>
		/// <returns>A stack containing the same items in the opposite order.</returns>
		public IStack<T> Reverse() {
			IStack<T> output = Stack<T>.Empty;

			IStack<T> probe = this;
			while ( !probe.IsEmpty ) {
				output = output.Push( probe.Peek() );
				probe = probe.Pop();
			}

			return output;
		}

		/// <summary>
		/// Returns the precomputed hash code for this immutable stack.
		/// </summary>
		/// <returns>The hash code for this stack instance.</returns>
		public sealed override System.Int32 GetHashCode() {
			return myHashCode;
		}
		#endregion methods


		#region static methods
		/// <summary>
		/// Calculates a circularly shifted zero-based index.
		/// </summary>
		/// <param name="index">The original zero-based index.</param>
		/// <param name="shift">The signed shift amount.</param>
		/// <param name="count">The number of positions in the circular range.</param>
		/// <returns>The shifted index normalized into the range from zero through <paramref name="count"/> minus one.</returns>
		internal static System.Int32 GetShiftedIndex( System.Int32 index, System.Int32 shift, System.Int32 count ) {
			System.Int32 output = ( index + shift ) % count;

			if ( output < 0 ) {
				output += count;
			} else if ( count <= output ) {
				output -= count;
			}

			return output;
		}
		#endregion static methods

	}

}
