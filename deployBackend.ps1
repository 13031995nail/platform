if (Test-Path "nupkgs") {
  Remove-Item -Path "nupkgs/*.nupkg"
}

dotnet pack Tmobis.Platform.slnx --configuration Release --output nupkgs
dotnet nuget push nupkgs/**/*.nupkg --source nexus
