using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserClientLib
{
    public class UserEngineFactory
    {
        public static IUserClientEngine CreateUserEngine(IUserEvents userEvents)
        {
            return new UserClientEngine(userEvents);
        }
    }
}
