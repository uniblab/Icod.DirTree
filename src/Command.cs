/*
	Icod.DirTree
	Cross-platform command-line tool to report subdirectory structure as text tree.
	Copyright (C) 2026 Timothy J. Bruce <uniblab@hotmail.com>
*/

using System.Globalization;
using System.Reflection;
using System.Text;
using Icod.CommandFramework.CommandLine;
using Icod.CommandFramework.Diagnostics;
using Icod.Path;

namespace Icod.DirTree;

/// <summary>
/// Implements the <c>dirtree</c> command. Usage: <c>dirtree [OPTION] [PATH]</c>.
/// </summary>
/// <remarks>
/// <para>
/// With no operand, the command renders the current directory. Supply one directory pathname to render another root.
/// Directories are shown by default; use <c>--files</c> to include files, <c>--all</c> to include hidden entries,
/// <c>--depth=N</c> to limit descent, <c>--ascii</c> for portable branches, and <c>--follow-links</c> to traverse directory links.
/// </para>
/// <para>
/// Output and diagnostics are written through the supplied <see cref="CommandContext"/>. The command returns 0 on success,
/// 1 after an operational failure, 2 after invalid command-line usage, and 130 after cancellation.
/// </para>
/// </remarks>
internal static class Command {
	private const string ProgramName = "dirtree";
	private const int OperationalFailure = 1;
	private const int UsageFailure = 2;
	private const string HelpText = """
Usage: dirtree [OPTION] [PATH]
Print a text tree for PATH, or for the current directory when PATH is omitted.

      -h | --help          display this help and exit
      -v | --version       output version information and exit
      -f | --files         include files in the output tree
      -a | --all           include hidden files and directories
      -L | --follow-links  follow directory symbolic links and junctions
           --ascii         use ASCII branch characters
      -d N | --depth=N     limit traversal to N levels below the root
      PATH                 root directory for the tree
""";

	private record TreeEntry(
		string Path,
		string Name,
		bool IsDirectory,
		bool IsLink,
		string? LinkTarget
	);

	private record TreeFrame(
		TreeEntry Entry,
		string Indent,
		bool IsLast,
		bool IsRoot,
		int Depth
	);

	private sealed record TreeOptions(
		bool IncludeFiles,
		bool IncludeHidden,
		bool FollowLinks,
		bool UseAscii,
		int MaximumDepth
	);

	private sealed record TreeRenderResult( bool HadOperationalFailure );

	/// <summary>
	/// Executes <c>dirtree</c> asynchronously with optional injected standard streams.
	/// </summary>
	/// <remarks>
	/// Pass command-line arguments without the executable name. A <see langword="null"/> stream selects the matching
	/// <see cref="Console"/> stream; supplied streams remain caller-owned. For example,
	/// <c>await Command.RunAsync(new[] { "--files", "--depth=2", "." }, stdout: writer)</c>
	/// captures a two-level tree including files.
	/// </remarks>
	/// <param name="args">The command-line arguments, excluding the executable name.</param>
	/// <param name="stdin">Standard input, or <see langword="null"/> to use <see cref="Console.In"/>.</param>
	/// <param name="stdout">Standard output, or <see langword="null"/> to use <see cref="Console.Out"/>.</param>
	/// <param name="stderr">Standard error, or <see langword="null"/> to use <see cref="Console.Error"/>.</param>
	/// <param name="cancellationToken">A token that cancels parsing, traversal, and output.</param>
	/// <returns>A task whose result is 0, 1, 2, or 130 as described in the type remarks.</returns>
	internal static Task<int> RunAsync(
		string[] args,
		TextReader? stdin = null,
		TextWriter? stdout = null,
		TextWriter? stderr = null,
		CancellationToken cancellationToken = default
	) => RunAsync(
		args ?? Array.Empty<string>(),
		new CommandContext(
			ProgramName,
			stdin ?? Console.In,
			stdout ?? Console.Out,
			stderr ?? Console.Error,
			cancellationToken: cancellationToken
		)
	);

