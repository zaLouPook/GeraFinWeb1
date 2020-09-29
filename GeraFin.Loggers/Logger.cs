using System;
using System.Data.SqlClient;
using GeraFin.DAL.Helpers;
using GeraFin.Models.Logging;
using Microsoft.Extensions.Configuration;

namespace GeraFin.Loggers
{
    public class Logging
    {
        public void LogExceptionInternal(int UserId, int ErrorId, string Page, string Service, string Exception, IConfiguration configuration)
        {
            SqlHelper helper = new SqlHelper(configuration.GetConnectionString("DevEnvR"));
            SqlParameter[] Parameters = new SqlParameter[5];
            Parameters.SetValue(helper.CreateParameter("@DT", UserId, 0), 0);
            Parameters.SetValue(helper.CreateParameter("@ENDDATE", ErrorId, 0), 1);
            Parameters.SetValue(helper.CreateParameter("@BranchId", Page, 0), 2);
            Parameters.SetValue(helper.CreateParameter("@BranchId", Service, 0), 3);
            Parameters.SetValue(helper.CreateParameter("@BranchId", Exception, 0), 4);

            helper.Insert("GeraFin.stp_LogExceptionInternal", System.Data.CommandType.StoredProcedure, Parameters);
        }

        public void RequestLog(int ResponseId, string Request, string RequestMethod)
        {
            DateTime now = new DateTime();
            RequestLog req = new RequestLog();
            req.RequestId = ResponseId;
            req.Request = Request;
            req.RequestMethod = RequestMethod;
            req.Date = now;

            //Code To Save Exceptopm
        }

        public void ResponseLog(int RequestId, string Respone, string ResponetMethod, string ResponeErrorId, string ResponeErrorMessage)
        {
            DateTime now = new DateTime();
            ResponseLog res = new ResponseLog();
            res.RequestId = RequestId;
            res.Respone = Respone;
            res.ResponetMethod = ResponetMethod;
            res.ResponeErrorId = ResponeErrorId;
            res.ResponeErrorMessage = ResponeErrorMessage;
            res.Date = now;

            // Code To Save Exception
        }
    }
}