# 源生成器错误排查

## 1. 编译错误MSB6006
>* 提示error MSB6006: “csc.exe”已退出,代码为 1

### 1.1 获取详细错误信息
>* dotnet build -v detailed > build.log 2>&1
>* 查看日志文件build.log
>* 大概率是引用错误