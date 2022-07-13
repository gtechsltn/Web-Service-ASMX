<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Home</title>
    <style>
        .container {
            margin-bottom: 5px;
        }

        .button {
            margin-bottom: 5px;
        }
        textarea {
            width: 500px;
            height: 200px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            Home
        </div>
        <div class="container">
            <button class="button" id="btnAjaxCallDemo">Call Ajax</button>&nbsp;
            <br />
            <input type="text" id="txtName" />
            <button class="button" id="btnAjaxCallHello">Say Hello</button>
        </div>
        <div class="container">
            <textarea id="txtMessage"></textarea>
        </div>
    </form>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript">
        $(function () {

            $("#btnAjaxCallHello").click(function () {
                $("#txtMessage").html('');
                var name = $.trim($("#txtName").val());
                $.ajax({
                    type: "POST",
                    url: "http://localhost/AdminWebService.asmx/Hello",
                    data: "{ name: '" + name + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (r) {
                        $("#txtMessage").html(r.d);
                        alert(r.d);
                    },
                    error: function (r) {

                        $("#txtMessage").html(r.responseText);
                        alert(r.responseText);
                    },
                    failure: function (r) {
                        $("#txtMessage").html(r.responseText);
                        alert(r.responseText);
                    }
                });
                return false;
            });

            $("#btnAjaxCallDemo").click(function () {
                //var name = $.trim($("[id*=txtName]").val());
                //var age = $.trim($("[id*=txtAge]").val());
                $("#txtMessage").html('');
                $.ajax({
                    type: "POST",
                    url: "http://localhost/AdminWebService.asmx/HelloWorld",
                    //data: "{ name: '" + name + "', age: " + age + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (r) {
                        $("#txtMessage").html(r.d);
                        alert(r.d);
                    },
                    error: function (r) {
                        $("#txtMessage").html(r.responseText);
                        alert(r.responseText);
                    },
                    failure: function (r) {
                        $("#txtMessage").html(r.responseText);
                        alert(r.responseText);
                    }
                });
                return false;
            });
        });
    </script>
</body>
</html>
