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
	public sealed class Queue<T> : IQueue<T> { 

		#region internal classes
		[System.Serializable]
		[System.Diagnostics.DebuggerDisplay( "Empty" )]
		private sealed class EmptyQueue : IQueue<T> { 
			private static readonly System.Int32 theHashCode;

			static EmptyQueue() { 
				theHashCode = System.Reflection.Assembly.GetExecutingAssembly().GetType().AssemblyQualifiedName.GetHashCode();
				unchecked { 
					theHashCode += typeof( T ).AssemblyQualifiedName.GetHashCode();
				}
			}
			public EmptyQueue() : base() { 
			}

			public IQueue<T> Empty { 
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

			public T Peek() { 
				throw new System.InvalidOperationException();
			}

			public IQueue<T> Dequeue() { 
				throw new System.InvalidOperationException();
			}

			public IQueue<T> Enqueue( T item ) { 
				return new Queue<T>( Stack<T>.Empty.Push( item ), Stack<T>.Empty );
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

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { 
				return this.GetEnumerator();
			}
			System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() { 
				return this.GetEnumerator();
			}
			public System.Collections.Generic.IEnumerator<T> GetEnumerator() { 
				yield break;
			}

			public IQueue<T> Duplicate() { 
				throw new System.InvalidOperationException();
			}
			public IQueue<T> Copy( System.Int32 count ) { 
				throw new System.InvalidOperationException();
			}
			public IQueue<T> Exchange() { 
				throw new System.InvalidOperationException();
			}

			public IQueue<T> Rotate( System.Int32 shift ) { 
				throw new System.InvalidOperationException();
			}
			public IQueue<T> Rotate( System.Int32 count, System.Int32 shift ) { 
				throw new System.InvalidOperationException();
			}

			public IQueue<T> Reverse() { 
				return this;
			}

			public sealed override System.Int32 GetHashCode() { 
				return theHashCode;
			}
		}
		#endregion internal classes


		#region fields
		private static readonly IQueue<T> theEmpty;

		private readonly IStack<T> myRead;
		private readonly IStack<T> myWrite;
		private readonly System.Int32 myCount;
		private readonly System.Int32 myHashCode;
		#endregion fields


		#region .ctor
		static Queue() { 
			theEmpty = new EmptyQueue();
		}

		private Queue() : base() { 
			myHashCode = theEmpty.GetHashCode();
		}
		private Queue( IStack<T> read, IStack<T> write ) : this() { 
			if ( null == write ) { 
				throw new System.ArgumentNullException( "write" );
			} else if ( null == read ) { 
				throw new System.ArgumentNullException( "read" );
			}
			myRead = read;
			myWrite = write;
			myCount = read.Count + write.Count;
			unchecked { 
				if ( false == myRead.IsEmpty ) { 
					myHashCode += myRead.GetHashCode();
				}
				if ( false == myWrite.IsEmpty ) { 
					myHashCode += myWrite.GetHashCode();
				}
			}
		}
		#endregion .ctor


		#region properties
		public static IQueue<T> Empty { 
			get { 
				return theEmpty;
			}
		}
		public System.Int32 Count {
			get {
				return myCount;
			}
		}
		public System.Boolean IsEmpty { 
			get { 
				return false;
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
			return myRead.Peek();
		}

		public IQueue<T> Enqueue( T item ) { 
			return new Queue<T>( myRead, myWrite.Push( item ) );
		}

		public IQueue<T> Dequeue() { 
			IStack<T> f = myRead.Pop();
			if ( f.IsEmpty ) { 
				if ( myWrite.IsEmpty ) { 
					return Queue<T>.Empty;
				} else { 
					return new Queue<T>( myWrite.Reverse(), Stack<T>.Empty );
				}
			} else { 
				return new Queue<T>( f, myWrite );
			}
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
			foreach ( T t in myRead ) { 
				yield return t;
			}
			foreach ( T t in myWrite.Reverse() ) { 
				yield return t;
			}
		}

		public IQueue<T> Duplicate() { 
			return new Queue<T>( myRead.Duplicate(), myWrite );
		}
		public IQueue<T> Copy( System.Int32 count ) { 
			if ( count < 0 ) { 
				throw new System.ArgumentOutOfRangeException( "count" );
			} else if ( 0 == count ) { 
				return this;
			} else if ( 1 == count ) { 
				return this.Duplicate();
			} else if ( this.Count < count ) { 
				throw new System.ArgumentException( "count is greater than number of elements in queue reference.", "count" );
			} else { 
				IStack<T> begin = Stack<T>.Empty;
				IStack<T> read = myRead;
				IStack<T> write = myWrite.Reverse();
				for ( System.Int32 i = 0; i < count; i++ ) { 
					begin = begin.Push( read.Peek() );
					read = read.Pop();
					if ( read.IsEmpty ) { 
						read = write;
					}
				}
				foreach ( T t in myRead.Reverse() ) { 
					write = write.Push( t );
				}
				return new Queue<T>( begin.Reverse(), write.Reverse() );
			}
		}
		public IQueue<T> Exchange() { 
			if ( this.Count <= 1 ) { 
				throw new System.InvalidOperationException();
			}

			IStack<T> read = myRead;
			IStack<T> write = myWrite;
			T a = read.Peek();
			read = read.Pop();
			if ( read.IsEmpty ) { 
				read = write.Reverse();
				write = Stack<T>.Empty;
			}
			T b = read.Peek();
			read = read.Pop();
			read = read.Push( a ).Push( b );
			return new Queue<T>( read, write );
		}

		public IQueue<T> Rotate( System.Int32 shift ) { 
			return this.Rotate( this.Count, shift );
		}
		public IQueue<T> Rotate( System.Int32 count, System.Int32 shift ) { 
			if ( count < 1 ) { 
				throw new System.ArgumentOutOfRangeException( "count" );
			} else if ( this.Count < count ) { 
				throw new System.ArgumentException( "count is greater than number of elements in the reference.", "count" );
			} else if ( 
				( 0 == shift ) 
				|| ( 1 == count ) 
				|| ( 0 == ( shift % count ) ) 
			) { 
				return this;
			}

			IStack<T> write = myWrite.Reverse();
			foreach ( T t in myRead ) { 
				write = write.Push( t );
			}
			write = write.Rotate( count, shift );

			return new Queue<T>( Stack<T>.Empty.Push( write.Peek() ), write.Pop().Reverse() );
		}

		public IQueue<T> Reverse() { 
			return new Queue<T>( myWrite, myRead );
		}

		public sealed override System.Int32 GetHashCode() { 
			return myHashCode;
		}
		#endregion methods

	}

}
