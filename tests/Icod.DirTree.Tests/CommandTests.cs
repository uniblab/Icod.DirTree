namespace Icod.DirTree.Tests;

using Xunit;

/// <summary>Verifies the <c>dirtree</c> command boundary and filesystem behavior.</summary>
public sealed class CommandTests {
	/// <summary>Verifies that version output comes from the 1.0.0 assembly metadata.</summary>
	[Fact]
	public async Task ReportsExactVersion() {
		var output = new StringWriter();

		var exitCode = await Command.RunAsync( new[] { "--version" }, stdout: output );

		Assert.Equal( 0, exitCode );
		Assert.Equal( $"dirtree (Icod.DirTree) 1.0.0{Environment.NewLine}", output.ToString() );
	}

	/// <summary>Verifies that invalid depth values are usage errors instead of unlimited traversal.</summary>
	[Theory]
	[InlineData( "-1" )]
	[InlineData( "not-a-number" )]
	public async Task RejectsInvalidDepth( string value ) {
		var output = new StringWriter();
		var error = new StringWriter();

		var exitCode = await Command.RunAsync(
			new[] { "--depth", value },
			stdout: output,
			stderr: error
		);

		Assert.Equal( 2, exitCode );
		Assert.Contains( "invalid depth", error.ToString(), StringComparison.OrdinalIgnoreCase );
	}

	/// <summary>Verifies that an unknown option is reported as a usage error.</summary>
	[Fact]
	public async Task RejectsUnknownOption() {
		var error = new StringWriter();

		var exitCode = await Command.RunAsync( new[] { "--unknown" }, stderr: error );

		Assert.Equal( 2, exitCode );
		Assert.NotEqual( string.Empty, error.ToString() );
	}

	/// <summary>Verifies that extra operands are reported as a usage error.</summary>
	[Fact]
	public async Task RejectsExtraOperand() {
		var error = new StringWriter();

		var exitCode = await Command.RunAsync( new[] { ".", "." }, stderr: error );

		Assert.Equal( 2, exitCode );
		Assert.Contains( "extra operand", error.ToString(), StringComparison.Ordinal );
	}

	/// <summary>Verifies that a missing root is an operational failure with a diagnostic.</summary>
	[Fact]
	public async Task RejectsMissingRoot() {
		var root = System.IO.Path.Combine( System.IO.Path.GetTempPath(), "icod-dirtree-missing-" + Guid.NewGuid().ToString( "N" ) );
		var output = new StringWriter();
		var error = new StringWriter();

		var exitCode = await Command.RunAsync( new[] { root }, stdout: output, stderr: error );

		Assert.Equal( 1, exitCode );
		Assert.Equal( string.Empty, output.ToString() );
		Assert.Contains( "does not exist", error.ToString(), StringComparison.OrdinalIgnoreCase );
	}

	/// <summary>Verifies that a file operand is rejected because the root must be a directory.</summary>
	[Fact]
	public async Task RejectsFileRoot() {
		using var fixture = new TemporaryDirectory();
		var file = System.IO.Path.Combine( fixture.Path, "file.txt" );
		await File.WriteAllTextAsync( file, "content" );
		var error = new StringWriter();

		var exitCode = await Command.RunAsync( new[] { file }, stderr: error );

		Assert.Equal( 1, exitCode );
		Assert.Contains( "not a directory", error.ToString(), StringComparison.OrdinalIgnoreCase );
	}

