using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace xggameplan.Services
{
    /// <summary>
    /// Custom Swagger document filter to allow control over what is documented.
    /// </summary>
    public class SwaggerDocumentFilter : IDocumentFilter
    {
        private string[] _tags = new string[0]; // new string[] { null, "Public", "Internal" };       // Tags to allow (null=Any tag)

        /// <summary>
        /// Class constructor
        /// </summary>
        public SwaggerDocumentFilter()  //params string[] tags)
        {
            //_tags = tags;
        }

        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {            
            List<ApiDescription> list = apiExplorer.ApiDescriptions.OrderBy(x => x.Route.RouteTemplate).ToList();
                foreach (var apiDescription in list)
                {
                    //string controller = apiDescription.GetControllerAndActionAttributes<System.Web.Http.RoutePrefixAttribute>().Any() ? "/" + apiDescription.GetControllerAndActionAttributes<System.Web.Http.RoutePrefixAttribute>().Single().Prefix : "Internal";
                    //System.Diagnostics.Debug.WriteLine(string.Format("API={0}, {1}, Template={2}, {3}, {4}", controller, apiDescription.HttpMethod, apiDescription.Route.RouteTemplate, apiDescription.ActionDescriptor.ActionName, apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName));
               

                    //if (!apiDescription.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<DocsAttribute>().Any() && !apiDescription.ActionDescriptor.GetCustomAttributes<DocsAttribute>().Any()) continue;
                    //var route = "/" + apiDescription.Route.RouteTemplate.TrimEnd('/');
                    //swaggerDoc.paths.Remove(route);
                
                    bool show = true;
                    if (_tags.Length > 0)    // Filter on DocsAttribute.Tag
                    {
                        List<DocsTagsAttribute> controllerAttributes = apiDescription.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<DocsTagsAttribute>().ToList().Where(attribute => attribute.ContainsAnyTag(_tags)).ToList();
                        List<DocsTagsAttribute> methodAttributes = apiDescription.ActionDescriptor.GetCustomAttributes<DocsTagsAttribute>().ToList().Where(attribute => attribute.ContainsAnyTag(_tags)).ToList();
                        if (controllerAttributes.Count == 0 && methodAttributes.Count == 0)
                        {
                            show = false;
                        }
                    }

                    if (!show)                            
                    {
                        var route = "/" + apiDescription.Route.RouteTemplate.TrimEnd('/');
                        swaggerDoc.paths.Remove(route);
                    }                    
            }                        
        }
    }
}