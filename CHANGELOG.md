# Changelog

All notable changes to `Icod.DirTree` are documented in this file.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and releases use [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - Unreleased

### Added

- Cross-platform command regression tests on Windows, Linux, and macOS.
- `-a`/`--all` to include entries hidden by default.
- `-L`/`--follow-links` with physical-path cycle protection.
- `--ascii` for terminals without Unicode box-drawing support.
- Link markers and displayed link targets.
- Control-character escaping for entry names and link targets.
- Packed-tool traversal smoke tests in addition to exact version checks.

### Changed

- Established `0`, `1`, `2`, and `130` as the success, operational, usage, and cancellation statuses.
- Made invalid and negative depth values usage errors.
- Made missing paths and nondirectory operands operational errors.
- Routed tree output through the injected command output stream.
- Propagated console cancellation through the full traversal.
- Aligned the command, assembly, and NuGet package version at 1.0.0.
- Made the command implementation internal because the package is distributed as a .NET tool, not a reusable API library.

### Fixed

- Prevented symbolic-link and junction cycles from causing unbounded traversal.
- Preserved a visible name when the requested root is a filesystem root.
- Reported I/O failures instead of silently returning success.

[1.0.0]: https://github.com/uniblab/Icod.DirTree/releases/tag/v1.0.0
