using Autofac.Extras.Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace AutomatedDesktopBackgroundLibrary.Test
{
    public class DataStorageBuilderTest
    {
        [Fact]
        void Build_DataStorage_Default()
        {
            using (var mock = AutoMock.GetLoose())
            {
               mock.Mock<IDataStorageBuilder>()
                   .Setup(x => x.Build(Database.Textfile))
                    .Returns(GetDefaultDataStorage());
                var controller = mock.Create<DataStorageBuilder>();
                var expected = GetDefaultDataStorage();
                var actual = controller.Build(Database.Textfile);
                Assert.IsType(expected.GetType(), actual);
                
            }
        }
        [Fact]
        void Build_DataStorage_Default_WhenInvalidInput()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IDataStorageBuilder>()
                    .Setup(x => x.Build(Database.WebServer))
                    .Returns(GetDefaultDataStorage());
                var controller = mock.Create<DataStorageBuilder>();
                var expected = GetDefaultDataStorage();
                var actual = controller.Build(Database.WebServer);
                Assert.IsType(expected.GetType(), actual);

            }
        }

        private IDataStorage GetDefaultDataStorage()
        {
            IDatabaseConnector _database = new TextFileConnector();
            DataStorage dataStorage = new DataStorage()
            {
                Database = _database,
                FavoritedImageFileProcessor = new FavoriteImageProcessor(_database),
                FileCollection = new FileCollection(),
                HatedImageFileProcessor = new HatedImageProcessor(_database),
                ImageFileProcessor = new ImageFileProcessor(_database),
                InterestFileProcessor = new InterestFileProcessor(_database),
                WallPaperFileProcessor = new WallpaperFileProcessor(_database)

            };
            dataStorage.WireUpEvents();
            return dataStorage;


        }
    }
}
