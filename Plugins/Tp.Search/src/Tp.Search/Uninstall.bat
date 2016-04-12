pushd

cd /d %0\..

NServiceBus.Host.exe /uninstall /serviceName:TpSearchPlugin
Tp.Integration.Plugin.UninstallUtil.exe Tp.PluginsSubscriptions Tp.Search

popd
exit 0
