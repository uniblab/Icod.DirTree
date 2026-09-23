param(
    [Parameter(Mandatory = $true)][string]$ArtifactDirectory,
    [ValidateSet('Debug', 'Staging', 'Release')][string]$Configuration = 'Release',
    [string]$ExpectedVersion = ''
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
if (-not [System.IO.Path]::IsPathRooted($ArtifactDirectory)) {
    $ArtifactDirectory = Join-Path $repositoryRoot $ArtifactDirectory
}
$ArtifactDirectory = [System.IO.Path]::GetFullPath($ArtifactDirectory)
if (-not (Test-Path -LiteralPath $ArtifactDirectory -PathType Container)) {
    throw "Artifact directory '$ArtifactDirectory' does not exist."
}

[xml]$project = Get-Content -LiteralPath (Join-Path $repositoryRoot 'Icod.DirTree.csproj') -Raw
function Get-ProjectProperty {
    param([Parameter(Mandatory = $true)][string]$Name)
    $node = $project.SelectSingleNode("/Project/PropertyGroup/$Name[normalize-space(.) != '']")
    if ($null -eq $node) { throw "Icod.DirTree.csproj does not declare '$Name'." }
    return $node.InnerText.Trim()
}

$packageId = Get-ProjectProperty -Name 'PackageId'
$packageVersion = Get-ProjectProperty -Name 'PackageVersion'
$targetFramework = Get-ProjectProperty -Name 'TargetFramework'
$readme = Get-ProjectProperty -Name 'PackageReadmeFile'
if ($packageId -cne 'Icod.DirTree' -or (Get-ProjectProperty -Name 'PackAsTool') -ne 'true' -or
    (Get-ProjectProperty -Name 'ToolCommandName') -cne 'dirtree') {
    throw 'Project must declare the Icod.DirTree package and its single dirtree tool.'
}
if ($ExpectedVersion -and $ExpectedVersion -cne $packageVersion) {
    throw "Project package version '$packageVersion' does not match expected '$ExpectedVersion'."
}

$packages = @(Get-ChildItem -LiteralPath $ArtifactDirectory -Filter '*.nupkg' -File)
if (1 -ne $packages.Count) { throw "Expected exactly one .nupkg; found $($packages.Count)." }
$package = $packages[0]
if ($package.Name -cne "$packageId.$packageVersion.nupkg") {
    throw "Package '$($package.Name)' does not match '$packageId.$packageVersion.nupkg'."
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [System.IO.Compression.ZipFile]::OpenRead($package.FullName)
try {
    $entries = @($archive.Entries | ForEach-Object { $_.FullName.Replace('\', '/') })
    $settingsPath = "tools/$targetFramework/any/DotnetToolSettings.xml"
    foreach ($required in @(
        "$packageId.nuspec", $settingsPath,
        "tools/$targetFramework/any/dirtree.dll",
        "tools/$targetFramework/any/dirtree.runtimeconfig.json",
        $readme, 'CHANGELOG.md', 'LICENSE', 'icon.png'
    )) {
        if ($required -notin $entries) { throw "Package '$($package.Name)' is missing '$required'." }
    }

    $nuspecEntry = $archive.GetEntry("$packageId.nuspec")
    $reader = [System.IO.StreamReader]::new($nuspecEntry.Open())
    try { [xml]$nuspec = $reader.ReadToEnd() } finally { $reader.Dispose() }
    if ("$($nuspec.package.metadata.id)" -cne $packageId -or
        "$($nuspec.package.metadata.version)" -cne $packageVersion -or
        "$($nuspec.package.metadata.readme)" -cne $readme) {
        throw 'Package nuspec id, version, or readme does not match the project.'
    }

    $settingsEntry = $archive.GetEntry($settingsPath)
    $reader = [System.IO.StreamReader]::new($settingsEntry.Open())
    try { [xml]$settings = $reader.ReadToEnd() } finally { $reader.Dispose() }
    $commands = @($settings.DotNetCliTool.Commands.Command)
    if (1 -ne $commands.Count -or "$($commands[0].Name)" -cne 'dirtree' -or
        "$($commands[0].Runner)" -cne 'dotnet') {
        throw "Package '$($package.Name)' does not declare exactly one dirtree/dotnet command."
    }
} finally {
    $archive.Dispose()
}

Write-Host "Verified $($package.Name) ($Configuration)."
