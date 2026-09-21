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
	along with this program.If not, see<https://gnu.org>.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Icod.Collections.Immutable;
using Icod.CommandFramework.CommandLine;
using Icod.CommandFramework.Diagnostics;

namespace Icod.DirTree {

	public static class Command {
		private const string PROGRAM = "dirtree";
		private const string VERSION = "dirtree (Icod.DirTree) 1.0";

		/// <summary>
		/// Executes <c>dirtree</c> synchronously with optional standard-stream substitution.
		/// </summary>
		/// <remarks>
		/// This compatibility entry point blocks on the TAP implementation. A <see langword="null"/> text stream selects the corresponding <see cref="Console"/> stream; caller-supplied streams remain caller-owned.
		/// </remarks>
		/// <param name="args">The command-line arguments, excluding the executable name.</param>
		/// <param name="stdin">The text reader to use as standard input, or <see langword="null"/> to use <see cref="Console.In"/>.</param>
		/// <param name="stdout">The text writer to use as standard output, or <see langword="null"/> to use <see cref="Console.Out"/>.</param>
		/// <param name="stderr">The text writer to use as standard error, or <see langword="null"/> to use <see cref="Console.Error"/>.</param>
		/// <returns>The GNU-compatible process exit status: zero for successful command execution and nonzero for a usage or operational failure.</returns>
		public static int Run( string[] args, TextReader? stdin = null, TextWriter? stdout = null, TextWriter? stderr = null ) =>
			RunAsync( args, stdin, stdout, stderr ).GetAwaiter().GetResult();

		/// <summary>
		/// Executes <c>dirtree</c> asynchronously with optional injected standard streams.
		/// </summary>
		/// <remarks>
		/// A <see langword="null"/> text stream selects the corresponding <see cref="Console"/> stream. Caller-supplied streams remain caller-owned.
		/// </remarks>
		/// <param name="args">The command-line arguments, excluding the executable name.</param>
		/// <param name="stdin">The text reader to use as standard input, or <see langword="null"/> to use <see cref="Console.In"/>.</param>
		/// <param name="stdout">The text writer to use as standard output, or <see langword="null"/> to use <see cref="Console.Out"/>.</param>
		/// <param name="stderr">The text writer to use as standard error, or <see langword="null"/> to use <see cref="Console.Error"/>.</param>
		/// <param name="cancellationToken">The token used to cancel parsing, platform queries, and asynchronous I/O.</param>
		/// <returns>The GNU-compatible process exit status: zero for successful command execution and nonzero for a usage or operational failure.</returns>
		public static Task<int> RunAsync(
			string[] args,
			TextReader? stdin = null,
			TextWriter? stdout = null,
			TextWriter? stderr = null,
			CancellationToken cancellationToken = default
		) => RunAsync(
			args ?? Array.Empty<string>(),
			new CommandContext(
				PROGRAM,
				stdin ?? Console.In,
				stdout ?? Console.Out,
				stderr ?? Console.Error,
				cancellationToken: cancellationToken
			)
		);

		/// <summary>
		/// Executes <c>dirtree</c> asynchronously using a complete shared command context.
		/// </summary>
		/// <remarks>
		/// The context carries text and optional binary standard streams, centralized diagnostics, and cancellation. The command does not dispose caller-owned standard streams.
		/// </remarks>
		/// <param name="args">The command-line arguments, excluding the executable name.</param>
		/// <param name="context">The command context that supplies standard streams, diagnostics, and cancellation.</param>
		/// <returns>The GNU-compatible process exit status: zero for successful command execution and nonzero for a usage or operational failure.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
		public static async Task<int> RunAsync( string[] args, CommandContext context ) {
			ArgumentNullException.ThrowIfNull( context );
			var parser = CreateParser(
				new OptionDefinition(
					"help",
					shortName: 'h',
					longNames: new[] { "help" }
				),
				new OptionDefinition(
					"version",
					shortName: 'v',
					longNames: new[] { "version" }
				),
				new OptionDefinition(
					"files",
					shortName: 'f',
					longNames: new[] { "files" }
				)
			);
			try {
				var result = parser.Parse( args );
				if ( await WriteParseErrorsAsync( result, context ).ConfigureAwait( false ) ) {
					return 1;
				}
				if ( result.HasOption( "help" ) ) {
					const string help = """
Usage: dirtree [OPTION]...
Print directory tree.

      --help     display this help and exit
      --version  output version information and exit

""";
					await context.StandardOutput.WriteAsync(
						help.ReplaceLineEndings( Environment.NewLine ).AsMemory(),
						context.CancellationToken
					).ConfigureAwait( false );
					return 0;
				}
				if ( result.HasOption( "version" ) ) {
					await context.StandardOutput.WriteLineAsync(
						VERSION.AsMemory(),
						context.CancellationToken
					).ConfigureAwait( false );
					return 0;
				}
				if ( 1 < result.Operands.Count ) {
					await context.Diagnostics.ErrorAsync(
						$"extra operand '{result.Operands[ 0 ]}'",
						context.CancellationToken
					).ConfigureAwait( false );
					return 1;
				}
				context.CancellationToken.ThrowIfCancellationRequested();

				// here is where we spit out the text
				foreach ( var entry in RenderDirectoryTree( args[ 0 ], result.HasOption( "files" ) ) ) {
					System.Console.Out.WriteLine( entry );
				}

				return 0;
			}
			catch ( OperationCanceledException ) {
				return CommandExitCodes.Canceled;
			}
		}