	/// <summary>
	/// Executes <c>dirtree</c> asynchronously with a complete shared command context.
	/// </summary>
	/// <remarks>
	/// Use this overload from an executable host that already owns a <see cref="CommandContext"/>. The context supplies
	/// standard streams, diagnostics, and cancellation; this method does not dispose any caller-owned resource.
	/// </remarks>
	/// <param name="args">The command-line arguments, excluding the executable name.</param>
	/// <param name="context">The command context that supplies streams, diagnostics, and cancellation.</param>
	/// <returns>A task whose result is 0, 1, 2, or 130 as described in the type remarks.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
	internal static async Task<int> RunAsync( string[] args, CommandContext context ) {
		ArgumentNullException.ThrowIfNull( context );
		try {
			var parser = CreateParser();
			var result = parser.Parse( args ?? Array.Empty<string>() );
			if ( await WriteParseErrorsAsync( result, context ).ConfigureAwait( false ) ) {
				return UsageFailure;
			}
			if ( result.HasOption( "help" ) ) {
				await WriteUsageAsync( context ).ConfigureAwait( false );
				return 0;
			}
			if ( result.HasOption( "version" ) ) {
				await context.StandardOutput.WriteLineAsync(
					$"{ProgramName} (Icod.DirTree) {GetVersion()}".AsMemory(),
					context.CancellationToken
				).ConfigureAwait( false );
				return 0;
			}
			if ( 1 < result.Operands.Count ) {
				await context.Diagnostics.ErrorAsync(
					$"extra operand '{EscapeText( result.Operands[ 1 ] )}'",
					context.CancellationToken
				).ConfigureAwait( false );
				return UsageFailure;
			}

			var maximumDepth = int.MaxValue;
			if ( result.HasOption( "depth" ) ) {
				var depthText = result.GetLastValue( "depth" );
				if ( !int.TryParse( depthText, NumberStyles.None, CultureInfo.InvariantCulture, out maximumDepth )
					|| 0 > maximumDepth ) {
					await context.Diagnostics.ErrorAsync(
						$"invalid depth '{EscapeText( depthText ?? string.Empty )}': expected a nonnegative integer",
						context.CancellationToken
					).ConfigureAwait( false );
					return UsageFailure;
				}
			}

			context.CancellationToken.ThrowIfCancellationRequested();
			var rootPath = 0 == result.Operands.Count
				? Environment.CurrentDirectory
				: result.Operands[ 0 ];
			var rootValidation = ValidateRoot( rootPath );
			if ( null != rootValidation ) {
				await context.Diagnostics.ErrorAsync(
					rootValidation,
					context.CancellationToken
				).ConfigureAwait( false );
				return OperationalFailure;
			}

			var options = new TreeOptions(
				IncludeFiles: result.HasOption( "files" ),
				IncludeHidden: result.HasOption( "all" ),
				FollowLinks: result.HasOption( "follow-links" ),
				UseAscii: result.HasOption( "ascii" ),
				MaximumDepth: maximumDepth
			);
			var renderResult = await RenderDirectoryTreeAsync(
				rootPath,
				options,
				context
			).ConfigureAwait( false );
			return renderResult.HadOperationalFailure ? OperationalFailure : 0;
		} catch ( OperationCanceledException ) {
			return CommandExitCodes.Canceled;
		}
	}

	private static OptionParser CreateParser() => new(
		new[] {
			new OptionDefinition( "help", shortName: 'h', longNames: new[] { "help" } ),
			new OptionDefinition( "version", shortName: 'v', longNames: new[] { "version" } ),
			new OptionDefinition( "files", shortName: 'f', longNames: new[] { "files" } ),
			new OptionDefinition( "all", shortName: 'a', longNames: new[] { "all" } ),
			new OptionDefinition( "follow-links", shortName: 'L', longNames: new[] { "follow-links" } ),
			new OptionDefinition( "ascii", longNames: new[] { "ascii" } ),
			new OptionDefinition(
				"depth",
				shortName: 'd',
				longNames: new[] { "depth" },
				valueArity: OptionValueArity.Required
			)
		},
		new OptionParserSettings {
			AllowLongOptionAbbreviations = true,
			Ordering = OptionOrdering.Permute
		}
	);

