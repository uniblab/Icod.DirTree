param(
    [Parameter(Mandatory = $true)][string]$ArtifactDirectory,
    [ValidateSet('Debug', 'Staging', 'Release')][string]$Configuration = 'Release',
    [string]$ExpectedVersion = ''
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
& (Join-Path $PSScriptRoot 'VerifyPackageArtifact.ps1') `
    -ArtifactDirectory $ArtifactDirectory -Configuration $Configuration -ExpectedVersion $ExpectedVersion

if (-not [System.IO.Path]::IsPathRooted($ArtifactDirectory)) {
    $ArtifactDirectory = Join-Path $repositoryRoot $ArtifactDirectory
}
$ArtifactDirectory = [System.IO.Path]::GetFullPath($ArtifactDirectory)
[xml]$project = Get-Content -LiteralPath (Join-Path $repositoryRoot 'Icod.DirTree.csproj') -Raw
$version = $project.SelectSingleNode('/Project/PropertyGroup/PackageVersion').InnerText.Trim()
$toolPath = Join-Path $repositoryRoot 'artifacts/tool'
$configPath = Join-Path $repositoryRoot 'artifacts/tool-install.NuGet.Config'
$escapedSource = [System.Security.SecurityElement]::Escape($ArtifactDirectory)
$config = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$escapedSource" />
    <add key="nuget" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="local"><package pattern="Icod.DirTree" /></packageSource>
    <packageSource key="nuget"><package pattern="*" /></packageSource>
  </packageSourceMapping>
</configuration>
"@
[System.IO.File]::WriteAllText($configPath, $config, [System.Text.UTF8Encoding]::new($false))

& dotnet tool install Icod.DirTree --tool-path $toolPath --version $version --configfile $configPath
if (0 -ne $LASTEXITCODE) { throw 'Packed tool installation failed.' }
$executable = if ([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform(
    [System.Runtime.InteropServices.OSPlatform]::Windows)) {
    Join-Path $toolPath 'dirtree.exe'
} else {
    Join-Path $toolPath 'dirtree'
}
if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) {
    throw "Tool installation did not produce '$executable'."
}
$output = & $executable --version
if (0 -ne $LASTEXITCODE -or [string]::IsNullOrWhiteSpace(($output -join ''))) {
    throw 'Packed dirtree --version failed or produced no output.'
}
Write-Host "Verified packed dirtree --version on $([System.Runtime.InteropServices.RuntimeInformation]::OSDescription): $($output -join ' ')"
