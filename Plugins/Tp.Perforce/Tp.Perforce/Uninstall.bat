pushd

cd /d %0\..

NServiceBus.Host.exe /uninstall /serviceName:TpPerforceIntegrationPlugin
Tp.Integration.Plugin.UninstallUtil.exe Tp.PluginsSubscriptions Tp.Perforce

popd
exit 0
