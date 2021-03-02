param (
    [Parameter(Mandatory=$true)][string]$FilePath,
    [Parameter(Mandatory=$true)][string]$VariableName
)

if (Test-Path $FilePath)
{
    Write-Host("##vso[task.setvariable variable=$VariableName]true")
    Write-Host("Found file: $FilePath")
}
else
{
    Write-Host("##vso[task.setvariable variable=$VariableName]false")
    Write-Host("Did not find file: $FilePath")
}
