using System;
namespace N_Dexed.Deployment.Common.Domain.Messaging
{
    public class ErrorInfo
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public object Data { get; set; }
        public ErrorInfo InnerError { get; set; }

        /// <summary>
        /// Unwraps all inner exceptions and returns the last one
        /// </summary>
        /// <returns></returns>
        public ErrorInfo GetInnermostError()
        {
            ErrorInfo returnValue = this;

            if (this.InnerError != null)
            {
                returnValue = this.InnerError.GetInnermostError();
            }

            return returnValue;
        }

        public ErrorInfo()
        {

        }

        public ErrorInfo(Exception ex)
        {
            this.Data = ex.Data;
            this.ErrorCode = ex.HResult.ToString();
            this.Message = ex.Message;
            this.StackTrace = ex.StackTrace;

            if (ex.InnerException != null)
            {
                this.InnerError = new ErrorInfo(ex.InnerException);
            }
        }
    }
}
