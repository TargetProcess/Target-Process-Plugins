using System;
using System.Runtime.InteropServices;
using LibGit2Sharp;

namespace Tp.Git.VersionControlSystem.Ssh
{
    /// <summary>
    /// Provides ssh credentials from memory. Libgit2Sharp-SSH misses this functionality. It can only load keys from files.
    /// Uses StrictUtf8Marshaler which is copy-pasted from Libgit2Sharp-SSH: https://github.com/leobuskin/libgit2sharp-ssh/blob/2d422a2792bc48cc76f55c57f6971b87266354e0/LibGit2Sharp/Core/Utf8Marshaler.cs
    /// </summary>
    public class SshCredentials : Credentials
    {
        // Update this if you update Libgit2Sharp-SSH.
        // This file is coming Libgit2Sharp-SSH nuget package and copied in output directory on build.
        private const string LibgitDll = @"lib\win32\x64\git2-ssh-baa87df.dll";

        [DllImport(LibgitDll)] 
        internal static extern int git_cred_ssh_key_memory_new(
            out IntPtr cred,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StrictUtf8Marshaler))] string username,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StrictUtf8Marshaler))] string publickey,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StrictUtf8Marshaler))] string privatekey,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StrictUtf8Marshaler))] string passphrase);

        private readonly IGitConnectionSettings _settings;
        private readonly string _userNameFromUrl;

        /// <param name="userNameFromUrl">
        /// User name from ssh url. E.g. in 'git@github.com:user/repo.git' it's first 'git' before @.
        /// Libgit2Sharp gives it to us, we just pass it through.
        /// </param>
        public SshCredentials(IGitConnectionSettings settings, string userNameFromUrl)
        {
            _settings = settings;
            _userNameFromUrl = userNameFromUrl;
        }

        protected override int GitCredentialHandler(out IntPtr cred)
        {
            return git_cred_ssh_key_memory_new(out cred, _userNameFromUrl, _settings.SshPublicKey ?? "", _settings.SshPrivateKey ?? "", "");
        }
    }
}
