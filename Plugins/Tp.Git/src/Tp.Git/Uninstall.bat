pushd

cd /d %0\..

NServiceBus.Host.exe /uninstall /serviceName:TpGitIntegrationPlugin
Tp.Integration.Plugin.UninstallUtil.exe Tp.PubSubStorage Tp.GitIntegration

popd
exit 0
