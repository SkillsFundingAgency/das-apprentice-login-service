using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.NUnit3;
using SFA.DAS.Apprentice.LoginService.Messages;
using System;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Steps
{
    public class DomainAutoDataAttribute : AutoDataAttribute
    {
        public DomainAutoDataAttribute()
            : base(() => CreateFixture())
        {
        }

        private static IFixture CreateFixture()
        {
            var fixture = new Fixture();
            fixture.Customize<SendInvitation>(c => c
                .With(p => p.Callback, fixture.Create<Uri>().ToString())
                .With(p => p.UserRedirect, fixture.Create<Uri>().ToString()));
            fixture.Customize(new AutoNSubstituteCustomization() { ConfigureMembers = true });
            return fixture;
        }
    }
}