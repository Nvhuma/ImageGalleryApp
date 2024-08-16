using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

using System.Collections.Generic;
using System.Linq;

namespace api;
public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var uploadFileParams = context.ApiDescription.ParameterDescriptions
            .Where(p => p.Type == typeof(IFormFile));

        if (uploadFileParams.Any())
        {
            operation.Parameters.Clear();

            operation.Parameters = new List<OpenApiParameter>
            {
                new OpenApiParameter
                {
                    Name = "Title",
                    In = ParameterLocation.Query,
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
                },
                new OpenApiParameter
                {
                    Name = "Description",
                    In = ParameterLocation.Query,
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
                },
            };

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["Image"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary"
                                }
                            },
                            Required = new HashSet<string> { "Image" }
                        }
                    }
                }
            };
        }
    }
}