var testIMS = 1;
IMS = {};
IMS.table = function (config) {
    this.id = config.id;
    this.url = config.url;
    this.columns = config.columns;
    this.queryParams = config.params;

    //if (!this.order) {
    //    this.order = [[1, "desc"]];
    //}
}
//加载datatable
IMS.table.prototype.load = function () {
    var tablePrefix = "#table_server_";
    var _this = this;
    var _table = $("#" + _this.id);
    _table.bootstrapTable({
              url: _this.url,         //请求后台的URL（*）
              method: 'post',                      //请求方式（*）
              toolbar: '#toolbar',                //工具按钮用哪个容器
              striped: true,                      //是否显示行间隔色
              cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
              pagination: true,                   //是否显示分页（*）
              sortable: true,                     //是否启用排序
              sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
              pageNumber: 1,                       //初始化加载第一页，默认第一页
              pageSize: 10,                       //每页的记录行数（*）
              pageList: [10, 25, 50, 100],        //可供选择的每页的行数（*）
              search: false,                       //是否显示表格搜索，此搜索是客户端搜索，不会进服务端，所以，个人感觉意义不大
              strictSearch: false,
              showColumns: true,                  //是否显示所有的列
              showRefresh: true,                  //是否显示刷新按钮
              minimumCountColumns: 1,             //最少允许的列数
              clickToSelect: true,                //是否启用点击选中行
              height: 500,                        //行高，如果没有设置height属性，表格自动根据记录条数觉得表格高度
             // uniqueId: "SBBH",                     //每一行的唯一标识，一般为主键列
              showToggle: true,                    //是否显示详细视图和列表视图的切换按钮
              cardView: false,                    //是否显示详细视图
              detailView: false,                   //是否显示父子表
              queryParams:_this.queryParams,
              columns: _this.columns,
          });
      }


