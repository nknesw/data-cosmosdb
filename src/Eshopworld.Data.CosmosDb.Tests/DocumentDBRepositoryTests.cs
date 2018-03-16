using System;
using System.Linq;
using Xunit;

namespace Eshopworld.Data.CosmosDb.Tests
{
    public class DocumentDBRepositoryTests
    {
        public DocumentDBRepositoryTests()
        {
            var endPoint = Environment.GetEnvironmentVariable("endPoint");
            var authKey = Environment.GetEnvironmentVariable("authKey");
            var databaseId = Environment.GetEnvironmentVariable("databaseId");
            var collectionId = Environment.GetEnvironmentVariable("collectionId");

            _documentDBRepository = new DocumentDBRepository<Dummy>();
            _documentDBRepository.Initialize(endPoint, authKey, databaseId, collectionId);
        }

        public void Dispose()
        {
            _documentDBRepository.Client?.Dispose();
        }

        private readonly DocumentDBRepository<Dummy> _documentDBRepository;

        [Fact]
        public void ItemIsCreated()
        {
            var id = Guid.NewGuid();
            _documentDBRepository.CreateItemAsync(new Dummy
            {
                Id = id,
                Name = "DUMMY01"
            }).Wait();

            var item = _documentDBRepository.GetItemAsync(id.ToString()).Result;
            Assert.Equal(id, item.Id);
            Assert.Equal("DUMMY01", item.Name);
            _documentDBRepository.DeleteItemAsync(id.ToString()).Wait();
        }

        [Fact]
        public void ItemIsDeleted()
        {
            var id = Guid.NewGuid();
            _documentDBRepository.CreateItemAsync(new Dummy
            {
                Id = id,
                Name = "DUMMY03"
            }).Wait();

            _documentDBRepository.DeleteItemAsync(id.ToString()).Wait();
            var item = _documentDBRepository.GetItemAsync(id.ToString()).Result;
            Assert.Null(item);
        }

        [Fact]
        public void ItemIsRetrieved()
        {
            var id = Guid.NewGuid();
            _documentDBRepository.CreateItemAsync(new Dummy
            {
                Id = id,
                Name = "DUMMY02"
            }).Wait();

            var item = _documentDBRepository.GetItemAsync(id.ToString()).Result;
            Assert.Equal(id, item.Id);
            Assert.Equal("DUMMY02", item.Name);
            _documentDBRepository.DeleteItemAsync(id.ToString()).Wait();
        }

        [Fact]
        public void ItemIsUpdated()
        {
            var id = Guid.NewGuid();
            _documentDBRepository.CreateItemAsync(new Dummy
            {
                Id = id,
                Name = "DUMMY04"
            }).Wait();

            var updatedDummy = new Dummy {Id = id, Name = "UPDATED"};
            _documentDBRepository.UpdateItemAsync(id.ToString(), updatedDummy).Wait();

            var item = _documentDBRepository.GetItemAsync(id.ToString()).Result;
            Assert.Equal("UPDATED", item.Name);
            _documentDBRepository.DeleteItemAsync(id.ToString()).Wait();
        }

        [Fact]
        public void ItemsAreRetrieved()
        {
            _documentDBRepository.CreateItemAsync(new Dummy
            {
                Id = Guid.NewGuid(),
                Name = "DUMMY05"
            }).Wait();

            _documentDBRepository.CreateItemAsync(new Dummy
            {
                Id = Guid.NewGuid(),
                Name = "DUMMY05"
            }).Wait();

            var items = _documentDBRepository.GetItemsAsync(d => d.Name == "DUMMY05").Result.ToList();
            Assert.Equal(2, items.Count);
            foreach (var item in items) _documentDBRepository.DeleteItemAsync(item.Id.ToString()).Wait();
        }
    }
}