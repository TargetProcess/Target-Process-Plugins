pushd
cd /d %0\..

NServiceBus.Host.exe /install /serviceName:TpSearchPlugin /displayName:"TP search plugin" /description:"Search for TP"
net start TpSearchPlugin

popd
exit 0