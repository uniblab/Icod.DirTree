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

	public interface IQueue<T> : System.Collections.Generic.ICollection<T> { 
		System.Boolean IsEmpty { 
			get;
		}

		IQueue<T> Enqueue( T item );
		IQueue<T> Dequeue();
		T Peek();

		IQueue<T> Duplicate();
		IQueue<T> Copy( System.Int32 count );
		IQueue<T> Exchange();
		IQueue<T> Reverse();

		IQueue<T> Rotate( System.Int32 shift );
		IQueue<T> Rotate( System.Int32 count, System.Int32 shift );
		System.Boolean Contains( T item, System.Collections.Generic.IEqualityComparer<T> comparer );

		new System.Collections.Generic.IEnumerator<T> GetEnumerator();

	}

}
