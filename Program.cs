namespace Icod.DirTree;

using Icod.CommandFramework.Diagnostics;

/// <summary>
/// Hosts the <c>dirtree</c> executable. Usage: <c>dirtree [OPTION] [PATH]</c>.
/// </summary>
/// <remarks>
/// <para>
/// The executable prints a text tree for the current directory or for the single directory named by <c>PATH</c>.
/// Use <c>--files</c> to include files, <c>--all</c> to include hidden entries, <c>--follow-links</c> to traverse
/// directory links, <c>--ascii</c> for portable branches, and <c>--depth=N</c> to limit traversal depth.
/// </para>
/// </remarks>
internal static class Program {
	/// <summary>
	/// Runs <c>dirtree</c> with the process console streams and converts a console interrupt into command cancellation.
	/// </summary>
	/// <param name="args">The command-line arguments supplied to <c>dirtree</c>, excluding the executable name.</param>
	/// <returns>A task whose result is the command exit status.</returns>
	/// <remarks>Pressing Ctrl+C cancels traversal and returns status 130.</remarks>
	public static async Task<int> Main( string[] args ) {
		ArgumentNullException.ThrowIfNull( args );

		using var cancellation = new CancellationTokenSource();
		ConsoleCancelEventHandler handler = ( _, eventArgs ) => {
			eventArgs.Cancel = true;
			cancellation.Cancel();
		};
		Console.CancelKeyPress += handler;

		var standardInput = Console.OpenStandardInput();
		var standardOutput = Console.OpenStandardOutput();
		var standardError = Console.OpenStandardError();
		var context = new CommandContext(
			"dirtree",
			Console.In,
			Console.Out,
			Console.Error,
			standardInput,
			standardOutput,
			standardError,
			cancellationToken: cancellation.Token
		);
		try {
			return await Command.RunAsync( args, context ).ConfigureAwait( false );
		} finally {
			Console.CancelKeyPress -= handler;
		}
	}
}
