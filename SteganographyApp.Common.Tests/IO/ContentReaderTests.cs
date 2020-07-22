using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SteganographyApp.Common.Arguments;
using SteganographyApp.Common.Providers;
using SteganographyApp.Common.IO.Content;
using SteganographyApp.Common.Data;

namespace SteganographyApp.Common.Tests
{

    [TestClass]
    public class ContentReaderTests
    {

        private static readonly int ChunkByteSize = 100;
        private static readonly string FileToEncode = "file_to_encode";
        private static readonly string Password = "password";
        private static readonly bool UseCompression = true;
        private static readonly int DummyCount = 10;
        private static readonly string RandomSeed = "randomSeed";

        private static readonly IInputArguments Arguments = new InputArguments
        {
            ChunkByteSize = ChunkByteSize,
            FileToEncode = FileToEncode,
            Password = Password,
            UseCompression = UseCompression,
            DummyCount = DummyCount,
            RandomSeed = RandomSeed
        }
        .ToImmutable();

        private Mock<IFileProvider> mockFileProvider;
        private Mock<IReadWriteStream> mockReadWriteStream;
        private Mock<IDataEncoderUtil> mockDataEncoderUtil;

        [TestInitialize]
        public void Initialize()
        {
            mockReadWriteStream = new Mock<IReadWriteStream>();

            mockDataEncoderUtil = new Mock<IDataEncoderUtil>();
            Injector.UseProvider(mockDataEncoderUtil.Object);

            mockFileProvider = new Mock<IFileProvider>();
            mockFileProvider.Setup(provider => provider.OpenFileForRead(It.IsAny<string>())).Returns(mockReadWriteStream.Object);
            Injector.UseProvider(mockFileProvider.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Injector.ResetProviders();
        }

        [TestMethod]
        public void TestReadContentChunkFromFile()
        {
            mockReadWriteStream.Setup(stream => stream.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(100);

            string expected = "encoded_value";
            mockDataEncoderUtil.Setup(encoder => encoder.Encode(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns("encoded_value");

            using (var reader = new ContentReader(Arguments))
            {
                string actual = reader.ReadContentChunkFromFile();
                Assert.AreEqual(expected, actual);
            }

            mockReadWriteStream.Verify(stream => stream.Flush(), Times.AtLeastOnce());
            mockReadWriteStream.Verify(stream => stream.Dispose(), Times.Once());
            mockFileProvider.Verify(provider => provider.OpenFileForRead(FileToEncode), Times.Once());
            mockReadWriteStream.Verify(stream => stream.Read(It.IsAny<byte[]>(), 0, ChunkByteSize), Times.Once());
            mockDataEncoderUtil
                .Verify(encoder => encoder.Encode(It.Is<byte[]>(bytes => bytes.Length == ChunkByteSize), It.IsAny<string>(),
                    It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>()),
                Times.Once());
        }

        [TestMethod]
        public void TestReadContentChunkFromFile0BitesAreReadreturnsNull()
        {
            mockReadWriteStream.Setup(stream => stream.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(0);

            using (var contentReader = new ContentReader(Arguments))
            {
                string result = contentReader.ReadContentChunkFromFile();
                Assert.IsNull(result);
            }

            mockReadWriteStream.Verify(stream => stream.Flush(), Times.AtLeastOnce());
            mockReadWriteStream.Verify(stream => stream.Dispose(), Times.Once());
            mockFileProvider.Verify(provider => provider.OpenFileForRead(FileToEncode), Times.Once());
            mockReadWriteStream.Verify(stream => stream.Read(It.IsAny<byte[]>(), 0, ChunkByteSize), Times.Once());
            mockDataEncoderUtil
                .Verify(encoder => encoder.Encode(It.Is<byte[]>(bytes => bytes.Length == ChunkByteSize), It.IsAny<string>(),
                    It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>()),
                Times.Never());
        }

        [TestMethod]
        public void TestReadContentChunkWhenBitsReadIsLessThanChunkSize()
        {
            int alternateByteCount = 10;
            mockReadWriteStream.Setup(stream => stream.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(alternateByteCount);

            using (var contentReader = new ContentReader(Arguments))
            {
                string result = contentReader.ReadContentChunkFromFile();
            }

            mockDataEncoderUtil
                .Verify(encoder => encoder.Encode(It.Is<byte[]>(bytes => bytes.Length == alternateByteCount), It.IsAny<string>(),
                    It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>()),
                Times.Once());
        }

    }

}