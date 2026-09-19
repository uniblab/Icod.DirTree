namespace Icod.DirTree;

using Icod.CommandFramework.Diagnostics;

public static class Program {
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
			standardError
		);
		try {
			return await Command.RunAsync( args, context ).ConfigureAwait( false );
		} finally {
			Console.CancelKeyPress -= handler;
		}
	}
}
