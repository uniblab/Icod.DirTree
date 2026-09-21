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
	public sealed class Deque<T> : IDeque<T> { 

		#region internal classes
		[System.Serializable]
		[System.Diagnostics.DebuggerDisplay( "Count = {Count}" )]
		private abstract class MiniDeque : System.Collections.Generic.IEnumerable<T> { 
			protected MiniDeque() : base() {
			}

			public abstract System.Int32 Count {
				get;
			}
			public System.Boolean IsEmpty {
				get {
					return false;
				}
			}
			public virtual System.Boolean IsFull {
				get {
					return false;
				}
			}

			public abstract T PeekLeft();
			public abstract T PeekRight();
			public abstract MiniDeque EnqueueLeft( T item );
			public abstract MiniDeque EnqueueRight( T item );
			public abstract MiniDeque DequeueLeft();
			public abstract MiniDeque DequeueRight();
			public abstract MiniDeque Reverse();

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return this.GetEnumerator();
			}
			public abstract System.Collections.Generic.IEnumerator<T> GetEnumerator();
			public abstract System.Collections.Generic.IEnumerator<T> GetEnumeratorReversed();
		}
		private sealed class Mini1 : MiniDeque { 
			private readonly System.Int32 myHashCode;
			private readonly T v1;

			private Mini1() : base() {
				myHashCode = Deque<T>.Empty.GetHashCode();
			}
			public Mini1( T t ) : this() { 
				v1 = t;
				unchecked { 
					if ( null != v1 ) { 
						myHashCode += v1.GetHashCode();
					}
				}
			}

			public sealed override System.Int32 Count {
				get {
					return 1;
				}
			}

			public sealed override T PeekLeft() {
				return v1;
			}
			public sealed override T PeekRight() {
				return v1;
			}
			public sealed override MiniDeque EnqueueLeft( T item ) {
				return new Mini2( item, v1 );
			}
			public sealed override MiniDeque EnqueueRight( T item ) {
				return new Mini2( v1, item );
			}
			public sealed override MiniDeque DequeueLeft() { 
				throw new System.NotSupportedException();
			}
			public sealed override MiniDeque DequeueRight() { 
				throw new System.NotSupportedException();
			}
			public sealed override MiniDeque Reverse() { 
				return this;
			}

			public sealed override System.Collections.Generic.IEnumerator<T> GetEnumerator() {
				yield return v1;
			}
			public sealed override System.Collections.Generic.IEnumerator<T> GetEnumeratorReversed() { 
				yield return v1;
			}

			public sealed override System.Int32 GetHashCode() {
				return myHashCode;
			}
		}
		private sealed class Mini2 : MiniDeque {
			private readonly System.Int32 myHashCode;
			private readonly T v1;
			private readonly T v2;

			private Mini2() : base() {
				myHashCode = Deque<T>.Empty.GetHashCode();
			}
			public Mini2( T t1, T t2 ) : this() {
				v1 = t1;
				v2 = t2;
				unchecked {
					if ( null != v1 ) {
						myHashCode += v1.GetHashCode();
					}
					if ( null != v2 ) {
						myHashCode += v2.GetHashCode();
					}
				}
			}

			public sealed override System.Int32 Count {
				get {
					return 2;
				}
			}

			public sealed override T PeekLeft() {
				return v1;
			}
			public sealed override T PeekRight() {
				return v2;
			}
			public sealed override MiniDeque EnqueueLeft( T item ) {
				return new Mini3( item, v1, v2 );
			}
			public sealed override MiniDeque EnqueueRight( T item ) {
				return new Mini3( v1, v2, item );
			}
			public sealed override MiniDeque DequeueLeft() {
				return new Mini1( v2 );
			}
			public sealed override MiniDeque DequeueRight() {
				return new Mini1( v1 );
			}
			public sealed override MiniDeque Reverse() { 
				return new Mini2( v2, v1 );
			}

			public sealed override System.Collections.Generic.IEnumerator<T> GetEnumerator() {
				return Stack<T>.Empty.Push( v2 ).Push( v1 ).GetEnumerator();
			}
			public sealed override System.Collections.Generic.IEnumerator<T> GetEnumeratorReversed() {
				return Stack<T>.Empty.Push( v1 ).Push( v2 ).GetEnumerator();
			}

			public sealed override System.Int32 GetHashCode() {
				return myHashCode;
			}
		}
		private sealed class Mini3 : MiniDeque {
			private readonly System.Int32 myHashCode;
			private readonly T v1;
			private readonly T v2;
			private readonly T v3;

			private Mini3() : base() {
				myHashCode = Deque<T>.Empty.GetHashCode();
			}
			public Mini3( T t1, T t2, T t3 ) : this() {
				v1 = t1;
				v2 = t2;
				v3 = t3;
				unchecked {
					if ( null != v1 ) {
						myHashCode += v1.GetHashCode();
					}
					if ( null != v2 ) {
						myHashCode += v2.GetHashCode();
					}
					if ( null != v3 ) {
						myHashCode += v3.GetHashCode();
					}
				}
			}

			public sealed override System.Int32 Count {
				get {
					return 3;
				}
			}

			public sealed override T PeekLeft() {
				return v1;
			}
			public sealed override T PeekRight() {
				return v3;
			}

			public sealed override MiniDeque EnqueueLeft( T item ) {
				return new Mini4( item, v1, v2, v3 );
			}
			public sealed override MiniDeque EnqueueRight( T item ) {
				return new Mini4( v1, v2, v3, item );
			}
			public sealed override MiniDeque DequeueLeft() {
				return new Mini2( v2, v3 );
			}
			public sealed override MiniDeque DequeueRight() {
				return new Mini2( v1, v2 );
			}
			public sealed override MiniDeque Reverse() { 
				return new Mini3( v3, v2, v1 );
			}

			public sealed override System.Collections.Generic.IEnumerator<T> GetEnumerator() {
				return Stack<T>.Empty.Push( v3 ).Push( v2 ).Push( v1 ).GetEnumerator();
			}
			public sealed override System.Collections.Generic.IEnumerator<T> GetEnumeratorReversed() {
				return Stack<T>.Empty.Push( v1 ).Push( v2 ).Push( v3 ).GetEnumerator();
			}

			public sealed override System.Int32 GetHashCode() {
				return myHashCode;
			}
		}
		private sealed class Mini4 : MiniDeque {
			private readonly System.Int32 myHashCode;
			private readonly T v1;
			private readonly T v2;
			private readonly T v3;
			private readonly T v4;

			private Mini4() : base() {
				myHashCode = Deque<T>.Empty.GetHashCode();
			}
			public Mini4( T t1, T t2, T t3, T t4 ) : this() {
				v1 = t1;
				v2 = t2;
				v3 = t3;
				v4 = t4;
				unchecked { 
					if ( null != v1 ) { 
						myHashCode += v1.GetHashCode();
					}
					if ( null != v2 ) { 
						myHashCode += v2.GetHashCode();
					}
					if ( null != v3 ) { 
						myHashCode += v3.GetHashCode();
					}
					if ( null != v4 ) { 
						myHashCode += v4.GetHashCode();
					}
				}
			}

			public sealed override System.Int32 Count {
				get {
					return 4;
				}
			}
			public sealed override System.Boolean IsFull {
				get {
					return true;
				}
			}

			public sealed override T PeekLeft() {
				return v1;
			}
			public sealed override T PeekRight() {
				return v4;
			}

			public sealed override MiniDeque EnqueueLeft( T item ) {
				throw new System.NotSupportedException();
			}
			public sealed override MiniDeque EnqueueRight( T item ) {
				throw new System.NotSupportedException();
			}
			public sealed override MiniDeque DequeueLeft() {
				return new Mini3( v2, v3, v4 );
			}
			public sealed override MiniDeque DequeueRight() {
				return new Mini3( v1, v2, v3 );
			}
			public sealed override MiniDeque Reverse() { 
				return new Mini4( v4, v3, v2, v1 );
			}

			public sealed override System.Collections.Generic.IEnumerator<T> GetEnumerator() { 
				return Stack<T>.Empty.Push( v4 ).Push( v3 ).Push( v2 ).Push( v1 ).GetEnumerator();
			}
			public sealed override System.Collections.Generic.IEnumerator<T> GetEnumeratorReversed() { 
				return Stack<T>.Empty.Push( v1 ).Push( v2 ).Push( v3 ).Push( v4 ).GetEnumerator();
			}

			public sealed override System.Int32 GetHashCode() {
				return myHashCode;
			}
		}

		[System.Serializable]
		[System.Diagnostics.DebuggerDisplay( "Empty" )]
		private sealed class EmptyDeque : IDeque<T> { 
			private static readonly System.Int32 theHashCode;

			static EmptyDeque() { 
				theHashCode = System.Reflection.Assembly.GetExecutingAssembly().GetType().AssemblyQualifiedName.GetHashCode();
				unchecked { 
					theHashCode += typeof( T ).AssemblyQualifiedName.GetHashCode();
				}
			}
			public EmptyDeque() : base() { 
			}

			public static IDeque<T> Empty { 
				get { 
					return Deque<T>.Empty;
				}
			}
			public System.Boolean IsEmpty { 
				get { 
					return true;
				}
			}
			public System.Int32 Count { 
				get { 
					return 0;
				}
			}
			public System.Boolean IsReadOnly { 
				get { 
					return true;
				}
			}

			public T PeekLeft() { 
				throw new System.InvalidOperationException();
			}
			public T PeekRight() { 
				throw new System.InvalidOperationException();
			}
			public IDeque<T> EnqueueLeft( T item ) { 
				return new Single( item );
			}
			public IDeque<T> EnqueueRight( T item ) { 
				return new Single( item );
			}
			public IDeque<T> DequeueLeft() { 
				throw new System.InvalidOperationException();
			}
			public IDeque<T> DequeueRight() { 
				throw new System.InvalidOperationException();
			}

			void System.Collections.Generic.ICollection<T>.Add( T item ) {
				throw new System.NotSupportedException();
			}
			void System.Collections.Generic.ICollection<T>.Clear() {
				throw new System.NotSupportedException();
			}
			public System.Boolean Contains( T item ) {
				System.Collections.Generic.IEqualityComparer<T> comparer = System.Collections.Generic.EqualityComparer<T>.Default;
				foreach ( T t in this ) {
					if ( comparer.Equals( t, item ) ) {
						return true;
					}
				}
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


			IQueue<T> IQueue<T>.Exchange() {
				throw new System.InvalidOperationException();
			}
			IQueue<T> IQueue<T>.Dequeue() {
				return this.DequeueLeft();
			}
			T IQueue<T>.Peek() {
				throw new System.InvalidOperationException();
			}
			IQueue<T> IQueue<T>.Duplicate() {
				throw new System.InvalidOperationException();
			}
			IQueue<T> IQueue<T>.Copy( System.Int32 count ) { 
				if ( 0 == count ) { 
					return this;
				} else if ( count < 0 ) { 
					throw new System.ArgumentOutOfRangeException( "count", "count must be non-negative." );
				} else { 
					throw new System.InvalidOperationException();
				}
			}
			IQueue<T> IQueue<T>.Rotate( System.Int32 shift ) {
				throw new System.InvalidOperationException();
			}
			IQueue<T> IQueue<T>.Rotate( System.Int32 count, System.Int32 shift ) {
				if ( ( 0 == count ) || ( 0 == shift ) ) {
					return this;
				} else {
					throw new System.InvalidOperationException();
				}
			}
			IQueue<T> IQueue<T>.Enqueue( T item ) {
				return this.EnqueueRight( item );
			}
			IQueue<T> IQueue<T>.Reverse() { 
				return this;
			}

			IStack<T> IStack<T>.Push( T item ) { 
				return this.EnqueueRight( item );
			}
			IStack<T> IStack<T>.Pop() { 
				throw new System.InvalidOperationException();
			}
			T IStack<T>.Peek() { 
				throw new System.InvalidOperationException();
			}
			IStack<T> IStack<T>.Duplicate() { 
				throw new System.InvalidOperationException();
			}
			IStack<T> IStack<T>.Copy( System.Int32 count ) { 
				if ( 0 == count ) { 
					return this;
				} else if ( count < 0 ) { 
					throw new System.ArgumentOutOfRangeException( "count", "count must be non-negative." );
				} else { 
					throw new System.InvalidOperationException();
				}
			}
			IStack<T> IStack<T>.Exchange() { 
				throw new System.InvalidOperationException();
			}
			IStack<T> IStack<T>.Rotate( System.Int32 shift ) { 
				throw new System.InvalidOperationException();
			}
			IStack<T> IStack<T>.Rotate( System.Int32 count, System.Int32 shift ) { 
				if ( count < 1 ) { 
					throw new System.ArgumentOutOfRangeException( "count" );
				} else if ( this.Count < count ) { 
					throw new System.ArgumentException( "count is greater than number of elements in stack reference.", "count" );
				} else if ( 0 == shift ) { 
					return this;
				}
				throw new System.InvalidOperationException();
			}
			IStack<T> IStack<T>.Reverse() { 
				return this;
			}

			public IDeque<T> Reverse() { 
				return this;
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return this.GetEnumerator();
			}
			public System.Collections.Generic.IEnumerator<T> GetEnumerator() { 
				yield break;
			}

			public sealed override System.Int32 GetHashCode() { 
				return theHashCode;
			}
		}

		[System.Serializable]
		[System.Diagnostics.DebuggerDisplay( "Count = {Count}" )]
		private sealed class Single : IDeque<T> { 
			private static readonly System.Int32 theHashCode;
			private readonly System.Int32 myHashCode;
			private readonly MiniDeque myStore;
			private readonly System.Int32 myCount;

			static Single() { 
				theHashCode = System.Reflection.Assembly.GetExecutingAssembly().GetType().AssemblyQualifiedName.GetHashCode();
				unchecked { 
					theHashCode += typeof( T ).AssemblyQualifiedName.GetHashCode();
				}
			}
			private Single() : base() { 
				myHashCode = theHashCode;
			}
			public Single( T t ) : this( new Mini1( t ) ) { 
			}
			public Single( MiniDeque store ) : this() { 
				if ( null == store ) { 
					throw new System.ArgumentNullException( "store" );
				} else { 
					myStore = store;
			 		unchecked { 
						myHashCode += myStore.GetHashCode();
					}
					myCount = myStore.Count;
				}
			}

			public static IDeque<T> Empty { 
				get {
					return Deque<T>.Empty;
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

			public T PeekLeft() { 
				return myStore.PeekLeft();
			}
			public T PeekRight() { 
				return myStore.PeekRight();
			}

			public IDeque<T> EnqueueLeft( T item ) { 
				if ( myStore.IsFull ) { 
					return new Double( new Mini1( item ), myStore );
				} else { 
					return new Single( myStore.EnqueueLeft( item ) );
				}
			}
			public IDeque<T> EnqueueRight( T item ) { 
				if ( myStore.IsFull ) { 
					return new Double( myStore, new Mini1( item ) );
				} else { 
					return new Single( myStore.EnqueueRight( item ) );
				}
			}
			public IDeque<T> DequeueLeft() { 
				if ( 1 == myStore.Count ) {
					return Deque<T>.Empty;
				} else { 
					return new Single( myStore.DequeueLeft() );
				}
			}
			public IDeque<T> DequeueRight() { 
				if ( 1 == myStore.Count ) {
					return Deque<T>.Empty;
				} else { 
					return new Single( myStore.DequeueRight() );
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

			IQueue<T> IQueue<T>.Dequeue() { 
				return this.DequeueLeft();
			}
			T IQueue<T>.Peek() { 
				return this.PeekLeft();
			}
			IQueue<T> IQueue<T>.Duplicate() { 
				return this.EnqueueLeft( this.PeekLeft() );
			}
			IQueue<T> IQueue<T>.Copy( System.Int32 count ) { 
				if ( count < 0 ) { 
					throw new System.ArgumentOutOfRangeException( "count" );
				} else if ( 0 == count ) { 
					return this;
				} else if ( 1 == count ) { 
					return ((IQueue<T>)this).Duplicate();
				} else if ( this.Count < count ) { 
					throw new System.ArgumentException( "count is greater than number of elements in stack reference.", "count" );
				} else { 
					T[] dupes = new T[ count ];
					IQueue<T> probe = this;
					System.Int32 i = count;
					while ( 0 < i-- ) { 
						dupes[ i ] = probe.Peek();
						probe = probe.Dequeue();
					}
					IQueue<T> output = this;
					for ( System.Int32 j = 0; j < count; j++ ) { 
						output = output.Enqueue( dupes[ j ] );
					}
					return output;
				}
			}
			IQueue<T> IQueue<T>.Rotate( System.Int32 shift ) { 
				return ((IQueue<T>)this).Rotate( this.Count, shift );
			}
			IQueue<T> IQueue<T>.Rotate( System.Int32 count, System.Int32 shift ) {
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
				} else { 
					return QueueRotate( this, count, shift );
				}
			}
			IQueue<T> IQueue<T>.Enqueue( T item ) {
				return this.EnqueueRight( item );
			}
			IQueue<T> IQueue<T>.Exchange() { 
				if ( myCount < 2 ) { 
					throw new System.InvalidOperationException();
				}
				IDeque<T> output = this;
				T a = output.PeekLeft();
				output = output.DequeueLeft();
				T b = output.PeekLeft();
				return output.DequeueLeft().EnqueueLeft( a ).EnqueueLeft( b );
			}
			IQueue<T> IQueue<T>.Reverse() { 
				return this.Reverse();
			}

			IStack<T> IStack<T>.Push( T item ) { 
				return this.EnqueueRight( item );
			}
			IStack<T> IStack<T>.Pop() { 
				return this.DequeueRight();
			}
			T IStack<T>.Peek() { 
				return this.PeekRight();
			}
			IStack<T> IStack<T>.Duplicate() { 
				return this.EnqueueRight( this.PeekRight() );
			}
			IStack<T> IStack<T>.Copy( System.Int32 count ) { 
				if ( count < 0 ) { 
					throw new System.ArgumentOutOfRangeException( "count" );
				} else if ( 0 == count ) { 
					return this;
				} else if ( 1 == count ) { 
					return ((IStack<T>)this).Duplicate();
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
			IStack<T> IStack<T>.Exchange() { 
				if ( this.Count < 2 ) { 
					throw new System.InvalidOperationException();
				}
				IDeque<T> probe = this;
				T a = probe.PeekRight();
				probe = probe.DequeueRight();
				T b = probe.PeekRight();
				return probe.DequeueRight().EnqueueRight( a ).EnqueueRight( b );
			}
			IStack<T> IStack<T>.Rotate( System.Int32 shift ) { 
				return ((IStack<T>)this).Rotate( this.Count, shift );
			}
			IStack<T> IStack<T>.Rotate( System.Int32 count, System.Int32 shift ) {
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
				} else { 
					return StackRotate( this, count, shift );
				}
			}
			IStack<T> IStack<T>.Reverse() { 
				return this.Reverse();
			}

			public IDeque<T> Reverse() {
				MiniDeque store = myStore.Reverse();
				if ( store == myStore ) {
					return this;
				} else {
					return new Single( store );
				}
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return this.GetEnumerator();
			}
			public System.Collections.Generic.IEnumerator<T> GetEnumerator() {
				return myStore.GetEnumerator();
			}
			System.Collections.Generic.IEnumerator<T> IStack<T>.GetEnumerator() { 
				return myStore.GetEnumeratorReversed();
			}

			public sealed override System.Int32 GetHashCode() {
				return myHashCode;
			}
		}

		[System.Serializable]
		[System.Diagnostics.DebuggerDisplay( "Count = {Count}" )]
		private sealed class Double : IDeque<T> { 
			private static readonly System.Int32 theHashCode;
			private readonly System.Int32 myHashCode;
			private readonly MiniDeque myLeft;
			private readonly MiniDeque myRight;
			private readonly System.Int32 myCount;

			static Double() { 
				theHashCode = System.Reflection.Assembly.GetExecutingAssembly().GetType().AssemblyQualifiedName.GetHashCode();
				unchecked { 
					theHashCode += typeof( T ).AssemblyQualifiedName.GetHashCode();
				}
			}
			private Double() : base() { 
				myHashCode = theHashCode;
			}
			public Double( MiniDeque left, MiniDeque right ) : this() { 
				if ( null == left ) { 
					throw new System.ArgumentNullException( "left" );
				} else if ( null == right ) { 
					throw new System.ArgumentNullException( "right" );
				} else { 
					myLeft = left;
					myRight = right;
					unchecked { 
						myHashCode += myLeft.GetHashCode();
						myHashCode += myRight.GetHashCode();
					}
					myCount = myLeft.Count + myRight.Count;
				}
			}

			public static IDeque<T> Empty { 
				get {
					return Deque<T>.Empty;
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

			public T PeekLeft() { 
				return myLeft.PeekLeft();
			}
			public T PeekRight() { 
				return myRight.PeekRight();
			}

			public IDeque<T> EnqueueLeft( T item ) { 
				if ( myLeft.IsFull ) { 
					if ( myRight.IsFull ) { 
						return new Triple( new Mini1( item ), myLeft, myRight );
					} else { 
						T remainder = myLeft.PeekRight();
						return new Double( myLeft.DequeueRight().EnqueueLeft( item ), myRight.EnqueueLeft( remainder ) );
					}
				} else { 
					return new Double( myLeft.EnqueueLeft( item ), myRight );
				}
			}
			public IDeque<T> EnqueueRight( T item ) { 
				if ( myRight.IsFull ) { 
					if ( myLeft.IsFull ) { 
						return new Triple( myLeft, myRight, new Mini1( item ) );
					} else { 
						T remainder = myRight.PeekLeft();
						return new Double( myLeft.EnqueueRight( remainder ), myRight.DequeueLeft().EnqueueRight( item ) );
					}
				} else { 
					return new Double( myLeft, myRight.EnqueueRight( item ) );
				}
			}
			public IDeque<T> DequeueLeft() { 
				if ( 1 == myLeft.Count ) { 
					return new Single( myRight );
				} else { 
					return new Double( myLeft.DequeueLeft(), myRight );
				}
			}
			public IDeque<T> DequeueRight() { 
				if ( 1 == myRight.Count ) { 
					return new Single( myLeft );
				} else { 
					return new Double( myLeft, myRight.DequeueRight() );
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

			IQueue<T> IQueue<T>.Dequeue() {
				return this.DequeueLeft();
			}
			T IQueue<T>.Peek() {
				return this.PeekLeft();
			}
			IQueue<T> IQueue<T>.Duplicate() {
				return this.EnqueueLeft( this.PeekLeft() );
			}
			IQueue<T> IQueue<T>.Copy( System.Int32 count ) {
				if ( count < 0 ) {
					throw new System.ArgumentOutOfRangeException( "count" );
				} else if ( 0 == count ) {
					return this;
				} else if ( 1 == count ) {
					return ( (IQueue<T>)this ).Duplicate();
				} else if ( this.Count < count ) {
					throw new System.ArgumentException( "count is greater than number of elements in stack reference.", "count" );
				} else {
					T[] dupes = new T[ count ];
					IQueue<T> probe = this;
					System.Int32 i = count;
					while ( 0 < i-- ) {
						dupes[ i ] = probe.Peek();
						probe = probe.Dequeue();
					}
					IQueue<T> output = this;
					for ( System.Int32 j = 0; j < count; j++ ) {
						output = output.Enqueue( dupes[ j ] );
					}
					return output;
				}
			}
			IQueue<T> IQueue<T>.Rotate( System.Int32 shift ) {
				return ( (IQueue<T>)this ).Rotate( this.Count, shift );
			}
			IQueue<T> IQueue<T>.Rotate( System.Int32 count, System.Int32 shift ) {
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
				} else {
					return QueueRotate( this, count, shift );
				}
			}
			IQueue<T> IQueue<T>.Enqueue( T item ) {
				return this.EnqueueRight( item );
			}
			IQueue<T> IQueue<T>.Exchange() {
				if ( myCount < 2 ) {
					throw new System.InvalidOperationException();
				}
				IDeque<T> output = this;
				T a = output.PeekLeft();
				output = output.DequeueLeft();
				T b = output.PeekLeft();
				return output.DequeueLeft().EnqueueLeft( a ).EnqueueLeft( b );
			}
			IQueue<T> IQueue<T>.Reverse() { 
				return this.Reverse();
			}

			IStack<T> IStack<T>.Push( T item ) {
				return this.EnqueueRight( item );
			}
			IStack<T> IStack<T>.Pop() {
				return this.DequeueRight();
			}
			T IStack<T>.Peek() {
				return this.PeekRight();
			}
			IStack<T> IStack<T>.Duplicate() {
				return this.EnqueueRight( this.PeekRight() );
			}
			IStack<T> IStack<T>.Copy( System.Int32 count ) {
				if ( count < 0 ) {
					throw new System.ArgumentOutOfRangeException( "count" );
				} else if ( 0 == count ) {
					return this;
				} else if ( 1 == count ) {
					return ( (IStack<T>)this ).Duplicate();
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
			IStack<T> IStack<T>.Exchange() {
				if ( this.Count < 2 ) {
					throw new System.InvalidOperationException();
				}
				IDeque<T> probe = this;
				T a = probe.PeekRight();
				probe = probe.DequeueRight();
				T b = probe.PeekRight();
				return probe.DequeueRight().EnqueueRight( a ).EnqueueRight( b );
			}
			IStack<T> IStack<T>.Rotate( System.Int32 shift ) {
				return ( (IStack<T>)this ).Rotate( this.Count, shift );
			}
			IStack<T> IStack<T>.Rotate( System.Int32 count, System.Int32 shift ) {
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
				} else {
					return StackRotate( this, count, shift );
				}
			}
			IStack<T> IStack<T>.Reverse() { 
				return this.Reverse();
			}

			public IDeque<T> Reverse() { 
				return new Double( myRight.Reverse(), myLeft.Reverse() );
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return this.GetEnumerator();
			}
			public System.Collections.Generic.IEnumerator<T> GetEnumerator() { 
				foreach ( T t in myLeft ) { 
					yield return t;
				}
				foreach ( T t in myRight ) { 
					yield return t;
				}
			}
			System.Collections.Generic.IEnumerator<T> IStack<T>.GetEnumerator() { 
				using ( System.Collections.Generic.IEnumerator<T> e = myRight.GetEnumeratorReversed() ) { 
					while ( e.MoveNext() ) { 
						yield return e.Current;
					}
				}
				using ( System.Collections.Generic.IEnumerator<T> e = myLeft.GetEnumeratorReversed() ) { 
					while ( e.MoveNext() ) { 
						yield return e.Current;
					}
				}
			}

			public sealed override System.Int32 GetHashCode() {
				return myHashCode;
			}
		}

		[System.Serializable]
		[System.Diagnostics.DebuggerDisplay( "Count = {Count}" )]
		private sealed class Triple : IDeque<T> { 
			private static readonly System.Int32 theHashCode;
			private readonly System.Int32 myHashCode;
			private readonly MiniDeque myLeft;
			private readonly MiniDeque myMiddle;
			private readonly MiniDeque myRight;
			private readonly System.Int32 myCount;

			static Triple() { 
				theHashCode = System.Reflection.Assembly.GetExecutingAssembly().GetType().AssemblyQualifiedName.GetHashCode();
				unchecked { 
					theHashCode += typeof( T ).AssemblyQualifiedName.GetHashCode();
				}
			}
			private Triple() : base() { 
				myHashCode = theHashCode;
			}
			public Triple( MiniDeque left, MiniDeque middle, MiniDeque right ) : this() { 
				if ( null == left ) { 
					throw new System.ArgumentNullException( "left" );
				} else if ( null == middle ) { 
					throw new System.ArgumentNullException( "middle" );
				} else if ( null == right ) { 
					throw new System.ArgumentNullException( "right" );
				} else { 
					myLeft = left;
					myMiddle = middle;
					myRight = right;
					unchecked { 
						myHashCode += myLeft.GetHashCode(); 
						myHashCode += myMiddle.GetHashCode();
						myHashCode += myRight.GetHashCode();
					}
					myCount = myLeft.Count + myMiddle.Count + myRight.Count;
				}
			}

			public static IDeque<T> Empty { 
				get {
					return Deque<T>.Empty;
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

			public T PeekLeft() { 
				return myLeft.PeekLeft();
			}
			public T PeekRight() { 
				return myRight.PeekRight();
			}

			public IDeque<T> EnqueueLeft( T item ) { 
				if ( myLeft.IsFull ) { 
					if ( myMiddle.IsFull ) { 
						if ( myRight.IsFull ) { 
							return new Deque<T>( 
								new Mini1( item ),
								Deque<MiniDeque>.Empty.EnqueueLeft( myLeft ).EnqueueRight( myMiddle ), 
								myRight, 
								myCount + 1 
							);
						} else { 
							T lRem = myLeft.PeekRight();
							T mRem = myMiddle.PeekRight();
							return new Triple( 
								myLeft.DequeueRight().EnqueueLeft( item ), 
								myMiddle.DequeueRight().EnqueueLeft( lRem ), 
								myRight.EnqueueLeft( mRem ) 
							);
						}
					} else { 
						T lRem = myLeft.PeekRight();
						return new Triple( 
							myLeft.DequeueRight().EnqueueLeft( item ), 
							myMiddle.EnqueueLeft( lRem ), 
							myRight 
						);
					}
				} else { 
					return new Triple( 
						myLeft.EnqueueLeft( item ), 
						myMiddle, 
						myRight 
					);
				}
			}
			public IDeque<T> EnqueueRight( T item ) { 
				if ( myRight.IsFull ) { 
					if ( myMiddle.IsFull ) { 
						if ( myLeft.IsFull ) { 
							return new Deque<T>( 
								myLeft,
								Deque<MiniDeque>.Empty.EnqueueLeft( myMiddle ).EnqueueRight( myRight ), 
								new Mini1( item ), 
								myCount + 1 
							);
						} else { 
							T mRem = myMiddle.PeekLeft();
							T rRem = myRight.PeekLeft();
							return new Triple( 
								myLeft.EnqueueRight( mRem ), 
								myMiddle.DequeueLeft().EnqueueRight( rRem ), 
								myRight.DequeueLeft().EnqueueRight( item ) 
							);
						}
					} else { 
						T rRem = myRight.PeekLeft();
						return new Triple( 
							myLeft, 
							myMiddle.EnqueueRight( rRem ), 
							myRight.DequeueLeft().EnqueueRight( item ) 
						);
					}
				} else { 
					return new Triple( 
						myLeft, 
						myMiddle, 
						myRight.EnqueueRight( item ) 
					);
				}
			}
			public IDeque<T> DequeueLeft() { 
				if ( 1 == myLeft.Count ) { 
					return new Double( myMiddle, myRight );
				} else { 
					return new Triple( myLeft.DequeueLeft(), myMiddle, myRight );
				}
			}
			public IDeque<T> DequeueRight() { 
				if ( 1 == myRight.Count ) { 
					return new Double( myLeft, myMiddle );
				} else { 
					return new Triple( myLeft, myMiddle, myRight.DequeueRight() );
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

			IQueue<T> IQueue<T>.Dequeue() {
				return this.DequeueLeft();
			}
			T IQueue<T>.Peek() {
				return this.PeekLeft();
			}
			IQueue<T> IQueue<T>.Duplicate() {
				return this.EnqueueLeft( this.PeekLeft() );
			}
			IQueue<T> IQueue<T>.Copy( System.Int32 count ) {
				if ( count < 0 ) {
					throw new System.ArgumentOutOfRangeException( "count" );
				} else if ( 0 == count ) {
					return this;
				} else if ( 1 == count ) {
					return ( (IQueue<T>)this ).Duplicate();
				} else if ( this.Count < count ) {
					throw new System.ArgumentException( "count is greater than number of elements in stack reference.", "count" );
				} else {
					T[] dupes = new T[ count ];
					IQueue<T> probe = this;
					System.Int32 i = count;
					while ( 0 < i-- ) {
						dupes[ i ] = probe.Peek();
						probe = probe.Dequeue();
					}
					IQueue<T> output = this;
					for ( System.Int32 j = 0; j < count; j++ ) {
						output = output.Enqueue( dupes[ j ] );
					}
					return output;
				}
			}
			IQueue<T> IQueue<T>.Rotate( System.Int32 shift ) {
				return ( (IQueue<T>)this ).Rotate( this.Count, shift );
			}
			IQueue<T> IQueue<T>.Rotate( System.Int32 count, System.Int32 shift ) {
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
				} else {
					return QueueRotate( this, count, shift );
				}
			}
			IQueue<T> IQueue<T>.Enqueue( T item ) {
				return this.EnqueueRight( item );
			}
			IQueue<T> IQueue<T>.Exchange() {
				if ( myCount < 2 ) {
					throw new System.InvalidOperationException();
				}
				IDeque<T> output = this;
				T a = output.PeekLeft();
				output = output.DequeueLeft();
				T b = output.PeekLeft();
				return output.DequeueLeft().EnqueueLeft( a ).EnqueueLeft( b );
			}
			IQueue<T> IQueue<T>.Reverse() { 
				return this.Reverse();
			}

			IStack<T> IStack<T>.Push( T item ) {
				return this.EnqueueRight( item );
			}
			IStack<T> IStack<T>.Pop() {
				return this.DequeueRight();
			}
			T IStack<T>.Peek() {
				return this.PeekRight();
			}
			IStack<T> IStack<T>.Duplicate() {
				return this.EnqueueRight( this.PeekRight() );
			}
			IStack<T> IStack<T>.Copy( System.Int32 count ) {
				if ( count < 0 ) {
					throw new System.ArgumentOutOfRangeException( "count" );
				} else if ( 0 == count ) {
					return this;
				} else if ( 1 == count ) {
					return ( (IStack<T>)this ).Duplicate();
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
			IStack<T> IStack<T>.Exchange() {
				if ( this.Count < 2 ) {
					throw new System.InvalidOperationException();
				}
				IDeque<T> probe = this;
				T a = probe.PeekRight();
				probe = probe.DequeueRight();
				T b = probe.PeekRight();
				return probe.DequeueRight().EnqueueRight( a ).EnqueueRight( b );
			}
			IStack<T> IStack<T>.Rotate( System.Int32 shift ) {
				return ( (IStack<T>)this ).Rotate( this.Count, shift );
			}
			IStack<T> IStack<T>.Rotate( System.Int32 count, System.Int32 shift ) {
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
				} else {
					return StackRotate( this, count, shift );
				}
			}
			IStack<T> IStack<T>.Reverse() { 
				return this.Reverse();
			}

			public IDeque<T> Reverse() { 
				return new Triple( myRight.Reverse(), myMiddle.Reverse(), myLeft.Reverse() );
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return this.GetEnumerator();
			}
			public System.Collections.Generic.IEnumerator<T> GetEnumerator() {
				foreach ( T t in myLeft ) { 
					yield return t;
				}
				foreach ( T t in myMiddle ) { 
					yield return t;
				}
				foreach ( T t in myRight ) { 
					yield return t;
				}
			}
			System.Collections.Generic.IEnumerator<T> IStack<T>.GetEnumerator() {
				using ( System.Collections.Generic.IEnumerator<T> e = myRight.GetEnumeratorReversed() ) { 
					while ( e.MoveNext() ) { 
						yield return e.Current;
					}
				}
				using ( System.Collections.Generic.IEnumerator<T> e = myMiddle.GetEnumeratorReversed() ) { 
					while ( e.MoveNext() ) { 
						yield return e.Current;
					}
				}
				using ( System.Collections.Generic.IEnumerator<T> e = myLeft.GetEnumeratorReversed() ) { 
					while ( e.MoveNext() ) { 
						yield return e.Current;
					}
				}
			}

			public sealed override System.Int32 GetHashCode() {
				return myHashCode;
			}
		}
		#endregion internal classes


		#region fields
		private static System.Int32 theHashCode;
		private static readonly IDeque<T> theEmpty;

		private readonly MiniDeque myLeft;
		private readonly IDeque<MiniDeque> myMiddle;
		private readonly MiniDeque myRight;
		private readonly System.Int32 myCount;
		private readonly System.Int32 myHashCode;
		#endregion fields


		#region .ctor
		static Deque() { 
			theHashCode = System.Reflection.Assembly.GetExecutingAssembly().GetType().AssemblyQualifiedName.GetHashCode();
			unchecked { 
				theHashCode += typeof( T ).AssemblyQualifiedName.GetHashCode();
			}
			theEmpty = new EmptyDeque();
		}

		private Deque() : base() { 
			myHashCode = theHashCode;
		}
		private Deque( MiniDeque left, IDeque<MiniDeque> middle, MiniDeque right, System.Int32 count ) : this() { 
			if ( count <= 0 ) { 
				throw new System.ArgumentOutOfRangeException( "count" );
			} else if ( null == left ) {
				throw new System.ArgumentNullException( "left" );
			} else if ( null == middle ) {
				throw new System.ArgumentNullException( "middle" );
			} else if ( null == right ) {
				throw new System.ArgumentNullException( "right" );
			}
			myLeft = left;
			myMiddle = middle;
			myRight = right;
			myCount = count;
			unchecked { 
				myHashCode += myLeft.GetHashCode();
				myHashCode += myMiddle.GetHashCode();
				myHashCode += myRight.GetHashCode();
			}
		}
		#endregion .ctor


		#region properties
		public static IDeque<T> Empty { 
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
		public T PeekLeft() { 
			return myLeft.PeekLeft();
		}
		public T PeekRight() { 
			return myRight.PeekRight();
		}

		public IDeque<T> EnqueueLeft( T item ) { 
			if ( myLeft.IsFull ) { 
				return new Deque<T>( 
					new Mini1( item ), 
					myMiddle.EnqueueLeft( myLeft ), 
					myRight, 
					myCount + 1 
				);
			} else { 
				return new Deque<T>( 
					myLeft.EnqueueLeft( item ), 
					myMiddle, 
					myRight, 
					myCount + 1 
				);
			}
		}
		public IDeque<T> EnqueueRight( T item ) { 
			if ( myRight.IsFull ) { 
				return new Deque<T>( 
					myLeft, 
					myMiddle.EnqueueRight( myRight ), 
					new Mini1( item ), 
					myCount + 1 
				);
			} else { 
				return new Deque<T>( 
					myLeft, 
					myMiddle, 
					myRight.EnqueueRight( item ), 
					myCount + 1 
				);
			}
		}
		public IDeque<T> DequeueLeft() { 
			MiniDeque left = myLeft;
			IDeque<MiniDeque> middle = myMiddle;
			MiniDeque right = myRight;
			if ( 1 == left.Count ) { 
				left = middle.PeekLeft();
				middle = middle.DequeueLeft();
			} else { 
				left = left.DequeueLeft();
			}
			if ( middle.IsEmpty ) { 
				return new Double( left, right );
			} else if ( 1 == middle.Count ) { 
				return new Triple( left, middle.PeekLeft(), right );
			} else { 
				return new Deque<T>( left, middle, right, myCount - 1 );
			}
		}
		public IDeque<T> DequeueRight() { 
			MiniDeque right = myRight;
			IDeque<MiniDeque> middle = myMiddle;
			MiniDeque left = myLeft;
			if ( 1 == right.Count ) { 
				right = middle.PeekRight();
				middle = middle.DequeueRight();
			} else { 
				right = right.DequeueRight();
			}
			if ( middle.IsEmpty ) { 
				return new Double( left, right );
			} else if ( 1 == middle.Count ) { 
				return new Triple( left, middle.PeekLeft(), right );
			} else { 
				return new Deque<T>( left, middle, right, myCount - 1 );
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

		IQueue<T> IQueue<T>.Dequeue() {
			return this.DequeueLeft();
		}
		T IQueue<T>.Peek() {
			return this.PeekLeft();
		}
		IQueue<T> IQueue<T>.Duplicate() {
			return this.EnqueueLeft( this.PeekLeft() );
		}
		IQueue<T> IQueue<T>.Copy( System.Int32 count ) {
			if ( count < 0 ) {
				throw new System.ArgumentOutOfRangeException( "count" );
			} else if ( 0 == count ) {
				return this;
			} else if ( 1 == count ) {
				return ( (IQueue<T>)this ).Duplicate();
			} else if ( this.Count < count ) {
				throw new System.ArgumentException( "count is greater than number of elements in stack reference.", "count" );
			} else {
				T[] dupes = new T[ count ];
				IQueue<T> probe = this;
				System.Int32 i = count;
				while ( 0 < i-- ) {
					dupes[ i ] = probe.Peek();
					probe = probe.Dequeue();
				}
				IQueue<T> output = this;
				for ( System.Int32 j = 0; j < count; j++ ) {
					output = output.Enqueue( dupes[ j ] );
				}
				return output;
			}
		}
		IQueue<T> IQueue<T>.Rotate( System.Int32 shift ) {
			return ( (IQueue<T>)this ).Rotate( this.Count, shift );
		}
		IQueue<T> IQueue<T>.Rotate( System.Int32 count, System.Int32 shift ) {
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
			} else {
				return QueueRotate( this, count, shift );
			}
		}
		IQueue<T> IQueue<T>.Enqueue( T item ) {
			return this.EnqueueRight( item );
		}
		IQueue<T> IQueue<T>.Exchange() {
			if ( myCount < 2 ) {
				throw new System.InvalidOperationException();
			}
			IDeque<T> output = this;
			T a = output.PeekLeft();
			output = output.DequeueLeft();
			T b = output.PeekLeft();
			return output.DequeueLeft().EnqueueLeft( a ).EnqueueLeft( b );
		}
		IQueue<T> IQueue<T>.Reverse() { 
			return this.Reverse();
		}

		IStack<T> IStack<T>.Push( T item ) {
			return this.EnqueueRight( item );
		}
		IStack<T> IStack<T>.Pop() {
			return this.DequeueRight();
		}
		T IStack<T>.Peek() {
			return this.PeekRight();
		}
		IStack<T> IStack<T>.Duplicate() {
			return this.EnqueueRight( this.PeekRight() );
		}
		IStack<T> IStack<T>.Copy( System.Int32 count ) {
			if ( count < 0 ) {
				throw new System.ArgumentOutOfRangeException( "count" );
			} else if ( 0 == count ) {
				return this;
			} else if ( 1 == count ) {
				return ( (IStack<T>)this ).Duplicate();
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
		IStack<T> IStack<T>.Exchange() {
			if ( this.Count < 2 ) {
				throw new System.InvalidOperationException();
			}
			IDeque<T> probe = this;
			T a = probe.PeekRight();
			probe = probe.DequeueRight();
			T b = probe.PeekRight();
			return probe.DequeueRight().EnqueueRight( a ).EnqueueRight( b );
		}
		IStack<T> IStack<T>.Rotate( System.Int32 shift ) {
			return ( (IStack<T>)this ).Rotate( this.Count, shift );
		}
		IStack<T> IStack<T>.Rotate( System.Int32 count, System.Int32 shift ) {
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
			} else {
				return StackRotate( this, count, shift );
			}
		}
		IStack<T> IStack<T>.Reverse() { 
			return this.Reverse();
		}

		public IDeque<T> Reverse() { 
			IDeque<MiniDeque> middle = Deque<MiniDeque>.Empty;
			IDeque<MiniDeque> source = myMiddle;
			while ( !source.IsEmpty ) { 
				middle = middle.EnqueueRight( source.PeekRight().Reverse() );
				source = source.DequeueRight();
			}

			return new Deque<T>( myRight.Reverse(), middle, myLeft.Reverse(), myCount );
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { 
			return this.GetEnumerator();
		}
		public System.Collections.Generic.IEnumerator<T> GetEnumerator() { 
			foreach ( T t in myLeft ) { 
				yield return t;
			}
			IDeque<MiniDeque> middle = myMiddle;
			while ( !middle.IsEmpty ) { 
				foreach ( T t in middle.PeekLeft() ) { 
					yield return t;
				}
				middle = middle.DequeueLeft();
			}
			foreach ( T t in myRight ) { 
				yield return t;
			}
		}
		System.Collections.Generic.IEnumerator<T> IStack<T>.GetEnumerator() { 
			using ( System.Collections.Generic.IEnumerator<T> e = myRight.GetEnumeratorReversed() ) { 
				while ( e.MoveNext() ) { 
					yield return e.Current;
				}
			}
			foreach ( MiniDeque m in myMiddle ) { 
				using ( System.Collections.Generic.IEnumerator<T> e = m.GetEnumeratorReversed() ) { 
					while ( e.MoveNext() ) { 
						yield return e.Current;
					}
				}
			}
			using ( System.Collections.Generic.IEnumerator<T> e = myLeft.GetEnumeratorReversed() ) { 
				while ( e.MoveNext() ) { 
					yield return e.Current;
				}
			}
		}


		public sealed override System.Int32 GetHashCode() { 
			return myHashCode;
		}
		#endregion methods


		#region static methods
		private static IStack<T> StackRotate( IDeque<T> deque, System.Int32 count, System.Int32 shift ) { 
			if ( null == deque ) { 
				throw new System.ArgumentNullException( "deque" );
			} else if ( count < 1 ) { 
				throw new System.ArgumentOutOfRangeException( "count" );
			} else if ( deque.Count < count ) { 
				throw new System.ArgumentException( "count is greater than number of elements in the specified deque.", "count" );
			} else if ( 
				( 0 == shift ) 
				|| ( 1 == count ) 
				|| ( 0 == ( shift % count ) ) 
			) { 
				return deque;
			}

			IDeque<T> source = deque;
			IStack<T> hold = Stack<T>.Empty;
			for ( System.Int32 i = 0; i < count; i++ ) { 
				hold = hold.Push( source.PeekRight() );
				source = source.DequeueRight();
			}
			hold = hold.Rotate( count, -shift );
			while ( !hold.IsEmpty ) { 
				source = source.EnqueueRight( hold.Peek() );
				hold = hold.Pop();
			}
			return source;
		}
		private static IQueue<T> QueueRotate( IDeque<T> deque, System.Int32 count, System.Int32 shift ) { 
			if ( null == deque ) { 
				throw new System.ArgumentNullException( "deque" );
			} else if ( count < 1 ) { 
				throw new System.ArgumentOutOfRangeException( "count" );
			} else if ( deque.Count < count ) { 
				throw new System.ArgumentException( "count is greater than number of elements in the specified deque.", "count" );
			} else if ( 
				( 0 == shift ) 
				|| ( 1 == count ) 
				|| ( 0 == ( shift % count ) ) 
			) { 
				return deque;
			}

			IStack<T> hold = Stack<T>.Empty;
			for ( System.Int32 i = 0; i < count; i++ ) { 
				hold = hold.Push( deque.PeekLeft() );
				deque = deque.DequeueLeft();
			}
			hold = hold.Rotate( count, shift );
			while ( !hold.IsEmpty ) { 
				deque = deque.EnqueueLeft( hold.Peek() );
				hold = hold.Pop();
			}

			return deque;
		}
		#endregion static methods

	}

}