	/// <summary>Verifies injected output, sorting, file inclusion, and depth limiting.</summary>
	[Fact]
	public async Task WritesTreeToInjectedOutput() {
		using var fixture = new TemporaryDirectory();
		Directory.CreateDirectory( System.IO.Path.Combine( fixture.Path, "zeta" ) );
		Directory.CreateDirectory( System.IO.Path.Combine( fixture.Path, "alpha" ) );
		Directory.CreateDirectory( System.IO.Path.Combine( fixture.Path, "alpha", "nested" ) );
		await File.WriteAllTextAsync( System.IO.Path.Combine( fixture.Path, "middle.txt" ), "content" );
		var output = new StringWriter();
		var error = new StringWriter();

		var exitCode = await Command.RunAsync(
			new[] { "--files", "--depth=1", fixture.Path },
			stdout: output,
			stderr: error
		);

		Assert.Equal( 0, exitCode );
		Assert.Equal( string.Empty, error.ToString() );
		var lines = ReadLines( output.ToString() );
		Assert.Equal( 4, lines.Length );
		Assert.Equal( $"[D] {System.IO.Path.GetFileName( fixture.Path )}", lines[ 0 ] );
		Assert.Contains( "[D] alpha", lines[ 1 ], StringComparison.Ordinal );
		Assert.Contains( "[F] middle.txt", lines[ 2 ], StringComparison.Ordinal );
		Assert.Contains( "[D] zeta", lines[ 3 ], StringComparison.Ordinal );
		Assert.DoesNotContain( "nested", output.ToString(), StringComparison.Ordinal );
	}

	/// <summary>Verifies depth zero renders the root only.</summary>
	[Fact]
	public async Task DepthZeroRendersRootOnly() {
		using var fixture = new TemporaryDirectory();
		Directory.CreateDirectory( System.IO.Path.Combine( fixture.Path, "child" ) );
		var output = new StringWriter();

		var exitCode = await Command.RunAsync( new[] { "--depth=0", fixture.Path }, stdout: output );

		Assert.Equal( 0, exitCode );
		Assert.Single( ReadLines( output.ToString() ) );
	}

	/// <summary>Verifies hidden entries are omitted by default and included by <c>--all</c>.</summary>
	[Fact]
	public async Task AllOptionIncludesHiddenEntries() {
		using var fixture = new TemporaryDirectory();
		Directory.CreateDirectory( System.IO.Path.Combine( fixture.Path, ".hidden" ) );
		var normal = new StringWriter();
		var all = new StringWriter();

		var normalExitCode = await Command.RunAsync( new[] { fixture.Path }, stdout: normal );
		var allExitCode = await Command.RunAsync( new[] { "--all", fixture.Path }, stdout: all );

		Assert.Equal( 0, normalExitCode );
		Assert.Equal( 0, allExitCode );
		Assert.DoesNotContain( ".hidden", normal.ToString(), StringComparison.Ordinal );
		Assert.Contains( ".hidden", all.ToString(), StringComparison.Ordinal );
	}

	/// <summary>Verifies the portable ASCII branch alphabet.</summary>
	[Fact]
	public async Task RendersAsciiBranches() {
		using var fixture = new TemporaryDirectory();
		Directory.CreateDirectory( System.IO.Path.Combine( fixture.Path, "alpha" ) );
		Directory.CreateDirectory( System.IO.Path.Combine( fixture.Path, "beta" ) );
		var output = new StringWriter();

		var exitCode = await Command.RunAsync( new[] { "--ascii", fixture.Path }, stdout: output );

		Assert.Equal( 0, exitCode );
		Assert.Contains( "|-- [D] alpha", output.ToString(), StringComparison.Ordinal );
		Assert.Contains( "`-- [D] beta", output.ToString(), StringComparison.Ordinal );
		Assert.DoesNotContain( "├", output.ToString(), StringComparison.Ordinal );
	}

	/// <summary>Verifies unsafe filename control characters are escaped into one output line.</summary>
	[Fact]
	public async Task EscapesControlCharactersInNames() {
		if ( OperatingSystem.IsWindows() ) {
			return;
		}
		using var fixture = new TemporaryDirectory();
		Directory.CreateDirectory( System.IO.Path.Combine( fixture.Path, "line\nbreak\u001b" ) );
		var output = new StringWriter();

		var exitCode = await Command.RunAsync( new[] { fixture.Path }, stdout: output );

		Assert.Equal( 0, exitCode );
		Assert.Contains( "line\\nbreak\\x1B", output.ToString(), StringComparison.Ordinal );
		Assert.Equal( 2, ReadLines( output.ToString() ).Length );
	}

