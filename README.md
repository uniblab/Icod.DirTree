# DIRTREE(1)

[![PR Staging build](https://github.com/uniblab/Icod.DirTree/actions/workflows/pull-request.yaml/badge.svg?event=pull_request)](https://github.com/uniblab/Icod.DirTree/actions/workflows/pull-request.yaml)
[![Main Release validation](https://github.com/uniblab/Icod.DirTree/actions/workflows/main.yaml/badge.svg?branch=main)](https://github.com/uniblab/Icod.DirTree/actions/workflows/main.yaml)

## NAME

**dirtree** - print a directory structure as a text tree

## SYNOPSIS

```text
dirtree [OPTION] [PATH]
```

## DESCRIPTION

`dirtree` prints the directory hierarchy rooted at `PATH`. When `PATH` is omitted, it uses the current directory. At most one path operand is accepted, and that path must identify an existing directory.

Directories are shown by default. Files and hidden entries are opt-in. Entries are sorted by name using ordinal, case-sensitive comparison on Linux and ordinal, case-insensitive comparison on Windows and macOS.

Symbolic links, junctions, and other supported reparse points are labeled `[L]` and are not followed by default. `--follow-links` enables directory-link traversal. Physical-path tracking prevents a followed link from recursively revisiting an ancestor or another directory already expanded by the traversal.

Control characters in names and displayed link targets are escaped so one filesystem entry cannot inject additional output lines or terminal control sequences. Newline, carriage return, tab, and escape are printed as `\n`, `\r`, `\t`, and `\x1B`; other control characters use `\uXXXX`.

## REQUIREMENTS

The tool targets .NET 10. Install the .NET 10 SDK to install from a package and build from source; a compatible .NET 10 runtime is required to run the installed command.

Supported operating systems are Windows, Linux, and macOS.

## INSTALLATION

Install the released tool from NuGet.org:

```text
dotnet tool install --global Icod.DirTree
```

The package installs one command named `dirtree`. Update an existing installation with:

```text
dotnet tool update --global Icod.DirTree
```

## OPTIONS

```text
-h, --help
    Display command help and exit.

-v, --version
    Display the package version and exit.

-f, --files
    Include files. By default, only directories and directory links are shown.

-a, --all
    Include hidden files and directories. Hidden entries are omitted by default.

-L, --follow-links
    Follow directory symbolic links and junctions. Links are displayed but not
    traversed by default. Repeated physical directories are not expanded again.

--ascii
    Use ASCII branch characters instead of Unicode box-drawing characters.

-d N, --depth=N
    Descend at most N levels below the root. N must be a nonnegative decimal
    integer. Zero prints only the root.

PATH
    Use PATH as the root. The current directory is used when PATH is omitted.
```

Long option names may be abbreviated when the abbreviation is unambiguous. Options may appear before or after `PATH`.

## OUTPUT

Each entry begins with a type marker:

```text
[D] project
├── [D] docs
├── [L] current -> releases/1.0.0
└── [D] src
    └── [F] Program.cs
```

`[D]` identifies a directory, `[F]` a file, and `[L]` a symbolic link, junction, or supported reparse point. Link targets are displayed when the host exposes them.

With `--ascii`, the same branch structure uses `|--`, `` `-- ``, and `|   ` so output remains readable in terminals without box-drawing support.

When a directory cannot be read, `dirtree` prints an `[Access Denied]` or `[I/O Error]` child marker, writes a diagnostic to standard error, continues where possible, and exits with status 1. A repeated physical directory reached while following links receives a `[Circular Link / Junction Loop Intercepted]` marker and is not expanded again.

## EXAMPLES

Print directories beneath the current directory:

```text
dirtree
```

Print files and directories beneath `src`, limited to two levels:

```text
dirtree --files --depth=2 src
```

Include hidden entries:

```text
dirtree --all .
```

Follow directory links while retaining cycle protection:

```text
dirtree --follow-links /srv/project
```

Generate portable plain-text output:

```text
dirtree --ascii --files . > tree.txt
```

## EXIT STATUS

```text
0    The command completed successfully, or help or version was displayed.
1    An operational failure occurred, such as a missing root or unreadable entry.
2    Command-line usage was invalid.
130  Traversal was canceled, including by Ctrl+C.
```

If one descendant cannot be read, the command may produce a partial tree before returning 1.

## BUILDING AND TESTING

Restore, build, and run the regression suite from the repository root:

```text
dotnet restore Icod.DirTree.sln
dotnet build Icod.DirTree.sln -c Release --no-restore
dotnet test Icod.DirTree.sln -c Release --no-build --no-restore
```

Build and verify the NuGet tool package with PowerShell 5.1 or later:

```text
./packaging/Invoke-Build.ps1 -Configuration Release
./packaging/VerifyDistribution.ps1 -Configuration Release
```

Pull requests and pushes to `main` build and test on Windows, Linux, and macOS, then install and exercise the generated package on all three systems. A tag named exactly `vX.Y.Z`, matching both project version properties, publishes that verified package to NuGet.org.

## AUTHORS AND PROVENANCE

Inspired by original work from **Greg Ordy**, author of the original `dirtree`; and **Steve Baker**, author of `tree`.

Independent managed .NET implementation by Timothy J. Bruce <uniblab@hotmail.com>.

## COPYRIGHT AND LICENSE

Copyright (c) 2026 Timothy J. Bruce.

This independent managed implementation is distributed under the GNU General Public License, version 3 or later. See `LICENSE`.

## SEE ALSO

`tree(1)`, `find(1)`, `ls(1)`
