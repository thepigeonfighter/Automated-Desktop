using System.Collections.Generic;
using Xunit;

namespace AutomatedDesktopBackgroundLibrary.Test
{
    public class FileCollectionExtensionsTests
    {
        [Fact]
        private void GetAllDownloadedImages_NormalCall()
        {
            IFileCollection fileCollection = GetFullSampleFileCollection();
            IFilteredFileResult result = fileCollection.GetAllDownloadedImages();
            Assert.True(result.SucessfulQuery);
            Assert.Equal(4, result.GetResults().Count);
        }

        [Fact]
        private void GetAllDownloadedImages_WhenThereAreNone()
        {
            IFileCollection fileCollection = new FileCollection();
            IFilteredFileResult result = fileCollection.GetAllDownloadedImages();
            Assert.True(result.SucessfulQuery);
            Assert.Empty(result.GetResults());
        }

        [Fact]
        private void GetAllDownloadedImages_WhenThereAreFilesButNoneAreDownloaded()
        {
            IFileCollection fileCollection = GetSampleFileCollection_JustHatedImages();
            IFilteredFileResult result = fileCollection.GetAllDownloadedImages();
            Assert.True(result.SucessfulQuery);
            Assert.Empty(result.GetResults());
        }

        [Fact]
        private void GetLastDownloadedImages_ValidateResult()
        {
            IFileCollection fileCollection = GetFullSampleFileCollection();
            IFilteredFileResult result = fileCollection.GetLastImageDownloaded();

            var images = GetSampleFavoriteImages();
            var lastDownloaded = images[2];
            Assert.True(result.SucessfulQuery);
            Assert.Equal(lastDownloaded, result.SelectedResult);
        }

        [Fact]
        private void GetAllAssociatedImagesByInterest_ValidCall()
        {
            IFileCollection file = GetFullSampleFileCollection();
            IFilteredFileResult actual = file.GetAllImagesAssociatedByInterest("Test2");
            const int expected = 20;
            Assert.Equal(expected, actual.GetResults().Count);
        }

        [Fact]
        private void GetInterestByName_EnsureSame_ValidCall()
        {
            IFileCollection sample = GetFullSampleFileCollection();
            IFilteredFileResult result = sample.GetInterestByName("Test1");
            var expected = GetSampleInterest()[0];
            var actual = (InterestModel)result.SelectedResult;
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Id, actual.Id);
        }

        [Fact]
        private void GetInterestByName_EnsureNull_InvalidCall()
        {
            IFileCollection sample = GetFullSampleFileCollection();
            IFilteredFileResult result = sample.GetInterestByName("Test01");

            var actual = (InterestModel)result.SelectedResult;
            Assert.Null(actual);
        }

        [Fact]
        private void GetNextPageInCollection_ValidCall()
        {
            IFileCollection sample = GetFullSampleFileCollection();
            var actual = sample.GetNextPageInCollection("Test2");
            const int expected = 3;

            Assert.Equal(expected, actual);
        }

        #region SampleData

        private IFileCollection GetFullSampleFileCollection()
        {
            IFileCollection sample = new FileCollection()
            {
                AllImages = GetSampleImages(),
                FavoriteImages = GetSampleFavoriteImages(),
                HatedImages = GetSampleHatedImages(),
                AllInterests = GetSampleInterest()
            };

            return sample;
        }

        private IFileCollection GetSampleFileCollection_JustHatedImages()
        {
            IFileCollection sample = new FileCollection()
            {
                HatedImages = GetSampleHatedImages()
            };

            return sample;
        }

