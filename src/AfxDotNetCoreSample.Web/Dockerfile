FROM microsoft/dotnet:2.2-aspnetcore-runtime
#FROM microsoft/dotnet:2.2-aspnetcore-runtime-gdi
MAINTAINER jerry.lai<jerrylai@aliyun.com>
#RUN apt-get update -y && apt-get install -y libgdiplus && apt-get install -y libc6-dev && apt-get install -y libunwind && apt-get install -y libicu
WORKDIR /app
COPY . .
ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
EXPOSE 80
ENTRYPOINT ["dotnet", "AfxDotNetCoreSample.Web.dll"]
