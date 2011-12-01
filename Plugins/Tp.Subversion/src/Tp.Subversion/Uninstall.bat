pushd
cd /d %0\.. 

NServiceBus.Host.exe /uninstall /serviceName:TpSubversionIntegrationPlugin
Tp.Integration.Plugin.UninstallUtil.exe Tp.PubSubStorage Tp.SubversionIntegration

popd
exit 0
