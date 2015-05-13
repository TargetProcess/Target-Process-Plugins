pushd
cd /d %0\.. 

NServiceBus.Host.exe /uninstall /serviceName:TPTaskCreator
Tp.Integration.Plugin.UninstallUtil.exe Tp.PluginsSubscriptions Tp.TaskCreator

popd
exit 0