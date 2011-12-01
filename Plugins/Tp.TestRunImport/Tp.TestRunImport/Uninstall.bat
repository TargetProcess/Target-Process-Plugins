pushd
cd /d %0\..

NServiceBus.Host.exe /uninstall /serviceName:TpTestRunImportIntegrationPlugin
Tp.Integration.Plugin.UninstallUtil.exe Tp.PubSubStorage

popd
exit 0