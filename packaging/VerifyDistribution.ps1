param(
    [ValidateSet('Debug', 'Staging', 'Release')][string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
Push-Location $repositoryRoot
try {
    dotnet restore Icod.DirTree.sln
    if (0 -ne $LASTEXITCODE) { throw 'Restore failed.' }
    dotnet build Icod.DirTree.sln -c $Configuration --no-restore -p:ContinuousIntegrationBuild=true
    if (0 -ne $LASTEXITCODE) { throw 'Build failed.' }
    dotnet test Icod.DirTree.sln -c $Configuration --no-build --no-restore --logger trx
    if (0 -ne $LASTEXITCODE) { throw 'Tests failed.' }
    dotnet pack Icod.DirTree.csproj -c $Configuration --no-build --no-restore -o artifacts/package -p:ContinuousIntegrationBuild=true
    if (0 -ne $LASTEXITCODE) { throw 'Pack failed.' }
    & (Join-Path $PSScriptRoot 'TestPackedTool.ps1') -ArtifactDirectory artifacts/package -Configuration $Configuration
} finally {
    Pop-Location
}
