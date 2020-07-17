using NewarePassPort.Common.Enums;

namespace NewWarePassPort.Models
{
    public class ApiResult
    {
        public ApiResult()
        {
            this.Success = true;
            this.ResultCode = ResultCode.Success;
        }

        public bool Success { get; set; }
        public ResultCode ResultCode { get; set; }
        public string ErrorMessage { get; set; }
        public object Data { get; set; }

        public void Error(string errorMessage, ResultCode code = ResultCode.API_Abnormal)
        {
            this.Success = false;
            this.ErrorMessage = errorMessage;
            this.ResultCode = code;
        }

        public void Error(ResultCode code)
        {
            this.Success = false;
            this.ErrorMessage = code.ToString();
            this.ResultCode = code;
        }

        public void Error(string errorMessage)
        {
            this.Success = false;
            this.ErrorMessage = errorMessage;
        }
    }
}
