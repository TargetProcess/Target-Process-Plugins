pushd

cd /d %0\..

NServiceBus.Host.exe /uninstall /serviceName:TpMercurialIntegrationPlugin
Tp.Integration.Plugin.UninstallUtil.exe Tp.PubSubStorage Tp.Mercurial

popd
exit 0
