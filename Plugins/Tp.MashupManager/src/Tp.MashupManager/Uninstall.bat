pushd
cd /d %0\.. 

NServiceBus.Host.exe /uninstall /serviceName:TPMashupManagerPlugin
Tp.Integration.Plugin.UninstallUtil.exe Tp.PluginsSubscriptions Tp.MashupManager

popd
exit 0
