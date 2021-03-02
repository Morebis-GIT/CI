using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using xggameplan.Filters;
using xggameplan.Model;

namespace xggameplan.Controllers
{
    /// <summary>
    /// API for accessing functional area data
    /// </summary>
    [RoutePrefix("FunctionalAreas")]
    public class FunctionalAreasController : ApiController
    {
        private IFunctionalAreaRepository _functionalAreaRepository;
        private IMapper _mapper;

        public FunctionalAreasController(IFunctionalAreaRepository functionalAreaRepository, IMapper mapper)
        {
            _functionalAreaRepository = functionalAreaRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all functional areas
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("FunctionalAreas")]
        public IEnumerable<FunctionalArea> Get()
        {
            var functionalAreas = _functionalAreaRepository.GetAll();
            return _mapper.Map<List<FunctionalArea>>(functionalAreas);
        }

        /// <summary>
        /// Update functional area fault type status
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("FunctionalAreas")]
        public IHttpActionResult Put([FromBody] List<UpdateFunctionalAreaFaultTypeStatusModel> request)
        {
            if (request is null || !request.Any() || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.Select(c => c.FaultTypeId).Distinct().Count() != request.Count)
            {
                return BadRequest("FaultTypeId must be unique");
            }

            var groupedUpdateList = request.GroupBy(u => u.FunctionalAreaId).Select(grp => grp.ToList()).ToList();
            foreach (var listfunctionalAreaFaulType in groupedUpdateList)
            {
                var functionalArea = _functionalAreaRepository.Find((Guid)listfunctionalAreaFaulType.First().FunctionalAreaId);
                if (functionalArea != null)
                {
                    listfunctionalAreaFaulType.ForEach(fa => functionalArea.FaultTypes.Find(f => f.Id == fa.FaultTypeId).IsSelected = (bool)fa.IsSelected);
                    _functionalAreaRepository.UpdateFaultTypesSelections(functionalArea);
                }
            }

            return Ok();
        }

        //private void GenerateJSON()
        //{
        //    List<FunctionalArea> functionalAreas = _functionalAreaRepository.GetAll().ToList();
        //    foreach (string language in new string[] { "ENG", "ARA" })
        //    {
        //        StringBuilder json = new StringBuilder("[" + Environment.NewLine);
        //        foreach (FunctionalArea functionalArea in functionalAreas)
        //        {
        //            json.Append(GenerateJSON(functionalArea, language));
        //        }
        //        json.AppendLine("]");

        //        System.IO.File.WriteAllText(Path.Combine(string.Format(@"C:\Temp\FunctionalAreas-{0}.json", language)), json.ToString());
        //    }
        //}

        //private string GenerateJSON(FunctionalArea functionalArea, string language)
        //{
        //    Char quotes = '\"';
        //    StringBuilder json = new StringBuilder("\t{" + Environment.NewLine);
        //    json.AppendLine(string.Format("\t\t{0}{1}{0}: ", quotes, functionalArea.Id.ToString()) + "{" + Environment.NewLine +
        //                    string.Format("\t\t\t{0}description{0}: {0}{1}{0},", quotes, functionalArea.Description[language]) + Environment.NewLine +
        //                    string.Format("\t\t\t{0}items{0}: ", quotes) + "{");

        //    foreach(FaultType faultType in functionalArea.FaultTypes)
        //    {
        //        json.Append(string.Format("\t\t\t\t{0}{1}{0}: {0}{2}{0}", quotes, faultType.Id, faultType.Description[language]));
        //        if (faultType != functionalArea.FaultTypes.Last())
        //        {
        //            json.Append(",");
        //        }
        //        json.Append(Environment.NewLine);
        //    }
        //    json.AppendLine("\t\t\t}");
        //    json.AppendLine("\t\t}");
        //    json.AppendLine("\t},");

        //    /*
        //    {
        //        “cbbba5e3 - 298c - 49a7 - b384 - 35a6f9a8f1d0”: {
        //        “description”: “Campaign Requirement”,
        //        “items”: {
        //            “1”: “Outside Campaign Strike Weights”,
        //            “2”: “Programme not in Requirement”
        //          }
        //        }
        //    }
        //    */
        //    return json.ToString();
        //}

        /// <summary>
        /// Loads functional area data to repository from CSV
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="languages"></param>
        /// <param name="funtionalAreaRepository"></param>
        private void LoadFunctionalAreasFromCSV(string inputFile, string[] languages, IFunctionalAreaRepository funtionalAreaRepository)
        {
            if (!File.Exists(inputFile))
            {
                return;
            }

            List<FunctionalArea> functionalAreas = new List<FunctionalArea>();
            Char delimiter = (Char)9;
            using (StreamReader reader = new StreamReader(inputFile))
            {
                int rowCount = 0;
                while (!reader.EndOfStream)
                {
                    rowCount++;
                    if (rowCount > 1)       // Ignore header
                    {
                        string[] values = reader.ReadLine().Split(delimiter);
                        FaultType faultType = new FaultType()
                        {
                            Id = Convert.ToInt32(values[0])
                        };
                        foreach (string language in languages)
                        {
                            faultType.Description.Add(language, values[1].Trim());
                        }
                        string functionalAreaDescription = values[2].Trim();

                        FunctionalArea functionalArea = null;
                        foreach (FunctionalArea currentFunctionalArea in functionalAreas)
                        {
                            if (currentFunctionalArea.Description[languages[0]].ToUpper() == functionalAreaDescription.ToUpper())
                            {
                                functionalArea = currentFunctionalArea;
                                break;
                            }
                        }

                        if (functionalArea == null)    // New functional area
                        {
                            functionalArea = new FunctionalArea()
                            {
                                Id = Guid.NewGuid()
                            };
                            foreach (string language in languages)
                            {
                                functionalArea.Description.Add(language, functionalAreaDescription);
                            }
                            functionalAreas.Add(functionalArea);
                        }
                        functionalArea.FaultTypes.Add(faultType);
                    }
                }
                reader.Close();
            }

            // Save
            foreach (FunctionalArea functionalArea in functionalAreas)
            {
                funtionalAreaRepository.Add(functionalArea);
            }
        }
    }
}

