// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhino.Mocks
{
    public static class RhinoMockExtensions
    {
        public static void ResetExpectations<T>(this T mock) where T : class
        {
            mock.BackToRecord(BackToRecordOptions.Expectations);
            mock.Replay();
        }

        public static IEnumerable<IEnumerable<TArg>> GetCallsMadeOn<TMockInstance, TArg>(this TMockInstance bus,
            Action<TMockInstance> action)
        {
            var callsMadeOn = bus.GetArgumentsForCallsMadeOn(action).ToList();
            foreach (var call in callsMadeOn)
            {
                var firstArg = call[0] as object[];
                if (firstArg == null)
                    continue;

                var callArgs = firstArg.OfType<TArg>();

                if (callArgs.Count() > 0)
                    yield return callArgs;
            }
        }
    }
}
