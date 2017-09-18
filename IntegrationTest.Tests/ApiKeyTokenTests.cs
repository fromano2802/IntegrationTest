using IntegrationTest.Features;
using IntegrationTest.Infrastructure;
using IntegrationTest.Models;
using IntegrationTest.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Results;
using IntegrationTest.Services;
using MediatR;
using MediatR.SimpleInjector;
using Moq;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace IntegrationTest.Tests
{
    [TestClass]
    public class ApiKeyTokenTests
    {
        private const string ApiKey = "65634301274e86001fc110f69ba0";
        private const string Token = "bbbce4cd8f310668f53a77759a3cf48a9b06d1a3c189c9efc625c5bb0366603d";
        private static Container Container { get; set; }

        #region Initialization
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Effort.Provider.EffortProviderConfiguration.RegisterProvider();
            Container = new Container();
            Container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();
            Container.RegisterApplicationDependencies();
            Container.BuildMediator(typeof(HomeController).GetTypeInfo().Assembly);
            Container.Verify(VerificationOption.VerifyAndDiagnose);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            EffortProviderFactory.ResetDb();
            var context = Container.GetInstance<AppDBContext>();
            context.Database.CreateIfNotExists();
            context.AppKeys.RemoveRange(context.AppKeys);
            context.KeyTokens.RemoveRange(context.KeyTokens);
            context.SaveChanges();

        }

        private void SeedData()
        {
            var context = Container.GetInstance<AppDBContext>();
            context.Database.CreateIfNotExists();

            var appKey = new AppKey {ApiKey = ApiKey};
            var keyToken = new KeyToken
            {
                ApiKey = ApiKey,
                Token = Token
            };

            context.AppKeys.Add(appKey);
            context.KeyTokens.Add(keyToken);
 
            context.SaveChanges();
        }
        #endregion

        #region Commands
        [TestMethod]
        public async Task ShouldSaveApiKey()
        {
            var context = Container.GetInstance<AppDBContext>();
            context.Database.CreateIfNotExists();

            var mediator = Container.GetInstance<IMediator>();
            var result = await mediator.Send(new SaveApiKey(ApiKey));

            Assert.IsNotNull(result);
            Assert.AreEqual(context.AppKeys.Local.FirstOrDefault(), result);
        }

        [TestMethod]
        public async Task ShouldRemoveApiKeyAndKeyToken()
        {
            SeedData();

            var mediator = Container.GetInstance<IMediator>();
            var result = await mediator.Send(new RemoveApiKey(ApiKey));

            var context = Container.GetInstance<AppDBContext>();
            Assert.IsTrue(result);
            Assert.AreEqual(context.AppKeys.Local.Count, 0);
            Assert.AreEqual(context.KeyTokens.Local.Count, 0);
        }

        [TestMethod]
        public async Task RemoveApiKeyHandlerShouldReturnFalseWhenApiKeyNotFound()
        {
            SeedData();

            var mediator = Container.GetInstance<IMediator>();
            var result = await mediator.Send(new RemoveApiKey("apikey"));

            var context = Container.GetInstance<AppDBContext>();
            Assert.IsFalse(result);
            Assert.AreEqual(context.AppKeys.Local.Count, 1);
        }

        [TestMethod]
        public async Task ShouldSaveKeyToken()
        {
            var context = Container.GetInstance<AppDBContext>();
            context.Database.CreateIfNotExists();

            var mediator = Container.GetInstance<IMediator>();
            var result = await mediator.Send(new SaveToken(ApiKey, Token));

            Assert.IsNotNull(result);
            Assert.AreEqual(context.KeyTokens.Local.FirstOrDefault(), result);
        }

        [TestMethod]
        public async Task ShouldRemoveKeyToken()
        {
            SeedData();

            var mediator = Container.GetInstance<IMediator>();
            var result = await mediator.Send(new RemoveToken(ApiKey));

            var context = Container.GetInstance<AppDBContext>();
            Assert.IsTrue(result);
            Assert.AreEqual(context.KeyTokens.Local.Count, 0);
        }
        #endregion

        #region Controller
        [TestMethod]
        public async Task RemoveApiKeyReturnsNotFoundResultWhenApiKeyIsNull()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<RemoveApiKey>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            var trelloService = new Mock<ITrelloService>();
 
            var sut = new ApiKeyTokenController(mediator.Object, trelloService.Object);
 
            var result = await sut.RemoveApiKey(It.IsAny<string>());
 
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task RemoveTokenReturnsNotFoundResultWhenApiKeyIsNull()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<RemoveToken>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            var trelloService = new Mock<ITrelloService>();
 
            var sut = new ApiKeyTokenController(mediator.Object, trelloService.Object);
 
            var result = await sut.RemoveToken(It.IsAny<string>());
 
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task RemoveApiKeyReturnsOkWhenApiKeyExists()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<RemoveApiKey>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            var trelloService = new Mock<ITrelloService>();
 
            var sut = new ApiKeyTokenController(mediator.Object, trelloService.Object);
 
            var result = await sut.RemoveApiKey(It.IsAny<string>());
 
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public async Task RemoveTokenReturnsOkWhenApiKeyExists()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<RemoveToken>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            var trelloService = new Mock<ITrelloService>();
 
            var sut = new ApiKeyTokenController(mediator.Object, trelloService.Object);
 
            var result = await sut.RemoveToken(It.IsAny<string>());
 
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }
        #endregion
    }
}
