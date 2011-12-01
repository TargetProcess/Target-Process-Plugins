pushd
cd /d %0\.. 

NServiceBus.Host.exe /install Tp.Integration.Plugin.Common.TpPluginProfile /serviceName:TPMashupManagerPlugin /displayName:"TP Mashup manager plugin" /description:"Allows to create custom mashups"
net start TPMashupManagerPlugin

popd
exit 0