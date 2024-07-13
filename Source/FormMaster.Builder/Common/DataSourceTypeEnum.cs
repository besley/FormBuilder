using System;
using System.Collections.Generic;
using System.Text;

namespace FormMaster.Builder.Common
{
    /// <summary>
    /// 数据源类型
    /// </summary>
    public enum DataSourceTypeEnum
    {
        /// <summary>
        /// 自定义
        /// </summary>
        Customized = 1,

        /// <summary>
        /// 本地数据表
        /// </summary>
        LocalDataTable = 2,

        /// <summary>
        /// SQL
        /// </summary>
        SQL = 3,

        /// <summary>
        /// Store Procedure
        /// </summary>
        StoreProcedure = 4,

        /// <summary>
        /// WebAPI
        /// </summary>
        WebAPIHttp = 5
    }
}
