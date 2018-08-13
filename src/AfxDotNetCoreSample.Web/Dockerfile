FROM microsoft/2.1.2-aspnetcore-runtime
MAINTAINER jerry.lai<jerrylai@aliyun.com>
WORKDIR /app
COPY . .
ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
EXPOSE 80
ENTRYPOINT ["dotnet", "AfxDotNetCoreSample.Web.dll"]