        private List<ImageModel> GetSampleImages()
        {
            List<ImageModel> images = new List<ImageModel>
            {
                new ImageModel() { Id = 1, InterestId = 2, IsDownloaded = true, LocalUrl = "at your mom's house", Name = "Monkey Photo" },
                new ImageModel() { Id = 2, InterestId = 2, IsDownloaded = true, LocalUrl = "at your mom's house1", Name = "Monkey Photo1" },
                new ImageModel() { Id = 3, InterestId = 2, IsDownloaded = false, LocalUrl = "at your mom's house2", Name = "Monkey Photo2" },
                new ImageModel() { Id = 4, InterestId = 2, IsDownloaded = false, LocalUrl = "at your mom's house3", Name = "Monkey Photo3" }
            };
            return images;
        }

        private List<ImageModel> GetSampleFavoriteImages()
        {
            List<ImageModel> images = new List<ImageModel>
            {
                new ImageModel() { Id = 5, InterestId = 2, IsDownloaded = true, LocalUrl = "at your mom's house", Name = "Monkey Photo" },
                new ImageModel() { Id = 6, InterestId = 2, IsDownloaded = false, LocalUrl = "at your mom's house1", Name = "Monkey Photo1" },
                new ImageModel() { Id = 7, InterestId = 2, IsDownloaded = true, LocalUrl = "at your mom's house2", Name = "Monkey Photo2" },
                new ImageModel() { Id = 8, InterestId = 2, IsDownloaded = false, LocalUrl = "at your mom's house3", Name = "Monkey Photo3" }
            };
            return images;
        }

        private List<ImageModel> GetSampleHatedImages()
        {
            List<ImageModel> images = new List<ImageModel>
            {
                new ImageModel() { Id = 9, InterestId = 2, IsDownloaded = false, LocalUrl = "at your mom's house", Name = "Monkey Photo" },
                new ImageModel() { Id = 10, InterestId = 2, IsDownloaded = false, LocalUrl = "at your mom's house1", Name = "Monkey Photo1" },
                new ImageModel() { Id = 11, InterestId = 2, IsDownloaded = false, LocalUrl = "at your mom's house2", Name = "Monkey Photo2" },
                new ImageModel() { Id = 12, InterestId = 2, IsDownloaded = false, LocalUrl = "at your mom's house3", Name = "Monkey Photo3" },
                new ImageModel() { Id = 13, InterestId = 2, IsDownloaded = false, LocalUrl = "at your mom's house", Name = "Monkey Photo" },
                new ImageModel() { Id = 14, InterestId = 2, IsDownloaded = false, LocalUrl = "at your mom's house1", Name = "Monkey Photo1" },
                new ImageModel() { Id = 15, InterestId = 2, IsDownloaded = false, LocalUrl = "at your mom's house2", Name = "Monkey Photo2" },
                new ImageModel() { Id = 16, InterestId = 2, IsDownloaded = false, LocalUrl = "at your mom's house3", Name = "Monkey Photo3" },
                new ImageModel() { Id = 17, InterestId = 2, IsDownloaded = false, LocalUrl = "at your mom's house", Name = "Monkey Photo" },
                new ImageModel() { Id = 18, InterestId = 2, IsDownloaded = false, LocalUrl = "at your mom's house1", Name = "Monkey Photo1" },
                new ImageModel() { Id = 19, InterestId = 2, IsDownloaded = false, LocalUrl = "at your mom's house2", Name = "Monkey Photo2" },
                new ImageModel() { Id = 20, InterestId = 2, IsDownloaded = false, LocalUrl = "at your mom's house3", Name = "Monkey Photo3" }
            };
            return images;
        }

        private List<InterestModel> GetSampleInterest()
        {
            List<InterestModel> interests = new List<InterestModel>
            {
                new InterestModel() { Id = 1, Name = "Test1" },
                new InterestModel() { Id = 2, Name = "Test2", TotalImages = 100, TotalPages = 10 },
                new InterestModel() { Id = 3, Name = "Test3" },
                new InterestModel() { Id = 4, Name = "Test4" },
                new InterestModel() { Id = 5, Name = "Test5" }
            };
            return interests;
        }

        #endregion SampleData
    }
}