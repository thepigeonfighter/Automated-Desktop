using Autofac.Extras.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using AutomatedDesktopBackgroundLibrary;

namespace AutomatedDesktopBackgroundLibrary.Test
{
    public class ImageFileProcessorTest
    {
        private static string imageFile = StringExtensions.StringExtensions.FullFilePath("Images.csv");
        [Fact]
        void Load_ImageFile_ValidCall()
        {
            //Get loose = if method is called = sucess
            // Get Strict =  only one method must be called
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IDatabaseConnector>()
                    .Setup(x => x.Load<ImageModel>(imageFile))
                    .Returns(GetSampleImages());
                var controller = mock.Create<ImageFileProcessor>();
                var expected = GetSampleImages();
                List<ImageModel> actual = controller.LoadAllEntries();
                Assert.True(actual != null);
                Assert.Equal(expected.Count, actual.Count);
            }
        }
        [Fact]
        void Load_ImageFile_ValidateItemEquality()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IDatabaseConnector>()
                    .Setup(x => x.Load<ImageModel>(imageFile))
                    .Returns(GetSampleImages());
                var controller = mock.Create<ImageFileProcessor>();
                var expected = GetSampleImages();
                List<ImageModel> actual = controller.LoadAllEntries();
                Assert.Equal(expected, actual);
            }
        }
        [Fact]
        void OverwriteImageFile_ValidateCall()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var images = GetSampleImages();
                mock.Mock<IDatabaseConnector>()
                    .Setup(x => x.SaveToFile(images, imageFile));
                var controller = mock.Create<ImageFileProcessor>();
                controller.OverwriteEntries(images);
                mock.Mock<IDatabaseConnector>()
                    .Verify(x => x.SaveToFile(images, imageFile), Times.Exactly(1));
             
                
                 
            }
        }
        [Fact]
        void CreateImageEntry_ValidateCall()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var image = GetSampleImages().First();
                mock.Mock<IDatabaseConnector>()
                    .Setup(x => x.CreateEntry(image, imageFile));
                var controller = mock.Create<ImageFileProcessor>();
                controller.CreateEntry(image);
                mock.Mock<IDatabaseConnector>()
                    .Verify(x => x.CreateEntry(image, imageFile), Times.Exactly(1));



            }
        }
        [Fact]
        void DeleteImageEntry_ValidateCall()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var image = GetSampleImages().First();
                mock.Mock<IDatabaseConnector>()
                    .Setup(x => x.Delete(image, imageFile));
                var controller = mock.Create<ImageFileProcessor>();
                controller.DeleteEntry(image);
                mock.Mock<IDatabaseConnector>()
                    .Verify(x => x.Delete(image, imageFile), Times.Exactly(1));



            }
        }
        [Fact]
        void Update_ImageFiles_ValidateCall()
        {
            //Get loose = if method is called = sucess
            // Get Strict =  only one method must be called
            using (var mock = AutoMock.GetLoose())
            {

                
                mock.Mock<IDatabaseConnector>()
                    .Setup(x => x.Update(GetUpdatedImages(), imageFile));
                var controller = mock.Create<ImageFileProcessor>();
                var images = GetUpdatedImages();
                controller.UpdateEntries(images);
                mock.Mock<IDatabaseConnector>()
                   .Verify(x => x.Update(images, imageFile), Times.Exactly(1));




            }
        }
        private List<ImageModel> GetSampleImages()
        {
            List<ImageModel> images = new List<ImageModel>();
            images.Add(new ImageModel() { Id = 1, InterestId = 2, LocalUrl = "at your mom's house", Name = "Monkey Photo" });
            images.Add(new ImageModel() { Id = 2, InterestId = 2, LocalUrl = "at your mom's house1", Name = "Monkey Photo1" });
            images.Add(new ImageModel() { Id = 3, InterestId = 2, LocalUrl = "at your mom's house2", Name = "Monkey Photo2" });
            images.Add(new ImageModel() { Id = 4, InterestId = 2, LocalUrl = "at your mom's house3", Name = "Monkey Photo3" });
            return images;
        }
        private List<ImageModel> GetUpdatedImages()
        {
            List<ImageModel> images = new List<ImageModel>();
            images.Add(new ImageModel() { Id = 1, InterestId = 2, LocalUrl = "at your mom's house", Name = "Monkey Photo" });
            images.Add(new ImageModel() { Id = 2, InterestId = 3, LocalUrl = "at your mom's house1", Name = "Monkey Photo1" });
            images.Add(new ImageModel() { Id = 3, InterestId = 4, LocalUrl = "at your mom's house2", Name = "Monkey Photo2" });
            images.Add(new ImageModel() { Id = 4, InterestId = 2, LocalUrl = "at your mom's house3", Name = "Monkey Photo3" });
            return images;
        }

    }
}
