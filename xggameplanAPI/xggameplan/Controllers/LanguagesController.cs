using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.Languages;
using xggameplan.Filters;
using xggameplan.Model;

namespace xggameplan.Controllers
{
    /// <summary>
    /// API for providing language details
    /// 
    /// TODO: Move to xG Core
    /// </summary>
    [RoutePrefix("Languages")]
    public class LanguagesController : ApiController
    {
        private ILanguageRepository _languageRepository;
        private IMapper _mapper;

        public LanguagesController(ILanguageRepository languageRepository, IMapper mapper)
        {
            _languageRepository = languageRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all languages
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("Languages")]
        public IEnumerable<LanguageModel> Get()
        {
            var languages = _languageRepository.GetAll();
            
            if (!languages.Any())    // None, load from CSV
            {                
                LoadLanguagesFromCSV(System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("/App_Data"), "Languages.txt"), _languageRepository);                
                languages = _languageRepository.GetAll();
            }

            return _mapper.Map<List<LanguageModel>>(languages);
        }

        /// <summary>
        /// Loads language data to repository from CSV
        /// </summary>
        /// <param name="file"></param>
        /// <param name="languageRepository"></param>
        private void LoadLanguagesFromCSV(string file, ILanguageRepository languageRepository)
        {            
            if (!File.Exists(file))
            {
                return;
            }

            List<Language> languages = new List<Language>();
            using (StreamReader reader = new StreamReader(file))
            {
                Char delimiter = (Char)9;
                int rowCount = 0;
                while (!reader.EndOfStream)
                {
                    rowCount++;
                    if (rowCount > 1)       // Ignore header
                    {
                        string[] values = reader.ReadLine().Split(delimiter);
                        Language language = new Language()
                        {
                            Alpha3b = values[0].Trim().ToUpper(),
                            Alpha2 = values[1].Trim().ToUpper(),
                            Description = values[2].Trim()
                        };
                        languages.Add(language);
                    }
                }
                reader.Close();
            }

            // Save
            languages.ForEach(language => languageRepository.Add(language));
        }
    }
}
