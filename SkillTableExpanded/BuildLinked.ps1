# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/SkillTableExpanded/*" -Force -Recurse
dotnet publish "./SkillTableExpanded.csproj" -c Release -o "$env:RELOADEDIIMODS/SkillTableExpanded" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location