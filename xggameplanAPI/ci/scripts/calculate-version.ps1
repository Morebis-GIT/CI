param (
    [Parameter(Mandatory=$true)][string]$BuildId,
    [Parameter(Mandatory=$true)][string]$MajorMinorPatch,
    [Parameter(Mandatory=$true)][string]$ShortSha,
    [Parameter(Mandatory=$true)][string]$EscapedBranchName,
    [string]$PreReleaseLabel = "",
    [string]$VariableName = "BuildVersion"
)

Write-Host("Calculating Build Version...")

$BuildMetadata = ".$ShortSha.$EscapedBranchName".ToLower()
$BuildNumber = ".$BuildId"

if (-not ([string]::IsNullOrEmpty("$PreReleaseLabel")))
{
    # Prerelease tag exists so this is not a tagged version
    $Version = "$MajorMinorPatch-$PreReleaseLabel$BuildNumber+ci$BuildMetadata"
}
else
{
    # Prerelease tag does not exist so this is a tagged version
    $Version = "$MajorMinorPatch+ci$BuildNumber$BuildMetadata"
}

# Write version to file
Out-File -FilePath .\version.txt -InputObject $Version -Encoding UTF8

# Now set the pipeline version variable
Write-Host("##vso[task.setvariable variable=$VariableName]$Version")

Write-Host("Build Version Calculated: $Version")