		private static OptionParser CreateParser( params OptionDefinition[] options ) => new(
			options,
			new OptionParserSettings {
				AllowLongOptionAbbreviations = true,
				Ordering = OptionOrdering.Permute
			}
		);
		private static async Task<bool> WriteParseErrorsAsync( OptionParseResult result, CommandContext context ) {
			if ( result.IsSuccess ) {
				return false;
			}
			foreach ( var error in result.Errors ) {
				await context.StandardError.WriteLineAsync(
					OptionDiagnosticFormatter.Format( context.ProgramName, error ).AsMemory(),
					context.CancellationToken
				).ConfigureAwait( false );
			}
			return true;
		}

		public static IEnumerable<string> GlobFiles( IReadOnlyList<string> segments, System.Boolean includeFiles ) {

			if ( segments == null || segments.Count == 0 ) {
				yield break;
			}

			// Initialize your immutable queue structure using its Empty factory pattern
			var frontier = Icod.Collections.Immutable.Queue<SearchState>.GetEmpty();

			// Seed the queue with the root path segment (e.g., "C:\") at segment index 1
			frontier = frontier.Enqueue( new SearchState( segments[ 0 ], 1 ) );

			// Loop until the immutable queue reports itself empty via your IIsEmpty interface contract
			while ( !frontier.IsEmpty ) {
				// Read the front element
				SearchState state = frontier.Peek();

				// Advance the frontier by assigning the returned modified queue instance
				frontier = frontier.Dequeue();

				string dir = state.CurrentDir;
				int idx = state.SegmentIndex;

				// Base Case: We have reached the final path segment
				if ( idx == segments.Count - 1 ) {
					string lastPattern = segments[ idx ];
					if ( System.IO.Directory.Exists( dir ) ) {
						// Directories are always reported if they match the final structural constraint
						// Check if the final segment pattern matches any subdirectories here
						string[] matchingDirs = System.IO.Directory.GetDirectories( dir, lastPattern, System.IO.SearchOption.TopDirectoryOnly );
						foreach ( var matchingDir in matchingDirs ) {
							yield return matchingDir;
						}

						// Files are only reported if the includeFiles flag is explicitly true
						if ( includeFiles ) {
							string[] matchingFiles = System.IO.Directory.GetFiles( dir, lastPattern, System.IO.SearchOption.TopDirectoryOnly );
							foreach ( var matchingFile in matchingFiles ) {
								yield return matchingFile;
							}
						}
					}
					continue;
				}

				string currentToken = segments[ idx ];

				// Case 1: The Wildcard Recurse Token '**'
				if ( currentToken == "**" ) {
					// 1a. '**' can match ZERO directories. 
					// We test this scenario by immediately matching the NEXT segment against the current directory.
					frontier = frontier.Enqueue( new SearchState( dir, idx + 1 ) );

					// 1b. '**' can match ONE OR MORE directories.
					// We grab immediate subdirectories, report them, and keep them at the SAME segment index to recurse further.
					if ( System.IO.Directory.Exists( dir ) ) {
						string[] subDirs = System.IO.Directory.GetDirectories( dir, "*", System.IO.SearchOption.TopDirectoryOnly );
						foreach ( string sub in subDirs ) {
							yield return sub; // Directory discovered via traversal is reported
							frontier = frontier.Enqueue( new SearchState( sub, idx ) );
						}
					}
				}
				// Case 2: A concrete directory name (e.g., "samples") or a localized wildcard segment (e.g., "src*")
				else {
					if ( System.IO.Directory.Exists( dir ) ) {
						// Filter directories immediately at the OS level using the path token string pattern
						string[] matchingSubs = System.IO.Directory.GetDirectories( dir, currentToken, System.IO.SearchOption.TopDirectoryOnly );
						foreach ( string sub in matchingSubs ) {
							yield return sub; // Directory discovered via traversal is reported
											  // Progress happily to the next segment constraint
							frontier = frontier.Enqueue( new SearchState( sub, idx + 1 ) );
						}
					}
				}
			}
		}

