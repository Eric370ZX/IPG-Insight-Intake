using FakeXrmEasy;
using FakeXrmEasy.FakeMessageExecutors;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    //Use AddExecutionMock instead
    public class FakeMessageExecutor<request,response> : IFakeMessageExecutor where request: OrganizationRequest, new() where response: OrganizationResponse, new()
    {
        private Func<request, bool> _canExecute;
        private Func<request, XrmFakedContext, response> _execute;

       public FakeMessageExecutor(Func<request, XrmFakedContext, response> execute = null, Func<request, bool> canExecute = null)
       {
            _canExecute = canExecute;
            _execute = execute;
       }
        public bool CanExecute(OrganizationRequest request)
        {
            return request is request && (_canExecute == null || _canExecute(request as request));
        }

        public OrganizationResponse Execute(OrganizationRequest request, XrmFakedContext ctx)
        {
            return _execute == null ? new response() : _execute(request as request, ctx);
        }

        public Type GetResponsibleRequestType()
        {
            throw new NotImplementedException();
        }
    }
}
