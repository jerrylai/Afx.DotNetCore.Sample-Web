FROM microsoft/dotnet:2.1-runtime
MAINTAINER jerry.lai<jerrylai@aliyun.com>
WORKDIR /app
COPY . .
ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
EXPOSE 80
ENTRYPOINT ["dotnet", "AfxDotNetCoreSample.Web.dll"]

