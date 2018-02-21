// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.MashupManager.CustomCommands.Args;
using Tp.MashupManager.MashupLibrary;

namespace Tp.MashupManager.CustomCommands
{
    public abstract class LibraryCommand<T> : IPluginCommand
        where T : LibraryCommandArg
    {
        private ILibrary _library;

        protected ILibrary Library
        {
            get { return _library ?? (_library = ObjectFactory.GetInstance<ILibrary>()); }
        }

        public PluginCommandResponseMessage Execute(string args, UserDTO user)
        {
            var commandArg = string.IsNullOrEmpty(args) ? default(T) : args.Deserialize<T>();
            return ExecuteOperation(commandArg);
        }

        protected abstract PluginCommandResponseMessage ExecuteOperation(T commandArg);

        public abstract string Name { get; }
    }
}
