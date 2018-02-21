using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Tests.Common;
using Tp.Search.Model.Document;
using Tp.Search.Model.Optimization;
using Tp.Testing.Common.NUnit;

namespace Tp.Search.Tests
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class SearchVersionTests : SearchTestBase
    {
        class FileServiceStub : IFileService
        {
            public IEnumerable<string> GetFiles(string _, string __)
            {
                yield return "Entity_digits_v1.doc";
                yield return "Entity_digits_v1.rec";
                yield return "Entity_digits_v1.idx";
            }
        }

        protected override void OnSetup()
        {
        }

        protected override void OnTearDown()
        {
        }

        [Test]
        public void CheckVersionParsing()
        {
            var t = new DocumentIndexType(DocumentIndexTypeToken.Entity, DocumentIndexDataTypeToken.Digits, "Entity",
                Enumerable.Empty<Enum>(), Enumerable.Empty<Enum>(), 2, new DigitsDocumentIndexDataTypeService(), new FileServiceStub());
            IEnumerable<int> versions = t.GetVersions(AccountName.Empty, new DocumentIndexSetup(string.Empty, 0, 0, 0));
            versions.Should(Be.EquivalentTo(new[] { 1 }), "versions.Should(Be.EquivalentTo(new[] {{i}}))");
        }

        [Test]
        public void RemovePreviousVersions()
        {
            var accountName = AccountName.Empty;
            var documentIndexSetup = new DocumentIndexSetup(string.Empty, 0, 0, 0);
            var activityLoggerFactory = ObjectFactory.GetInstance<IActivityLoggerFactory>();
            var documentIndexOfVersion1 = CreateDocumentIndex(accountName, 1, documentIndexSetup, activityLoggerFactory);
            documentIndexOfVersion1.Shutdown(new DocumentIndexShutdownSetup(forceShutdown: true, cleanStorage: false));
            var documentIndexOfVersion3 = CreateDocumentIndex(accountName, 3, documentIndexSetup, activityLoggerFactory);
            documentIndexOfVersion3.Shutdown(new DocumentIndexShutdownSetup(forceShutdown: true, cleanStorage: false));
            var documentIndexOfVersion2 = CreateDocumentIndex(accountName, 2, documentIndexSetup, activityLoggerFactory);
            IEnumerable<int> versions = documentIndexOfVersion2.Type.GetVersions(accountName, documentIndexSetup).ToList();
            versions.Should(Be.EquivalentTo(new[] { 1, 2, 3 }), "versions.Should(Be.EquivalentTo(new[]{1,2,3}))");
            foreach (var version in versions.Except(new[] { documentIndexOfVersion2.Type.Version }))
            {
                DocumentIndexType indexType = documentIndexOfVersion2.Type.CreateVersion(version);
                var documentIndex = CreateDocumentIndex(accountName, indexType, documentIndexSetup, activityLoggerFactory);
                documentIndex.Shutdown(new DocumentIndexShutdownSetup(forceShutdown: true, cleanStorage: true));
            }
            documentIndexOfVersion2.Type.GetVersions(accountName, documentIndexSetup)
                .Should(Be.EquivalentTo(new[] { documentIndexOfVersion2.Type.Version }),
                    "documentIndexOfVersion2.Type.GetVersions(accountName, documentIndexSetup).Should(Be.EquivalentTo(new[] { documentIndexOfVersion2 .Type.Version}))");
            documentIndexOfVersion2.Shutdown(new DocumentIndexShutdownSetup(forceShutdown: true, cleanStorage: true));
        }

        private IDocumentIndex CreateDocumentIndex(AccountName accountName, int version, DocumentIndexSetup documentIndexSetup,
            IActivityLoggerFactory activityLoggerFactory)
        {
            var indexType = new DocumentIndexType(DocumentIndexTypeToken.Entity, DocumentIndexDataTypeToken.Digits, "Entity",
                Enumerable.Empty<Enum>(), Enumerable.Empty<Enum>(), version, new DigitsDocumentIndexDataTypeService(), new FileService());
            return CreateDocumentIndex(accountName, indexType, documentIndexSetup, activityLoggerFactory);
        }

        private IDocumentIndex CreateDocumentIndex(AccountName accountName, DocumentIndexType documentIndexType,
            DocumentIndexSetup documentIndexSetup, IActivityLoggerFactory activityLoggerFactory)
        {
            return new DocumentIndexTyped(documentIndexType, new PluginContextMock { AccountName = accountName, ProfileName = "Qq" },
                () => { }, documentIndexSetup, activityLoggerFactory, new DocumentIndexOptimizeHintFactory());
        }
    }
}
