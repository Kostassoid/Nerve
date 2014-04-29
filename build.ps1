properties {
    $BaseDir = Resolve-Path "."
    $OutputPath = "$BaseDir\output"
    $SolutionPath = "$BaseDir\src\Nerve.sln"
    $NugetPath = "$BaseDir\src\.nuget\nuget.exe"
    $OpenCoverVersion = "4.5.2506"
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

task TestPrerequisites {
	Exec { & "$NugetPath" install OpenCover -OutputDirectory ".\src\packages" -version $OpenCoverVersion }
}

task Test -depends Build, TestPrerequisites {
	$TestDlls = ls "$BaseDir\src\specs\*\bin\$Configuration" -rec `
	    | where { $_.Name.EndsWith(".Specs.dll") } `
	    | foreach { $_.FullName }

#	Exec { & ".\src\packages\Machine.Specifications-Signed.0.8.2\tools\mspec-clr4.exe" $TestDlls -x Unstable }

	Exec { & ".\src\packages\OpenCover.$OpenCoverVersion\OpenCover.Console.exe" -register:user `
		"-target:.\src\packages\Machine.Specifications-Signed.0.8.2\tools\mspec-clr4.exe" `
		"-targetargs:$TestDlls -x Unstable" "-output:$OutputPath\coverage.xml" "-filter:+[*]* -[*-Specs]*" }

#	$xslt = New-Object System.Xml.Xsl.XslCompiledTransform
#	$xslt.Load(".\src\packages\OpenCover.$OpenCoverVersion\Transform\simple_report.xslt")
#	$xslt.Transform("$OutputPath\coverage.xml", "$OutputPath\coverage.html")
}

task Pack -depends Test {
        $AssemblyVersionPattern = 'AssemblyVersion\((\"\d+\.\d+\.\d+\.\d+\")\)'
        $Version = Get-Content "$BaseDir\src\GlobalAssemblyInfo.cs" |
		Select-String -pattern $AssemblyVersionPattern |
		Select -first 1 |
		% { $_.Matches[0].Groups[1] }

	Exec { & ".\src\.nuget\nuget.exe" pack nuget\Nerve.Core.nuspec -version $Version -OutputDirectory $OutputPath }
}

task Build -depends Clean {
	msbuild "$SolutionPath" /t:Build /p:Configuration=$Configuration
	
	$LibPath = "$OutputPath\net40"
	New-Item $LibPath -Type Directory
	Copy-Item "$BaseDir\src\main\Nerve.Core\bin\$Configuration\*.*" $LibPath -Recurse
}
