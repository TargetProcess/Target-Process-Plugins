pushd
cd /d %0\..

echo executing TestRunImportProfileConversionUtil
set ConnectionString=%1

Tp.TestRunImport.LegacyProfileConversion.exe -tpdb %ConnectionString%

NServiceBus.Host.exe /install Tp.Integration.Plugin.Common.TpPluginProfile /serviceName:TpTestRunImportIntegrationPlugin /displayName:"TP Test Run Import integration plugin" /description:"Synchronizes automatic test run results with TP"
net start TpTestRunImportIntegrationPlugin

popd
exit 0