		private record SearchState( string CurrentDir, int SegmentIndex );

		private record TreeFrame( string Path, string Indent, bool IsLast, bool IsRoot );

		/// <summary>
		/// Explicit implementation of your ICanonicalPathFileSystemProvider interface 
		/// to safely bridge .NET 10 file system capabilities to the CanonicalPathResolver.
		/// </summary>
		private class LocalFileSystemProvider : Icod.Path.ICanonicalPathFileSystemProvider {
			public string GetCanonicalPath( string path ) => System.IO.Path.GetFullPath( path );

			public FileSystemInfo? ResolveLinkTarget( string path, bool returnFinalTarget ) =>
				Directory.ResolveLinkTarget( path, returnFinalTarget );


			public Icod.Path.PathPlatformSemantics Semantics => OperatingSystem.IsWindows()
				? Icod.Path.PathPlatformSemantics.Windows
				: Icod.Path.PathPlatformSemantics.Posix
			;

			public string CurrentDirectory => Environment.CurrentDirectory;

			public ValueTask<Icod.Path.PathComponentObservation> ObserveAsync( string path, CancellationToken cancellationToken ) {
				return ValueTask.FromResult( default( Icod.Path.PathComponentObservation ) );
			}
		}

		public static IEnumerable<string> RenderDirectoryTree( string rootPath, bool includeFiles ) {
			if ( !Directory.Exists( rootPath ) ) {
				yield break;
			}

			bool ignoreCase = !OperatingSystem.IsLinux();
			var stringComparer = ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

			// Use our local bridge implementation to cleanly initialize the resolver
			var fsProvider = new LocalFileSystemProvider();
			var pathResolver = new Icod.Path.CanonicalPathResolver( fsProvider );

			var visitedPhysicalPaths = new HashSet<string>( stringComparer );
			var stack = Icod.Collections.Immutable.Stack<TreeFrame>.GetEmpty();

			string absoluteRoot = System.IO.Path.GetFullPath( rootPath );
			stack = stack.Push( new TreeFrame( absoluteRoot, string.Empty, IsLast: true, IsRoot: true ) );

			while ( !stack.IsEmpty ) {
				TreeFrame current = stack.Peek();
				stack = stack.Pop();

				string path = current.Path;
				bool isDir = Directory.Exists( path );

				if ( current.IsRoot ) {
					yield return $"[D] {System.IO.Path.GetFileName( path )}";
				} else {
					string marker = isDir ? "[D] " : "[F] ";
					string branch = current.IsLast ? "└── " : "├── ";
					yield return $"{current.Indent}{branch}{marker}{System.IO.Path.GetFileName( path )}";
				}

				if ( isDir ) {
					string canonicalPath = fsProvider.GetCanonicalPath( path );

					// Protect against structural cycles by tracking physical path target strings directly
					if ( !visitedPhysicalPaths.Add( canonicalPath ) && !current.IsRoot ) {
						string childIndent = current.Indent + ( current.IsLast ? "    " : "│   " );
						yield return $"{childIndent}└── [Circular Link / Junction Loop Intercepted]";
						continue;
					}

					var children = new List<string>();
					System.Boolean accessDenied = false;
					try {
						children.AddRange( Directory.GetDirectories( path, "*", SearchOption.TopDirectoryOnly ) );

						if ( includeFiles ) {
							children.AddRange( Directory.GetFiles( path, "*", SearchOption.TopDirectoryOnly ) );
						}
					} catch ( UnauthorizedAccessException ) {
						accessDenied = true;
					} catch ( IOException ) {
						continue;
					}
					if ( accessDenied ) {
						string childIndent = current.Indent + ( current.IsLast ? "    " : "│   " );
						yield return $"{childIndent}└── [Access Denied]";
						continue;
					}

					children.Sort( stringComparer );

					string nextIndent = current.IsRoot ? string.Empty : current.Indent + ( current.IsLast ? "    " : "│   " );

					for ( int i = children.Count - 1; i >= 0; i-- ) {
						bool isLastChild = ( i == children.Count - 1 );
						stack = stack.Push( new TreeFrame( children[ i ], nextIndent, isLastChild, IsRoot: false ) );
					}
				}
			}
		}

	}

}
