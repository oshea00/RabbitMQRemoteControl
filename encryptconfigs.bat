@echo off

copy %1 %1.original
rename %1 web.config
C:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis -pef secureAppSettings . -prov DataProtectionConfigurationProvider
rename web.config %1

