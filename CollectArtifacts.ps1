#Requires -Version 7.2

param(
    [Parameter(Mandatory)]
    [String] $Configuration,
    [Parameter(Mandatory)]
    [String] $TargetFramework
)

$OutputDir = Join-Path 'publish' $TargetFramework
$OutputPluginDir = Join-Path $OutputDir 'plugin'
if (!(Test-Path $OutputPluginDir)) {
    New-Item -Path $OutputPluginDir -ItemType 'directory' | Out-Null
}

Get-ChildItem $(Join-Path 'Common\bin' $Configuration $TargetFramework '*.dll') | ForEach-Object -Process {
    Copy-Item -Path $_.FullName -Destination $OutputDir
}

Get-ChildItem "Th*" -Directory | Where-Object Name -ne 'Th11Replay' | ForEach-Object -Process {
    $Project = $_.Name
    $Arguments = @{
        Path = Join-Path $_.FullName 'bin' $Configuration $TargetFramework "ReimuPlugins.$Project.dll"
        Destination = Join-Path $OutputPluginDir "ReimuPlugins.$Project.rpi"
    }
    Copy-Item @Arguments
}

Copy-Item -Path 'ManualGenerator\_build\html' -Destination $(Join-Path $OutputDir 'doc') -Recurse
