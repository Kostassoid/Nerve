properties {
    $BaseDir = Resolve-Path "."
    $OutputPath = "$BaseDir\output"
    $SolutionPath = "$BaseDir\src\Nerve.sln"
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

task Test -depends Build {
	$TestDlls = ls "$BaseDir\src\specs\*\bin\$Configuration" -rec `
	    | where { $_.Name.EndsWith("-Specs.dll") } `
	    | foreach { $_.FullName }

	Exec { & ".\src\packages\Machine.Specifications-Signed.0.7.0\tools\mspec-clr4.exe" $TestDlls }
}

task Build -depends Clean {
	msbuild "$SolutionPath" /t:Build /p:Configuration=$Configuration
	Copy-Item "$BaseDir\src\main\Nerve-Core\bin\$Configuration\*.*" $OutputPath -Recurse
}
