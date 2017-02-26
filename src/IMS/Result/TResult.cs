using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS
{
    public class Result<T> : Result
    {
        public Result()
        {
        }
        public Result(ResultStatus status)
            : base(status)
        {
        }
        public Result(ResultStatus status, T data)
            : base(status)
        {
            this.Data = data;
        }

        public Result(ResultStatus status, List<T> dataList)
            : base(status)
        {
            this.DataList = dataList;
        }
        public Result(ResultStatus status,string msg, List<T> dataList)
            : base(status)
        {
            this.DataList = dataList;
            this.Msg = msg;
        }

        public T Data { get; set; }
        private IEnumerable<T> DataList { get; set; }
    }
}
