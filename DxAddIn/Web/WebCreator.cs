using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxAddIn.Web
{
    public class WebCreator
    {
        public string ClassName { get; set; }
        public string LabelPrefix { get; set; }
        public List<ZhEn> Data { get; set; }
        public string ControllerName { get; set; }
        public string ExportName { get; set; }
        public string CamelLabel => $"{char.ToUpper(LabelPrefix[0])}{LabelPrefix.Substring(1)}";
        public string CreateClass()
        {
            var allprops = Data.Where(n => n.EnName != "Id");

            var head = $"public class {ClassName}\r\n{{\r\n\t";
            var props = "/// <summary>\r\n\t/// 系统编号\r\n\t/// </summary>\r\n\t[Description(\"系统编号\")]\r\n\tpublic string Id{ get; set; }\r\n\t";
            props += string.Join("", Data.Select(n => $"/// <summary>\r\n\t/// {n.ZhName}\r\n\t/// </summary>\r\n\t[Description(\"{n.ZhName}\")]\r\n\tpublic string {n.EnName}{{ get; set; }}\r\n\t"));
            var changeMethod = "public int Change()\r\n" +
                "\t{\r\n" +
                "\t\tvar sp = new SqlAndParam();\r\n" +
                "\t\tif (string.IsNullOrEmpty(Id))\r\n" +
                "\t\t{\r\n" +
                $"\t\t\tsp.Sql = \"insert into {ClassName}({string.Join(",", allprops.Select(n => n.EnName))}) values(:{string.Join(",:", allprops.Select(n => n.EnName))})\";\r\n" +
                $"\t\t\tsp.Param = new {{ {string.Join(",", allprops.Select(n => $"{n.EnName}={n.EnName}"))} }};\r\n" +
                "\t\t}\r\n" +
                "\t\telse\r\n" +
                "\t\t{\r\n" +
                $"\t\t\tsp.Sql = \"update {ClassName} set {string.Join(",", allprops.Select(n => $"{n.EnName}=:{n.EnName}"))} where Id=:Id\";\r\n" +
                $"\t\t\tsp.Param = new {{ Id = Id, {string.Join(",", allprops.Select(n => $"{n.EnName}={n.EnName}"))} }};\r\n" +
                "\t\t}\r\n" +
                "\t\treturn DBHelper.ExcuteWithTrans(sp);\r\n" +
                "\t}\r\n\t";
            var deleteMethod = "public int Delete()\r\n" +
                "\t{\r\n" +
                "\t\tvar sp = new SqlAndParam();\r\n" +
                $"\t\tsp.Sql = \"delete from {ClassName} where Id =:Id\";\r\n" +
                "\t\tsp.Param = new { Id = Id };\r\n" +
                "\t\treturn DBHelper.ExcuteWithTrans(sp);\r\n" +
                "\t}\r\n";

            var end = "}";

            var epqhead = $"public class {ClassName}Epq : EasyuiPagedQuery\r\n{{\r\n";
            var epqmiddle = string.Join("", Data.Select(n => $"\tpublic string {n.EnName}{{ get; set; }}\r\n"));
            var epqend = "}";


            return $"{head}{props}{changeMethod}{deleteMethod}{end}\r\n{epqhead}{epqmiddle}{epqend}";
        }
        public string JsSearchString()
        {
            return string.Join("", Data.Select(n => $"\t\t\t\t\tparam.{n.EnName}=$('#C{n.EnName.ToLower()}').val();\r\n"));
        }
        public string SearchString()
        {
            return string.Join("", Data.Select(n => $"\t{n.ZhName}:\r\n\t<input id=\"C{n.EnName.ToLower()}\" style=\"width:110px\" class=\"easyui-textbox\" />\r\n"));
        }
        public string ParamString()
        {
            return string.Join("", Data.Select(n => $"\t\t\t\t\tparam.{n.ZhName} = $('#C{n.EnName.ToLower()}').val();\r\n"));
        }
        public string ColumnString()
        {
            return string.Join("", Data.Select(n => $"\t\t\t\t\t{{ field: '{n.EnName}', title: '{n.ZhName}', width: 100,align: 'center'}},\r\n"));
        }
        public string SetValueString()
        {
            return string.Join("", Data.Select((n, i) => {
                if (i % 2 == 0)
                    //labelwidth=\"100%\"
                    return $"\t\t\t<input labelwidth=\"100%\" class=\"easyui-textbox\" required style=\"width:49%\" name=\"{n.EnName}\" id=\"{n.EnName}\" label=\"{n.ZhName}:\" />\r\n";
                else
                    return $"\t\t\t<input labelwidth=\"100%\" class=\"easyui-textbox\" required style=\"width:49%\"  name=\"{n.EnName}\" id=\"{n.EnName}\" label=\"{n.ZhName}:\" />\r\n\t\t</div>\r\n\t\t<div style=\"margin:10px;\">\r\n";
            }));
        }
        public string ExportString()
        {
            //Tc0='+$('#Ctc0').val()
            return string.Join("+'&", Data.Select(n => $"{n.EnName}='+$('#C{n.EnName.ToLower()}').val()"));
        }
        public string SqlSearchString()
        {
            //sql += SearchData.CreateMuLikeSql(o.Tc0,"Tc0");
            return string.Join("", Data.Select(n=>$"\tsql += SearchData.CreateMuLikeSql(o.{n.EnName},\"{n.EnName}\");\r\n"));
        }
        public string CreatePage1()
        {
            var table = $"<table id=\"dg\" fit=\"true\"></table>\r\n";
            var toolbar = $"<div id=\"tb\" style=\"padding:2px 5px;\">\r\n" +
            SearchString() +
            "\t\t<a href=\"javascript:$('#dg').datagrid().reload;\" class=\"easyui-linkbutton\" iconCls=\"icon-search\">查询</a>\r\n" +
            "\t\t<a href=\"javascript:void(0);\" class=\"easyui-linkbutton\" iconCls=\"icon-save\" onclick=\"save()\">导出</a>\r\n" +
            "\t\t<a href=\"javascript:void(0);\" class=\"easyui-linkbutton\" iconCls=\"icon-add\" onclick=\"add()\">新增</a>\r\n" +
            "\t\t<a href=\"javascript:void(0);\" class=\"easyui-linkbutton\" iconCls=\"icon-edit\" onclick=\"edit()\">修改</a>\r\n" +
            "\t\t<a href=\"javascript:void(0);\" class=\"easyui-linkbutton\" iconCls=\"icon-remove\" onclick=\"destory()\">删除</a>" +
            "</div>\r\n";
            var dialog = "<div id=\"dlg\" class=\"easyui-dialog\" style=\"width:800px;height:660px\" data-options=\"modal:true\" title=\"维护\" closed=\"true\">\r\n" +
            $"\t<form id=\"fm\" method=\"post\">\r\n" +
            "\t\t<div style=\"margin:10px;\">\r\n" +
                SetValueString() +
             "\t\t</div>\r\n" +
             "\t\t<div style=\"margin:10px;display:none\">\r\n" +
             "\t\t\t@*<div style=\"margin:10px\">*@\r\n" +
             "\t\t\t<input name=\"Id\" label=\"系统编号:\" class=\"easyui-textbox\" editable=\"false\" style=\"width:49%\" />\r\n" +
             "\t\t</div>\r\n" +
             $"\t\t<a href=\"#\" onclick=\"submit()\" class=\"easyui-linkbutton\" style=\"width:99%\" iconCls=\"icon-ok\">提交</a>\r\n" +
             "\t</form>\r\n" +
             "</div>\r\n";
            var script = "@section scripts{\r\n" +
            "\t<script>\r\n" +
            "\t\t$(function () {\r\n" +
            "\t\t\tinit();\r\n" +
            "\t\t})\r\n" +
            "\t\tfunction init() {\r\n" +
            "\t\t\t$('#dg').datagrid({\r\n" +
            "\t\t\t\trownumbers: true,\r\n" +
            "\t\t\t\tstriped: true,\r\n" +
            "\t\t\t\ttoolbar: '#tb',\r\n" +
            //"\t\t\t\tfitColumns: true,\r\n" +
            "\t\t\t\tpagination: true,\r\n" +
            "\t\t\t\tsingleSelect: true,\r\n" +
            "\t\t\t\tpageSize: 10,\r\n" +
            "\t\t\t\tloader: function (param, success, error) {\r\n" +
            JsSearchString() +
            $"\t\t\t\t\t$.post(\"/{ControllerName}/Get{CamelLabel}Data\", param, function (data) {{\r\n" +
            "\t\t\t\t\t\tsuccess(data);\r\n" +
            "\t\t\t\t\t});\r\n" +
            "\t\t\t\t},\r\n" +
            "\t\t\t\tcolumns: [[\r\n" +
            "\t\t\t\t\t{ field: 'Id', title: '系统编号', hidden: true },\r\n" +
            ColumnString() +
            "\t\t\t\t]]\r\n" +
            //"\t\t\t\tonDblClickRow: function (index, row) {\r\n" +
            //$"\t\t\t\t\tedit();\r\n" +
            //"\t\t\t\t}\r\n" +
            "\t\t\t});\r\n" +
            "\t\t}\r\n" +
            "\r\tfunction add(){\r\n"+
            "\t\t\t$('#dlg').dialog('open').dialog('center');\r\n" +
            "\t\t\t$('#fm').form('clear');\r\n" +
            "\t\t}\r\n"+
            "\t\tfunction edit() {\r\n" +
            "\t\t\tvar row = $('#dg').datagrid('getSelected');\r\n" +
            "\t\t\tif (row) {\r\n" +
            "\t\t\t$('#dlg').dialog('open').dialog('center');\r\n" +
            "\t\t\t\t$('#fm').form('load', row);\r\n" +
            "\t\t\t}\r\n" +
            "\t\t}\r\n" +
            "\t\tfunction destory() {\r\n" +
            "\t\t\tvar row = $('#dg').datagrid('getSelected');\r\n" +
            "\t\t\tif (row && row.Id != null) {\r\n" +
            "\t\t\t\tswal({ title: \"确定要删除？\", icon: \"warning\", buttons: [\"取消\", \"确定\"], })\r\n" +
            "\t\t\t\t\t.then((willDelete) => {\r\n" +
            "\t\t\t\t\t\tif (willDelete) {\r\n" +
            $"\t\t\t\t\t\t\t$.post(\"/{ControllerName}/{CamelLabel}Delete\", {{ Id: row.Id }}, function (data) {{\r\n" +
            "\t\t\t\t\t\t\t\tif (data.success) {\r\n" +
            "\t\t\t\t\t\t\t\t\tswal(\"信息\", \"删除成功\", \"success\");\r\n" +
            "\t\t\t\t\t\t\t\t\t$('#dg').datagrid('reload');\r\n" +
            "\t\t\t\t\t\t\t\t} else {\r\n" +
            "\t\t\t\t\t\t\t\t\tswal(\"信息\", data.errorMsg, \"error\");\r\n" +
            "\t\t\t\t\t\t\t\t}\r\n" +
            "\t\t\t\t\t\t\t});\r\n" +
            "\t\t\t\t\t\t}\r\n" +
            "\t\t\t\t\t});\r\n" +
            "\t\t\t} else {\r\n" +
            "\t\t\t\tswal(\"信息\", \"请选择有系统编号的数据\", \"error\");\r\n" +
            "\t\t\t}\r\n" +
            "\t\t}\r\n" +
            "\t\tfunction save(){\r\n"+
            $"\t\t\twindow.location.href = '/{ControllerName}/{CamelLabel}Export?" +ExportString()+
            "}\r\n"+
            "\t\tfunction submit() {\r\n" +
            "\t\t\t$('#fm').form('submit', {\r\n" +
            $"\t\t\t\turl: '/{ControllerName}/{CamelLabel}Change',\r\n" +
            "\t\t\t\tonSubmit: function (param) {\r\n" +
            "\t\t\t\t\treturn $(this).form('validate');\r\n" +
            "\t\t\t\t},\r\n" +
            "\t\t\t\tsuccess: function (result) {\r\n" +
            "\t\t\t\t\tvar result = eval('(' + result + ')');\r\n" +
            "\t\t\t\t\tif (result.success) {\r\n" +
            "\t\t\t\t\t\tswal(\"信息\", \"提交成功\", \"success\");\r\n" +
            "\t\t\t\t\t\t$('#fm').form('clear');\r\n" +
            "\t\t\t\t\t\t$('#dg').datagrid('reload');\r\n" +
            "\t\t\t\t\t} else {\r\n" +
            "\t\t\t\t\t\tswal(\"信息\", result.errorMsg, \"error\");\r\n" +
            "\t\t\t\t\t}\r\n" +
            "\t\t\t\t}\r\n" +
            "\t\t\t});\r\n" +
            "\t\t}\r\n" +
            "\t</script>\r\n" +
            "}";
            return $"{table}{toolbar}{dialog}{script}";
        }

        public string CreatePage()
        {
            LabelPrefix = LabelPrefix.ToLower();
            var table = $"<table id=\"{LabelPrefix}dg\" width=\"100%\" height=\"360\"></table>\r\n";
            var toolbar = $"<div id=\"{LabelPrefix}tb\" style=\"padding:2px 5px;\">\r\n" +
                //$"\t//<input type=\"checkbox\" id=\"ck{LabelPrefix}\" onclick=\"initHd()\" />\r\n" +
                //$"\t//<label for=\"ck{LabelPrefix}\">只显示无法匹配的信息</label>\r\n" +
            SearchString() +
             $"\t<a href=\"javascript:$('#{LabelPrefix}dg').datagrid().reload;\" class=\"easyui-linkbutton\" iconCls=\"icon-search\">查询</a>\r\n" +
             $"\t<a href=\"#\" class=\"easyui-linkbutton\" iconCls=\"icon-edit\" onclick=\"edit{CamelLabel}()\">操作</a>\r\n" +
             $"\t<a href=\"#\" class=\"easyui-linkbutton\" iconCls=\"icon-remove\" onclick=\"delete{CamelLabel}()\">删除</a>\r\n" +
             "\t<br />\r\n" +
             //"\t<label style=\"color:red\">*注:双击数据行可触发操作功能</label>\r\n" +
             "</div>\r\n";
            var panel = "<div class=\"easyui-panel\" fit=\"true\" title=\"操作\" style=\"padding:10px;\">\r\n" +
            $"\t<form id=\"{LabelPrefix}fm\" method=\"post\">\r\n" +
            "\t\t<div style=\"margin:10px;\">\r\n" +
                SetValueString() +
             "\t\t</div>\r\n" +
             "\t\t<div style=\"margin:10px;display:none\">\r\n" +
             "\t\t\t@*<div style=\"margin:10px\">*@\r\n" +
             "\t\t\t<input name=\"Id\" label=\"系统编号:\" class=\"easyui-textbox\" editable=\"false\" style=\"width:49%\" />\r\n" +
             "\t\t</div>\r\n" +
             $"\t\t<a href=\"#\" onclick=\"submit{CamelLabel}()\" class=\"easyui-linkbutton\" style=\"width:99%\" iconCls=\"icon-ok\">提交</a>\r\n" +
             "\t</form>\r\n" +
             "</div>\r\n";

            var script = "@section scripts{\r\n" +
            "\t<script>\r\n" +
            "\t\t$(function () {\r\n" +
            $"\t\t\tinit{CamelLabel}();\r\n" +
            "\t\t})\r\n" +
            $"\t\tfunction init{CamelLabel}() {{\r\n" +
            $"\t\t\t$('#{LabelPrefix}dg').datagrid({{\r\n" +
            "\t\t\t\trownumbers: true,\r\n" +
            "\t\t\t\tstriped: true,\r\n" +
            $"\t\t\t\ttoolbar: '#{LabelPrefix}tb',\r\n" +
            "\t\t\t\tfitColumns: true,\r\n" +
            "\t\t\t\tpagination: true,\r\n" +
            "\t\t\t\tsingleSelect: true,\r\n" +
            "\t\t\t\tpageSize: 10,\r\n" +
            "\t\t\t\tloader: function (param, success, error) {\r\n" +
            $"\t\t\t\t\t//param.Flag = document.getElementById('ck{LabelPrefix}').checked;\r\n" +
            "\t\t\t\t\t//param.Hdlx = $('#Chd').val();\r\n" +
            JsSearchString()+
            $"\t\t\t\t\t$.post(\"/{ControllerName}/Get{CamelLabel}Data\", param, function (data) {{\r\n" +
            "\t\t\t\t\t\tsuccess(data);\r\n" +
            "\t\t\t\t\t});\r\n" +
            "\t\t\t\t},\r\n" +
            "\t\t\t\tcolumns: [[\r\n" +
            "\t\t\t\t\t{ field: 'Id', title: '系统编号', hidden: true },\r\n" +
            ColumnString() +
            "\t\t\t\t]],\r\n" +
            "\t\t\t\tonDblClickRow: function (index, row) {\r\n" +
            $"\t\t\t\t\tedit{CamelLabel}();\r\n" +
            "\t\t\t\t}\r\n" +
            "\t\t\t});\r\n" +
            "\t\t}\r\n" +
            $"\t\tfunction edit{CamelLabel}() {{\r\n" +
            $"\t\t\tvar row = $('#{LabelPrefix}dg').datagrid('getSelected');\r\n" +
            "\t\t\tif (row) {\r\n" +
            $"\t\t\t\t$('#{LabelPrefix}fm').form('load', row);\r\n" +
            "\t\t\t}\r\n" +
            "\t\t}\r\n" +
            $"\t\tfunction delete{CamelLabel}() {{\r\n" +
            $"\t\t\tvar row = $('#{LabelPrefix}dg').datagrid('getSelected');\r\n" +
            "\t\t\tif (row && row.Id != null) {\r\n" +
            "\t\t\t\tswal({ title: \"确定要删除？\", icon: \"warning\", buttons: [\"取消\", \"确定\"], })\r\n" +
            "\t\t\t\t\t.then((willDelete) => {\r\n" +
            "\t\t\t\t\t\tif (willDelete) {\r\n" +
            $"\t\t\t\t\t\t\t$.post(\"/{ControllerName}/{CamelLabel}Delete\", {{ Id: row.Id }}, function (data) {{\r\n" +
            "\t\t\t\t\t\t\t\tif (data.success) {\r\n" +
            "\t\t\t\t\t\t\t\t\tswal(\"信息\", \"删除成功\", \"success\");\r\n" +
            $"\t\t\t\t\t\t\t\t\t$('#{LabelPrefix}dg').datagrid('reload');\r\n" +
            "\t\t\t\t\t\t\t\t} else {\r\n" +
            "\t\t\t\t\t\t\t\t\tswal(\"信息\", data.errorMsg, \"error\");\r\n" +
            "\t\t\t\t\t\t\t\t}\r\n" +
            "\t\t\t\t\t\t\t});\r\n" +
            "\t\t\t\t\t\t}\r\n" +
            "\t\t\t\t\t});\r\n" +
            "\t\t\t} else {\r\n" +
            "\t\t\t\tswal(\"信息\", \"请选择有系统编号的数据\", \"error\");\r\n" +
            "\t\t\t}\r\n" +
            "\t\t}\r\n" +
            $"\t\tfunction submit{CamelLabel}() {{\r\n" +
            $"\t\t\t$('#{LabelPrefix}fm').form('submit', {{\r\n" +
            $"\t\t\t\turl: '/{ControllerName}/{CamelLabel}Change',\r\n" +
            "\t\t\t\tonSubmit: function (param) {\r\n" +
            "\t\t\t\t\treturn $(this).form('validate');\r\n" +
            "\t\t\t\t},\r\n" +
            "\t\t\t\tsuccess: function (result) {\r\n" +
            "\t\t\t\t\tvar result = eval('(' + result + ')');\r\n" +
            "\t\t\t\t\tif (result.success) {\r\n" +
            "\t\t\t\t\t\tswal(\"信息\", \"提交成功\", \"success\");\r\n" +
            $"\t\t\t\t\t\t$('#{LabelPrefix}fm').form('clear');\r\n" +
            $"\t\t\t\t\t\t$('#{LabelPrefix}dg').datagrid('reload');\r\n" +
            "\t\t\t\t\t} else {\r\n" +
            "\t\t\t\t\t\tswal(\"信息\", result.errorMsg, \"error\");\r\n" +
            "\t\t\t\t\t}\r\n" +
            "\t\t\t\t}\r\n" +
            "\t\t\t});\r\n" +
            "\t\t}\r\n" +
            "\t</script>\r\n" +
            "}";

            return $"{table}{toolbar}{panel}{script}";
        }
        public string CreateController()
        {
            var view = $"public ActionResult {CamelLabel}Index() => View();\r\n";
            var getdata = $"public async Task<JsonResult> Get{CamelLabel}Data({ClassName}Epq o) => Json(await services.Get{CamelLabel}(o));\r\n";
            var change = $"public async Task<JsonResult> {CamelLabel}Change({ClassName} o) => Json(await CURDHelper.AsyncRunner(() => o.Change()));\r\n";
            var delete  = $"public async Task<JsonResult> {CamelLabel}Delete({ClassName} o) => Json(await CURDHelper.AsyncRunner(() => o.Delete()));\r\n";
            var export = $"public async Task<FileResult> {CamelLabel}Export({ClassName}Epq o)\r\n"+
                "{\r\n"+
                $"\tvar dt = await services.Get{CamelLabel}Dt(o);\r\n" +
                $"\tvar data = EppHelper.ExportByDt(dt, \"{ExportName}\");\r\n" +
                $"\treturn File(data, \"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet\", $\"{{DateTime.Now.ToString(\"yyyy_MM_dd_HH_mm_ss\")}}{ExportName}.xlsx\");\r\n" +
                "}";
            return $"{view}{getdata}{change}{delete}{export}";
        }
        public string CreateServices()
        {
            var sql = $"string Create{CamelLabel}FindSql({ClassName}Epq o)\r\n" +
                $"{{\r\n" +
                $"\tvar sql = \"(select * from {ClassName})t where 1=1\";\r\n" +
                SqlSearchString()+
                $"\treturn sql;\r\n" +
                $"}}\r\n";
            var pagedata = $"public async Task<EasyuiPaged<{ClassName}>> Get{CamelLabel}({ClassName}Epq o)\r\n" +
                    $"{{\r\n" +
                    $"\tvar middle = Create{CamelLabel}FindSql(o);\r\n" +
                    $"\tvar datasql = SearchData.CreateFindSql(middle, o.start, o.end);\r\n" +
                    $"\tvar data = await DBHelper.AsyncQuery<{ClassName}>(datasql);\r\n" +
                    $"\tvar countsql = SearchData.CreateCountSql(middle);\r\n" +
                    $"\tvar count = await DBHelper.AsyncExecuteScalar(countsql);\r\n" +
                    $"\treturn new EasyuiPaged<{ClassName}>(count.ToInt(), data);\r\n" +
                    $"}}\r\n";
            var dtdata = $"public async Task<DataTable> Get{CamelLabel}Dt({ClassName}Epq o)\r\n" +
                    $"{{\r\n" +
                    $"\tvar middle = Create{CamelLabel}FindSql(o);\r\n" +
                    $"\tvar sql = SearchData.CreateAllDataSql(middle);\r\n" +
                    $"\tvar data = await DBHelper.AsyncQuery<{ClassName}>(sql);\r\n" +
                    $"\tvar dt = await data.ToDataTable();\r\n" +
                    $"\treturn dt;\r\n" +
                    $"}}";
            return $"{sql}{pagedata}{dtdata}";
        }
        public string CreateDbStr()
        {
            var tablename = ClassName.ToUpper();
            var result = $"drop table {tablename}\r\ncreate table {tablename}\r\n(\r\n\tId NUMBER(20),\r\n";
            var lst = Data.Select(n => $"\t{n.EnName} nvarchar2(255),");
            result += string.Join("\r\n", lst);
            result = result.Substring(0, result.Length - 1);
            result += "\r\n)\r\n";
            result += $"--alter table {tablename} drop constraint PK_{tablename}_ID;\r\n--SELECT * from user_cons_columns c where c.table_name = '{tablename}';\r\nalter table {tablename} add constraint PK_{tablename}_ID primary key(ID);";
            result += $"\r\ndrop SEQUENCE SE_{tablename}ID;\r\nCREATE SEQUENCE SE_{tablename}ID\r\nINCREMENT BY 1 -- 每次加几个\r\nSTART WITH 1 -- 从1开始计数\r\nNOMAXVALUE -- 不设置最大值\r\nNOCYCLE -- 一直累加，不循环\r\nNOCACHE -- 不建缓冲区\r\n";
            result += string.Format("--创建触发器\r\ncreate or replace trigger {0}_AUTOID\r\nbefore insert on {0} for each row\r\nbegin\r\nselect SE_{0}ID.nextval into :new.ID from dual;\r\nend {0}_AUTOID;\r\n\r\n", tablename);
            return result;
        }
        public class ZhEn
        {
            private string _zhName;

            public string ZhName
            {
                get => $"{char.ToUpper(_zhName[0])}{_zhName.ToLower().Substring(1)}";
                set => _zhName = value;
            }

            private string _enName;

            public string EnName
            {
                get => $"{char.ToUpper(_enName[0])}{_enName.ToLower().Substring(1)}";
                set => _enName = value;
            }

        }
    }
}