	private static async Task<TreeRenderResult> RenderDirectoryTreeAsync(
		string rootPath,
		TreeOptions options,
		CommandContext context
	) {
		var cancellationToken = context.CancellationToken;
		var absoluteRoot = System.IO.Path.GetFullPath( rootPath );
		var rootEntry = InspectEntry( absoluteRoot );
		var comparer = OperatingSystem.IsLinux()
			? StringComparer.Ordinal
			: StringComparer.OrdinalIgnoreCase;
		var resolver = new CanonicalPathResolver();
		var visitedPhysicalPaths = new HashSet<string>( comparer );
		var stack = new Stack<TreeFrame>();
		stack.Push( new TreeFrame( rootEntry, string.Empty, IsLast: true, IsRoot: true, Depth: 0 ) );
		var hadOperationalFailure = false;

		while ( 0 < stack.Count ) {
			cancellationToken.ThrowIfCancellationRequested();
			var current = stack.Pop();
			await context.StandardOutput.WriteLineAsync(
				FormatEntry( current, options.UseAscii ).AsMemory(),
				cancellationToken
			).ConfigureAwait( false );

			if ( !current.Entry.IsDirectory
				|| current.Depth >= options.MaximumDepth
				|| ( current.Entry.IsLink && !options.FollowLinks ) ) {
				continue;
			}

			if ( options.FollowLinks ) {
				var physical = await resolver.ResolvePhysicalAsync(
					current.Entry.Path,
					new CanonicalPathResolutionOptions { RequireFinalDirectory = true },
					cancellationToken
				).ConfigureAwait( false );
				if ( !physical.Succeeded ) {
					hadOperationalFailure = true;
					await WriteTraversalFailureAsync(
						context,
						current.Entry.Path,
						physical.Failure?.Message ?? "the directory could not be resolved"
					).ConfigureAwait( false );
					continue;
				}
				if ( !visitedPhysicalPaths.Add( physical.Path! ) ) {
					if ( !current.IsRoot ) {
						await WriteMarkerAsync(
							context.StandardOutput,
							ChildIndent( current, options.UseAscii ),
							"[Circular Link / Junction Loop Intercepted]",
							options.UseAscii,
							cancellationToken
						).ConfigureAwait( false );
					}
					continue;
				}
			}

			List<TreeEntry> children;
			try {
				children = EnumerateChildren( current.Entry.Path, options, cancellationToken );
			} catch ( UnauthorizedAccessException exception ) {
				hadOperationalFailure = true;
				await WriteMarkerAsync(
					context.StandardOutput,
					ChildIndent( current, options.UseAscii ),
					"[Access Denied]",
					options.UseAscii,
					cancellationToken
				).ConfigureAwait( false );
				await WriteTraversalFailureAsync( context, current.Entry.Path, exception.Message ).ConfigureAwait( false );
				continue;
			} catch ( IOException exception ) {
				hadOperationalFailure = true;
				await WriteMarkerAsync(
					context.StandardOutput,
					ChildIndent( current, options.UseAscii ),
					"[I/O Error]",
					options.UseAscii,
					cancellationToken
				).ConfigureAwait( false );
				await WriteTraversalFailureAsync( context, current.Entry.Path, exception.Message ).ConfigureAwait( false );
				continue;
			}

			children.Sort( ( left, right ) => comparer.Compare( left.Name, right.Name ) );
			var nextIndent = current.IsRoot
				? string.Empty
				: ChildIndent( current, options.UseAscii );
			for ( var index = children.Count - 1; 0 <= index; index-- ) {
				cancellationToken.ThrowIfCancellationRequested();
				stack.Push( new TreeFrame(
					children[ index ],
					nextIndent,
					IsLast: index == children.Count - 1,
					IsRoot: false,
					Depth: current.Depth + 1
				) );
			}
		}

		return new TreeRenderResult( hadOperationalFailure );
	}

	private static List<TreeEntry> EnumerateChildren(
		string path,
		TreeOptions options,
		CancellationToken cancellationToken
	) {
		var children = new List<TreeEntry>();
		foreach ( var childPath in Directory.EnumerateFileSystemEntries( path ) ) {
			cancellationToken.ThrowIfCancellationRequested();
			var entry = InspectEntry( childPath );
			if ( !options.IncludeHidden && IsHidden( entry ) ) {
				continue;
			}
			if ( entry.IsDirectory || options.IncludeFiles ) {
				children.Add( entry );
			}
		}
		return children;
	}

	private static TreeEntry InspectEntry( string path ) {
		var attributes = File.GetAttributes( path );
		var isLink = 0 != ( attributes & FileAttributes.ReparsePoint );
		var isDirectory = 0 != ( attributes & FileAttributes.Directory )
			|| ( isLink && Directory.Exists( path ) );
		string? linkTarget = null;
		if ( isLink ) {
			var information = isDirectory
				? new DirectoryInfo( path ) as FileSystemInfo
				: new FileInfo( path );
			linkTarget = information.LinkTarget;
		}
		return new TreeEntry(
			path,
			DisplayName( path ),
			isDirectory,
			isLink,
			linkTarget
		);
	}

