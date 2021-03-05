using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Services
{
    public class WaitForResult
    {
        public bool HasTimedOut { get; set; }
        public bool HasStarted { get; set; }
        public bool HasErrored { get; private set; }
        public Exception LastException { get; private set; }
        public bool HasCompleted { get; set; }

        public void SetHasErrored(Exception ex)
        {
            HasErrored = true;
            LastException = ex;
        }
    }


}
