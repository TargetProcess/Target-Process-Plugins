pushd
cd /d %0\..

NServiceBus.Host.exe /install /serviceName:TpGitIntegrationPlugin /displayName:"TP git integration plugin" /description:"Synchronizes git log with TP"
net start TpGitIntegrationPlugin

popd
exit 0