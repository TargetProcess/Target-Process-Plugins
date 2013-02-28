pushd
cd /d %0\..

NServiceBus.Host.exe /install /serviceName:TpMercurialIntegrationPlugin /displayName:"TP mercurial integration plugin" /description:"Synchronizes mercurial log with TP"
net start TpMercurialIntegrationPlugin

popd
exit 0