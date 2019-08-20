using NUnit.Framework;

namespace PEG.Samples.Url
{
    [TestFixture]
    public class UrlGrammarTest
    {
        [Test]
        public void JustDomain()
        {
            Url url = Url.Parse("http://test.com");
            Assert.AreEqual("test.com", url.Domain);
        }

        [Test]
        public void DomainAndPort()
        {
            Url url = Url.Parse("http://test.com:433");
            Assert.AreEqual("test.com", url.Domain);
            Assert.AreEqual("433", url.Port);
        }

        [Test]
        public void DomainAndPortAndFile()
        {
            Url url = Url.Parse("http://test.com:433/Test.txt");
            Assert.AreEqual("test.com", url.Domain);
            Assert.AreEqual("433", url.Port);
            Assert.AreEqual("/Test.txt", url.Path);
        }

        [Test]
        public void DomainAndFile()
        {
            Url url = Url.Parse("http://test.com/Test.txt");
            Assert.AreEqual("test.com", url.Domain);
            Assert.AreEqual("/Test.txt", url.Path);
        }

        [Test]
        public void DomainAndFileQueryString()
        {
            Url url = Url.Parse("http://test.com/Test.txt?name1=value1");
            Assert.AreEqual("test.com", url.Domain);
            Assert.AreEqual("/Test.txt", url.Path);
            Assert.AreEqual("name1", url.QueryString[0].Name);
            Assert.AreEqual("value1", url.QueryString[0].Value);
        }

        [Test]
        public void DomainAndFileQueryString2()
        {
            Url url = Url.Parse("http://test.com/Test.txt?name1=value1&name2=value2");
            Assert.AreEqual("test.com", url.Domain);
            Assert.AreEqual("/Test.txt", url.Path);
            Assert.AreEqual("name1", url.QueryString[0].Name);
            Assert.AreEqual("value1", url.QueryString[0].Value);
            Assert.AreEqual("name2", url.QueryString[1].Name);
            Assert.AreEqual("value2", url.QueryString[1].Value);
        }
    }
}