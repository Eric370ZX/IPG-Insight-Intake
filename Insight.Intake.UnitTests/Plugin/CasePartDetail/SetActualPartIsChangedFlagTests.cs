using FakeXrmEasy;
using Insight.Intake.Plugin.CasePartDetail;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.CasePartDetail
{
    public class SetActualPartIsChangedFlagTests : PluginTestsBase
    {
        [Fact]
        public void SetIsChangedFlagWhenActualPartHasBeenChanged()
        {
            var fakedContext = new XrmFakedContext();           

            Intake.SalesOrderDetail salesOrderDetail = new Intake.SalesOrderDetail().Fake().RuleFor(x => x.Quantity, x => 50);

            Intake.ipg_casepartdetail casePart = new Intake.ipg_casepartdetail().Fake().WithClaimRelatedData()
                                                                                       .RuleFor(x => x.ipg_IsChanged, x => false)
                                                                                       .RuleFor(x => x.ipg_SalesOrderDetail, x => salesOrderDetail.ToEntityReference());

            var listForInit = new List<Entity>() { salesOrderDetail, casePart };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", casePart } };

            var postImage = new EntityImageCollection();
            postImage.Add("PostImage", casePart);
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_casepartdetail.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = postImage,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<SetActualPartIsChangedFlag>(pluginContext);

            var resultPart = (from part in fakedContext.CreateQuery<ipg_casepartdetail>()
                                          where part.ipg_casepartdetailId == casePart.Id
                                          select part).FirstOrDefault();

            Assert.Equal(true, resultPart.ipg_IsChanged);

        }

        [Fact]
        public void SetIsChangedFlagWhenOrderHasBeenDeleted()
        {
            var fakedContext = new XrmFakedContext();

            Intake.SalesOrder order = new Intake.SalesOrder().Fake();

            Intake.ipg_casepartdetail casePart = new Intake.ipg_casepartdetail().Fake().WithClaimRelatedData()
                                                                                       .RuleFor(x => x.ipg_IsChanged, x => false)
                                                                                       .WithOrderRef(order);
            var listForInit = new List<Entity>() { order, casePart };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", order.ToEntityReference() } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Delete",
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = SalesOrder.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<SetActualPartIsChangedFlag>(pluginContext);

            var resultPart = (from part in fakedContext.CreateQuery<ipg_casepartdetail>()
                              where part.ipg_casepartdetailId == casePart.Id
                              select part).FirstOrDefault();

            Assert.Equal(true, resultPart.ipg_IsChanged);

        }
    }
}
