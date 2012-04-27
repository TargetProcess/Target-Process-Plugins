pushd
cd /d %0\.. 

echo executing EmailProfileConversionUtil
set ConnectionString=%1
Tp.PopEmailIntegration.LegacyProfileConversion.exe -tpdb %ConnectionString%

NServiceBus.Host.exe /install Tp.Integration.Plugin.Common.TpPluginProfile /serviceName:TPPopEmailIntegration /displayName:"TP POP Email Integration" /description:"Synchronizes emails from specified email account to TP"
net start TPPopEmailIntegration

popd
exit 0
