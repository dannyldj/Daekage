using System;
using System.Collections;
using System.IO;
using System.Windows;
using Daekage.Contracts.Services;
using Daekage.Core.Contracts.Services;
using Daekage.Models;

namespace Daekage.Services
{
    public class PersistAndRestoreService : IPersistAndRestoreService
    {
        private readonly IFileService _fileService;
        private readonly AppConfig _appConfig;
        private readonly string _localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public PersistAndRestoreService(IFileService fileService, AppConfig appConfig)
        {
            _fileService = fileService;
            _appConfig = appConfig;
        }

        public void PersistData()
        {
            string folderPath = Path.Combine(_localAppData, _appConfig.ConfigurationsFolder);
            string fileName = _appConfig.AppPropertiesFileName;
            _fileService.Save(folderPath, fileName, Application.Current.Properties);
        }

        public void RestoreData()
        {
            string folderPath = Path.Combine(_localAppData, _appConfig.ConfigurationsFolder);
            string fileName = _appConfig.AppPropertiesFileName;
            var properties = _fileService.Read<IDictionary>(folderPath, fileName);
            if (properties != null)
            {
                foreach (DictionaryEntry property in properties)
                {
                    Application.Current.Properties.Add(property.Key, property.Value);
                }
            }
        }
    }
}
