using Autofac.Extras.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace AutomatedDesktopBackgroundLibrary.Test
{
    public class DataStorageTest
    {
        [Fact]
        void CreateAllImagesEntry_ValidCall()
        {
            ImageModel image = new ImageModel();
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IDataStorage>()
                    .Setup(x => x.ImageFileProcessor.CreateEntry(image))
                    .Returns(image);
                var controler = mock.Create<IDataStorage>();
                var actual = controler.ImageFileProcessor.CreateEntry(image);
                var expected = image;
                Assert.Equal(expected, actual);
            }
        }
    }
}
