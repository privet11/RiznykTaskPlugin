using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TestTaskPlugin
{
    public class FollowupPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));


            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];

                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    List<Guid> ListAccountId = new List<Guid>();

                    QueryExpression query = new QueryExpression("account");
                    query.ColumnSet.AddColumns("name", "accountid");
                    EntityCollection result1 = service.RetrieveMultiple(query);

                    foreach (var a in result1.Entities)
                    {
                        ListAccountId.Add((Guid)a.Attributes["accountid"]);
                    }
                    foreach (var id in ListAccountId)
                    {
                        Entity testAccount = new Entity("account")
                        {
                            Id = id
                        };
                        if (testAccount.Attributes.Contains("telephone1"))
                        {
                            testAccount.Attributes["cr195_isvalidated"] = true;
                            testAccount.Attributes["telephone1"] = "3333";
                        }
                        else
                        {
                            testAccount.Attributes["cr195_isvalidated"] = false;
                        }
                        service.Update(testAccount);
                    }

                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("FollowUpPlugin: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
    }
