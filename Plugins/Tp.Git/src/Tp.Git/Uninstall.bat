pushd

cd /d %0\..

NServiceBus.Host.exe /uninstall /serviceName:TpGitIntegrationPlugin
Tp.Integration.Plugin.UninstallUtil.exe Tp.PluginsSubscriptions Tp.Git

popd
exit 0
