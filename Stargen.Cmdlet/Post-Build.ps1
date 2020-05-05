param(
	[Parameter()] $ProjectName,
	[Parameter()] $ConfigurationName,
	[Parameter()] $TargetDir
)

New-Item -Path "$TargetDir" -Name "$ProjectName" -Force -ItemType "directory" -Verbose
Move-Item -Path "$TargetDir/*" -Destination "$TargetDir/$ProjectName" -Force -Verbose
Move-Item -Path "$TargetDir/$ProjectName/Start-Debug.ps1" -Destination "$TargetDir"