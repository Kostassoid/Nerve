properties {
    $BaseDir = Resolve-Path "."
    $OutputPath = "$BaseDir\output"
    $SolutionPath = "$BaseDir\src\Nerve.sln"
    $NugetPath = "$BaseDir\src\.nuget\nuget.exe"
    $BuiltPath = "$OutputPath\built"
    $MergedPath = "$OutputPath\merged"
    $OpenCoverVersion = "4.5.2506"
    $MSpecVersion = "0.8.2"
    $ILMergeVersion = "2.13.0307"
    $Configuration = "Release"
}

task default -depends Build

task Clean {
	if (Test-Path -Path $OutputPath)
	{
		Remove-Item -Recurse -Force $OutputPath
	}
	New-Item -ItemType Directory -Force $OutputPath
	msbuild "$SolutionPath" /t:Clean /p:Configuration=$Configuration
}

task Prerequisites {
	Exec { & "$NugetPath" install OpenCover -OutputDirectory ".\src\packages" -version $OpenCoverVersion }
	Exec { & "$NugetPath" install ILMerge -OutputDirectory ".\src\packages" -version $ILMergeVersion }
}

task Test -depends Build, Prerequisites {
	$TestDlls = ls "$BaseDir\src\specs\*\bin\$Configuration" -rec `
	    | where { $_.Name.EndsWith(".Specs.dll") } `
	    | foreach { $_.FullName }

	Exec { & ".\src\packages\OpenCover.$OpenCoverVersion\OpenCover.Console.exe" -register:user `
		"-target:.\src\packages\Machine.Specifications-Signed.$MSpecVersion\tools\mspec-clr4.exe" `
		"-targetargs:$TestDlls -x Unstable" "-output:$OutputPath\coverage.xml" "-filter:+[*]* -[*-Specs]*" "-returntargetcode" }
}

task Merge -depends Build, Prerequisites {
	New-Item $MergedPath -Type Directory

	Exec { & ".\src\packages\ILMerge.$ILMergeVersion\ilmerge.exe" `
		/keyfile:src\Nerve.snk /internalize /target:library `
		/targetplatform:"v4,C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319" `
		"/out:$MergedPath\Nerve.Core.dll" `
		"$BuiltPath\Nerve.Core.dll" `
		"$BuiltPath\Fasterflect.dll" `
		"$BuiltPath\NProxy.Core.dll" }

	Copy-Item "$BuiltPath\Nerve.Core.xml" $MergedPath
}

task Pack -depends Test, Merge {
        $AssemblyVersionPattern = 'AssemblyVersion\((\"\d+\.\d+\.\d+\.\d+\")\)'
        $Version = Get-Content "$BaseDir\src\GlobalAssemblyInfo.cs" |
		Select-String -pattern $AssemblyVersionPattern |
		Select -first 1 |
		% { $_.Matches[0].Groups[1] }

	Exec { & ".\src\.nuget\nuget.exe" pack nuget\Nerve.Core.nuspec -version $Version -OutputDirectory $OutputPath }
}

task Build -depends Clean {
	msbuild "$SolutionPath" /t:Build /p:Configuration=$Configuration
	
	New-Item $BuiltPath -Type Directory
	Copy-Item "$BaseDir\src\main\Nerve.Core\bin\$Configuration\*.*" $BuiltPath -Recurse
}
