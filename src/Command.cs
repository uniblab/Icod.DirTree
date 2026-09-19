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
Usage: arch [OPTION]...
Print machine architecture.

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
z
				// here is where we spit out the text
				var syntax = Icod.Path.PathSyntaxParser.Parse(
					args[ 0 ],
					Icod.Path.PathPlatformSemantics.Windows
				);
				var components = new System.Collections.Generic.List<System.String>();
				if ( !System.String.IsNullOrEmpty( syntax.RootPath ) ) {
					components.Add( syntax.RootPath );
				}
				components.AddRange( syntax.Components );
				System.Console.Out.WriteLine( syntax.IsAbsolute );
				System.Console.Out.WriteLine( System.String.Empty );
				var results = GlobFiles( components.AsReadOnly(), result.HasOption( "files" ) );
				foreach ( var file in results ) {
					System.Console.Out.WriteLine( file );
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
			// We track both the working directory path and the index of the path token we are evaluating
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

				// Base Case: We have reached the final segment, which is the file pattern (e.g., "TC07*.cs")
				if ( idx == segments.Count - 1 ) {
					string filePattern = segments[ idx ];
					if ( System.IO.Directory.Exists( dir ) ) {
						// Safe execution using standard TopDirectoryOnly to keep disk operations bounded
						string[] matchingFiles = System.IO.Directory.GetFiles( dir, filePattern, System.IO.SearchOption.TopDirectoryOnly );
						foreach ( var matchingFile in matchingFiles ) {
							yield return matchingFile;
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
					// We grab immediate subdirectories and keep them at the SAME segment index so they continue to recurse.
					if ( System.IO.Directory.Exists( dir ) ) {
						string[] subDirs = System.IO.Directory.GetDirectories( dir, "*", System.IO.SearchOption.TopDirectoryOnly );
						foreach ( string sub in subDirs ) {
							frontier = frontier.Enqueue( new SearchState( sub, idx ) );
						}
					}
				}
				// Case 2: A concrete directory name (e.g., "samples") or a localized wildcard segment (e.g., "src*")
				else {
					if ( Directory.Exists( dir ) ) {
						// Filter directories immediately at the OS level using the path token string pattern
						string[] matchingSubs = System.IO.Directory.GetDirectories( dir, currentToken, System.IO.SearchOption.TopDirectoryOnly );
						foreach ( string sub in matchingSubs ) {
							// Progress happily to the next segment constraint
							frontier = frontier.Enqueue( new SearchState( sub, idx + 1 ) );
						}
					}
				}
			}

		}

		private record SearchState( string CurrentDir, int SegmentIndex );

	}

}
