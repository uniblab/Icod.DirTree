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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Icod.Collections.Immutable;
using Icod.CommandFramework.CommandLine;
using Icod.CommandFramework.Diagnostics;

namespace Icod.DirTree {

	public static class Command {

		#region nested types
		private record TreeFrame( string Path, string Indent, bool IsLast, bool IsRoot, int Depth );

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
				return ValueTask.FromResult( default( Icod.Path.PathComponentObservation )! );
			}
		}
		#endregion nested types


		#region fields
		private const string PROGRAM = "dirtree";
		private const string VERSION = "dirtree (Icod.DirTree) 1.0";
		private const string theHelpText = """
Usage: dirtree [OPTION] [PATH]...
Print directory tree.

      -h | --help         display this help and exit
      -v | --version      output version information and exit
      -f | --files        include files in the output tree
	  -H | --hidden       omit hidden files and directories in the output tree
      -d N | --depth=N    limit directory tree traversal to N levels deep
      PATH                the root directory from where to start the tree (default: current directory)
""";
		#endregion fields


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
					"hidden",
					shortName: 'H',
					longNames: new[] { "hidden" }
				),
				new OptionDefinition(
					"depth",
					shortName: 'd',
					longNames: new[] { "depth" },
					valueArity: OptionValueArity.Required
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
					await context.StandardOutput.WriteAsync(
						theHelpText.ReplaceLineEndings( Environment.NewLine ).AsMemory(),
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
					await context.StandardOutput.WriteAsync(
						theHelpText.ReplaceLineEndings( Environment.NewLine ).AsMemory(),
						context.CancellationToken
					).ConfigureAwait( false );
					return 1;
				}
				context.CancellationToken.ThrowIfCancellationRequested();
				System.String directoryPathName = ( 0 == result.Operands.Count )
					? Environment.CurrentDirectory
					: result.Operands[ 0 ]
				;
				// here is where we spit out the text
				int maxDepth = int.MaxValue;
				if ( result.HasOption( "depth" ) && int.TryParse( result.GetLastValue( "depth" ), out int depthValue ) && ( 0 <= depthValue ) ) {
					maxDepth = depthValue;
				}
				foreach ( var entry in RenderDirectoryTree( directoryPathName, result.HasOption( "files" ), !result.HasOption( "hidden" ), maxDepth ) ) {
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

		public static IEnumerable<string> RenderDirectoryTree( string rootPath, bool includeFiles, bool showHidden, int maxDepth ) {
			if ( !Directory.Exists( rootPath ) ) {
				yield break;
			}

			bool ignoreCase = !OperatingSystem.IsLinux();
			var stringComparer = ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

			// Use our local bridge implementation to cleanly initialize the resolver
			var fsProvider = new LocalFileSystemProvider();
			var pathResolver = new Icod.Path.CanonicalPathResolver( fsProvider );

			var visitedPhysicalPaths = new HashSet<string>( stringComparer );
			var stack = Icod.Collections.Immutable.Stack<TreeFrame>.Empty;

			string absoluteRoot = System.IO.Path.GetFullPath( rootPath );
			stack = stack.Push( new TreeFrame( absoluteRoot, string.Empty, IsLast: true, IsRoot: true, Depth: 0 ) );

			while ( !stack.IsEmpty ) {
				TreeFrame current = stack.Peek();
				stack = stack.Pop();

				string path = current.Path;
				bool isDir = Directory.Exists( path );

				if ( current.IsRoot ) {
					yield return $"[D] {System.IO.Path.GetFileName( path )}";
				}
				else {
					string marker = isDir
						? "[D] "
						: "[F] "
					;
					string branch = current.IsLast
						? "└── "
						: "├── "
					;
					yield return $"{current.Indent}{branch}{marker}{System.IO.Path.GetFileName( path )}";
				}

				if ( isDir && ( current.Depth < maxDepth ) ) {
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
						children.AddRange( Directory.GetDirectories( path, "*", SearchOption.TopDirectoryOnly ).Where(
							d => showHidden
								|| 0 == ( new DirectoryInfo( d ).Attributes & FileAttributes.Hidden )
						) );

						if ( includeFiles ) {
							children.AddRange( Directory.GetFiles( path, "*", SearchOption.TopDirectoryOnly ).Where(
								f => showHidden
									|| 0 == ( new FileInfo( f ).Attributes & FileAttributes.Hidden )
							) );
						}
					}
					catch ( UnauthorizedAccessException ) {
						accessDenied = true;
					}
					catch ( IOException ) {
						continue;
					}
					if ( accessDenied ) {
						string childIndent = current.Indent + ( current.IsLast ? "    " : "│   " );
						yield return $"{childIndent}└── [Access Denied]";
						continue;
					}

					children.Sort( stringComparer );

					string nextIndent = current.IsRoot ? string.Empty : current.Indent + ( current.IsLast ? "    " : "│   " );

					for ( int i = children.Count - 1; 0 <= i; i-- ) {
						bool isLastChild = ( i == children.Count - 1 );
						stack = stack.Push( new TreeFrame( children[ i ], nextIndent, isLastChild, IsRoot: false, Depth: current.Depth + 1 ) );
					}
				}
			}
		}

	}

}
