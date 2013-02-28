pushd
cd /d %0\.. 

vcredist_x86.exe /q

echo executing SubversionProfileConversionUtil
set ConnectionString=%1

Tp.Subversion.LegacyProfileConversion.exe -tpdb %ConnectionString%

NServiceBus.Host.exe /install Tp.Integration.Plugin.Common.TpPluginProfile /serviceName:TpSubversionIntegrationPlugin /displayName:"TP subversion integration plugin" /description:"Synchronizes svn log with TP"
net start TpSubversionIntegrationPlugin

popd
exit 0