	/// <summary>Verifies a pre-canceled invocation returns the conventional canceled status.</summary>
	[Fact]
	public async Task HonorsCancellation() {
		using var cancellation = new CancellationTokenSource();
		cancellation.Cancel();
		var output = new StringWriter();

		var exitCode = await Command.RunAsync(
			Array.Empty<string>(),
			stdout: output,
			cancellationToken: cancellation.Token
		);

		Assert.Equal( 130, exitCode );
		Assert.Equal( string.Empty, output.ToString() );
	}

	/// <summary>Verifies the filesystem root receives a visible label.</summary>
	[Fact]
	public async Task DisplaysFilesystemRootName() {
		var root = System.IO.Path.GetPathRoot( Environment.CurrentDirectory )!;
		var output = new StringWriter();

		var exitCode = await Command.RunAsync( new[] { "--depth=0", root }, stdout: output );

		Assert.Equal( 0, exitCode );
		Assert.StartsWith( "[D] ", output.ToString(), StringComparison.Ordinal );
		Assert.NotEqual( $"[D] {Environment.NewLine}", output.ToString() );
	}

	/// <summary>Verifies links are identified and are not traversed unless requested.</summary>
	[Fact]
	public async Task DoesNotFollowLinksByDefault() {
		using var fixture = new TemporaryDirectory();
		var target = Directory.CreateDirectory( System.IO.Path.Combine( fixture.Path, "target" ) );
		Directory.CreateDirectory( System.IO.Path.Combine( target.FullName, "nested" ) );
		var link = System.IO.Path.Combine( fixture.Path, "link" );
		if ( !TryCreateDirectoryLink( link, target.FullName ) ) {
			return;
		}
		var output = new StringWriter();

		var exitCode = await Command.RunAsync( new[] { fixture.Path }, stdout: output );

		Assert.Equal( 0, exitCode );
		Assert.Contains( "[L] link ->", output.ToString(), StringComparison.Ordinal );
		Assert.Equal( 1, CountOccurrences( output.ToString(), "nested" ) );
	}

	/// <summary>Verifies followed links cannot recurse through an ancestor indefinitely.</summary>
	[Fact]
	public async Task StopsFollowedLinkCycles() {
		using var fixture = new TemporaryDirectory();
		var child = Directory.CreateDirectory( System.IO.Path.Combine( fixture.Path, "child" ) );
		var loop = System.IO.Path.Combine( child.FullName, "loop" );
		if ( !TryCreateDirectoryLink( loop, fixture.Path ) ) {
			return;
		}
		var output = new StringWriter();

		var exitCode = await Command.RunAsync(
			new[] { "--follow-links", "--depth=20", fixture.Path },
			stdout: output
		);

		Assert.Equal( 0, exitCode );
		Assert.Contains( "[Circular Link / Junction Loop Intercepted]", output.ToString(), StringComparison.Ordinal );
		Assert.True( ReadLines( output.ToString() ).Length < 10 );
	}

	private static int CountOccurrences( string value, string searchedValue ) {
		var count = 0;
		var index = 0;
		while ( 0 <= ( index = value.IndexOf( searchedValue, index, StringComparison.Ordinal ) ) ) {
			count++;
			index += searchedValue.Length;
		}
		return count;
	}

	private static string[] ReadLines( string value ) =>
		value.Split( Environment.NewLine, StringSplitOptions.RemoveEmptyEntries );

	private static bool TryCreateDirectoryLink( string linkPath, string targetPath ) {
		try {
			Directory.CreateSymbolicLink( linkPath, targetPath );
			return true;
		} catch ( UnauthorizedAccessException ) {
			return false;
		} catch ( PlatformNotSupportedException ) {
			return false;
		} catch ( IOException ) when ( OperatingSystem.IsWindows() ) {
			return false;
		}
	}

	private sealed class TemporaryDirectory : IDisposable {
		internal TemporaryDirectory() {
			this.Path = System.IO.Path.Combine(
				System.IO.Path.GetTempPath(),
				"icod-dirtree-" + Guid.NewGuid().ToString( "N" )
			);
			Directory.CreateDirectory( this.Path );
		}

		internal string Path { get; }

		public void Dispose() {
			try {
				Directory.Delete( this.Path, true );
			} catch ( IOException ) {
			} catch ( UnauthorizedAccessException ) {
			}
		}
	}
}
