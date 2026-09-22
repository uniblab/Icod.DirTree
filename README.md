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

`Icod.DirTree` is a cross-platform .NET command-line tool that prints the structure beneath a directory as a text tree.

When `PATH` is omitted, `dirtree` uses the current directory. The command accepts at most one path operand. By default, it prints directories and includes hidden entries. Options can include files, omit hidden entries, and limit the traversal depth.

Directory entries are sorted by name. Comparisons are ordinal and case-sensitive on Linux, and ordinal and case-insensitive on Windows and macOS.

The repository root `README.md` is also the NuGet package README declared by `Icod.DirTree.csproj`, so package consumers receive this same command reference.

## INSTALLATION

Install the .NET tool from NuGet.org:

```text
dotnet tool install --global Icod.DirTree
```

The package installs a single command named `dirtree`.

Update an existing installation with:

```text
dotnet tool update --global Icod.DirTree
```

## OPTIONS

```text
-h, --help
    Display command help and exit.

-v, --version
    Display version information and exit.

-f, --files
    Include files in the output tree. By default, only directories are shown.

-H, --hidden
    Omit hidden files and directories. Hidden entries are included by default.

-d N, --depth=N
    Limit traversal to N levels beneath the root directory. N must be a
    nonnegative integer. A depth of zero prints only the root.

PATH
    Use PATH as the root directory. The current directory is used when PATH is
    omitted.
```

Long option names may be abbreviated when the abbreviation is unambiguous. Options may appear before or after `PATH`.

## OUTPUT

The root and each descendant are labeled by entry type:

```text
[D] project
├── [D] docs
├── [D] src
│   └── [F] Program.cs
└── [F] README.md
```

`[D]` identifies a directory and `[F]` identifies a file. Branch characters show the relationship between entries. A directory that cannot be read is followed by an `[Access Denied]` marker.

## EXAMPLES

Print the directory structure beneath the current directory:

```text
dirtree
```

Print directories beneath `src`, limited to two levels below the root:

```text
dirtree --depth=2 src
```

Include files while omitting hidden entries:

```text
dirtree --files --hidden .
```

Use abbreviated long options to include files through one level below the root:

```text
dirtree --fi --dep=1 .
```

## EXIT STATUS

```text
0    The command completed successfully, or help or version information was
     displayed.
1    Command-line parsing failed or more than one path operand was supplied.
```

Cancellation returns the cancellation status defined by `Icod.CommandFramework`.

## PLATFORM NOTES

The implementation targets .NET 10 and is intended to run on Windows, Linux, and macOS.

Hidden-entry detection uses the host platform's file attributes. Name ordering follows the platform-sensitive comparison described above. Output uses Unicode box-drawing characters; the active terminal and font must support them for the tree branches to display correctly.

## AUTHORS

The original `dirtree` was written by Greg Ordy and Steve Baker.

Migrated to .NET by Timothy J. Bruce <uniblab@hotmail.com>.

## COPYRIGHT

Copyright (c) 2026 Timothy J. Bruce

See the repository `LICENSE` file for licensing terms applicable to this managed implementation.

## SEE ALSO

`tree(1)`, `find(1)`, `ls(1)`
