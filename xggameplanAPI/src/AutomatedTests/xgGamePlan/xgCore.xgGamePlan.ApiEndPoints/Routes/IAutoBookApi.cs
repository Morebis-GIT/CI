using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.AutoBook;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IAutoBookApi
    {
        [Get("/AutoBooks/InstanceConfigurations")]
        Task<IEnumerable<AutoBookInstanceConfiguration>> GetAutoBookInstanceConfigurations();
        
        [Get("/AutoBooks/Settings")]
        Task<AutoBookSettingsModel> GetAutoBookSettings();

        [Put("/AutoBooks/Settings")]
        Task UpdateAutoBookSettings(AutoBookSettingsModel autoBookSettingsModel);

        [Get("/AutoBooks")]
        Task<IEnumerable<AutoBookModel>> GetAll();

        [Get("/AutoBooks/{id}")]
        Task<AutoBookModel> GetById(string id);

        [Post("/AutoBooks")]
        Task<AutoBookModel> Create(CreateAutoBookModel model);

        [Delete("/AutoBooks/{id}")]
        Task Delete(string id);

        [Get("/AutoBooks/{id}/scenarios/{scenarioId}/logs")]
        Task<string> GetScenarioLogs(string id, Guid scenarioId);

        [Get("/AutoBooks/{id}/scenarios/{scenarioId}/audittrail")]
        Task<string> GetScenarioAuditTrail(string id, Guid scenarioId);
    }
}
