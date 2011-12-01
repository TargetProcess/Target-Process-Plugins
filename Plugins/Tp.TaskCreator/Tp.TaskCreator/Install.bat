pushd
cd /d %0\.. 

NServiceBus.Host.exe /install Tp.Integration.Plugin.Common.TpPluginProfile /serviceName:TPTaskCreator /displayName:"TP Task Creator" /description:"Creates tasks for user stories on user story creation"
net start TPTaskCreator

popd
exit 0