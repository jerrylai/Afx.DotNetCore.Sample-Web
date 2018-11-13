@echo off
if exist publish rd /s /q publish
dotnet publish "src/AfxDotNetCoreSample.Web/AfxDotNetCoreSample.Web.csproj" -c Release -o ../../publish/any-sdk
cd publish
del /q/s *.pdb