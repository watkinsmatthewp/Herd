# dotnet build Herd.Web\Herd.Web.csproj
Get-ChildItem -name | select-string -pattern "\.UnitTests" | ForEach-Object {
    cd $_;
    dotnet test;
    cd ..\;
}