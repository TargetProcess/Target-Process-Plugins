pushd

cd /d %0\..

NServiceBus.Host.exe /uninstall /serviceName:TpTfsIntegrationPlugin
Tp.Integration.Plugin.UninstallUtil.exe Tp.PubSubStorage Tp.Tfs

popd
exit 0
