@echo off
if exist publish rd /s /q publish
dotnet publish "src/AfxDotNetCoreSample.Web/AfxDotNetCoreSample.Web.csproj" -c Release -o ../../publish/any-sdk
dotnet publish "src/AfxDotNetCoreSample.Web/AfxDotNetCoreSample.Web.csproj" -c Release -o ../../publish/win-x64 -r win-x64 --self-contained true
dotnet publish "src/AfxDotNetCoreSample.Web/AfxDotNetCoreSample.Web.csproj" -c Release -o ../../publish/linux-x64 -r linux-x64 --self-contained true
cd publish
del /q/s *.pdb