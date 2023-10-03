using NUnit.Framework;
using udpwinformgit;
using Moq;

namespace TestProjectforUDP
{
    class Presenter_test
    {

        [Test]
        public void Test_IsValidMAC_validCase()
        {
            Assert.AreEqual(true, Presenter.IsValidMAC("90-90-90-90-90-90"));
        }

        [Test]
        public void Test_IsValidMAC_incompliteCase1()
        {
            Assert.AreEqual(false, Presenter.IsValidMAC("90-90-90-90-90-9"));
        }

        [Test]
        public void Test_IsValidMAC_incompliteCase2()
        {
            Assert.AreEqual(false, Presenter.IsValidMAC("90"));
        }

        [Test]
        public void Test_IsValidMAC_wrongLettersCase()
        {
            Assert.AreEqual(false, Presenter.IsValidMAC("J0-90-90-90-90-90"));
        }


        [Test]
        public void Test_IsValidIP_validCase()
        {
            Assert.AreEqual(true, Presenter.IsValidIP("192.168.1.1"));
        }

        [Test]
        public void Test_IsValidIP_incompliteCase1()
        {
            Assert.AreEqual(false, Presenter.IsValidIP("192.168.1"));
        }

        [Test]
        public void Test_IsValidIP_incompliteCase2()
        {
            Assert.AreEqual(false, Presenter.IsValidIP("90"));
        }

        [Test]
        public void Test_IsValidIP_wrongNumberCase()
        {
            Assert.AreEqual(false, Presenter.IsValidIP("392.168.1.1"));
        }

        //[Test]
        //public void Test_TextChangedIP_validCase()
        //{
        //    var viewMock = new Mock<IView>();
        //    var p = new Presenter(viewMock.Object);
        //    Assert.AreEqual(true, p.TextChangedIP);
        //}

        //TODO add other tests
    }
}
