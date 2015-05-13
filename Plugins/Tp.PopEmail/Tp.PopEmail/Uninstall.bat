pushd
cd /d %0\.. 

NServiceBus.Host.exe /uninstall /serviceName:TPPopEmailIntegration
Tp.Integration.Plugin.UninstallUtil.exe Tp.PluginsSubscriptions Tp.PopEmailIntegration

popd
exit 0
