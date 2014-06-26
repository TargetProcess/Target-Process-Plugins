pushd

cd /d %0\..

NServiceBus.Host.exe /uninstall /serviceName:TpSearchPlugin
Tp.Integration.Plugin.UninstallUtil.exe Tp.PubSubStorage Tp.Search

popd
exit 0
