pushd
cd /d %0\.. 

NServiceBus.Host.exe /install Tp.Integration.Plugin.Common.TpPluginProfile /serviceName:TPBugzillaIntegrationPlugin /displayName:"TP Bugzilla integration plugin" /description:"Synchronizes Bugzilla with TP"
net start TPBugzillaIntegrationPlugin

popd
exit 0