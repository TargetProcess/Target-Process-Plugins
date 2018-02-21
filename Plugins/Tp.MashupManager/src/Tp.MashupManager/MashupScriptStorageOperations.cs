using System;
using Tp.Integration.Plugin.Common.Validation;
using Tp.MashupManager.MashupStorage;

namespace Tp.MashupManager
{
    class MashupScriptStorageOperations
    {
        public static PluginProfileErrorCollection Save(IMashupScriptStorage scriptStorage, Mashup mashup)
        {
            return ReifyError(() => scriptStorage.SaveMashup(mashup));
        }

        public static PluginProfileErrorCollection Delete(IMashupScriptStorage scriptStorage, string mashup)
        {
            return ReifyError(() => scriptStorage.DeleteMashup(mashup));
        }

        public static PluginProfileErrorCollection ReifyError(Action action)
        {
            var errors = new PluginProfileErrorCollection();
            try
            {
                action();
            }
            catch (BadMashupNameException e)
            {
                return Add(errors, e);
            }
            catch (BadMashupFileNameException e)
            {
                return Add(errors, e);
            }
            return errors;
        }

        private static PluginProfileErrorCollection Add(PluginProfileErrorCollection errors, Exception e)
        {
            errors.Add(new PluginProfileError
            {
                Message = e.Message
            });
            return errors;
        }
    }
}
