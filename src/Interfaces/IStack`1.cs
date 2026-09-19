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

	public interface IStack<T> : System.Collections.Generic.ICollection<T> { 

		IStack<T> Empty { 
			get;
		}
		System.Boolean IsEmpty { 
			get;
		}

		IStack<T> Push( T item );
		IStack<T> Pop();
		T Peek();

		IStack<T> Duplicate();
		IStack<T> Copy( System.Int32 count );
		IStack<T> Exchange();
		IStack<T> Reverse();

		IStack<T> Rotate( System.Int32 shift );
		IStack<T> Rotate( System.Int32 count, System.Int32 shift );
		System.Boolean Contains( T item, System.Collections.Generic.IEqualityComparer<T> comparer );

		new System.Collections.Generic.IEnumerator<T> GetEnumerator();

	}

}
