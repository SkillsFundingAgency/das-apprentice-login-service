using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.LoginService.Application.ChangeEmail.ConfirmChangeEmail;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Application.Services;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using NServiceBus;

namespace SFA.DAS.LoginService.Application.UnitTests.ChangeEmail.ConfirmChangeEmail
{
    [TestFixture]
    public class ConfirmChangeEmailBase
    {
        protected Fixture _fixture;
        protected ConfirmChangeEmailRequestHandler _sut;
        protected LoginUser _user;
        protected ConfirmChangeEmailRequest _request;
        protected UserManager<LoginUser> _userManager;
        protected IMessageSession _messageSession;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoNSubstituteCustomization { ConfigureMembers = true });
            _fixture.Customize<StoreOptions>(c => c.With(p => p.ProtectPersonalData, false));

            _userManager = Substitute.For<UserManager<LoginUser>>(
                _fixture.Create<IUserStore<LoginUser>>(),
                _fixture.Create<IOptions<IdentityOptions>>(),
                _fixture.Create<IPasswordHasher<LoginUser>>(),
                _fixture.Create<IEnumerable<IUserValidator<LoginUser>>>(),
                _fixture.Create<IEnumerable<IPasswordValidator<LoginUser>>>(),
                _fixture.Create<ILookupNormalizer>(),
                _fixture.Create<IdentityErrorDescriber>(),
                _fixture.Create<IServiceProvider>(),
                _fixture.Create<ILogger<UserManager<LoginUser>>>()
                                                                 );
            var dbContextOptions = new DbContextOptionsBuilder<LoginContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _fixture.Inject(_userManager);
            _fixture.Inject(new LoginContext(dbContextOptions));
            _fixture.Register<IWebUserService>(() => _fixture.Create<UserService>());
            
            _messageSession = Substitute.For<IMessageSession>();
            _fixture.Inject(_messageSession);

            _fixture.Inject(Substitute.For<ILogger<ConfirmChangeEmailRequestHandler>>());

            _sut = _fixture.Create<ConfirmChangeEmailRequestHandler>();

            _fixture.Customize<ConfirmChangeEmailRequest>(c => c
                .With(p => p.CurrentEmailAddress, (MailAddress email) => email.Address)
                .With(p => p.NewEmailAddress, (MailAddress email) => email.Address));

            _request = _fixture.Create<ConfirmChangeEmailRequest>();
            _user = _fixture.Build<LoginUser>()
                .With(c => c.Email, _request.CurrentEmailAddress)
                .Create();

            _userManager.FindByEmailAsync(_request.CurrentEmailAddress).Returns(_user);
            _userManager.FindByIdAsync(_request.SubjectId).Returns(_user);
            _userManager.ChangeEmailAsync(default, default, default).ReturnsForAnyArgs(IdentityResult.Success);
        }
    }
}