using ClassLibrary1;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestProject
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public async Task Test1Async()
        {
            // Assert
            var class1 = new MongoPayService();

            //Act

            await class1.GetTransactionsAsync();

            //Assert
        }
    }
}
