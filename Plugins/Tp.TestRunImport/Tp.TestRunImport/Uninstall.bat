pushd
cd /d %0\..

NServiceBus.Host.exe /uninstall /serviceName:TpTestRunImportIntegrationPlugin
Tp.Integration.Plugin.UninstallUtil.exe Tp.PluginsSubscriptions Tp.TestRunImportIntegration

popd
exit 0