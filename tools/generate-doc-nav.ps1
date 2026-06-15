[CmdletBinding()]
param(
    [string]$Root,
    [string]$DocsDir = 'docs',
    [string]$OutputPath,
    [switch]$Force
)

$ErrorActionPreference = 'Stop'
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new()

if ([string]::IsNullOrWhiteSpace($Root)) {
    $scriptRoot = if ($PSScriptRoot) {
        $PSScriptRoot
    }
    else {
        Split-Path -Parent $MyInvocation.MyCommand.Path
    }

    $Root = (Resolve-Path (Join-Path $scriptRoot '..')).Path
}

function ConvertTo-RepoPath {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$RootPath
    )

    $rootFull = [System.IO.Path]::GetFullPath($RootPath).TrimEnd('\', '/')
    $pathFull = [System.IO.Path]::GetFullPath($Path)
    $rootUri = [System.Uri]::new($rootFull + [System.IO.Path]::DirectorySeparatorChar)
    $pathUri = [System.Uri]::new($pathFull)
    $relative = [System.Uri]::UnescapeDataString($rootUri.MakeRelativeUri($pathUri).ToString())
    return ($relative -replace '\\', '/')
}

function Get-TitleFromMarkdown {
    param([Parameter(Mandatory = $true)][string]$Path)

    $heading = Select-String -LiteralPath $Path -Pattern '^#{1,2}\s+' -Encoding UTF8 | Select-Object -First 1
    if ($heading) {
        return (($heading.Line -replace '^#{1,2}\s+', '').Trim())
    }

    return [System.IO.Path]::GetFileNameWithoutExtension($Path)
}

function Get-NavBucket {
    param([Parameter(Mandatory = $true)][string]$RepoPath)

    if ($RepoPath -eq 'index.md' -or
        $RepoPath -eq 'PROJECT_MAP.md' -or
        $RepoPath -eq 'IMPLEMENTATION_INDEX.md' -or
        $RepoPath -eq 'screenshots/README.md' -or
        $RepoPath -eq 'HANDOVER.md' -or
        $RepoPath -eq 'MILESTONE_PLAN.md') {
        return [pscustomobject]@{ Section = 'Overview'; Group = '' }
    }

    if ($RepoPath -like 'spec/*') {
        return [pscustomobject]@{ Section = 'Specs'; Group = '' }
    }

    if ($RepoPath -eq 'WORKFLOW_STATE_SSOT.md' -or
        $RepoPath -eq 'dev/HANDOFF_LOCAL_DOC_VIEW.md' -or
        $RepoPath -eq 'dev/RESUME.md' -or
        $RepoPath -eq 'tasks/TASK_026_ProjectCompletion_Assessment.md' -or
        $RepoPath -eq 'dev/PHASE5_VALIDATION_PREFLIGHT.md') {
        return [pscustomobject]@{ Section = 'Runtime State'; Group = '' }
    }

    if ($RepoPath -like 'dev/*' -or
        $RepoPath -like 'Windsurf_AI_Collab_Rules_*.md') {
        return [pscustomobject]@{ Section = 'Development Notes'; Group = '' }
    }

    if ($RepoPath -like 'tasks/*') {
        return [pscustomobject]@{ Section = 'Artifacts'; Group = 'Tasks' }
    }

    if ($RepoPath -like 'reports/*') {
        return [pscustomobject]@{ Section = 'Artifacts'; Group = 'Reports' }
    }

    if ($RepoPath -like 'inbox/*') {
        return [pscustomobject]@{ Section = 'Artifacts'; Group = 'Inbox' }
    }

    return [pscustomobject]@{ Section = 'Misc'; Group = '' }
}

function Format-YamlText {
    param([Parameter(Mandatory = $true)][string]$Text)

    $escaped = $Text -replace "'", "''"
    return "'$escaped'"
}

function Get-MarkdownFiles {
    param([Parameter(Mandatory = $true)][string]$RootPath)

    $rg = Get-Command rg -ErrorAction SilentlyContinue
    $globs = @('-g', '*.md')

    if ($rg) {
        Push-Location -LiteralPath $RootPath
        try {
            return (& rg --files @globs) | ForEach-Object { Join-Path $RootPath $_ }
        }
        finally {
            Pop-Location
        }
    }

    return Get-ChildItem -LiteralPath $RootPath -Recurse -File -Filter *.md -Force |
        ForEach-Object { $_.FullName }
}

$repoRoot = (Resolve-Path -LiteralPath $Root).Path
$docsPath = (Resolve-Path -LiteralPath (Join-Path $repoRoot $DocsDir)).Path
$items = Get-MarkdownFiles -RootPath $docsPath |
    Sort-Object |
    ForEach-Object {
        $repoPath = ConvertTo-RepoPath -Path $_ -RootPath $docsPath
        $bucket = Get-NavBucket -RepoPath $repoPath
        [pscustomobject]@{
            Path = $repoPath
            Title = Get-TitleFromMarkdown -Path $_
            Section = $bucket.Section
            Group = $bucket.Group
        }
    }

$sectionOrder = @('Overview', 'Specs', 'Runtime State', 'Development Notes', 'Artifacts', 'Misc')
$groupOrder = @('Tasks', 'Reports', 'Inbox')
$lines = [System.Collections.Generic.List[string]]::new()

$lines.Add('# Generated nav candidate. Review before pasting into mkdocs.yml.')
$lines.Add('nav:')

foreach ($section in $sectionOrder) {
    $sectionItems = @($items | Where-Object { $_.Section -eq $section })
    if ($sectionItems.Count -eq 0) {
        continue
    }

    $lines.Add("  - ${section}:")

    if ($section -eq 'Artifacts') {
        foreach ($group in $groupOrder) {
            $groupItems = @($sectionItems | Where-Object { $_.Group -eq $group } | Sort-Object Path)
            if ($groupItems.Count -eq 0) {
                continue
            }

            $lines.Add("      - ${group}:")
            foreach ($item in $groupItems) {
                $lines.Add("          - $(Format-YamlText -Text $item.Title): $($item.Path)")
            }
        }

        $ungrouped = @($sectionItems | Where-Object { $_.Group -eq '' } | Sort-Object Path)
        foreach ($item in $ungrouped) {
            $lines.Add("      - $(Format-YamlText -Text $item.Title): $($item.Path)")
        }
        continue
    }

    foreach ($item in ($sectionItems | Sort-Object Path)) {
        $lines.Add("      - $(Format-YamlText -Text $item.Title): $($item.Path)")
    }
}

if ($OutputPath) {
    $resolvedOutput = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($OutputPath)
    if ((Test-Path -LiteralPath $resolvedOutput) -and -not $Force) {
        throw "OutputPath already exists. Pass -Force to overwrite: $resolvedOutput"
    }

    $lines | Set-Content -LiteralPath $resolvedOutput -Encoding UTF8
    Write-Host "Wrote nav candidate: $resolvedOutput"
    exit 0
}

$lines -join [Environment]::NewLine
