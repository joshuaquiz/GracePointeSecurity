using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Util;
using GracePointeSecurity.Library;
using Newtonsoft.Json;
using DynamoDBContextConfig = Amazon.DynamoDBv2.DataModel.DynamoDBContextConfig;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace GracePointeSecurity.ProductKeyEndpoint;

public class Function
{
    public async Task<APIGatewayProxyResponse?> FunctionHandler(APIGatewayProxyRequest? request, ILambdaContext context)
    {
        context.Logger.LogLine(JsonConvert.SerializeObject(request));
        if (string.IsNullOrWhiteSpace(request?.QueryStringParameters["productKey"])
            || string.IsNullOrWhiteSpace(request.QueryStringParameters["orgName"]))
        {
            context.Logger.LogLine("No product key");
            return default;
        }

        var dynamoDbClient = new AmazonDynamoDBClient();
        AWSConfigsDynamoDB.Context.TypeMappings[typeof(ProductKey)] =
            new TypeMapping(typeof(ProductKey), "DriveInfoProductKeys");
        var dbContext = new DynamoDBContext(
            dynamoDbClient,
            new DynamoDBContextConfig
            {
                Conversion = DynamoDBEntryConversion.V2
            });
        var productKeyData = await dbContext.LoadAsync(
            new ProductKey
            {
                InstallGuid = Guid.Parse(request.QueryStringParameters["productKey"]),
                OrgName = request.QueryStringParameters["orgName"]
            });
        var iamClient = new AmazonIdentityManagementServiceClient();
        var iamResults = await iamClient.ListUsersAsync();
        context.Logger.LogLine(JsonConvert.SerializeObject(iamResults));
        var maybeUsers = iamResults?.Users?.Where(x => x.UserName == request.QueryStringParameters["orgName"]).ToList();
        ProductKeyResponse productKeyResponse;
        if (maybeUsers is {Count: 1})
        {
            productKeyResponse = new ProductKeyResponse
            {
                IsAlreadySetup = true
            };
        }
        else
        {
            var createResult = await iamClient.CreateUserAsync(
                new CreateUserRequest
                {
                    UserName = productKeyData.OrgName
                });
            context.Logger.LogLine(JsonConvert.SerializeObject(createResult));
            var iamResult = await iamClient.AddUserToGroupAsync(
                new AddUserToGroupRequest
                {
                    UserName = productKeyData.OrgName,
                    GroupName = "GpDriveInfo"
                });
            context.Logger.LogLine(JsonConvert.SerializeObject(iamResult));
            var accessKey = await iamClient.CreateAccessKeyAsync(
                new CreateAccessKeyRequest
                {
                    UserName = productKeyData.OrgName
                });
            context.Logger.LogLine(JsonConvert.SerializeObject(accessKey));
            productKeyResponse = new ProductKeyResponse
            {
                IsAlreadySetup = false,
                AccessKeyId = accessKey.AccessKey.AccessKeyId,
                SecretAccessKey = accessKey.AccessKey.SecretAccessKey
            };
        }

        return new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = JsonConvert.SerializeObject(productKeyResponse)
        };
    }
}