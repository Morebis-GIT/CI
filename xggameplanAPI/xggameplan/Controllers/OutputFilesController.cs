using System;
using System.Collections.Generic;
using System.Web.Http;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.OutputFiles;
using xggameplan.Filters;
using xggameplan.Model;

namespace xggameplan.Controllers
{
    /// <summary>
    /// Provides details of all possible output files
    /// </summary>
    [RoutePrefix("OutputFiles")]
    public class OutputFilesController : ApiController
    {
        private IOutputFileRepository _outputFileRepository;
        private IMapper _mapper;

        public OutputFilesController(IOutputFileRepository outputFileRepository, IMapper mapper)
        {
            _outputFileRepository = outputFileRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all output files
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("OutputFiles")]
        [Obsolete("This method is obsolete and will soon be removed.")]
        public IEnumerable<OutputFileModel> Get()
        {
            return new List<OutputFileModel>();
        }
    }
}
