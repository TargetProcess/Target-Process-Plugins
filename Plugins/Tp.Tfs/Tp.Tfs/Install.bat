pushd
cd /d %0\..

NServiceBus.Host.exe /install /serviceName:TpTfsIntegrationPlugin /displayName:"TP tfs integration plugin" /description:"Synchronizes tfs log with TP"
net start TpTfsIntegrationPlugin

popd
exit 0