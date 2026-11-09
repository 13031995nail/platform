if test -d nupkgs; then rm  nupkgs/*.nupkg; fi

dotnet pack Tmobis.Platform.sln --configuration Release --output nupkgs
dotnet nuget push nupkgs/**/*.nupkg --source nexus --api-key $NEXUS_API_KEY