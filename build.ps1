# Requires psake module
Import-Module Psake

# Task to parse slnx file and extract project paths
task ParseSolution {
    # Path to the solution file (modify as needed)
    $solutionFile = Join-Path $PSScriptRoot "ChoreoHelper.slnx"

    # Parse the XML to extract project paths
    $solutionXml = [xml](Get-Content $solutionFile)
    $projects = $solutionXml.Solution.Project | ForEach-Object { $_.Path }

    # Make project paths absolute (assuming relative paths in .slnx file)
    $global:ProjectPaths = $projects | ForEach-Object {
        Join-Path -Path (Split-Path -Path $solutionFile) -ChildPath $_
    }

    Write-Host "Found projects:"
    $global:ProjectPaths | ForEach-Object { Write-Host $_ }
}

# Task to restore projects
task Restore -depends ParseSolution {
    Write-Host "Restoring dependencies for all projects..."

    foreach ($projectPath in $global:ProjectPaths) {
        Write-Host "Restoring: $projectPath"
        & dotnet restore $projectPath
        if ($LASTEXITCODE -ne 0) {
            throw "Failed to restore: $projectPath"
        }
    }
}

# Task to build projects
task Build -depends Restore {
    Write-Host "Building all projects..."

    foreach ($projectPath in $global:ProjectPaths) {
        Write-Host "Building: $projectPath"
        & dotnet build $projectPath --no-restore
        if ($LASTEXITCODE -ne 0) {
            throw "Failed to build: $projectPath"
        }
    }
}

# Default task
task Default -depends Build

# Run psake
Invoke-psake
