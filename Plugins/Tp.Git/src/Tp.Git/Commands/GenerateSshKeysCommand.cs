using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Tp.Integration.Common;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.Logging;

namespace Tp.Git.Commands
{
    public class GenerateSshKeysCommand : IPluginCommand
    {
        private readonly ILogManager _logManager;

        public GenerateSshKeysCommand(ILogManager logManager)
        {
            _logManager = logManager;
        }

        public PluginCommandResponseMessage Execute(string args, UserDTO user = null)
        {
            string privateKeyFile, publicKeyFile;
            do
            {
                privateKeyFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                publicKeyFile = privateKeyFile + ".pub";
            } while (File.Exists(privateKeyFile) || File.Exists(publicKeyFile));


            var assemblyPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            var keygenPath = Path.Combine(Path.GetDirectoryName(assemblyPath), "ssh-keygen", "ssh-keygen.exe");
            _logManager.GetLogger(GetType()).Info($"Executing {keygenPath}");

            string privateKey, publicKey;
            try
            {
                var keygen = Process.Start(new ProcessStartInfo
                {
                    FileName = keygenPath,
                    Arguments = $"-t rsa -b 4096 -f {privateKeyFile} -N \"\" -C \"\" -q",
                    UseShellExecute = false,
                    RedirectStandardError = true
                });

                keygen.WaitForExit();

                if (keygen.ExitCode != 0)
                {
                    var error = keygen.StandardError.ReadToEnd();
                    _logManager.GetLogger(GetType()).Error($"ssh-keygen exited with code {keygen.ExitCode}: {error}");
                    return new PluginCommandResponseMessage { PluginCommandStatus = PluginCommandStatus.Error, ResponseData = "Failed to generate key pair. Please contact your administrator." };
                }

                privateKey = File.ReadAllText(privateKeyFile);
                publicKey = File.ReadAllText(publicKeyFile);
            }
            finally
            {
                File.Delete(privateKeyFile);
                File.Delete(publicKeyFile);
            }

            var data = JsonConvert.SerializeObject(new { publicKey, privateKey });
            return new PluginCommandResponseMessage { PluginCommandStatus = PluginCommandStatus.Succeed, ResponseData = data };
        }

        public string Name => "GenerateSshKeys";
    }
}