	private static bool IsHidden( TreeEntry entry ) {
		var name = entry.Name;
		return ( 1 < name.Length && '.' == name[ 0 ] && "." != name && ".." != name )
			|| 0 != ( File.GetAttributes( entry.Path ) & FileAttributes.Hidden );
	}

	private static string FormatEntry( TreeFrame frame, bool useAscii ) {
		var marker = frame.Entry.IsLink
			? "[L] "
			: frame.Entry.IsDirectory ? "[D] " : "[F] ";
		var branch = frame.IsRoot
			? string.Empty
			: frame.IsLast ? ( useAscii ? "`-- " : "└── " ) : ( useAscii ? "|-- " : "├── " );
		var target = frame.Entry.IsLink && !string.IsNullOrEmpty( frame.Entry.LinkTarget )
			? $" -> {EscapeText( frame.Entry.LinkTarget )}"
			: string.Empty;
		return $"{frame.Indent}{branch}{marker}{EscapeText( frame.Entry.Name )}{target}";
	}

	private static string ChildIndent( TreeFrame frame, bool useAscii ) =>
		frame.Indent + ( frame.IsLast ? "    " : useAscii ? "|   " : "│   " );

	private static async Task WriteMarkerAsync(
		TextWriter output,
		string indent,
		string marker,
		bool useAscii,
		CancellationToken cancellationToken
	) {
		var branch = useAscii ? "`-- " : "└── ";
		await output.WriteLineAsync(
			$"{indent}{branch}{marker}".AsMemory(),
			cancellationToken
		).ConfigureAwait( false );
	}

	private static ValueTask WriteTraversalFailureAsync(
		CommandContext context,
		string path,
		string message
	) => context.Diagnostics.ErrorAsync(
		$"cannot read '{EscapeText( path )}': {EscapeText( message )}",
		context.CancellationToken
	);

	private static string? ValidateRoot( string rootPath ) {
		try {
			if ( !File.Exists( rootPath ) && !Directory.Exists( rootPath ) ) {
				return $"path '{EscapeText( rootPath )}' does not exist";
			}
			var attributes = File.GetAttributes( rootPath );
			if ( 0 == ( attributes & FileAttributes.Directory ) ) {
				return $"path '{EscapeText( rootPath )}' is not a directory";
			}
			return null;
		} catch ( UnauthorizedAccessException exception ) {
			return $"cannot access '{EscapeText( rootPath )}': {EscapeText( exception.Message )}";
		} catch ( IOException exception ) {
			return $"cannot inspect '{EscapeText( rootPath )}': {EscapeText( exception.Message )}";
		} catch ( ArgumentException exception ) {
			return $"invalid path '{EscapeText( rootPath )}': {EscapeText( exception.Message )}";
		}
	}

	private static string DisplayName( string path ) {
		var trimmedPath = System.IO.Path.TrimEndingDirectorySeparator( path );
		var name = System.IO.Path.GetFileName( trimmedPath );
		if ( !string.IsNullOrEmpty( name ) ) {
			return name;
		}
		return System.IO.Path.GetPathRoot( path ) ?? path;
	}

	private static string EscapeText( string value ) {
		var builder = new StringBuilder( value.Length );
		foreach ( var character in value ) {
			switch ( character ) {
				case '\n':
					builder.Append( "\\n" );
					break;
				case '\r':
					builder.Append( "\\r" );
					break;
				case '\t':
					builder.Append( "\\t" );
					break;
				case '\u001b':
					builder.Append( "\\x1B" );
					break;
				default:
					if ( char.IsControl( character ) ) {
						builder.Append( "\\u" );
						builder.Append( ( (int)character ).ToString( "X4", CultureInfo.InvariantCulture ) );
					} else {
						builder.Append( character );
					}
					break;
			}
		}
		return builder.ToString();
	}

	private static string GetVersion() {
		var version = typeof( Command ).Assembly
			.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
			.InformationalVersion;
		if ( string.IsNullOrWhiteSpace( version ) ) {
			return typeof( Command ).Assembly.GetName().Version?.ToString( 3 ) ?? "unknown";
		}
		var metadataIndex = version.IndexOf( '+', StringComparison.Ordinal );
		return 0 <= metadataIndex ? version[ ..metadataIndex ] : version;
	}

	private static async Task<bool> WriteParseErrorsAsync(
		OptionParseResult result,
		CommandContext context
	) {
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

	private static Task WriteUsageAsync( CommandContext context ) =>
		context.StandardOutput.WriteAsync(
			HelpText.ReplaceLineEndings( Environment.NewLine ).AsMemory(),
			context.CancellationToken
		);
}
