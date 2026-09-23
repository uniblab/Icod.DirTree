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
if ($ExpectedVersion -and $ExpectedVersion -cne $version) {
    throw "Project package version '$version' does not match expected '$ExpectedVersion'."
}
$toolPath = Join-Path $repositoryRoot 'artifacts/tool'
$configPath = Join-Path $repositoryRoot 'artifacts/tool-install.NuGet.Config'
if (Test-Path -LiteralPath $toolPath) {
    Remove-Item -LiteralPath $toolPath -Recurse -Force
}
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
$versionOutput = @(& $executable --version)
if (0 -ne $LASTEXITCODE) {
    throw 'Packed dirtree --version failed.'
}
$expectedVersionOutput = "dirtree (Icod.DirTree) $version"
if (1 -ne $versionOutput.Count -or $versionOutput[0] -cne $expectedVersionOutput) {
    throw "Packed dirtree --version returned '$($versionOutput -join ' ')' instead of '$expectedVersionOutput'."
}

$fixtureRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("icod-dirtree-" + [Guid]::NewGuid().ToString('N'))
$fixture = Join-Path $fixtureRoot 'fixture'
try {
    $null = New-Item -ItemType Directory -Path (Join-Path (Join-Path $fixture 'alpha') 'nested') -Force
    $null = New-Item -ItemType Directory -Path (Join-Path $fixture 'beta') -Force
    [System.IO.File]::WriteAllText((Join-Path $fixture 'sample.txt'), 'sample')

    $treeOutput = @(& $executable --files --depth=1 --ascii $fixture)
    if (0 -ne $LASTEXITCODE) {
        throw 'Packed dirtree traversal smoke test failed.'
    }
    $expectedTreeOutput = @(
        '[D] fixture',
        '|-- [D] alpha',
        '|-- [D] beta',
        '`-- [F] sample.txt'
    )
    if ($treeOutput.Count -ne $expectedTreeOutput.Count) {
        throw "Packed dirtree traversal produced $($treeOutput.Count) lines instead of $($expectedTreeOutput.Count)."
    }
    for ($index = 0; $index -lt $expectedTreeOutput.Count; $index++) {
        if ($treeOutput[$index] -cne $expectedTreeOutput[$index]) {
            throw "Packed dirtree traversal line $($index + 1) was '$($treeOutput[$index])' instead of '$($expectedTreeOutput[$index])'."
        }
    }
} finally {
    if (Test-Path -LiteralPath $fixtureRoot) {
        Remove-Item -LiteralPath $fixtureRoot -Recurse -Force
    }
}

Write-Host "Verified packed dirtree $version on $([System.Runtime.InteropServices.RuntimeInformation]::OSDescription)."
