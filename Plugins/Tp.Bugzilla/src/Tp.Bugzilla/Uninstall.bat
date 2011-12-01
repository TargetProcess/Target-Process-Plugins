pushd
cd /d %0\.. 

NServiceBus.Host.exe /uninstall /serviceName:TPBugzillaIntegrationPlugin
Tp.Integration.Plugin.UninstallUtil.exe Tp.PubSubStorage Tp.Bugzilla

popd
exit 0
