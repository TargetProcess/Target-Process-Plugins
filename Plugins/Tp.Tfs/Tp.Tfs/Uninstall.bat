pushd

cd /d %0\..

NServiceBus.Host.exe /uninstall /serviceName:TpTfsIntegrationPlugin
Tp.Integration.Plugin.UninstallUtil.exe Tp.PluginsSubscriptions Tp.Tfs

popd
exit 0
