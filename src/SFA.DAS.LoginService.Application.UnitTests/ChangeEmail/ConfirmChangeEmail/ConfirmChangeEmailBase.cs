using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Application.StartChangeEmail;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.Data.Entities;
using System;
using System.Net.Mail;

namespace SFA.DAS.LoginService.Application.UnitTests.ChangeEmail.ConfirmChangeEmail
{
    [TestFixture]
    public class ConfirmChangeEmailBase
    {
        protected Fixture _fixture;
        protected IWebUserService _userService;
        protected LoginContext _loginContext;
        protected ConfirmChangeEmailRequestHandler _sut;
        protected LoginUser _user;
        protected ConfirmChangeEmailRequest _request;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoNSubstituteCustomization { ConfigureMembers = true });
            _fixture.Customize<ConfirmChangeEmailRequest>(c => c
                .With(p => p.CurrentEmailAddress, (MailAddress email) => email.Address)
                .With(p => p.NewEmailAddress, (MailAddress email) => email.Address));

            _userService = _fixture.Create<IWebUserService>();

            var dbContextOptions = new DbContextOptionsBuilder<LoginContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _loginContext = new LoginContext(dbContextOptions);

            _sut = new ConfirmChangeEmailRequestHandler(_userService, _loginContext);

            _request = _fixture.Create<ConfirmChangeEmailRequest>();
            _user = _fixture.Build<LoginUser>()
                .With(c => c.Email, _request.CurrentEmailAddress)
                .Create();

            _userService
                .FindByEmail(_request.CurrentEmailAddress)
                .Returns(_user);
        }
    }
}