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
		public static IStack<T> Empty { 
			get { 
				return theEmpty;
			}
		}
		public System.Boolean IsEmpty {
			get {
				return false;
			}
		}
		public System.Int32 Count { 
			get { 
				return myCount;
			}
		}
		public System.Boolean IsReadOnly { 
			get { 
				return true;
			}
		}
		#endregion properties


		#region methods
		public T Peek() {
			return myValue;
		}
		public IStack<T> Pop() {
			return myTail;
		}
		public IStack<T> Push( T item ) {
			return new Stack<T>( item, this );
		}

		void System.Collections.Generic.ICollection<T>.Add( T item ) { 
			throw new System.NotSupportedException();
		}
		void System.Collections.Generic.ICollection<T>.Clear() { 
			throw new System.NotSupportedException();
		}
		public System.Boolean Contains( T item ) {
			return this.Contains( item, System.Collections.Generic.EqualityComparer<T>.Default );
		}
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
		public System.Collections.Generic.IEnumerator<T> GetEnumerator() { 
			for ( IStack<T> stack = this; false == stack.IsEmpty; stack = stack.Pop() ) { 
				yield return stack.Peek();
			}
		}

		public IStack<T> Duplicate() { 
			return this.Push( this.Peek() );
		}
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

		public IStack<T> Rotate( System.Int32 shift ) { 
			return this.Rotate( this.Count, shift );
		}
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

		public IStack<T> Reverse() {
			IStack<T> output = Stack<T>.Empty;

			IStack<T> probe = this;
			while ( !probe.IsEmpty ) { 
				output = output.Push( probe.Peek() );
				probe = probe.Pop();
			}

			return output;
		}

		public sealed override System.Int32 GetHashCode() { 
			return myHashCode;
		}
		#endregion methods


		#region static methods
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
