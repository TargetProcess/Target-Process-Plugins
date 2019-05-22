pushd
cd /d %0\..

NServiceBus.Host.exe /install /serviceName:TpPerforceIntegrationPlugin /displayName:"TP Perforce integration plugin" /description:"Synchronizes Perforce log with TP"
net start TpPerforceIntegrationPlugin

popd
exit 0
