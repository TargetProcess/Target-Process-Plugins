pushd
cd /d %0\.. 

NServiceBus.Host.exe /uninstall /serviceName:TPPopEmailIntegration
Tp.Integration.Plugin.UninstallUtil.exe Tp.PubSubStorage Tp.PopEmailIntegration

popd
exit 0
